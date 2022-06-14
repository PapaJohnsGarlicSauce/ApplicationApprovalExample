using ApplicationApproval.Services;
using System.Text.RegularExpressions;

namespace ApplicationApproval.Models
{
    public class Loan : IRegexClass<Loan>
    {
        public static string LoanRegex { get; } =
            @"Loan (?<ApplicationId>{0}) (?<PrincipalAmount>\d+) (?<Years>\d+) (?<Rate>\d+\.*\d+) (?<MonthlyPayment>\d+\.*\d+)";

        public string? ApplicationId { get; set; }

        // I can't imagine anyone with a principal amount of more than ~2 billion (int)
        // but let's just handle any Elon Musks from the get-go
        public long PrincipalAmount { get; set; }

        public int Years { get; set; }

        public decimal Rate { get; set; }

        public decimal MonthlyPayment { get; set; }

        public Loan(string? applicationId, long principalAmount, int years, decimal rate, decimal monthlyPayment)
        {
            ApplicationId = applicationId;
            PrincipalAmount = principalAmount;
            Years = years;
            Rate = rate;
            MonthlyPayment = monthlyPayment;
        }

        public Loan(string applicationId)
        {
            ApplicationId = applicationId;
        }

        public Loan? GetFromFile(string fileContent)
        {
            var loanRegex = new Regex(string.Format(LoanRegex, ApplicationId), RegexOptions.IgnoreCase);
            return loanRegex.GetDeserializedObject<Loan>(fileContent);
        }

        public Loan() { }
    }

}
