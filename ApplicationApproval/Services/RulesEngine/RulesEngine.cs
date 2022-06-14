using ApplicationApproval.Models;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationApproval.Services.RulesEngine
{
    public class RulesEngine : IRulesEngine
    {
        private IEnumerable<IRule> Rules { get; }

        public RulesEngine(IEnumerable<IRule> rules)
        {
            Rules = rules;
        }

        public IEnumerable<RuleResult> Execute(Application application)
            => Rules.Where(rule => rule.ShouldRun(application)).Select(rule => rule.Execute(application));
    }
}
