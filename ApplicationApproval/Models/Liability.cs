using ApplicationApproval.Services;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ApplicationApproval.Models
{
    public class Liability : IRegexClass<Liability>
    {
        public static string LiabilityRegex { get; } =
            @"Liability (?<ApplicationId>{0}) (?<RawNames>(\S+\,*\S+)) (?<Kind>\S+) (?<MonthlyPayment>\d+\.*\d+) (?<OutstandingBalance>\d+\.*\d+)";

        public string? ApplicationId { get; set; }

        public string? RawNames { get; set; }

        public IEnumerable<string>? Names { get; set; }

        public string? Kind { get; set; }

        public decimal MonthlyPayment { get; set; }

        public decimal OutstandingBalance { get; set; }

        public Liability(string? applicationId, string? rawNames, IEnumerable<string>? names, string? kind, decimal monthlyPayment, decimal outstandingBalance)
        {
            ApplicationId = applicationId;
            RawNames = rawNames;
            Names = names;
            Kind = kind;
            MonthlyPayment = monthlyPayment;
            OutstandingBalance = outstandingBalance;
        }

        public Liability(string applicationId)
        {
            ApplicationId = applicationId;
        }

        public Liability? GetFromFile(string fileContent)
        {
            var liabilityRegex = new Regex(string.Format(LiabilityRegex, ApplicationId), RegexOptions.IgnoreCase);
            var liability = liabilityRegex.GetDeserializedObject<Liability>(fileContent);
            if (liability != null)
                liability.Names = liability?.RawNames?.Split(',');
            return liability;
        }

        // Parameterless constructor because we'll be setting the properties via reflection
        public Liability() { }
    }
}
