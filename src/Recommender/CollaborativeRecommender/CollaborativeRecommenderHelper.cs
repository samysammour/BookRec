namespace BookRec.Recommender
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using BookRec.Infrastructure.EntityFramework.Models;
    using BookRec.Recommender.Models;
    using EnsureThat;

    public class CollaborativeRecommenderHelper
    {
        public CollaborativeRecommenderHelper(List<UserBook> inputs)
        {
            EnsureArg.IsNotNull(inputs);

            this.CalculateNoi(inputs);
            this.CalculateTTM(inputs);
            this.SetIds(inputs);
        }

        /// <summary>
        /// Number of Inputs
        /// </summary>
        public int NOI { get; set; }

        /// <summary>
        /// Total Temperature
        /// </summary>
        public int TTM { get; set; }

        /// <summary>
        /// Input ids
        /// </summary>
        public List<Guid> Ids { get; set; }

        /// <summary>
        /// Caclculate User Temperature (UTM)
        /// </summary>
        /// <param name="userGroup">All books read by the user</param>
        /// <returns>UTM</returns>
        public double CalculateUTM(IGrouping<string, UserBook> userGroup)
            => (double)userGroup.Where(x => this.Ids.Contains(x.BookId)).Sum(x => x.Rating * this.NOI) / this.TTM;

        /// <summary>
        /// Calculate Number of Similiraty between the user and the input set (NOS)
        /// </summary>
        /// <param name="userGroup">All books read by the user</param>
        /// <returns>NOS</returns>
        public int CalculateNOS(IGrouping<string, UserBook> userGroup)
            => userGroup.Where(ub => this.Ids.Contains(ub.BookId)).Count();

        /// <summary>
        /// Calculate the output weight of a prediction (OPW)
        /// </summary>
        /// <param name="temperature">Reader temperature</param>
        /// <param name="rating">Reader rating</param>
        /// <returns>OPW</returns>
        public double CalculateOPW(double temperature, int rating)
            => temperature * rating / 5;

        /// <summary>
        /// Calculate the number of inputs (NOI)
        /// </summary>
        /// <param name="inputs">Input set</param>
        private void CalculateNoi(List<UserBook> inputs)
            => this.NOI = inputs.Count();

        /// <summary>
        /// Calculate the total temperature (TTM)
        /// </summary>
        /// <param name="inputs">Input set</param>
        private void CalculateTTM(List<UserBook> inputs)
            => this.TTM = inputs.Sum(x => x.Rating * inputs.Count());

        /// <summary>
        /// Set input ids
        /// </summary>
        /// <param name="inputs">Input set</param>
        private void SetIds(List<UserBook> inputs)
            => this.Ids = inputs.Select(x => x.BookId).Distinct().ToList();
    }
}
