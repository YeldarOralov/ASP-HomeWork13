using ASPHW13.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPHW13.DataAccess
{
    public class UserContext:DbContext
    {
        public UserContext(DbContextOptions options):base(options)
        {
            Database.Migrate();
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasData(new User {
                Login="admin",
                Password="123456"
            });
        }
    }
}
