using ApplicationApproval.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicationApproval.Services.Readers
{
    public interface IReader
    {
        public Task<IEnumerable<Application>> Read(string pathToFile);
    }
}
