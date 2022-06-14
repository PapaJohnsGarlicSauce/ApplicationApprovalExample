using ApplicationApproval.Models;
using ApplicationApproval.Services.RulesEngine.Rules;
using AutoFixture;
using AutoFixture.NUnit3;
using NUnit.Framework;
using Shouldly;
using System.Linq;

namespace ApplicationApprovalTest.Services.RulesEngine.Rules
{
    public class CreditScoreRuleTest
    {
        [Theory, AutoData]
        public void Execute_Approved_WithBorrowerOnly_AboveLimit(
            Application application,
            Generator<int> scoreGenerator)
        {
            var minScore = scoreGenerator.First();
            var rule = new CreditScoreRule(minScore);
            application.Coborrower = null;
            application.Borrower.CreditScore = scoreGenerator.First(i => i > minScore);

            var result = rule.Execute(application);

            result.IsValid.ShouldBeTrue();
        }

        [Theory, AutoData]
        public void Execute_Denied_WithBorrowerOnly_BelowLimit(
            Application application,
            Generator<int> scoreGenerator)
        {
            var minScore = scoreGenerator.First();
            var rule = new CreditScoreRule(minScore);
            application.Coborrower = null;
            application.Borrower.CreditScore = scoreGenerator.First(i => i < minScore);

            var result = rule.Execute(application);

            result.IsValid.ShouldBeFalse();
        }

        [Theory, AutoData]
        public void Execute_Approved_WithBorrowerAndCoborrower_BothAboveLimit(
            Application application,
            Generator<int> scoreGenerator)
        {
            var minScore = scoreGenerator.First();
            var rule = new CreditScoreRule(minScore);
            // Autofixture guarantees coborrower will not be null
            // I would probably add customization to FULLY ensure this if I had time
            application.Coborrower!.CreditScore = scoreGenerator.First(i => i > minScore);
            application.Borrower.CreditScore = scoreGenerator.First(i => i > minScore
                                                    && i != application.Coborrower.CreditScore);

            var result = rule.Execute(application);

            result.IsValid.ShouldBeTrue();
        }

        [Theory, AutoData]
        public void Execute_Denied_WithBorrowerAndCoborrower_BothBelowLimit(
            Application application,
            Generator<int> scoreGenerator)
        {
            var minScore = scoreGenerator.First();
            var rule = new CreditScoreRule(minScore);
            application.Coborrower!.CreditScore = scoreGenerator.First(i => i < minScore);
            application.Borrower.CreditScore = scoreGenerator.First(i => i < minScore
                                                    && i != application.Coborrower.CreditScore);

            var result = rule.Execute(application);

            result.IsValid.ShouldBeFalse();
        }

        [Theory, AutoData]
        // Again, this business rule doesn't make sense to me?
        public void Execute_Denied_WithBorrowerAndCoborrower_CoborrowerAboveLimitBorrowerBelow(
            Application application,
            Generator<int> scoreGenerator)
        {
            var minScore = scoreGenerator.First();
            var rule = new CreditScoreRule(minScore);
            application.Coborrower!.CreditScore = scoreGenerator.First(i => i > minScore);
            application.Borrower.CreditScore = scoreGenerator.First(i => i < minScore);

            var result = rule.Execute(application);

            result.IsValid.ShouldBeFalse();
        }

        [Theory, AutoData]
        public void Execute_Denied_WithBorrowerAndCoborrower_BorrowerAboveLimitCoborrowerBelow(
            Application application,
            Generator<int> scoreGenerator)
        {
            var minScore = scoreGenerator.First();
            var rule = new CreditScoreRule(minScore);
            application.Coborrower!.CreditScore = scoreGenerator.First(i => i < minScore);
            application.Borrower.CreditScore = scoreGenerator.First(i => i > minScore);

            var result = rule.Execute(application);

            result.IsValid.ShouldBeFalse();
        }
    }
}
