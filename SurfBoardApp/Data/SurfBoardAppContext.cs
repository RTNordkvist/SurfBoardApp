using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SurfBoardApp.Models;

namespace SurfBoardApp.Data
{
    public class SurfBoardAppContext : IdentityDbContext
    {

        public SurfBoardAppContext (DbContextOptions<SurfBoardAppContext> options)
            : base(options)
        {
        }

        public DbSet<Board> Board { get; set; } = default!;
        public DbSet<Image> Image { get; set; }
        public DbSet<Booking> Booking { get; set; }

    }
}
