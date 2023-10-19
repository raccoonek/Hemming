using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.EntityFrameworkCore.SqlServer;

using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;




namespace Hemming
{
    public class Statistic1
    {
        [Key]
        public int ID { get; set; }
        //входная матрица
        public string A { get; set; }
        //вектор ошибки
        public string E { get; set; }
        //выход после декодера
        public string Y { get; set; }
        //синдром
        public string S { get; set; }

        //была ли ошибка
        public Boolean HasError { get; set; }
        //обнаружена ли ошибка
        public Boolean DiscoveredError { get; set; }
        //исправлена ли ошибка
        public Boolean FixedError { get; set; }
        public int Coder { get; set; }
        public int Kanal { get; set; }
    }
    

    public class ApplicationContext : DbContext
    {

        public DbSet<Statistic1> Statistic1 { get; set; }
        public Statistic1 Statistics1
        {
            get => default;
            set { }
        }

        public ApplicationContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            // установка пути к текущему каталогу
            //MessageBox.Show(Directory.GetCurrentDirectory());
            builder.SetBasePath(System.IO.Directory.GetCurrentDirectory());
            // получаем конфигурацию из файла appsettings.json
            builder.AddJsonFile("jsonconfigconnection.json");
            // создаем конфигурацию
            var config = builder.Build();
            // получаем строку подключения

            string connectionString = config.GetConnectionString("DefaultConnection");

            // optionsBuilder.UseSqlServer("Server=DESKTOP-PS969HM; Database=well; Trusted_Connection=True; ");
            optionsBuilder.UseSqlServer(connectionString);

        }
    }

}