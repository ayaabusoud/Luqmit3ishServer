using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Luqmit3ish_forMobile.Models;
using Luqmit3ishBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace Luqmit3ishBackend.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }
        public DbSet<Order> Order { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Dish> Dish { get; set; }
        public object UserRegister { get; internal set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.id);

            modelBuilder.Entity<Order>()
                .HasKey(o => new { o.user_id, o.dish_id, o.date });
            

            modelBuilder.Entity<Dish>()
                .HasKey(o => new { o.id });
        }
    }

}
