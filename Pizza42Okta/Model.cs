using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace Pizza42Okta
{
    public class PizzaContext : DbContext
    {
        public DbSet<Models.Order> Orders { get; set; }
        public DbSet<Models.Type> Types { get; set; }

        public string DbPath { get; }

        public PizzaContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "pizza42.db");
        }

        // The following configures EF to create a Sqlite database file in the
        // special "local" folder for your platform.
        //protected override void OnConfiguring(DbContextOptionsBuilder options)
        //    => options.UseSqlite($"Data Source={DbPath}");
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Data Source=tcp:pizza42oktadbserver.database.windows.net,1433;Initial Catalog=Pizza42Okta_db;User Id=Pizza42Admin@pizza42oktadbserver;Password=OktaPizzaFreddy11!;");
        }
    }
}