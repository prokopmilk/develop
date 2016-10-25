using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Models;
using WebSite.Models.Repository;

namespace WebSite.Controllers
{
    
    public class HomeController : Controller
    {
        
       
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MakeOrder()
        {
           
            return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Catalog()
        {
            Repository repos = new Repository();
            return View(repos.Categories);
        }
        
        public ActionResult LetClient()
        {
            
            return View();
        }
        public ActionResult Prodaction()
        {

            return View();
        }

        public ActionResult _MailLetter()
        {

            return View();
        }
        public ViewResult Sale(string category = null)
        {
           
               
                Repository repos = new Repository();
                SaleModel model = new SaleModel
                {
                    Tovars = repos.Tovars
                    .Where(t => category == null || t.Category == category)
                    .OrderBy(tovar => tovar.Tovar_ID)

                };
            
            return View(model);
           
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public PartialViewResult _UserPartial()
        {
            var userId = User.Identity.GetUserId();
            var CurUser = UserManager.Users.FirstOrDefault(x => x.Id == userId);
            var model = new NavBarViewModel();
            model.name = CurUser.name;
            return PartialView(model);
        }
        



    }
}