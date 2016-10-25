using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using WebSite.Models;
using System.Net.Mail;
using System.Net;
using System.Text;
using WebSite.Models.Repository;
using WebSite.Models.Concrete;
using WebSite.Controllers;

namespace WebSite
{
    public class EmailService : IIdentityMessageService
    {
        EmailSettings mailsettings;
        
        
       
       
       
        public static string Adress { get;  set; }

        
        public Task SendAsync(IdentityMessage message)
        {
             mailsettings = new Repository().MailSettings.FirstOrDefault(x => x.Description.Equals("Интернет-магазин"));
            using (var smtpClient = new SmtpClient(mailsettings.ServerName, mailsettings.ServerPort))
            {


               
                smtpClient.EnableSsl = mailsettings.UseSsl;
                    
                    smtpClient.UseDefaultCredentials = false;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials
                        = new NetworkCredential(mailsettings.Username, mailsettings.Password);

                    if (mailsettings.WriteAsFile)
                    {
                        smtpClient.DeliveryMethod
                            = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        smtpClient.PickupDirectoryLocation = mailsettings.FileLocation;
                        smtpClient.EnableSsl = mailsettings.UseSsl;
                    }

                var mailController = new MailController();
 var mModel = new MailModel()
                {
                    text = message.Body,
                    News = new Repository().News
                };
                var email = mailController.Account(mModel, Adress, message.Subject);

                MailMessage mailMessage = email.Mail;

                mailMessage.To.Add(Adress);
                mailMessage.From = new MailAddress(mailsettings.MailFromAddress);
                mailMessage.IsBodyHtml = true;
                    if (mailsettings.WriteAsFile)
                    {
                        mailMessage.BodyEncoding = Encoding.UTF8;
                    }

                    smtpClient.Send(mailMessage);
                }
            
        
            return Task.FromResult(0);
        }
    }
   /* public class MailSettings
    {
        public string MailToAddress = "";
        public string MailFromAddress = "prokopmilk@mail.ru";
        public bool UseSsl = true;
        public string Username = "prokopmilk@mail.ru";
        public string Password = "sale89049680743";
        public string ServerName = "smtp.mail.ru";
        public int ServerPort = 25;
        public bool WriteAsFile = false;
        public string FileLocation = @"c:\store_emails";
    }*/

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }

    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context) 
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                
               
            };

            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug it in here.
            manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Ваш код безопасности на сайте молочная ферма - {0}"
            });
            manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Подтверждение входа",
                BodyFormat = "Ваш код безопасности {0}"
            });
            
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = 
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}
