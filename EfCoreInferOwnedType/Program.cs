using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfCoreInferOwnedType
{
    class Program
    {
        static void Main(string[] args)
        {
            var c = new Context();
            _ = c.Model;
            _ = Console.ReadLine();
        }
    }

    public class Order
    {
        public int Id { get; set; }
        public List<OrderLine> Lines { get; set; }
    }

    public class OrderLine
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public Price Price { get; set; }
    }

    public class Price
    {
        public Price(int amountInCent) => AmountInCent = amountInCent;
        public int AmountInCent { get; private set; }
        [NotMapped]
        public decimal Amount => AmountInCent /100m;
    }

    public class Context : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer("whatever");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Owned<Price>();
            modelBuilder.Entity<Order>();
            modelBuilder.Entity<OrderLine>(builder =>
            {
                builder.Property(o => o.Price).HasColumnName("PriceInCent").HasConversion(p => p.AmountInCent, i => new Price(i));
            });

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                Console.WriteLine($"Name: {entity.Name,-30} Owned: {entity.IsOwned(),-5} Keyless: {entity.IsKeyless}");
            }
        }
    }
}
