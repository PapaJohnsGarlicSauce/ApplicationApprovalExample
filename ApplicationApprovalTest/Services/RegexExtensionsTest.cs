using ApplicationApproval.Models;
using ApplicationApproval.Services;
using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ApplicationApprovalTest.Services.RegexExtensionsTest
{
    public class RegexExtensionsTest
    {
        [Theory, TestCaseSource("HappyPathObjectsToDeserialize")]
        public void GetDeserializedObject_ReturnsObjectWithProperties_WhenValid<T>(
            T obj,
            string objRegex,
            string applicationId,
            string textToSearch)
            where T : new()
        {
            var regex = new Regex(string.Format(objRegex, applicationId), RegexOptions.IgnoreCase | RegexOptions.Multiline);

            var sut = regex.GetDeserializedObject<T>(textToSearch);

            // Check that some properties were set to not null values from string
            // All properties will be not null except Names for Income/Liability
            // Could make this more robust by checking for that
            sut?.GetType().GetProperties().Select(p => p.GetValue(sut)).Any(p => p != null).ShouldBeTrue();
        }

        [Theory, TestCaseSource("SadPathObjectsToDeserialize")]
        public void GetDeserializedObject_ReturnsNull_WhenNoMatch<T>(
            T obj,
            string objRegex,
            string applicationId,
            string textToSearch)
            where T : new()
        {
            var regex = new Regex(string.Format(objRegex, applicationId), RegexOptions.IgnoreCase | RegexOptions.Multiline);

            var sut = regex.GetDeserializedObject<T>(textToSearch);

            //ShouldBeNull doesn't work with generic it seems so let's use good all fashioned assert here
            Assert.IsNull(sut);
        }

        #region TestHelpers

        //These tests are a little astro-turfed...¯\_(ツ)_/¯
        public static IEnumerable<object> HappyPathObjectsToDeserialize
        {
            get
            {
                yield return new object[]
                {
                    new Loan(),
                    Loan.LoanRegex,
                    "abc",
                    "LOAN abc 1450 30 3.1 810.2"
                };
                yield return new object[]
                {
                    new Income(),
                    Income.IncomeRegex,
                    "def",
                    "INCOME def John Salary 4021.29"
                };
                yield return new object[]
                {
                    new Liability(),
                    Liability.LiabilityRegex,
                    "ghi",
                    "LIABILITY A2 John CreditCard 1304.00 20000.10"
                };
                yield return new object[]
                {
                    new Borrower(),
                    Borrower.BorrowerRegex,
                    "jkl",
                    "BORROWER jkl John 621"
                };
                yield return new object[]
                {
                    new Borrower(),
                    Borrower.CoborrowerRegex,
                    "mno",
                    "COBORROWER A2 Jane 750"
                };
            }
        }

        public static IEnumerable<object> SadPathObjectsToDeserialize
        {
            get
            {
                yield return new object[]
                {
                    new Loan(),
                    Loan.LoanRegex,
                    "abc",
                    "LOdddddAN fdst43t634 1450 30 3.1 81dd0.2"
                };
                yield return new object[]
                {
                    new Income(),
                    Income.IncomeRegex,
                    "def",
                    "INCOME fdsf34rt34 John Salary"
                };
                yield return new object[]
                {
                    new Liability(),
                    Liability.LiabilityRegex,
                    "ghi",
                    "LIABfdsfsILITY A2 John CreditCard 1dfs304.00 20000.10"
                };
                yield return new object[]
                {
                    new Borrower(),
                    Borrower.BorrowerRegex,
                    "jkl",
                    "BORROfsdWEfsdR jkl Jofsdfsdfsdfsdfhn 621"
                };
                yield return new object[]
                {
                    new Borrower(),
                    Borrower.CoborrowerRegex,
                    "mno",
                    "COBORdfsdfROWER A2 Jane 7fsdf50"
                };
            }
        }
        #endregion
    }
}