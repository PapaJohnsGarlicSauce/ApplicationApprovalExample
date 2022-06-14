using ApplicationApproval.Models;
using System.Collections.Generic;

namespace ApplicationApproval.Services.RulesEngine
{
    public interface IRulesEngine
    {
        IEnumerable<RuleResult> Execute(Application application);
    }
}
