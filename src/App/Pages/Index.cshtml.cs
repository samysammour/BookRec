namespace BookRec.App.Pages
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using EnsureThat;
    using Infrastructure.EntityFramework.Models;
    using Infrastructure.EntityFramework.Repositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Recommender;

    public class IndexModel : PageModel
    {
        private readonly IUserBookRepository userBookRepository;
        private readonly IContentBasedRecommender contentBasedRecommender;

        public IndexModel(IUserBookRepository userBookRepository, IContentBasedRecommender contentBasedRecommender)
        {
            EnsureArg.IsNotNull(userBookRepository);
            EnsureArg.IsNotNull(contentBasedRecommender);

            this.userBookRepository = userBookRepository;
            this.contentBasedRecommender = contentBasedRecommender;
        }

        public List<PredicationModel> Predications { get; set; }

        public void OnGet()
        {
        }

        public async Task OnPostFindRecommendationAsync()
        {
            var inputs = await this.userBookRepository.GetByUsernameAsync(this.User.Identity.Name).ConfigureAwait(false);
            this.Predications = await this.contentBasedRecommender.GetPredicationsByBooksAsync(inputs.Select(x => x.Book).ToList()).ConfigureAwait(false);
        }
    }
}
