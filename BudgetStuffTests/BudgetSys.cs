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
        private readonly IRepository<Dictionary<string, int>> _repo; 

        public BudgetSys(IRepository<Dictionary<string, int>> repo)
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

        private static decimal GetFullMonthBudget(string currYearMonth, DateTime start, DateTime end, Dictionary<string, int> budget)
        {
            if (budget.ContainsKey(currYearMonth))
            {
                return (decimal)(budget[currYearMonth] / DateTime.DaysInMonth(start.Year, start.Month) *
                                  ((end - start).TotalDays + 1));
            }
            return 0;
        }

        private static decimal GetFirstMonthBudget(string currYearMonth, DateTime start, Dictionary<string, int> budget)
        {

            if (budget.ContainsKey(currYearMonth))
            {
                return (decimal)(budget[currYearMonth] / DateTime.DaysInMonth(start.Year, start.Month) *
                                         ((new DateTime(start.Year, start.Month,
                                               DateTime.DaysInMonth(start.Year, start.Month)) - start).TotalDays +
                                          1));
            }

            return 0;
        }

        private static decimal GetLastMonthBudget(string currYearMonth, DateTime end, Dictionary<string, int> budget)
        {
            if (budget.ContainsKey(currYearMonth))
            {
                return (decimal)(budget[currYearMonth] / DateTime.DaysInMonth(end.Year, end.Month) *
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


        private static decimal GetMiddleMonthBudget(DateTime start, DateTime end, Dictionary<string, int> budget)
        {
            decimal totalBudget = 0;
            for (int i = 1; i < TotalMonths(start, end); i++)
            {
                DateTime calMonth = start.AddMonths(i);
                var currYearMonth = GetMonthKey(calMonth);
                if (budget.ContainsKey(currYearMonth))
                {
                    totalBudget += budget[currYearMonth];
                }
            }

            return totalBudget;
        }

        private static decimal CalculateAmount(DateTime start, DateTime end, Dictionary<string, int> budget)
        {

            if (start.Year == end.Year && start.Month == end.Month)
            {
                return GetFullMonthBudget(GetMonthKey(end), start, end, budget);
            }

            decimal totalBudget = 0;
            totalBudget += GetFirstMonthBudget(GetMonthKey(start), start, budget);
            totalBudget += GetMiddleMonthBudget(start, end, budget);
            totalBudget += GetLastMonthBudget(GetMonthKey(end), end, budget);

            return totalBudget;
        }
    }



}
