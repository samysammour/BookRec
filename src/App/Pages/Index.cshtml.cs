namespace BookRec.App.Pages
{
    using System.Collections.Generic;
    using System.Linq;
    using BookRec.Infrastructure.EntityFramework.Models;
    using BookRec.Infrastructure.EntityFramework.Repositories;
    using EnsureThat;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class IndexModel : PageModel
    {
        private readonly IBookRepository repository;

        public IndexModel(IBookRepository repository)
        {
            EnsureArg.IsNotNull(repository);
            this.repository = repository;
        }

        public IEnumerable<Book> Books { get; set; }

        public void OnGet() => this.Books = Enumerable.Empty<Book>(); // this.repository.GetAllAsync().Result;
    }
}
