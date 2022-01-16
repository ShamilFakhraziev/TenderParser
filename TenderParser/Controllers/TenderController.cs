using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TenderParser.Services;

namespace TenderParser.Controllers
{
    public class TenderController:Controller
    {
        [HttpGet]
        public IActionResult ViewTender()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ViewTender(int id)
        {
            var tender = await Parser.GetTenderAsync(id);

            if (tender == null)
            {
                ViewBag.ErrorMessage = "Не существующий номер тендера!";
                return View();
            }

            return View(tender);

        }
    }
}
