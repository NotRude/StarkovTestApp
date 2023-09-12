using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.EntityFrameworkCore;
using StarkovTestApp.Models;
using System.Configuration;
using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace StarkovTestApp
{
    internal class DataContext : DbContext
    {
        public DbSet< Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DataContext()
        {
            try
            {
                Database.EnsureCreated();
            }
            catch
            {
                Console.WriteLine("Неверно введён доступ к базе данных в dbsettings.txt");
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string text;
            using (StreamReader reader = new StreamReader(@"./dbsettings.txt"))
            {
                text = reader.ReadToEnd();
            }

            optionsBuilder.UseNpgsql(text);
        }
    }
}
