using ApplicationApproval.Services;
using ApplicationApproval.Services.Readers;
using ApplicationApproval.Services.RulesEngine;
using ApplicationApproval.Services.RulesEngine.Rules;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace ApplicationApproval
{
    internal class Program
    {
        static async Task Main(string file)
        {
            var host = CreateHostBuilder().Build();
            var job = host.Services.GetRequiredService<ApplicationApprovalService>();
            await job.Execute(file);
        }

        static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    // DI
                    services.AddTransient<ApplicationApprovalService>();
                    services.AddScoped<IFileSystem, FileSystem>();
                    services.AddScoped<IReader, LoanApplicationsReader>();
                    //Can change this to some provider/factory if you need different rules per job
                    services.AddSingleton<IRulesEngine>(rulesEngine =>
                        new RulesEngine(new IRule[] { new DtiRatioRule(0.5), new CreditScoreRule(620) }));
                });
    }
}
