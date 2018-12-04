using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AngleSharp;
using AngleSharp.Parser.Html;
using FunApp.Data;
using FunApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sandbox
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine($"{typeof(Program).Namespace} ({string.Join(" ", args)}) starts working...");
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            IServiceProvider serviceProvider = serviceCollection.BuildServiceProvider(true);

            using (var serviceScope = serviceProvider.CreateScope())
            {
                serviceProvider = serviceScope.ServiceProvider;
                SandboxCode(serviceProvider);
            }
        }

        private static void SandboxCode(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetService<FunAppContext>();

            //Getting the joke from dir.bg with AngleSharp   
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var webClient = new WebClient() {Encoding = Encoding.GetEncoding("windows-1251")};
            var parser = new HtmlParser();

            for (int i = 1; i < 10000; i++)
            {
                var url = "http://fun.dir.bg/vic_open.php?id=" + i;
                var html = webClient.DownloadString(url);

                var document = parser.Parse(html);
                var jokeContent = document.QuerySelector("#newsbody")?.TextContent.Trim();
                var categoryName = document.QuerySelector(".tag-links-left a")?.TextContent.Trim();

                if (!string.IsNullOrWhiteSpace(jokeContent) && !string.IsNullOrWhiteSpace(categoryName))
                {
                    var category = dbContext.Categories.FirstOrDefault(x => x.Name == categoryName);
                    if (category == null)
                    {
                        category = new Category() {Name = categoryName};
                    }

                    var joke = new Joke()
                    {
                        Category = category,
                        Content = jokeContent
                    };

                    dbContext.Jokes.Add(joke);
                    dbContext.SaveChanges();
                }

                Console.WriteLine($"{i} => Joke number {i} added");
            }          
            //

            // TODO: Code here
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddEnvironmentVariables()
                .Build();

            services.AddDbContext<FunAppContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));            
        }
    }
}
