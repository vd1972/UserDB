using Microsoft.EntityFrameworkCore;
using userdb.Models;

namespace userdb.Models
{
    public class DBcontext : DbContext
    {
        public DBcontext(DbContextOptions option) : base (option) {}

         public DbSet<User> users {get; set;}
         public DbSet<Message> messages {get; set;}
         public DbSet<Comment> comments {get; set;}
    }
}