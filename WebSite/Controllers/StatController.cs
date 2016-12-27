using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WebSite.Models;

namespace WebSite.Controllers
{
    public class StatController : Controller
    {
        // GET: Stat
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult getMetrix()
        {
            string filename = "https://api-metrika.yandex.com.tr/stat/traffic/summary?id=38564750&oauth_token=AQAAAAAEgPKiAANb8MT2t8wJqkKejG_6IBjNQ7U&date1=20160901";
            XmlDocument document = new XmlDocument();
            List<YandexMetrix> list = null;
            try
            {
                document.Load(filename);
                XmlNodeList elementsByTagName = document.GetElementsByTagName("row");
                list = new List<YandexMetrix>();
                foreach (XmlNode node in elementsByTagName)
                {
                    YandexMetrix item = new YandexMetrix
                    {
                        Day = DateTime.ParseExact(node["date"].InnerText, "yyyyMMdd", null).ToShortDateString(),
                        Visits = Convert.ToInt32(node["visits"].InnerText),
                        Visitors = Convert.ToInt32(node["visitors"].InnerText),
                        PageViews = Convert.ToInt32(node["page_views"].InnerText),
                        NewVisitors = Convert.ToInt32(node["new_visitors"].InnerText)
                    };
                    list.Add(item);
                }
            }
            catch (Exception exception)
            {
                base.TempData["message"] = $"Не удалось загрузить данные по аналитике из-за ошибки: {exception.Message}";
                list = new List<YandexMetrix>();
                YandexMetrix metrix2 = new YandexMetrix
                {
                    Day = DateTime.Now.ToString(),
                    Visits = Convert.ToInt32(0),
                    Visitors = Convert.ToInt32(0),
                    PageViews = Convert.ToInt32(0),
                    NewVisitors = Convert.ToInt32(0)
                };
                list.Add(metrix2);
            }
            return new JsonResult
            {
                Data = list,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult StaForUser(int ID)
        {
            try
            {
                WebSite.Models.Repository.Repository repository = new WebSite.Models.Repository.Repository();
                Stat stat = new Stat();
                int num = repository.Categories.Count<Category>();
                string[] strArray = new string[num];
                int index = 0;
                foreach (Category category in repository.Categories)
                {
                    strArray[index] = category.Category_Name;
                    index++;
                }
                IEnumerable<Order> enumerable = from c in repository.Orders
                                                where c.Customer.CustomerId == ID
                                                select c;
                List<int> list = new List<int>();
                foreach (Order order in enumerable)
                {
                    list.Add(order.OrderId);
                }
                stat.cProduct = new int[num];
                stat.sumProduct = new float[num];
                foreach (OrderItem item in repository.OrderItems)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        for (int j = 0; j < num; j++)
                        {
                            if ((item.tovar.Category == strArray[j]) && (item.Order.OrderId == list[i]))
                            {
                                stat.cProduct[j] += item.Quantity;
                                stat.sumProduct[j] += item.tovar.Price * item.Quantity;
                            }
                        }
                    }
                }
                stat.categoryName = strArray;
                return new JsonResult
                {
                    Data = stat,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }

        
        public ActionResult Stat()
        {
            try
            {
                WebSite.Models.Repository.Repository repository = new WebSite.Models.Repository.Repository();
                Stat stat = new Stat();
                int num = repository.Categories.Count();
                string[] Ca_Name = new string[num];
                int index = 0;
                foreach (Category category in repository.Categories)
                {
                    Ca_Name[index] = category.Category_Name;
                    index++;
                }
                stat.cProduct = new int[num];
                stat.sumProduct = new float[num];
                foreach (var item in repository.OrderItems)
                {
                    
                    for (int i = 0; i < num; i++)
                    {
                        if (item.tovar.Category == Ca_Name[i])
                        {
                            stat.cProduct[i] += item.Quantity;
                            stat.sumProduct[i] += item.Quantity * item.tovar.Price;
                        }
                      
                                                
                    }
                }
                stat.categoryName = Ca_Name;
                return new JsonResult
                {
                    Data = stat,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception)
            {
                return HttpNotFound();
            }
        }
        public ActionResult StatForMe()
        {
            try
            {
            
               
                string userId = User.Identity.GetUserId();
                ApplicationUser CurUser = this.UserManager.Users.FirstOrDefault(x => x.Id == userId);
                WebSite.Models.Repository.Repository repository = new WebSite.Models.Repository.Repository();
                int ID = repository.Customers.FirstOrDefault(x => (x.Email == CurUser.Email)).CustomerId;
                Stat stat = new Stat();
                int num = repository.Categories.Count();
                string[] strArray = new string[num];
                int index = 0;
                foreach (Category category in repository.Categories)
                {
                    strArray[index] = category.Category_Name;
                    index++;
                }
                IEnumerable<Order> enumerable = from c in repository.Orders
                                                where c.Customer.CustomerId == ID
                                                select c;
                List<int> list = new List<int>();
                foreach (Order order in enumerable)
                {
                    list.Add(order.OrderId);
                }
                stat.cProduct = new int[num];
                foreach (OrderItem item in repository.OrderItems)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        for (int j = 0; j < num; j++)
                        {
                            if ((item.tovar.Category == strArray[j]) && (item.Order.OrderId == list[i]))
                            {
                                stat.cProduct[j] += item.Quantity;
                            }
                        }
                    }
                }
                stat.categoryName = strArray;
                return new JsonResult
                {
                    Data = stat,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            catch (Exception)
            {
                return base.HttpNotFound();
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
            model.name = CurUser.name;
            return PartialView(model);
        }
        public ActionResult Statistics()
        {
            return View();
        }
    }
}