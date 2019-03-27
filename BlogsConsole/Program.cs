using NLog;
using BlogsConsole.Models;
using System;
using System.Linq;

namespace BlogsConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                var db = new BloggingContext();

                string choice = "";
                do
                {
                    Console.WriteLine("");
                    Console.WriteLine("1) Display all blogs.");
                    Console.WriteLine("2) Add a new blog.");
                    Console.WriteLine("3) Create a post.");
                    Console.WriteLine("Press any other key to exit.");
                    // input response
                    choice = Console.ReadLine();
                    if (choice == "1")
                    {
                        displayAllBlogs(db);
                    }
                    else if (choice == "2")
                    {
                        addNewBlog(db);                        
                    }
                    if (choice == "3")
                    {
                        int blogId=selectBlog(db);
                        Console.WriteLine($"Adding to blog ID {blogId}.\n");
                        addNewPost(db, blogId);
                    }

                } while (choice == "1" || choice == "2" || choice == "3");

            }
            catch(ExternalException ex)
            {
                logger.Error("Data exception "+ex.Message);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            logger.Info("Program ended");
        }

        public static void displayAllBlogs(BloggingContext db)
        {

            // Display all Blogs from the database
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine($"Blog #{item.BlogId}: {item.Name}");
            }
        }

        public static void addNewBlog(BloggingContext db)
        {
            // Create and save a new Blog
            string name = "";
            Boolean done = false;
            while (!done)
            {
                Console.Write("Enter a name for a new Blog: ");
                name = Console.ReadLine();
                if (name.Length<3)
                {
                    Console.WriteLine("Blog name must be at least two characters.");
                }
                else
                {
                    done = true;
                }
            }

            var blog = new Blog { Name = name };
            var db = new BloggingContext();

            db.AddBlog(blog);
            logger.Info("Blog added - {name}", name);
        }

    }
}
