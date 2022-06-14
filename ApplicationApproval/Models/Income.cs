using ApplicationApproval.Services;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApplicationApproval.Models
{
    public class Income
    {
        public static string IncomeRegex { get; } =
            @"Income (?<ApplicationId>{0}) (?<RawNames>(\S+\,*\S+)) (?<Kind>\S+) (?<MonthlyAmount>\d+\.*\d+)";

        public string? ApplicationId { get; set; }

        public string? RawNames { get; set; }

        public IEnumerable<string>? Names { get; set; }

        public string? Kind { get; set; }

        public decimal MonthlyAmount { get; set; }

        public Income(string? applicationId, string? rawNames, IEnumerable<string>? names, string? kind, decimal monthlyAmount)
        {
            ApplicationId = applicationId;
            RawNames = rawNames;
            Names = names;
            Kind = kind;
            MonthlyAmount = monthlyAmount;
        }

        public static Income? Parse(string applicationId, string fileContent)
        {
            var incomeRegex = new Regex(string.Format(IncomeRegex, applicationId), RegexOptions.IgnoreCase);
            var income = incomeRegex.GetDeserializedObject<Income>(fileContent);
            if (income != null)
                income.Names = income?.RawNames?.Split(',');
            return income;
        }

        public Income() { }
    }
}
