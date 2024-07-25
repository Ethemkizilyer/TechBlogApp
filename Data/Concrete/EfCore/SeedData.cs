using BlogApp.Entity;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Data.Concrete.EfCore{

    public static class SeedData{

        public static void TestVerileriniDoldur(IApplicationBuilder app){

            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<BlogContext>();

            if(context != null){
                if(context.Database.GetPendingMigrations().Any()){
                    context.Database.Migrate();
                }

                if(!context.Tags.Any()){
                    context.Tags.AddRange(
                        new Tag {Text = "Web Programlama",Url="web-programlama", Color = TagColors.primary},
                        new Tag {Text = "Fullstack Developer",Url="full-stack", Color = TagColors.danger},
                        new Tag {Text = "Game Developer",Url="game", Color = TagColors.info},
                        new Tag {Text = "Backend Developer",Url="backend", Color = TagColors.success},
                        new Tag {Text = "Frontend Developer",Url="frontend", Color = TagColors.secondary}
                    );
                    context.SaveChanges();
                }
                if(!context.Users.Any()){
                    context.Users.AddRange(
                        new User{UserName = "EThemKZLYR", Name="Ethem KIZILYER", Email = "ethemkizilyer@gmail.com",Password="123456,", Image = "p1.jpg"},
                        new User{UserName = "TalhaKZLYR", Name="Talha KIZILYER", Email = "talhakizilyer@gmail.com",Password="123456,", Image = "p2.jpg"}
                    );
                    context.SaveChanges();
                }
                if(!context.Posts.Any()){
                    context.Posts.AddRange(
                        new Post{
                            Title = "Asp.net Core",
                            Content = "Asp.net core güzel bir kütüphanedir.",
                            Description = "Asp.net core güzeldir",
                            Url ="asp-net-core",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-15),
                            Tags = context.Tags.Take(3).ToList(),
                            Image = "3.png",
                            UserId = 1,
                            Comments = new List<Comment>{
                                new Comment {Text = "Başarılı", PublishedOn = new DateTime(),UserId=1},
                                new Comment {Text = "Başarılı, tavsiye ederim", PublishedOn = new DateTime(),UserId=2}
                            }
                        },
                        new Post{
                            Title = "Unity ile oyun geliştirme",
                            Content = "Unity editörü ile oyunlar geliştirebilirsiniz.",
                            Description = "Unity editörünü tanıyalım",
                            Url ="Unity-ile-oyun-geliştirme",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-10),
                            Tags = context.Tags.Take(2).ToList(),
                            Image = "2.png",
                            UserId = 2
                        },
                        new Post{
                            Title = "Full Stack Developer Olmak",
                            Content = "Full Stack Developer Olmak Güzeldir.",
                            Description = "Full Stack Developer nasıl olunur",
                            Url ="Full-Stack-Developer",
                            IsActive = true,
                            PublishedOn = DateTime.Now.AddDays(-5),
                            Tags = context.Tags.Take(4).ToList(),
                            Image = "1.png",
                            UserId = 1
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
}