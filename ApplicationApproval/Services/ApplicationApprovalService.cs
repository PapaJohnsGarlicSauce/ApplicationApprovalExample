using ApplicationApproval.Models;
using ApplicationApproval.Services.Readers;
using ApplicationApproval.Services.RulesEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationApproval.Services
{
    internal sealed class ApplicationApprovalService
    {
        private IReader Reader { get; }

        private IRulesEngine RulesEngine { get; }

        private string SummaryMessageFormat { get; } =
            "Summary: {0} application{1} approved, {2} application{3} received, {4} approval rate";
        private string ApprovalMessageFormat { get; } =
            @"{0}: {1}, {2}";

        public ApplicationApprovalService(
            IRulesEngine rulesEngine,
            IReader reader)
        {
            RulesEngine = rulesEngine;
            Reader = reader;
        }

        public async Task Execute(string pathToFile)
        {
            try
            {
                var applications = await Reader.Read(pathToFile);

                var results = new List<RuleResult>();
                var outputMessages = new List<string>();
                foreach (var application in applications)
                {
                    results = RulesEngine.Execute(application).ToList();
                    application.Status = results.Any(x => !x.IsValid)
                        ? ApplicationStatus.Denied
                        : ApplicationStatus.Approved;
                    outputMessages.Add(string.Format(
                        ApprovalMessageFormat,
                        application.Id, application.Status?.ToString().ToLower(),
                        string.Join(", ", results.Select(x => x.Message))));
                }

                Console.Out.WriteLine(GetSummaryMessage(applications));
                foreach (var message in outputMessages)
                {
                    Console.Out.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine("An error has occurred: {0}", ex.Message);
            }
        }

        private string GetSummaryMessage(IEnumerable<Application> applications)
        {
            var totalApplications = applications.Count();
            var totalApprovedApplications = applications.Count(x => x.Status == ApplicationStatus.Approved);
            return string.Format(SummaryMessageFormat,
                totalApprovedApplications,
                totalApprovedApplications != 1 ? "s" : string.Empty,
                totalApplications,
                totalApplications != 1 ? "s" : string.Empty,
                GetApprovalRate(applications));
        }

        private static string GetApprovalRate(IEnumerable<Application> applications)
        {
            if (!applications.Any()) return "0%";
            var rate = (double)applications
                .Count(a => a.Status == ApplicationStatus.Approved) / applications.Count() * 100;
            return string.Concat(Math.Round(rate, 3), '%');
        }
    }
}
