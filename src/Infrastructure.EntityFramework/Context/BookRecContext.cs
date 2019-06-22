namespace BookRec.Infrastructure.EntityFramework.Context
{
    using BookRec.Infrastructure.EntityFramework.Models;
    using Microsoft.EntityFrameworkCore;

    public partial class BookRecContext : DbContext
    {
        private readonly string schema = "BookRec";

        public BookRecContext()
        {
        }

        public BookRecContext(DbContextOptions<BookRecContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(this.schema);

            // Model Configuration
            modelBuilder
                .BuildBookAggregateConfiguration();

            // Seeds
            //modelBuilder
            //    .BookAggregateSeeds();
        }
    }
}
