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

        public BudgetSys(IRepository<List<Budget>> repo)
        {
            _repo = repo;
        }

        public decimal TotalAmount(DateTime start, DateTime end)
        {
            var budget = this._repo.GetBudgets();
            if (start > end)
            {
                throw new Exception("Start Date must before End Date");
            }

            return CalculateAmount(start, end, budget);

        }

        private static decimal GetFullMonthBudget(DateTime start, DateTime end, List<Budget> budgets)
        {
            var budget = budgets.Where((c => c.Year == start.Year && c.Month == start.Month)).FirstOrDefault();
            if (budget != null)
            {
                return (decimal)(budget.Amount / DateTime.DaysInMonth(start.Year, start.Month) *
                                  ((end - start).TotalDays + 1));
            }
            return 0;
        }

        private static decimal GetFirstMonthBudget(DateTime start, List<Budget> budgets)
        {
            var budget = budgets.Where((c => c.Year == start.Year && c.Month == start.Month)).FirstOrDefault();
            if (budget != null)
            {
                return (decimal)(budget.Amount / DateTime.DaysInMonth(start.Year, start.Month) *
                                         ((new DateTime(start.Year, start.Month,
                                               DateTime.DaysInMonth(start.Year, start.Month)) - start).TotalDays +
                                          1));
            }

            return 0;
        }

        private static decimal GetLastMonthBudget(DateTime end, List<Budget> budgets)
        {
            var budget = budgets.Where((c => c.Year == end.Year && c.Month == end.Month)).FirstOrDefault();
            if (budget != null)
            {
                return (decimal)(budget.Amount / DateTime.DaysInMonth(end.Year, end.Month) *
                                         ((end - (new DateTime(end.Year, end.Month, 1))).TotalDays + 1));
            }

            return 0;
        }

        private static string GetMonthKey(DateTime date)
        {
            return date.Year.ToString() + date.Month.ToString().PadLeft(2, '0');
        }


        public static int TotalMonths(DateTime start, DateTime end)
        {
            return Math.Abs((start.Year * 12 + start.Month) - (end.Year * 12 + end.Month));
        }


        private static decimal GetMiddleMonthBudget(DateTime start, DateTime end, List<Budget> budgets)
        {
            decimal totalBudget = 0;
            for (int i = 1; i < TotalMonths(start, end); i++)
            {
                DateTime calMonth = start.AddMonths(i);
                var budget = budgets.Where((c => c.Year == calMonth.Year && c.Month == calMonth.Month)).FirstOrDefault();
                if (budget != null)
                {
                    totalBudget += budget.Amount;
                }
            }

            return totalBudget;
        }

        private static decimal CalculateAmount(DateTime start, DateTime end, List<Budget> budget)
        {

            if (start.Year == end.Year && start.Month == end.Month)
            {
                return GetFullMonthBudget(start, end, budget);
            }

            decimal totalBudget = 0;
            totalBudget += GetFirstMonthBudget(start, budget);
            totalBudget += GetMiddleMonthBudget(start, end, budget);
            totalBudget += GetLastMonthBudget(end, budget);

            return totalBudget;
        }
    }



}
