using ApplicationApproval.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApplicationApproval.Services.Readers
{
    public class LoanApplicationsReader : IReader
    {
        public async Task<IEnumerable<Application>> Read(string pathToFile)
        {
            using var reader = File.OpenText(pathToFile);
            var fileContent = await reader.ReadToEndAsync();
            return ConvertToApplications(fileContent);
        }

        private static IEnumerable<Application> ConvertToApplications(string fileContent)
        {
            var result = new List<Application>();
            var applicationIds = new Regex(Application.ApplicationRegex, RegexOptions.IgnoreCase)
                .Matches(fileContent);
            if (!applicationIds.Any())
                throw new ArgumentNullException(nameof(applicationIds), "Application IDs not found.");

            var ids = applicationIds.Select(x => x.Groups[1].Value);

            foreach (var applicationId in ids)
            {
                var loan = new Loan(applicationId).GetFromFile(fileContent);
                if (loan == null)
                    throw new ArgumentNullException(nameof(loan), "Loan information not provided.");

                var incomes = GetIncomes(applicationId, fileContent);
                var liabilities = GetLiabilities(applicationId, fileContent);
                var borrower = new Borrower(applicationId).GetFromFile(fileContent);
                if (borrower == null)
                    throw new ArgumentNullException(nameof(borrower), "Borrower information not provided.");

                AssignIncomeAndLiabilities(borrower, incomes, liabilities);
                var coborrower = new Borrower(applicationId).GetCoborrowerFromFile(fileContent);
                if (coborrower != null)
                    AssignIncomeAndLiabilities(coborrower, incomes, liabilities);
                var application = new Application(applicationId, loan, borrower, coborrower);
                result.Add(application);
            }
            return result;
        }

        private static IEnumerable<Liability>? GetLiabilities(string applicationId, string fileContent)
        {
            //TODO: Ideally, I would get the extension GetDeserializedObject to work with IEnumerable
            // so I can just pass in GetDeserializedObject<IEnumerable<Liability>> in ctor
            // Future enhancement?
            var liabilityRegex = new Regex(string.Format(Liability.LiabilityRegex, applicationId),
                RegexOptions.IgnoreCase);
            var matches = liabilityRegex.Matches(fileContent);
            return matches.Select(x => new Liability(applicationId).GetFromFile(x.Value));
        }

        private static IEnumerable<Income>? GetIncomes(string applicationId, string fileContent)
        {
            var incomeRegex = new Regex(string.Format(Income.IncomeRegex, applicationId),
                RegexOptions.IgnoreCase);
            var matches = incomeRegex.Matches(fileContent);
            return matches.Select(x => new Income(applicationId).GetFromFile(x.Value));
        }

        private static Borrower? AssignIncomeAndLiabilities(Borrower borrower, IEnumerable<Income>? incomes,
            IEnumerable<Liability>? liabilities)
        {
            if (borrower == null) return null;
            borrower.Incomes = incomes?.Where(x => x?.Names?.Contains(borrower.Name) ?? false);
            borrower.Liabilities = liabilities?.Where(x => x?.Names?.Contains(borrower.Name) ?? false);
            return borrower;
        }
    }
}
