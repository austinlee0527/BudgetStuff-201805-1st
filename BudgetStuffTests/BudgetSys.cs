using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NSubstitute.Exceptions;
using NodaTime;

namespace BudgetStuffTests
{

    public class BudgetSys
    {
        private readonly IRepository<List<Budget>> _repo;
        private List<Budget> _budget;

        public BudgetSys(IRepository<List<Budget>> repo)
        {
            _repo = repo;
        }

        public decimal TotalAmount(DateTime start, DateTime end)
        {
            _budget = this._repo.GetBudgets();
            if (start > end)
            {
                throw new Exception("Start Date must before End Date");
            }

            return CalculateAmount(start, end);

        }

        private Budget GetBudget(DateTime month)
        {
            return _budget.Where((c => c.Year == month.Year && c.Month == month.Month)).FirstOrDefault();
        }

        private decimal GetFullMonthBudget(DateTime start, DateTime end, Budget budget)
        {
            if (budget != null)
            {
                return (decimal)(budget.Amount / DateTime.DaysInMonth(start.Year, start.Month) *
                                  ((end - start).TotalDays + 1));
            }
            return 0;
        }

        private decimal GetFirstMonthBudget(DateTime start, Budget budget)
        {
            if (budget != null)
            {
                return (decimal)(budget.Amount / DateTime.DaysInMonth(start.Year, start.Month) *
                                         ((new DateTime(start.Year, start.Month,
                                               DateTime.DaysInMonth(start.Year, start.Month)) - start).TotalDays +
                                          1));
            }

            return 0;
        }

        private decimal GetLastMonthBudget(DateTime end, Budget budget)
        {
            if (budget != null)
            {
                return (decimal)(budget.Amount / DateTime.DaysInMonth(end.Year, end.Month) *
                                         ((end - (new DateTime(end.Year, end.Month, 1))).TotalDays + 1));
            }

            return 0;
        }

        private string GetMonthKey(DateTime date)
        {
            return date.Year.ToString() + date.Month.ToString().PadLeft(2, '0');
        }


        public int TotalMonths(DateTime start, DateTime end)
        {
            return Math.Abs((start.Year * 12 + start.Month) - (end.Year * 12 + end.Month));
        }


        private decimal GetMiddleMonthBudget(DateTime start, DateTime end)
        {
            decimal totalBudget = 0;
            for (int i = 1; i < TotalMonths(start, end); i++)
            {
                DateTime calMonth = start.AddMonths(i);
                var budget = GetBudget(calMonth);
                if (budget != null)
                {
                    totalBudget += budget.Amount;
                }
            }

            return totalBudget;
        }

        private decimal CalculateAmount(DateTime start, DateTime end)
        {

            if (start.Year == end.Year && start.Month == end.Month)
            {
                return GetFullMonthBudget(start, end, GetBudget(start));
            }

            decimal totalBudget = 0;
            totalBudget += GetFirstMonthBudget(start, GetBudget(start));
            totalBudget += GetMiddleMonthBudget(start, end);
            totalBudget += GetLastMonthBudget(end, GetBudget(end));

            return totalBudget;
        }
    }



}
