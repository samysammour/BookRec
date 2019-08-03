namespace BookRec.App.Pages
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using EnsureThat;
    using Infrastructure.EntityFramework.Models;
    using Infrastructure.EntityFramework.Repositories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Recommender;

    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IUserBookRepository userBookRepository;
        private readonly IContentBasedRecommender contentBasedRecommender;
        private readonly ICollaborativeRecommender collaborativeRecommender;

        public IndexModel(IUserBookRepository userBookRepository, IContentBasedRecommender contentBasedRecommender, ICollaborativeRecommender collaborativeRecommender)
        {
            EnsureArg.IsNotNull(userBookRepository);
            EnsureArg.IsNotNull(contentBasedRecommender);
            EnsureArg.IsNotNull(collaborativeRecommender);

            this.userBookRepository = userBookRepository;
            this.contentBasedRecommender = contentBasedRecommender;
            this.collaborativeRecommender = collaborativeRecommender;
        }

        public List<PredicationModel> Predications { get; set; }

        public void OnGet()
        {
        }

        public async Task OnPostFindCBFRecommendationAsync()
        {
            var inputs = await this.userBookRepository.GetByUsernameAsync(this.User.Identity.Name).ConfigureAwait(false);
            this.Predications = await this.contentBasedRecommender.GetPredicationsByBooksAsync(inputs.Select(x => x.Book).ToList()).ConfigureAwait(false);
        }

        public async Task OnPostFindCFRecommendationAsync()
        {
            var inputs = await this.userBookRepository.GetByUsernameAsync(this.User.Identity.Name).ConfigureAwait(false);
            this.Predications = await this.collaborativeRecommender.GetPredicationsByBooksAsync(inputs.ToList(), this.User.Identity.Name).ConfigureAwait(false);
        }
    }
}
