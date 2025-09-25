using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoListApp
{

    public class TodoItem
    {
        public int Id { get; set; }
        public string Task { get; set; }
        public string DueDate { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public string? CompleteDate { get; set; }

    }

    public class TodoContext : DbContext
    {
        public DbSet<TodoItem> Todo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=todo.db");
    }
}

