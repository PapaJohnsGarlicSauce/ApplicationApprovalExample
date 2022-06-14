using ApplicationApproval.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationApproval.Services.RulesEngine.Rules
{
    public class DtiRatioRule : IRule
    {
        private string Message { get; } = @"DTI: {0}";

        private double MaximumRatio { get; }

        // Want to inject minimum score so this can value change easier
        // And we can test ratio rule logic independent of what the value is
        public DtiRatioRule(double maximumRatio)
        {
            MaximumRatio = maximumRatio;
        }

        public RuleResult Execute(Application application)
        {
            // Need to pay attention to nullability of arguments
            var sharedLiabilities = application.Borrower.Liabilities?
                .Concat(application.Coborrower?.Liabilities ?? Array.Empty<Liability>())
                .Where(x => IsShared(x.Names, application));

            var liabilities = application.Borrower?.Liabilities?
                .Concat(application.Coborrower?.Liabilities ?? Array.Empty<Liability>())
                .Where(x => !IsShared(x.Names, application))
                .Append(sharedLiabilities?.FirstOrDefault())
                .Select(x => x?.MonthlyPayment)
                .Append(application.Loan.MonthlyPayment)
                .Sum();

            // The requirements didn't state if income could also be shared so this may be overkill...
            // Borrower is not null here
            var sharedIncomes = application.Borrower!.Incomes?
                .Concat(application.Coborrower?.Incomes ?? Array.Empty<Income>())
                .Where(x => IsShared(x.Names, application));

            var incomes = application.Borrower?.Incomes?
                .Concat(application.Coborrower?.Incomes ?? Array.Empty<Income>())
                .Where(x => !IsShared(x.Names, application))
                .Append(sharedIncomes?.FirstOrDefault())
                .Select(x => x?.MonthlyAmount)
                .Sum();

            // DTI may be undefined if there is no income but liabilities exist
            double? dti;
            if (incomes is null or 0)
                dti = null;
            else
            {
                dti = (double)((liabilities ?? 0) / incomes);
            }

            return new RuleResult
            {
                // Will evaluate to false if null
                IsValid = dti < MaximumRatio,
                Message = string.Format(Message, dti.HasValue ? Math.Round(dti!.Value, 3) : "Undefined")
            };
        }

        private static bool IsShared(IEnumerable<string>? names, Application application)
        {
            if (names == null) return false;
            return names.Contains(application.Borrower.Name) && names.Contains(application.Coborrower?.Name);
        }

        // Can add execution logic to determine if this should run
        // Right now, always run it
        public bool ShouldRun(Application application) => true;
    }
}
