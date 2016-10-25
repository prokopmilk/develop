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
    public class NewController : Controller
    {
        private Repository repo = new Repository();
        // GET: New
        public ActionResult Index()
        {
            return View(repo.News.OrderByDescending(x=>x.id));
        }
        public ActionResult Delete(int id)
        {
            New deleted = repo.DeleteNews(id);
            if (deleted != null)
            {
                TempData["message"] = string.Format("Статья \"{0}\" была удалена",
                    deleted.Header);
            }
            return RedirectToAction("Index");
        }
        public ActionResult NewPost(int id)
        {
            NewPost model = new NewPost()
            {
                news = repo.News.FirstOrDefault(x => x.id==id),
                News = repo.News
            };
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            New news = repo.News.FirstOrDefault(x => x.id == id);
            return View(news);
        }

        [HttpPost]
        public ActionResult Edit(New news, HttpPostedFileBase image)
        {
            if (news.id == 0)
            {
                news.DataPub = System.DateTime.Now;
            }
            if (ModelState.IsValid)
            {

                if (image != null)
                {

                    news.Picture = new byte[image.ContentLength];
                    image.InputStream.Read(news.Picture, 0, image.ContentLength);
                }

                repo.SaveNews(news);
                
                TempData["message"] = string.Format("Изменения в новости \"{0}\" были сохранены", news.Header);
                return RedirectToAction("/Index");

            }
            else
            {
                // Что-то не так со значениями данных
                TempData["message"] = string.Format("Изменения в новости \"{0}\" не удалось сохранить", news.Header);

                return View(news);
            }
        }

        public ActionResult Create()
        {
            return View("Edit", new New());
        }


        public FileContentResult GetImage(int id)
        {
            New tovar = repo.News
                     .FirstOrDefault(g => g.id == id);

            if (tovar != null)
            {
                if (tovar.Picture != null)
                {
                    return File(tovar.Picture, "image/png");
                }
                else return null;
            }
            else
            {

                return null;
            }
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
            if (CurUser != null)
            {
                model.name = CurUser.name;
            }
            else model.name = "";
            return PartialView(model);
        }

    }
}