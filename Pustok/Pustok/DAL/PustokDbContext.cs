using Microsoft.EntityFrameworkCore;
using Pustok.Models;

namespace Pustok.DAL
{
    public class PustokDbContext:DbContext
    {
        public PustokDbContext(DbContextOptions<PustokDbContext> options):base(options) { }

        public DbSet<Slider> Sliders { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BookTag> BookTags { get; set; }
        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<Setting> Settings   { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BookTag>().HasKey(x => new { x.TagId, x.BookId });
            base.OnModelCreating(modelBuilder);
        }

    }
}
