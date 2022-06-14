using ApplicationApproval.Models;
using ApplicationApproval.Services.RulesEngine.Rules;
using AutoFixture;
using AutoFixture.NUnit3;
using NSubstitute;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationApprovalTest.Services.RulesEngine.Rules
{
    public class DtiRatioRuleTest
    {
        [Theory, AutoData]
        public void Execute_Approved_WithBorrowerOnly_NoLiabilitiesSomeIncome_DtiBelowLimit(
            Random random,
            Application application,
            Generator<decimal> decimalGenerator)
        {
            var ratio = random.NextDouble();
            var rule = new DtiRatioRule(ratio);
            application.Coborrower = null;
            application.Borrower.Liabilities = null;
            // Autofixture guarantees incomes will not be null
            // I would probably add customization to FULLY ensure this if I had time
            foreach (var income in application.Borrower.Incomes!)
            {
                // Pretty sure this won't ever be 0 but let's be extra safe, shall we
                income.MonthlyAmount = decimalGenerator.First(i => i > 0);
            }

            var result = rule.Execute(application);

            result.IsValid.ShouldBeTrue();
        }

        [Theory, AutoData]
        public void Execute_Denied_WithBorrowerOnly_NoIncomeSomeLiabilities_DtiBelowLimit(
            Random random,
            Application application,
            Generator<decimal> decimalGenerator)
        {
            var ratio = random.NextDouble();
            var rule = new DtiRatioRule(ratio);
            application.Coborrower = null;
            application.Borrower.Incomes = null;
            foreach (var liability in application.Borrower.Liabilities!)
            {
                // Pretty sure this won't ever be 0 but let's be extra safe, shall we
                liability.MonthlyPayment = decimalGenerator.First(i => i > 0);
            }

            var result = rule.Execute(application);

            result.IsValid.ShouldBeFalse();
        }

        [Theory, AutoData]
        public void Execute_Approved_WithBorrowerOnly_SomeIncomeSomeLiabilities_DtiBelowLimit(
            Random random)
        {
            var ratio = random.NextDouble();
            var rule = new DtiRatioRule(ratio);
            var application = GetApprovedApplication(ratio);
            application.Coborrower = null;

            var result = rule.Execute(application);

            result.IsValid.ShouldBeTrue();
        }

        [Theory, AutoData]
        public void Execute_Denied_WithBorrowerOnly_SomeIncomeSomeLiabilities_DtiAboveLimit(
            Random random)
        {
            var ratio = random.NextDouble();
            var rule = new DtiRatioRule(ratio);
            var application = GetDeniedApplication(ratio);
            application.Coborrower = null;

            var result = rule.Execute(application);

            result.IsValid.ShouldBeFalse();
        }

        [Theory, AutoData]
        public void Execute_Approved_WithBorrowerAndCoborrower_NoLiabilitiesSomeIncome_DtiBelowLimit(
            Random random)
        {
            var ratio = random.NextDouble();
            var rule = new DtiRatioRule(ratio);
            var application = GetApprovedApplication(ratio);
            application.Borrower.Liabilities = null;

            var result = rule.Execute(application);

            result.IsValid.ShouldBeTrue();
        }

        [Theory, AutoData]
        public void Execute_Denied_WithBorrowerAndCoborrower_NoIncomeSomeLiabilities_DtiBelowLimit(
            Random random)
        {
            var ratio = random.NextDouble();
            var rule = new DtiRatioRule(ratio);
            var application = GetApprovedApplication(ratio);
            application.Borrower.Incomes = null;

            var result = rule.Execute(application);

            result.IsValid.ShouldBeFalse();
        }

        [Theory, AutoData]
        public void Execute_Approved_WithBorrowerAndCoborrower_BorrowerIncomeCoborrowerNoIncome_NoSharedLiabilities_BelowDti(
            Random random)
        {
            var ratio = random.NextDouble();
            var rule = new DtiRatioRule(ratio);
            var application = GetApprovedApplication(ratio);
            application.Coborrower!.Incomes = null;

            var result = rule.Execute(application);

            result.IsValid.ShouldBeTrue();
        }

        [Theory, AutoData]
        public void Execute_Denied_WithBorrowAndCoborrower_BorrowIncomeCoborrowerNoIncome_NoSharedLiabilities_AboveDti(
            Random random)
        {
            var ratio = random.NextDouble();
            var rule = new DtiRatioRule(ratio);
            var application = GetDeniedApplication(ratio);
            application.Coborrower!.Incomes = null;

            var result = rule.Execute(application);

            result.IsValid.ShouldBeFalse();
        }

        [Theory, AutoData]
        public void Execute_Approved_WithBorrowerAndCoborrower_SharedLiabilities_BelowDti(
            Liability sharedLiability,
            Random random)
        {
            var ratio = random.NextDouble();
            var rule = new DtiRatioRule(ratio);
            var application = GetApprovedApplication(ratio);
            // Used autogenerated properties so not null
            sharedLiability.Names = new List<string>() { application.Borrower.Name!, application.Coborrower!.Name! };
            application.Borrower.Liabilities!.Append(sharedLiability);
            application.Coborrower!.Liabilities!.Append(sharedLiability);

            var result = rule.Execute(application);

            result.IsValid.ShouldBeTrue();
        }

        [Theory, AutoData]
        public void Execute_Denied_WithBorrowerAndCoborrower_SharedLiabilities_AboveDti(
            Liability sharedLiability,
            Random random)
        {
            var ratio = random.NextDouble();
            var rule = new DtiRatioRule(ratio);
            var application = GetDeniedApplication(ratio);
            // Used autogenerated properties so not null
            sharedLiability.Names = new List<string>() { application.Borrower.Name!, application.Coborrower!.Name! };
            application.Borrower.Liabilities!.Append(sharedLiability);
            application.Coborrower!.Liabilities!.Append(sharedLiability);

            var result = rule.Execute(application);

            result.IsValid.ShouldBeFalse();
        }

        [Theory, AutoData]
        public void Execute_WithBorrowerAndCoborrower_SharedLiabilitiesOnlyCountedOnce(
            Application application,
            Income income1,
            Income income2,
            Liability sharedLiability,
            Random random)
        {
            var ratio = random.NextDouble();
            var rule = new DtiRatioRule(ratio);
            // Used autogenerated properties so not null
            application.Borrower.Incomes = new List<Income>() { income1 };
            application.Coborrower!.Incomes = new List<Income>() { income2 };
            sharedLiability.Names = new List<string>() { application.Borrower.Name!, application.Coborrower!.Name! };
            //For ease of testing purposes, they ONLY have the shared liability
            application.Borrower.Liabilities = new List<Liability>() { sharedLiability };
            application.Coborrower!.Liabilities = new List<Liability>() { sharedLiability };

            var expectedDti = (sharedLiability.MonthlyPayment + application.Loan.MonthlyPayment) /
                (income1.MonthlyAmount + income2.MonthlyAmount);
            var exp = Math.Round(expectedDti, 3).ToString();

            var result = rule.Execute(application);

            // Only real way to test is check if RuleResult.Message contains expected dti
            // Could potentially have issues if rounding value rounds to 0
            // This is very brittle so might consider adding a "value" to RuleResult to retrieve calculated value
            result.Message.ShouldNotBeNull().ShouldContain(exp);
        }

        #region TestHelpers

        // Ideally these would be data customizations I could reuse
        public Application GetApprovedApplication(double ratio)
        {
            var fixture = new Fixture();

            var incomes = fixture.Build<Income>()
                .With(x => x.MonthlyAmount, Int32.MaxValue)
                .CreateMany();
            var liabilities = fixture.Build<Liability>()
                // Make each liability less than ratio * individual liabilties by dividing by larger factor
                .With(x => x.MonthlyPayment, (decimal)(ratio * Int32.MaxValue) / 1000)
                .CreateMany();

            var borrower = fixture.Build<Borrower>()
                .WithAutoProperties()
                .With(x => x.Incomes, incomes)
                .With(x => x.Liabilities, liabilities)
                .Create();

            //Make coborrower a separate item to avoid unexpected copy/delete behavior ny reference
            var coborrower = fixture.Build<Borrower>()
                .WithAutoProperties()
                .With(x => x.Incomes, incomes)
                .With(x => x.Liabilities, liabilities)
                .Create();

            var loan = fixture.Build<Loan>()
                .With(x => x.MonthlyPayment, 0)
                .Create();

            return fixture.Build<Application>()
                .WithAutoProperties()
                .With(a => a.Loan, loan)
                .With(a => a.Borrower, borrower)
                .With(a => a.Coborrower, coborrower)
                .Create();
        }

        public Application GetDeniedApplication(double ratio)
        {
            var fixture = new Fixture();

            var smallIncome = (decimal)0.000000001;
            var incomes = fixture.Build<Income>()
                // so smol
                .With(x => x.MonthlyAmount, smallIncome)
                .CreateMany();
            var liabilities = fixture.Build<Liability>()
                // Make each liability more than ratio * individual liabilties by multiplying by larger factor
                .With(x => x.MonthlyPayment, (decimal)ratio * smallIncome * Int32.MaxValue)
                .CreateMany();

            var borrower = fixture.Build<Borrower>()
                .WithAutoProperties()
                .With(x => x.Incomes, incomes)
                .With(x => x.Liabilities, liabilities)
                .Create();

            //Make coborrower a separate item to avoid unexpected copy/delete behavior ny reference
            var coborrower = fixture.Build<Borrower>()
                .WithAutoProperties()
                .With(x => x.Incomes, incomes)
                .With(x => x.Liabilities, liabilities)
                .Create();

            var loan = fixture.Build<Loan>()
                .With(x => x.MonthlyPayment, Int32.MaxValue)
                .Create();

            return fixture.Build<Application>()
                .WithAutoProperties()
                .With(a => a.Loan, loan)
                .With(a => a.Borrower, borrower)
                .With(a => a.Coborrower, coborrower)
                .Create();
        }

        #endregion TestHelpers
    }
}
