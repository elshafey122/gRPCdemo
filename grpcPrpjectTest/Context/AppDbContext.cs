using grpcPrpjectTest.Models;
using Microsoft.EntityFrameworkCore;

namespace grpcPrpjectTest.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {

        }
        public DbSet<TodoItem> todoItems { get; set; }
    }
}
