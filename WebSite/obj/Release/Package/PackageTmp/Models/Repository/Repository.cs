using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using WebSite.Models.Concrete;

namespace WebSite.Models.Repository
{
    public class Repository
    {

        private EFDbContext context;

        public Repository()
        {
            context = new EFDbContext(ConfigurationManager.ConnectionStrings[0].ConnectionString);
        }

        public IEnumerable<Category> Categories
        {
            get { return context.Categories; }
        }

        public IEnumerable<Tovar> Tovars
        {
            get { return context.Tovars; }
        }
        public IEnumerable<Customer> Customers
        {
            get { return context.Customers.Include(c=>c.Orders); }
        }
        public IEnumerable<Order> Orders
        {

            get {
                var t = context.Orders.Include(p => p.Customer).Include(o=>o.OrderItems);
                return t;
            }
        }
        public IEnumerable<OrderItem> OrderItems
        {
            get { return context.OrderItems.Include(t=>t.tovar).Include(o=>o.Order); }
        }
        public IEnumerable<New> News
        {
            get { return context.News; }
        }
        public IEnumerable<EmailSettings> MailSettings
        {
            get { return context.MailSettings; }
        }
        


        public void SaveTovar(Tovar tovar)
        {
            if (tovar.Tovar_ID == 0)
            {
                context.Tovars.Add(tovar);
            }
            else
            {
                Tovar bdTov = new Tovar();
                bdTov = context.Tovars.Find(tovar.Tovar_ID);
              
                Tovar dbEntry = bdTov;
                
               if (dbEntry != null)
               {
                    dbEntry.Category = tovar.Category;
                    dbEntry.Volume = tovar.Volume;
                    dbEntry.Price = tovar.Price;
                    if (tovar.HOT.Equals(null))
                    {
                        
                        dbEntry.HOT = false;
                    }else
                    {
                        dbEntry.HOT = tovar.HOT;
                    }
                    dbEntry.isSale = tovar.isSale;
                    
                    if (tovar.Pict != null)
                    {
                        dbEntry.Pict = tovar.Pict;
                    }
                    dbEntry.Dimension = tovar.Dimension;
                }
            }
            context.SaveChanges();
        }
        public void SaveCategory(Category category)
        {
            if (category.Ca_ID == 0)
                context.Categories.Add(category);
            else
            {
                Category dbEntry = context.Categories.Find(category.Ca_ID);
                if (dbEntry != null)
                {
                    
                    dbEntry.Category_Name= category.Category_Name;
                    dbEntry.Description = category.Description;
                    if (category.Pict != null)
                    {
                        dbEntry.Pict = category.Pict;
                    }
                }
            }
            context.SaveChanges();
        }
        public Tovar DeleteTovar(int Tovar_ID)
        {
            Tovar dbEntry = context.Tovars.Find(Tovar_ID);
            if (dbEntry != null)
            {
                context.Tovars.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Category DeleteCategory(int Ca_ID)
        {
            Category dbEntry = context.Categories.Find(Ca_ID);
            if (dbEntry != null)
            {
                context.Categories.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public void SaveMail(EmailSettings settings)
        {
            if (context.MailSettings.Find(settings.MailFromAddress)==null)
            {
                context.MailSettings.Add(settings);
            }else
            {
                EmailSettings dbEntry = context.MailSettings.Find(settings.MailFromAddress);
                if (dbEntry != null)
                {
                    dbEntry.MailFromAddress = settings.MailFromAddress;
                    dbEntry.Password = settings.Password;
                    dbEntry.Username = settings.Username;
                    dbEntry.UseSsl = settings.UseSsl;
                    dbEntry.WriteAsFile = settings.WriteAsFile;
                    dbEntry.ServerName = settings.ServerName;
                    dbEntry.ServerPort = settings.ServerPort;
                    dbEntry.FileLocation = settings.FileLocation;
                    dbEntry.MailToAddress = "";
                    
                }
            }
            context.SaveChanges();
        }
        public EmailSettings DeleteMailSettings(string MailFromAddress)
        {
            EmailSettings dbEntry = context.MailSettings.Find(MailFromAddress);
            if (dbEntry != null)
            {
                context.MailSettings.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public OrderItem DeleteOrderItem(int OrderItemId)
        {
            OrderItem dbEntry = context.OrderItems.Find(OrderItemId);
            if (dbEntry != null)
            {
                context.OrderItems.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public void SaveOrderItem(OrderItem orderitem)
        { 
            if (orderitem.OrderItemId == 0)
                 context.OrderItems.Add(orderitem);
            else
            {
                OrderItem dbEntry = context.OrderItems.Find(orderitem.OrderItemId);
                if (dbEntry != null)
                {
                    dbEntry.Order = orderitem.Order;
                    dbEntry.Tovar_ID = orderitem.Tovar_ID;
                    dbEntry.Quantity = orderitem.Quantity;
                   // dbEntry.tovar = orderitem.tovar;

                }
            }
            context.SaveChanges();
        }
        public void SaveOrder(Order orderitem)
        {
            if (orderitem.OrderId == 0)
                context.Orders.Add(orderitem);
            else
            {
                Order dbEntry = context.Orders.Find(orderitem.OrderId);
                if (dbEntry != null)
                {
                    dbEntry.Customer = orderitem.Customer;
                    dbEntry.PurchaseDate= orderitem.PurchaseDate;
                    dbEntry.StatusOrder = orderitem.StatusOrder;
                    dbEntry.TotalSum = orderitem.TotalSum;
                    dbEntry.OrderItems = orderitem.OrderItems;
                    dbEntry.SolveOrder = orderitem.SolveOrder;

                }
            }
            context.SaveChanges();
        }
        public Order DeleteOrder(int OrderId)
        {
            Order dbEntry = context.Orders.Find(OrderId);
            if (dbEntry != null)
            {
                context.Orders.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public Customer DeleteClient(int CustomerId)
        {
           Customer dbEntry = context.Customers.Find(CustomerId);
            if (dbEntry != null)
            {
                context.Customers.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
        public void SaveClient(Customer client)
        {
            if (client.CustomerId == 0)
            {
                client.StartData = System.DateTime.Now;
                context.Customers.Add(client);
            }
            else
            {
                Customer dbEntry = context.Customers.Find(client.CustomerId);
                if (dbEntry != null)
                {
                    dbEntry.Name = client.Name;
                    dbEntry.Email = client.Email;
                    dbEntry.City = client.City;
                    dbEntry.Phone = client.Phone;
                    dbEntry.Adres = client.Adres;
                    dbEntry.Photo = client.Photo;
                    dbEntry.Orders = client.Orders;
                }
            }
            context.SaveChanges();
        }

        public void SaveNews(New news)
        {
            if (news.id == 0)
                context.News.Add(news);
            else
            {
                New dbEntry = context.News.Find(news.id);
                if (dbEntry != null)
                {
                    dbEntry.Header = news.Header;
                    dbEntry.Description = news.Description;
                    dbEntry.Text = news.Text;
                    dbEntry.DataPub = news.DataPub;
                    if (news.Picture != null)
                    {
                        dbEntry.Picture = news.Picture;
                    }
                }
            }
            context.SaveChanges();
        }
        public New DeleteNews(int id)
        {
            New dbEntry = context.News.Find(id);
            if (dbEntry != null)
            {
                context.News.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

    }
}