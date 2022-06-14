using ApplicationApproval.Models;
using System;

namespace ApplicationApproval.Services.RulesEngine.Rules
{
    public class CreditScoreRule : IRule
    {
        private string Message { get; } = @"Credit Score: {0}";

        private int MinimumScore { get; }

        // Want to inject minimum score so this can value change easier
        // And we can test credit score rule logic independent of what the score is
        public CreditScoreRule(int minimumScore)
        {
            MinimumScore = minimumScore;
        }

        public RuleResult Execute(Application application)
        {
            // Why would it be the lower of the two scores? Why even have a coborrower at all in that case...?
            var scoreToEvaluate = application.Borrower.CreditScore;
            if (application.Coborrower != null)
            {
                scoreToEvaluate = Math.Min(scoreToEvaluate, application.Coborrower.CreditScore);
            }
            return new RuleResult
            {
                IsValid = scoreToEvaluate > MinimumScore,
                Message = string.Format(Message, scoreToEvaluate)
            };
        }

        // Can add execution logic to determine if this should run
        // Right now, always run it
        public bool ShouldRun(Application application) => true;
    }
}
