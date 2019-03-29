﻿using NLog;
using BlogsConsole.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
                    Console.WriteLine(" ");
                    Console.WriteLine("Enter your selection:");
                    Console.WriteLine("1) Display all blogs");
                    Console.WriteLine("2) Add blog");
                    Console.WriteLine("3) Create post");
                    Console.WriteLine("4) Display post");
                    Console.WriteLine("Enter q to quit");
                    // input response
                    choice = Console.ReadLine();
                    logger.Info("Option \"" + choice + "\" selected");
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
                        logger.Info($"Adding to blog ID {blogId}.\n");
                        addNewPost(db, blogId);
                    }
                    if (choice == "4")
                    {
                        displayPosts(db);
                    }

                } while (choice == "1" || choice == "2" || choice == "3" || choice =="4");

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
            var num = db.Blogs.Count();
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine($"{num} blog(s) returned");
            foreach (var item in query)
            {
                Console.WriteLine($"{item.Name}");
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
                if (name.Length<1)
                {
                    logger.Error("Blog name cannot be null");
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
                Console.WriteLine("1) Pick blog by title");
                Console.WriteLine("2) Pick blog by ID");
                Console.WriteLine("3) Pick blog ID from a list of blogs");
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
                                logger.Info($"There are {num} blogs with this title. Please enter more characters.");
                            else if (num == 0)
                                logger.Info("There are no blogs with this title. Please try again.");
                            else if (num == 1)
                            {
                                logger.Info("There is one blog with this title. Retrieving...");
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
                                    logger.Info("There are no blogs with this blog ID. Please try again.");
                                else
                                {
                                    logger.Info("There is one blog with this ID. Retrieving...");
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
                            logger.Error("Invalid Blog Id. Please try again.");
                        }
                    }
                }
                if (choice == "3")
                {
                    var query = db.Blogs.OrderBy(b => b.Name);
                    Console.WriteLine("");
                    Console.WriteLine("Select the blog you would like to post to:");
                    foreach (var item in query)
                    {
                        Console.WriteLine($"{item.Name}: Blog Id #{item.BlogId}");
                    }
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
                                    logger.Info("There are no blogs with this blog ID. Please try again.");
                                else
                                {
                                    logger.Info("There is one blog with this ID. Retrieving...");
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
                            logger.Error("Invalid Blog Id. Please try again.");
                        }
                    }
                }

            } while ((choice == "1" || choice == "2" || choice == "3") && !done) ;

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
                Console.WriteLine("Enter the Post title: ");
                title = Console.ReadLine();
                if (title.Length < 1)
                {
                    logger.Info("Post title cannot be null");
                }
                else
                {
                    done = true;
                }
            }

            // Post content can be null
            Console.WriteLine("Enter the Post content: ");
            content = Console.ReadLine();

            try
            {
                var post = new Post { Title = title, Content = content, BlogId = blogId };

                db.AddPost(post);
                logger.Info($"Post added - \"{title}\"");
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

        public static void displayPosts(BloggingContext db)
        {
            
            string choice = "";
            Console.WriteLine("Select the blog's posts to display: ");
            Console.WriteLine("1) Posts from all blogs");
            Console.WriteLine("2) Pick blog");
            // input response
            choice = Console.ReadLine();
            if (choice == "1")
            {
                displayAllPosts(db, 0);
            }
            else if (choice == "2")
            {
                int blogId = selectBlog(db);
                displayAllPosts(db, blogId);
            }

        }

        public static void displayAllPosts(BloggingContext db, int blogId)
        {
            
            if (blogId == 0)
            {

                // Display all posts from all blogs in the database
                var num = db.Posts.Count();
                Console.WriteLine($"{num} post(s) returned");

                var query = db.Posts.Include("Blog").OrderBy(p => p.Blog.Name);
                foreach (var item in query)
                {
                    Console.Write($"Blog: {item.Blog.Name}\nTitle: {item.Title}\nContent: {item.Content}\n\n");
                }

            }
            else
            {
                // Blog id selected. Display all posts from one blog.
                Console.WriteLine("");
                var num = db.Posts.Where(p => p.BlogId.Equals(blogId)).Count();
                Console.WriteLine($"{num} post(s) returned");
                IEnumerable<Post> postList = db.Posts.Where(p => p.BlogId.Equals(blogId));
                
                foreach (Post p in postList)
                {
                    Console.Write($"Blog: { p.Blog.Name }\nTitle: {p.Title}\nContent: {p.Content}\n\n");
                }
            }
        }


    }
}