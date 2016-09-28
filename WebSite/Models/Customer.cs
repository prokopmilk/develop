using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WebSite.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

       
        public DateTime StartData { get; set; }
        public string Phone { get; set; }
        public string City { get; set; }
        public string Adres { get; set; }
        [Column(TypeName = "image")]
        public byte[] Photo { get; set; }

        // Ссылка на заказы
        public virtual List<Order> Orders { get; set; } = new List<Order>();

        public Customer()
        {
            CustomerId = 0;
            
           
        }
        public  void CreateUserAsCustomer()
        {
            ApplicationUserManager UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var user = new ApplicationUser { UserName = Email, Email = Email, PhoneNumber = Phone, name = Name, city = City, adres = Adres };
            var result =  UserManager.CreateAsync(user, Phone);
            if (result.Result.Succeeded)
            {

                if (!user.Email.Contains("nullmail.ru"))
                {

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = UserManager.GenerateEmailConfirmationToken(user.Id);

                    UrlHelper ur = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    var callbackUrl = ur.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Current.Request.Url.Scheme);
                    var siteUrl = ur.Action("Index", "Home", null, protocol: HttpContext.Current.Request.Url.Scheme);
                    EmailService.Adress = user.Email;
                    UserManager.SendEmailAsync(user.Id, "Подтверждение учетной записи", "Пожалуйста подтвердите, что это ваш электронный адрес, кторый зарегистрировали на сайте Молочная ферма. Для подтверждения перейдите  <a href=\"" + callbackUrl + "\">по ссылке</a><br> <i> Если вы не регистрировались на данном сайте, то скорее всего кто-то ошибся при вводе e-mail, тогда ничего делать не нужно.<i><br><table border='+0' cellpadding='0' cellspacing='0' width='50%' class='emailButton' style='background-color: #3498DB;'><tr><td align ='center' valign ='middle' class='buttonContent' style='padding-top:15px;padding-bottom:15px;padding-right:15px;padding-left:15px;'><a style='color:#FFFFFF;text-decoration:none;font-family:Helvetica,Arial,sans-serif;font-size:20px;line-height:135%;' href=\"" + callbackUrl + "\" target='_blank'>Подтвердить</a></td></tr></table>");
                }

            }
        }
        

    }

   
}