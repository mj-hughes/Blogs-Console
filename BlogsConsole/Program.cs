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
                        displayAllBlogs();
                    }
                    else if (choice == "2")
                    {
                        addNewBlog();                        
                    }
                    if (choice == "3")
                    {
                        
                    }

                } while (choice == "1" || choice == "2" || choice == "3");

            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

            Console.WriteLine("Press enter to quit");
            string x = Console.ReadLine();

            logger.Info("Program ended");
        }

        public static void displayAllBlogs()
        {
            var db = new BloggingContext();

            // Display all Blogs from the database
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
            {
                Console.WriteLine(item.Name);
            }
        }

        public static void addNewBlog()
        {
            // Create and save a new Blog
            Boolean done = false;
            while (!done)
            {
                Console.Write("Enter a name for a new Blog: ");
                var name = Console.ReadLine();
                if (name.Length>2)
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
