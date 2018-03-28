using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetStuffTests
{
    public interface IRepository<T>
    {
        Dictionary<string, int> GetBudgets();
    }
}
