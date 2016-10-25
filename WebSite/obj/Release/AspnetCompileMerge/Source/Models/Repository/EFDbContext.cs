using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using WebSite.Models.Concrete;

namespace WebSite.Models.Repository
{
    public class EFDbContext: DbContext
    {
        public EFDbContext(string ConnString)
        {
            Database.Connection.ConnectionString = ConnString;
        }
        public EFDbContext()
        {
           
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tovar> Tovars { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<New> News { get; set; }
        public DbSet<EmailSettings> MailSettings { get; set; }
    }
    
}