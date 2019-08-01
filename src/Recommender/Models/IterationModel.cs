namespace BookRec.Recommender.Models
{
    using BookRec.Infrastructure.EntityFramework.Models;

    public class IterationModel
    {
        public Book Book { get; set; }

        public int Iteration { get; set; }

        public int Rating { get; set; }
    }
}
