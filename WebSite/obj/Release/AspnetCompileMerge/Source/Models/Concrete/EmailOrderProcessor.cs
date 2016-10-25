using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Web;
using WebSite.Abstract;
using WebSite.Controllers;
using WebSite.Models.Repository;

namespace WebSite.Models.Concrete
{
    public class EmailSettings
    {
        
        public string MailToAddress { get; set; }//"maxim.vasin95@gmail.com";
        [Key]
        public string MailFromAddress { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ServerName { get; set; }//= "smtp.mail.ru";
        public int ServerPort { get; set; } //25
        public bool WriteAsFile { get; set; }
        public string FileLocation { get; set; }
        public string Description { get; set; }
    }

    public class EmailOrderProcessor : IOrderProcessor
    {
        private EmailSettings emailSettings;

        public EmailOrderProcessor(EmailSettings settings)
        {
            emailSettings = settings;
        }
        public void TestSend(Cart cart, ShippingDetails shippingInfo, string adres, string sub)
        {
            /*SmtpClient client = new SmtpClient("smtp.mail.ru", 25);
            string To_send = "maxim.vasin95@gmail.com";
            string Subject_send = "Новый заказ";
            string Body_send = String.Format("Письмо письменное...");
            string log1_send = "maxim.vasin@mail.ru";
            // вкл Ssl
            client.EnableSsl = true;
            // обрабатываем исходящее сообщение
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            // отключаем запросы
            client.UseDefaultCredentials = false;
            // проверка отправителя
            client.Credentials = new NetworkCredential(log1_send, "adrenalin");
            // отправление

            try
            {
                client.Send(log1_send, To_send, Subject_send, Body_send);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }*/
            textmsg = "";
            using (var smtpClient = new SmtpClient(emailSettings.ServerName, emailSettings.ServerPort))
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);


                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod
                        = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = emailSettings.UseSsl;
                }
                textmsg = "<html><head><meta http-equiv='Content-Type' Content='text/html; charset=utf-8'> Получен новый заказ от <b>" + shippingInfo.Name + " (" + shippingInfo.Phone + ")</b>";
                textmsg += "<br>"+shippingInfo.Mail+"<br><table border='2'><thead><tr><th>Товар</th> <th>Масса [ л(кг) ]</th><th>Кол-во</th><th>Общая цена</th></tr></thead><tbody>";
                foreach (var line in cart.Lines)
                {
                    textmsg += "<tr><td>" + line.tovar.Category + "</td>";
                    textmsg += "<td align='center'>" + line.tovar.Volume + "</td>";
                    textmsg += "<td align='center'>" + line.Quantity.ToString() + "</td>";
                    textmsg += "<td>" + (line.Quantity * line.tovar.Price).ToString("# руб") + "</td>" + "</tr>";
                }

                textmsg += "<td colspan='3' align='right'><b>ИТОГО:</b></td><td><b>" + cart.ComputeTotalValue() + " руб<b/></td>";
                textmsg += "</tbody>" + "</table><hr>";
                StringBuilder dostavka = new StringBuilder();
                dostavka.AppendFormat("Доставка:{0}", shippingInfo.SelfDeliv ? "Самовывоз" : "по адресу\n г." + shippingInfo.City + " " + shippingInfo.Line1);
                textmsg += dostavka.ToString();
                textmsg += "</body>" + "</html>";
                var mModel = new MailModel()
                {
                    text = textmsg,
                    News=new Repository.Repository().News
                };
               
                var mailController = new MailController();

                var email = mailController.Subscription(mModel, adres, sub);
               
                MailMessage mailMessage = email.Mail;
                mailMessage.To.Add(adres);
                mailMessage.From = new MailAddress(emailSettings.MailFromAddress);
                mailMessage.IsBodyHtml = true;
                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.UTF8;
                }

