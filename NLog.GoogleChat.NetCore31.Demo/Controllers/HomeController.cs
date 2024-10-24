using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NLog.GoogleChat.NetCore31.Demo.Helpers;
using NLog.GoogleChat.NetCore31.Demo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NLog.GoogleChat.NetCore31.Demo.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        public IActionResult Index()
        {
            LogHelper.LogInfo("MySite", LogHelper.GetMethodName(), "Start to Log!", "Phill");
            return View();
        }

        public IActionResult Privacy()
        {
            try
            {
                int a = 1;
                int b = 0;
                int c = a / b;

            }
            catch (Exception ex)
            {
                LogHelper.LogError("MySite", LogHelper.GetMethodName(), ex.Message, ex, "Phill");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
