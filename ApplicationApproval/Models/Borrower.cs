using ApplicationApproval.Services;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApplicationApproval.Models
{
    public class Borrower
    {
        public static string BorrowerRegex { get; } = @"^Borrower (?<ApplicationId>{0}) (?<Name>\S+) (?<CreditScore>\d+)";

        public static string CoborrowerRegex { get; } = @"^Coborrower (?<ApplicationId>{0}) (?<Name>\S+) (?<CreditScore>\d+)";

        public string? ApplicationId { get; set; }

        public string? Name { get; set; }

        public int CreditScore { get; set; }

        public IEnumerable<Income>? Incomes { get; set; }

        public IEnumerable<Liability>? Liabilities { get; set; }

        public Borrower(string? applicationId, string? name, int creditScore, IEnumerable<Income>? incomes, IEnumerable<Liability>? liabilities)
        {
            ApplicationId = applicationId;
            Name = name;
            CreditScore = creditScore;
            Incomes = incomes;
            Liabilities = liabilities;
        }

        public static Borrower? Parse(string applicationId, string fileContent)
        {
            var borrowerRegex = new Regex(string.Format(BorrowerRegex, applicationId), RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return borrowerRegex.GetDeserializedObject<Borrower>(fileContent);
        }

        public static Borrower? ParseCoborrower(string applicationId, string fileContent)
        {
            var coborrowerRegex = new Regex(string.Format(CoborrowerRegex, applicationId), RegexOptions.IgnoreCase | RegexOptions.Multiline);
            return coborrowerRegex.GetDeserializedObject<Borrower>(fileContent);
        }

        public Borrower() { }
    }
}
