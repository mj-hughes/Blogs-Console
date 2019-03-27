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

            try
            {
                var blog = new Blog { Name = name };

                db.AddBlog(blog);
                logger.Info("Blog added - {name}", name);
            }
            catch (ExternalException ex)
            {
                logger.Error("Data exception on new blog add: " + ex.Message);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

        }

        public static int selectBlog(BloggingContext db)
        {
            int returnBlogID=0;
            string blogTitle = "";
            Boolean done = false;
            string choice = "";
            do
            {
                done = false;
                Console.WriteLine("");
                Console.WriteLine("1) Pick blog by title.");
                Console.WriteLine("2) Pick blog by ID.");
                // input response
                choice = Console.ReadLine();
                if (choice == "1")
                {
                    while (!done)
                    {
                        Console.Write("Enter title: ");
                        blogTitle = Console.ReadLine().ToUpper();

                        try
                        {

                            var num = db.Blogs.Where(b => b.Name.ToUpper().Contains(blogTitle)).Count();
                            if (num > 1)
                                Console.WriteLine("There are more than one blogs with this title. Please enter more characters.");
                            else if (num == 0)
                                Console.WriteLine("There are no blogs with this title. Please try again.");
                            else if (num == 1)
                            {
                                Console.WriteLine("There is one blog with this title. Retrieving...");
                                IEnumerable<Blog> blogList = db.Blogs.Where(b => b.Name.ToUpper().Contains(blogTitle));
                                foreach (Blog b in blogList)
                                {
                                    returnBlogID = b.BlogId;
                                }
                                done = true;
                            }
                            else
                                Console.WriteLine("There are an unknown number of blogs with this title. Please try again.");
                        }
                        catch (ExternalException ex)
                        {
                            logger.Error("Data exception selecting blog by title: " + ex.Message);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex.Message);
                        }

                    }
                }
                if (choice == "2")
                {
                    while (!done)
                    {
                        Console.Write("Enter blog ID: ");
                        if (int.TryParse(Console.ReadLine(), out int blogId))
                        {
                            returnBlogID = blogId;
                            try
                            {
                                var num = db.Blogs.Where(b => b.BlogId.Equals(returnBlogID)).Count();
                                if (num != 1)
                                    Console.WriteLine("There are no blogs with this blog ID. Please try again.");
                                else
                                {
                                    Console.WriteLine("There is one blog with this ID. Retrieving...");
                                    returnBlogID = blogId;
                                    done = true;
                                }
                            }
                            catch (ExternalException ex)
                            {
                                logger.Error("Data exception selecting blog by ID: " + ex.Message);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                            }

                        }
                        else
                        {
                            logger.Error("Blog ID is not an integer value. Please try again.");
                        }
                    }
                }
            } while ((choice == "1" || choice == "2") && !done) ;

            return returnBlogID;
        }

        public static void addNewPost(BloggingContext db, int blogId)
        {
            // Create and save a new Post
            string title = "";
            string content = "";
            Boolean done = false;
            while (!done)
            {
                Console.Write("Enter a title for the new Post: ");
                title = Console.ReadLine();
                if (title.Length < 3)
                {
                    Console.WriteLine("Post title must be at least two characters.");
                }
                else
                {
                    done = true;
                }
            }

            done = false;
            while (!done)
            {
                Console.Write("Enter content for the new Post: ");
                content = Console.ReadLine();
                if (content.Length < 3)
                {
                    Console.WriteLine("Post content must be at least two characters.");
                }
                else
                {
                    done = true;
                }

            }

            try
            {
                var post = new Post { Title = title, Content = content, BlogId = blogId };

                db.AddPost(post);
                logger.Info($"Post added - {title}");
            }
            catch (ExternalException ex)
            {
                logger.Error("Data exception adding new post: " + ex.Message);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }

        }

    }
}