                smtpClient.Send(mailMessage);

            }

        }
        public void SendAdmin(Cart cart, ShippingDetails shippingInfo, string adres, string sub)
        {
            
            textmsg = "";
            using (var smtpClient = new SmtpClient(emailSettings.ServerName, emailSettings.ServerPort))
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);


                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod
                        = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }
                textmsg = "<html><head><meta http-equiv='Content-Type' Content='text/html; charset=utf-8'> Получен новый заказ от <b>" + shippingInfo.Name + " (" + shippingInfo.Phone + ")</b>";
                textmsg += "<br>" + shippingInfo.Mail + "<br><table border='2'><thead><tr><th>Товар</th> <th>Масса [ л(кг) ]</th><th>Кол-во</th><th>Общая цена</th></tr></thead><tbody>";
                foreach (var line in cart.Lines)
                {
                    textmsg += "<tr><td>" + line.tovar.Category + "</td>";
                    textmsg += "<td align='center'>" + line.tovar.Volume + "</td>";
                    textmsg += "<td align='center'>" + line.Quantity.ToString() + "</td>";
                    textmsg += "<td>" + (line.Quantity * line.tovar.Price).ToString("# руб") + "</td>" + "</tr>";
                }

                textmsg += "<td colspan='3' align='right'><b>ИТОГО:</b></td><td><b>" + cart.ComputeTotalValue() + " руб<b/></td>";
                textmsg += "</tbody>" + "</table><hr>";
                StringBuilder dostavka = new StringBuilder();
                dostavka.AppendFormat("Доставка:{0}", shippingInfo.SelfDeliv ? "Самовывоз" : "по адресу\n г." + shippingInfo.City + " " + shippingInfo.Line1);
                textmsg += dostavka.ToString();
                textmsg += "</body>" + "</html>";
                var mModel = new MailModel()
                {
                    text = textmsg,
                    News = new Repository.Repository().News
                };

                var mailController = new MailController();

                var email = mailController.AdminMail(mModel, adres, sub);

                MailMessage mailMessage = email.Mail;
                mailMessage.To.Add(adres);
                mailMessage.From = new MailAddress(emailSettings.MailFromAddress);
                mailMessage.IsBodyHtml = true;
                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.UTF8;
                }

                smtpClient.Send(mailMessage);

            }

        }
        public void formMail(string fromAddress, string toAddress)
        {
            var mailHtml = "<html><head><meta http-equiv='Content-Type' Content='text/html; charset=utf-8'></head><body>тело письма</body></html>";
            var message = new MailMessage(fromAddress, toAddress)
            {
                SubjectEncoding = Encoding.UTF8,
                Subject = "Новый заказ",
                Body = "Принят новый заказ\n Товары:\n\tТовар1\n\tТовар2\nКлиент:имя фамилия\n Сумма заказ:0000руб.",
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };
            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(mailHtml, Encoding.UTF8, MediaTypeNames.Text.Html));

        }
        public string textmsg { get; set; }
        public void ProcessOrder(Cart cart, ShippingDetails shippingInfo)
        {
            textmsg = "";
            using (var smtpClient = new SmtpClient(emailSettings.ServerName, emailSettings.ServerPort))
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials=new NetworkCredential(emailSettings.Username, emailSettings.Password);


                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod
                        = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

               
  
      

          

                       textmsg = "<html><head><meta http-equiv='Content-Type' Content='text/html; charset=utf-8'> Получен новый заказ от <b>" + shippingInfo.Name +" ("+shippingInfo.Phone+")</b>";
                textmsg += "<br><table border='2'><thead><tr><th>Товар</th> <th>Масса [ л(кг) ]</th><th>Кол-во</th><th>Общая цена</th></tr></thead><tbody>";
               
                foreach (var line in cart.Lines)
                {
                    textmsg +=  "<tr><td>" + line.tovar.Category + "</td>";
                    textmsg += "<td>" + line.tovar.Volume + "</td>";
                    textmsg +=  "<td>" + line.Quantity.ToString() + "</td>";
                    textmsg +=  "<td>" + (line.Quantity * line.tovar.Price).ToString("# руб") + "</td>" + "</tr>";
                }

                textmsg += "<td colspan='3' align='right'><b>ИТОГО:</b></td><td><b>"+cart.ComputeTotalValue()+" руб<b/></td>";
               textmsg += "</tbody>" + "</table><hr>";
                StringBuilder dostavka = new StringBuilder();
                dostavka.AppendFormat("Доставка:{0}", shippingInfo.SelfDeliv ? "Самовывоз" : "по адресу\n г."+ shippingInfo.City+" " +shippingInfo.Line1);
                textmsg+= dostavka.ToString();
                textmsg += "</body>" + "</html>";
                
                MailMessage mailMessage = new MailMessage(
                                       emailSettings.MailFromAddress,	// От кого
                                       emailSettings.MailToAddress,		// Кому
                                       "Офрмлен новый заказ!",		    // Тема
                                       textmsg); 				// Тело письма
                mailMessage.IsBodyHtml = true;
                if (emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.UTF8;
                }
               
                smtpClient.Send(mailMessage);
               
            }
        }


       
        }
    }