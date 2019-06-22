namespace BookRec.Infrastructure.EntityFramework.Context
{
    using Models;
    using Microsoft.EntityFrameworkCore;

    public static partial class Extensions
    {
        public static ModelBuilder BuildBookAggregateConfiguration(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd().IsRequired();
            });

            return modelBuilder;
        }
    }
}
