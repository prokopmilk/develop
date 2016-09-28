using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using WebSite.Abstract;
using WebSite.Models;
using WebSite.Models.Concrete;
using WebSite.Models.Repository;

namespace WebSite.Controllers
{
    public class CartController : Controller
    {



        Repository rep = new Repository();

        public ViewResult Checkout()
        {
            if (User.Identity.IsAuthenticated)
            {
                ShippingDetails sh = new ShippingDetails();
                sh.City = this.getCity();
                sh.Name = this.getName();
                sh.Line1 = this.getAdres();
                sh.Mail = User.Identity.Name;
                var uid = User.Identity.GetUserId();
                var usermanager = UserManager.GetPhoneNumber(uid);
                sh.Phone = usermanager;
                
                    return View(sh);
               
                
            }else  return View(new ShippingDetails());
        }

        [HttpPost]
        public ViewResult Checkout(Cart cart, ShippingDetails shippingDetails)
        {

            if (User.Identity.IsAuthenticated)
            {
                if (cart.Lines.Count() == 0)
                {
                    ModelState.AddModelError("", "Извините, ваша корзина пуста!");
                }

                if (ModelState.IsValid)
                {

                   EmailSettings msetting = new Repository().MailSettings.FirstOrDefault(x => x.Description.Equals("Интернет магазин - уведомление о заказах"));
                    msetting.MailToAddress = shippingDetails.Mail;
                    if (msetting.MailToAddress != null)
                    {
                        EmailOrderProcessor mail = new EmailOrderProcessor(msetting);
                        mail.TestSend(cart, shippingDetails, shippingDetails.Mail, "Оформлен новый заказ");
                        mail.ProcessOrder(cart, shippingDetails);
                        msetting.MailToAddress = new Repository().MailSettings.FirstOrDefault(x => x.Description.Equals("Интернет-магазин")).MailFromAddress;
                    if (msetting.MailToAddress != null)
                    {
                       mail = new EmailOrderProcessor(msetting);
                            mail.SendAdmin(cart, shippingDetails, msetting.MailToAddress, "Оформлен новый заказ");
                    }
                    }
                   

                    
                    Customer client = rep.Customers.FirstOrDefault(c => c.Email == User.Identity.Name);

                    Order order = new Order();
                    order.OrderId = 0;
                    order.StatusOrder = false;
                   
                    order.Customer = client;
                    order.TotalSum = cart.Lines.Sum(c => c.tovar.Price*c.Quantity);
                    order.PurchaseDate = System.DateTime.Now;
                    

                    OrderItem OI = null;
                    
                    order.OrderItems = new List<OrderItem>();
                    
                    foreach (var car in cart.Lines)
                    {
                        Tovar tv = car.tovar;
                        OI = new OrderItem();
                                       
                        OI.Order = order;
                        OI.Tovar_ID = tv.Tovar_ID;
                        //OI.tovar = tv;
                        OI.Quantity = car.Quantity;
                        order.OrderItems.Add(OI);
                        tv.orderitems.Add(OI);
                        rep.SaveOrderItem(OI);
                        
                        
                    }
                    
                    rep.SaveOrder(order);
                    client.Orders.Add(order);
                    rep.SaveClient(client);
                    
                    cart.Clear();
                    return View("Completed");
                }
                else
                {
                    return View(shippingDetails);
                }
            }else
            {
                if (cart.Lines.Count() == 0)
                {
                    ModelState.AddModelError("", "Извините, ваша корзина пуста!");
                }

                if (ModelState.IsValid)
                {

                    EmailSettings msetting = null;
                    if (shippingDetails.Mail!=null)
                    {
                        msetting = new Repository().MailSettings.FirstOrDefault(x => x.Description.Equals("Интернет магазин - уведомление о заказах"));
                        msetting.MailToAddress = shippingDetails.Mail;
                        EmailOrderProcessor mail = new EmailOrderProcessor(msetting);
                        mail.TestSend(cart, shippingDetails, shippingDetails.Mail, "Оформлен новый заказ");
                    }
                    msetting.MailToAddress = new Repository().MailSettings.FirstOrDefault(x => x.Description.Equals("Интернет-магазин")).MailFromAddress;
                    if (msetting.MailToAddress != null)
                    {
                        EmailOrderProcessor mail = new EmailOrderProcessor(msetting);
                        mail.SendAdmin(cart, shippingDetails, msetting.MailToAddress, "Оформлен новый заказ");
                    }

                    Customer client = new Customer();
                    client.CustomerId = 0;
                    client.Adres = shippingDetails.Line1;
                    client.City = shippingDetails.City;
                    client.Name = shippingDetails.Name;
                    client.Phone = shippingDetails.Phone;
                    if (shippingDetails.Mail != null)
                    {
                        client.Email = shippingDetails.Mail;
                    }
                    else client.Email = shippingDetails.Phone+"@nullmail.ru";
                    
                    
                    
                    Order order = new Order();
                    order.OrderId = 0;
                    order.StatusOrder = false;
                    order.Customer = client;
                    order.TotalSum = cart.Lines.Sum(c => c.tovar.Price);
                    order.PurchaseDate = System.DateTime.Now;
                    client.Orders = new List<Order>();
                    
                    
                    OrderItem OI = null;
                    
                    order.OrderItems = new List<OrderItem>();
                    foreach(var car in cart.Lines)
                    {   OI = new OrderItem();
                        OI.OrderItemId = 0;
                        OI.Order = order;
                        OI.Tovar_ID = car.tovar.Tovar_ID;
                        OI.Quantity = car.Quantity;
                        rep.SaveOrderItem(OI);
                        
                        car.tovar.orderitems.Add(OI);
                    }
                   
                     
                    
                   rep.SaveOrder(order);
                    client.Orders.Add(order);
                    
                    client.CreateUserAsCustomer();
                    rep.SaveClient(client);
                                       
                    
                    cart.Clear();
                    return View("CompletedAndNewUser");
                }
                else
                {
                    return View(shippingDetails);
                }
            }

            }
        public string getCity()
        {
            var ident = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var city = ident.Claims.Where(c => c.Type == "city").Select(c => c.Value).SingleOrDefault();
            return city;
        }
        public string getAdres()
        {
            var ident = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var adres = ident.Claims.Where(c => c.Type == ClaimTypes.StreetAddress).Select(c => c.Value).SingleOrDefault();
            return adres;
        }
        public string getName()
        {
            var ident = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var name = ident.Claims.Where(c => c.Type == "name").Select(c => c.Value).SingleOrDefault();
            return name;
        }

        public CartController() { }
        
        // GET: Cart
        public ActionResult Index(Cart cart,string returnUrl)
        {
            if (returnUrl==null)
            {
                returnUrl = "../Home/Sale";
            }
            
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });


        }


        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }
        public PartialViewResult _CartPartipal(Cart cart)
        {
            return PartialView(cart);
        }
        public PartialViewResult _LoginForm(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return PartialView();
        }
        public RedirectToRouteResult AddToCart(Cart cart, int Tovar_ID, string returnUrl,int quantity)
        {
            
           
           Tovar tovar = rep.Tovars
                .FirstOrDefault(g => g.Tovar_ID == Tovar_ID);

            if (tovar != null)
            {
                cart.AddItem(tovar, quantity);
            }
            
            return RedirectToAction("Index", new { returnUrl });
        }

        public RedirectToRouteResult RemoveFromCart(Cart cart, int Tovar_ID,string returnUrl)
        {
           
            Tovar tovar = rep.Tovars
                .FirstOrDefault(g => g.Tovar_ID == Tovar_ID);

            if (tovar != null)
            {
                cart.RemoveLine(tovar);
            }
            return RedirectToAction("Index", new { returnUrl });
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