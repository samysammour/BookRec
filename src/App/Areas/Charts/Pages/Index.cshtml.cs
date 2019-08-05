namespace BookRec.App.Areas.Charts.Pages
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BookRec.App.Model;
    using BookRec.Recommender;
    using EnsureThat;
    using Infrastructure.EntityFramework.Repositories;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IContentBasedRecommender contentBasedRecommender;
        private readonly ICollaborativeRecommender collaborativeRecommender;
        private readonly IHybridRecommender hybridRecommender;

        private readonly IUserBookRepository userBookRepository;

        public IndexModel(IContentBasedRecommender contentBasedRecommender, ICollaborativeRecommender collaborativeRecommender, IHybridRecommender hybridRecommender, IUserBookRepository userBookRepository)
        {
            EnsureArg.IsNotNull(contentBasedRecommender);
            EnsureArg.IsNotNull(collaborativeRecommender);
            EnsureArg.IsNotNull(hybridRecommender);
            EnsureArg.IsNotNull(userBookRepository);

            this.contentBasedRecommender = contentBasedRecommender;
            this.collaborativeRecommender = collaborativeRecommender;
            this.hybridRecommender = hybridRecommender;
            this.userBookRepository = userBookRepository;
        }

        public List<ChartModel> Charts { get; set; } = new List<ChartModel>();

        public string[] Labels { get; set; } = new string[] { };

        public double[] Values { get; set; } = new double[] { };

        public async Task OnGet()
        {
            var inputs = await this.userBookRepository.GetByUsernameAsync(this.User.Identity.Name).ConfigureAwait(false);
            var prediction = await this.contentBasedRecommender.GetPredicationsByBooksAsync(inputs.Select(x => x.Book).ToList()).ConfigureAwait(false);
            if (prediction != null)
            {
                this.Charts.Add(new ChartModel() { Label = "CBF", Value = prediction.Sum(x => x.Score) / prediction.Count() });
            }

            inputs = await this.userBookRepository.GetByUsernameAsync(this.User.Identity.Name).ConfigureAwait(false);
            prediction = await this.collaborativeRecommender.GetPredicationsByBooksAsync(inputs, this.User.Identity.Name).ConfigureAwait(false);
            if (prediction != null)
            {
                this.Charts.Add(new ChartModel() { Label = "CF", Value = prediction.Sum(x => x.Score) / prediction.Count() });
            }

            inputs = await this.userBookRepository.GetByUsernameAsync(this.User.Identity.Name).ConfigureAwait(false);
            prediction = await this.hybridRecommender.GetPredicationsByBooksAsync(inputs, this.User.Identity.Name).ConfigureAwait(false);
            if (prediction != null)
            {
                this.Charts.Add(new ChartModel() { Label = "HF", Value = prediction.Sum(x => x.Score) / prediction.Count() });
            }
        }
    }
}