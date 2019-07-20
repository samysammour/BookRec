namespace BookRec.App.Pages
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BookRec.Infrastructure.EntityFramework.Models;
    using BookRec.Infrastructure.EntityFramework.Repositories;
    using BookRec.Recommender;
    using Common;
    using EnsureThat;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class IndexModel : PageModel
    {
        private readonly IBookRepository repository;
        private readonly IContentBasedRecommender contentBasedRecommender;

        public IndexModel(IBookRepository repository, IContentBasedRecommender contentBasedRecommender)
        {
            EnsureArg.IsNotNull(repository);
            EnsureArg.IsNotNull(contentBasedRecommender);

            this.repository = repository;
            this.contentBasedRecommender = contentBasedRecommender;
        }

        public IEnumerable<Book> Books { get; set; }

        public async Task OnGet()
        {
            var ids = new string[]
            {
                "C2A48DF5-EF5B-4752-8352-00326A1B60AC",
                "62B28E90-4460-4BD3-8781-0038C42CB528",
                "7B378BF8-9A99-4A97-91B1-003F708A0B32"
            };
            var input = await this.repository.GetByIdAsync("C2A48DF5-EF5B-4752-8352-00326A1B60AC".ToGuid().Value).ConfigureAwait(false);
            this.Books = await this.contentBasedRecommender.GetPredicationAsync(input).ConfigureAwait(false);
        }
    }
}
