using System;
using System.Diagnostics;
using System.Linq;
using FunApp.Data.Common;
using FunApp.Data.Migrations;
using FunApp.Data.Models;
using Microsoft.AspNetCore.Mvc;
using FunApp.Web.Models;
using FunApp.Web.Models.Home;

namespace FunApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRepository<Joke> jokeRepository;

        public HomeController(IRepository<Joke> jokeRepository)
        {
            this.jokeRepository = jokeRepository;
        }

        public IActionResult Index()
        {
            var jokes = this.jokeRepository.All()
                .OrderBy(x => Guid.NewGuid())
                .Select(x => new IndexJokeViewModel
            {
                Content = x.Content,
                CategoryName = x.Category.Name
            }).Take(20).ToList();

            var viewModel = new IndexViewModel
            {
                Jokes = jokes
            };

            return View(viewModel);
        }

        public IActionResult About()
        {
            ViewData["Message"] = $"My application has {this.jokeRepository.All().Count()} jokes.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
