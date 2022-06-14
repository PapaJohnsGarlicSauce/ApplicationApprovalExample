using ApplicationApproval.Models;

namespace ApplicationApproval.Services.RulesEngine
{
    public interface IRule
    {
        bool ShouldRun(Application application);

        RuleResult Execute(Application application);
    }
}
