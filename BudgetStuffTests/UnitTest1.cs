using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace BudgetStuffTests
{
    [TestClass]
    public class UnitTest1
    {
        private IRepository<List<Budget>> repo = Substitute.For<IRepository<List<Budget>>>();
        private BudgetSys budgetSys;

        [TestInitializeAttribute]
        public void TestInit()
        {
            budgetSys = new BudgetSys(repo);
        }
        [TestMethod]
        public void OneMontbNoBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 1, Amount = 0}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201701", 0 }
            //};

            intiBudget(budget);
            TotalAmountShouldBe(0, new DateTime(2017, 1, 1), new DateTime(2017, 1, 31));
        }

        [TestMethod]
        public void OneMonthWithBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 1, Amount = 3100}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201701", 3100 }
            //};

            intiBudget(budget);
            TotalAmountShouldBe(3100, new DateTime(2017, 1, 1), new DateTime(2017, 1, 31));
        }

        [TestMethod]
        public void HalfMonthWithBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 1, Amount = 3100}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201701", 3100 }
            //};

            intiBudget(budget);
            TotalAmountShouldBe(1100, new DateTime(2017, 1, 10), new DateTime(2017, 1, 20));
        }

        [TestMethod]
        public void TwoMonths_Empty_NoEmptyBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 3, Amount = 3100}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201703", 3100 }
            //};
            intiBudget(budget);
            TotalAmountShouldBe(3100, new DateTime(2017, 2, 21), new DateTime(2017, 3, 31));
        }
        [TestMethod]
        public void TwoMonths_NoEmpty_EmptyBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 2, Amount = 2800}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201702", 2800 }
            //};
            intiBudget(budget);
            TotalAmountShouldBe(800, new DateTime(2017, 2, 21), new DateTime(2017, 3, 31));
        }

        [TestMethod]
        public void ThreeMonths_Empty_NoEmpty_EmptyBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 2, Amount = 2800}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201702", 2800 }
            //};
            intiBudget(budget);
            TotalAmountShouldBe(2800, new DateTime(2017, 1, 1), new DateTime(2017, 3, 31));
        }

        [TestMethod]
        public void ThreeWholeMonths_NoEmpty_Empty_NoEmptyBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 2, Amount = 2800},
                new Budget() {Year =  2017, Month = 4 ,Amount = 300}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201702", 2800 },{"201704",300}
            //};
            intiBudget(budget);
            TotalAmountShouldBe(3100, new DateTime(2017, 2, 1), new DateTime(2017, 4, 30));
        }

        [TestMethod]
        public void ThreeHalfMonths_NoEmpty_Empty_NoEmptyBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 2, Amount = 2800},
                new Budget() {Year =  2017, Month = 4 ,Amount = 300}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201702", 2800 },{"201704",300}
            //};
            intiBudget(budget);
            TotalAmountShouldBe(250, new DateTime(2017, 2, 28), new DateTime(2017, 4, 15));
        }

        [TestMethod]
        public void FourHalfMonths_NoEmpty_Empty_NoEmpty_EmptyBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 2, Amount = 2800},
                new Budget() {Year =  2017, Month = 4, Amount = 300}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201702", 2800 },{"201704",300}
            //};
            intiBudget(budget);
            TotalAmountShouldBe(400, new DateTime(2017, 2, 28), new DateTime(2017, 5, 15));
        }

        [TestMethod]
        public void StartTime_Older_than_Endtime()
        {
            try
            {
                TotalAmountShouldBe(400, new DateTime(2017, 1, 28), new DateTime(2017, 1, 20));
                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.AreEqual("Start Date must before End Date", e.Message);
            }
        }

        [TestMethod]
        public void CrossYearBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2016, Month = 12, Amount = 3100},
                new Budget() {Year =  2017, Month = 1, Amount = 310},
                new Budget() {Year =  2017, Month = 2, Amount = 28}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201612", 3100 },{"201701",310},{"201702",28}
            //};
            intiBudget(budget);
            TotalAmountShouldBe(3438, new DateTime(2016, 12, 1), new DateTime(2017, 2, 28));
        }

        [TestMethod]
        public void OnlyOneDayBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 1, Amount = 3100}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201701", 3100 }
            //};
            intiBudget(budget);
            TotalAmountShouldBe(100, new DateTime(2017, 1, 1), new DateTime(2017,1, 1));
        }


        [TestMethod]
        public void NotInMonthBeforeBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 4, Amount = 3100}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201704", 3100 }
            //};
            intiBudget(budget);
            TotalAmountShouldBe(0, new DateTime(2017, 3, 1), new DateTime(2017, 3, 31));
        }

        [TestMethod]
        public void NotInMonthAfterBudget()
        {
            List<Budget> budget = new List<Budget>()
            {
                new Budget() {Year =  2017, Month = 4, Amount = 3100}
            };
            //Dictionary<string, int> budget = new Dictionary<string, int>
            //{
            //    { "201704", 3100 }
            //};
            intiBudget(budget);
            TotalAmountShouldBe(0, new DateTime(2017, 5, 1), new DateTime(2017, 5, 31));
        }

        private void TotalAmountShouldBe(int expected, DateTime start, DateTime end)
        {
            Assert.AreEqual(expected, budgetSys.TotalAmount(start, end));
        }

        private void intiBudget(List<Budget> budgets)
        {
            repo.GetBudgets().Returns(budgets);
        }
    }
}