//This code file defines the SurfBoardAppContext class in the SurfBoardApp.Data namespace
using SurfBoardApp.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SurfBoardApp.Data
{
    // DbSet is a class in Entity Framework that represents a collection of entities from a database table or view,
    // allowing CRUD (Create, Read, Update, Delete) operations to be performed on the entities. In this code file,
    // the DbSet property is used to specify the tables for the SurfBoardAppContext class.
    // Inheriting from IdentityDbContext provides default implementations for the Identity framework
    public class SurfBoardAppContext : IdentityDbContext
    {
        // Constructor that takes DbContextOptions<SurfBoardAppContext> and passes them to the base class constructor
        public SurfBoardAppContext(DbContextOptions<SurfBoardAppContext> options)
        : base(options)
        {
        }

        // DbSet for ApplicationUsers table
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        // DbSet for Board table
        public DbSet<Board> Board { get; set; } = default!;

        // DbSet for Image table
        public DbSet<Image> Image { get; set; }

        // DbSet for Booking table
        public DbSet<Booking> Booking { get; set; }
    }
}