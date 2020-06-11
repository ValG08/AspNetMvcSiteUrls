using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UrlsProject.Common.Models;
using UrlsProject.Helpers;
using UrlsProject.Services;

namespace UrlsProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeControllerService _homeControllerService;

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        public HomeController()
        {
            _homeControllerService = new HomeControllerService();
        }    

        [HttpPost]
        public async Task<ActionResult> GetResults(string url)
        {
            if(_homeControllerService == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError,
                    "homeControllerService is null");
            }

            try
            {
                PageResultInfoViewModel res = await _homeControllerService.Results(url);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }          
        }

        [HttpPost]
        public async Task<ActionResult> GetHostHistory(string url)
        {
            if (_homeControllerService == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError,
                    "homeControllerService is null");
            }

            try
            {
                string hostUrl = Helper.GetHost(url);
               
                if (hostUrl != null)
                {
                    HistoryViewModel history = await _homeControllerService.GetHostHistory(hostUrl);
                    return Json(history, JsonRequestBehavior.AllowGet);
                }

                return Json(HistoryViewModel.HistoryError(url, "History is empty"),
                       JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }
    }
}