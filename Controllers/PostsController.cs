using System.Security.Claims;
using BlogApp.Data;
using BlogApp.Data.Abstract;
using BlogApp.Entity;
using BlogApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{

    public class PostsController : Controller
    {

        private IPostRepository _postRepository;
        private ICommentRepository _commentRepository;
        private ITagRepository _tagRepository;
        private readonly ILogger<PostsController> _logger;
        public PostsController(IPostRepository postRepository, ICommentRepository commentRepository, ITagRepository tagRepository, ILogger<PostsController> logger)
        {
            _postRepository = postRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
            _logger = logger;
        }
        public async Task<IActionResult> Index(string tag)
        {
            var posts = _postRepository.Posts.Where(i => i.IsActive);

            if (!string.IsNullOrEmpty(tag))
            {
                posts = posts.Where(x => x.Tags.Any(t => t.Url == tag));
            }

            return View(new PostViewModel { Posts = await posts.ToListAsync() });
        }

        public async Task<IActionResult> Details(string url)
        {
            return View(await _postRepository.Posts.Include(x=>x.User).Include(x => x.Tags).Include(x => x.Comments).ThenInclude(x => x.User).FirstOrDefaultAsync(p => p.Url == url));
        }

        [HttpPost]
        public JsonResult AddComment(int PostId, string Text)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var avatar = User.FindFirstValue(ClaimTypes.UserData);

            var entity = new Comment
            {
                PostId = PostId,
                Text = Text,
                PublishedOn = DateTime.Now,
                UserId = int.Parse(userId ?? "")
            };

            _commentRepository.CreateComment(entity);
            return Json(new
            {
                username,
        text = entity.Text,
        publishedOn = entity.PublishedOn,
        avatar
            });
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult Create(PostCreateViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                _postRepository.CreatePost(
                    new Post
                    {
                        Title = model.Title,
                        Content = model.Content,
                        Description = model.Description,
                        Url = model.Url,
                        UserId = int.Parse(userId ?? ""),
                        Image = "1.png",
                        IsActive = false
                    }
                );
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "");
            var role = User.FindFirstValue(ClaimTypes.Role);

            var posts = _postRepository.Posts;

            if (string.IsNullOrEmpty(role))
            {
                posts = posts.Where(i => i.UserId == userId);
            }

            return View(await posts.ToListAsync());
        }

        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var post = _postRepository.Posts.Include(x=>x.Tags).FirstOrDefault(i => i.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            ViewBag.Tags = _tagRepository.Tags.ToList();
            return View(
                new PostCreateViewModel
                {
                    PostId = post.PostId,
                    Title = post.Title,
                    Description = post.Description,
                    Content = post.Content,
                    Url = post.Url,
                    IsActive = post.IsActive,
                    Tags = post.Tags
                }

                );
        }

        [Authorize]
        [HttpPost]

        public IActionResult Edit(PostCreateViewModel model, int[] tagIds)
        {
            if (ModelState.IsValid)
            {
                var entityUpdate = new Post
                {
                    PostId = model.PostId,
                    Title = model.Title,
                    Description = model.Description,
                    Content = model.Content,
                    Url = model.Url,

                };
                if (User.FindFirstValue(ClaimTypes.Role) == "admin")
                {
                    entityUpdate.IsActive = model.IsActive;
                }
                _postRepository.EditPost(entityUpdate,tagIds);
                return RedirectToAction("List");
            }
            return View(model);
        }
    
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _postRepository.Posts.FirstOrDefaultAsync(p => p.PostId == id);

            if (post == null)
            {
                return NotFound();
            }

            _postRepository.DeletePost(post);
            return RedirectToAction("List");
        }
    
    }

}
