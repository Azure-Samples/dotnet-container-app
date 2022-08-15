using Microsoft.AspNetCore.Mvc;
using System;
using System.Text.Encodings.Web;

namespace aspnetcorewebapp.Controllers
{
     public class HelloWorldController : Controller
    {
        // 
        // GET: /HelloWorld/

        public IActionResult Index()
        {
            return View();
        }

        // 
        // GET: /HelloWorld/Welcome/ 

        public IActionResult Welcome()
        {
            string name = "My Name";
            string msg = string.Format("Hello, {0}!", name.ToString());

            return View(msg);
        }
    }
}