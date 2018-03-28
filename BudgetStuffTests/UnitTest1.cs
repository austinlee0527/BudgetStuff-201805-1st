using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace BudgetStuffTests
{
    [TestClass]
    public class UnitTest1
    {
        private IRepository<Dictionary<string, int>> repo = Substitute.For<IRepository<Dictionary<string, int>>>();
        private BudgetSys budgetSys;

        [TestInitializeAttribute]
        public void TestInit()
        {
            budgetSys = new BudgetSys(repo);
        }
        [TestMethod]
        public void OneMontbNoBudget()
        {
            Dictionary<string, int> budget = new Dictionary<string, int>
            {
                { "201701", 0 }
            };

            intiBudget(budget);
            TotalAmountShouldBe(0, new DateTime(2017, 1, 1), new DateTime(2017, 1, 31));
        }

        [TestMethod]
        public void OneMonthWithBudget()
        {
            Dictionary<string, int> budget = new Dictionary<string, int>
            {
                { "201701", 3100 }
            };

            intiBudget(budget);
            TotalAmountShouldBe(3100, new DateTime(2017, 1, 1), new DateTime(2017, 1, 31));
        }

        [TestMethod]
        public void HalfMonthWithBudget()
        {
            Dictionary<string, int> budget = new Dictionary<string, int>
            {
                { "201701", 3100 }
            };

            intiBudget(budget);
            TotalAmountShouldBe(1100, new DateTime(2017, 1, 10), new DateTime(2017, 1, 20));
        }

        [TestMethod]
        public void TwoMonths_Empty_NoEmptyBudget()
        {
            Dictionary<string, int> budget = new Dictionary<string, int>
            {
                { "201703", 3100 }
            };
            intiBudget(budget);
            TotalAmountShouldBe(3100, new DateTime(2017, 2, 21), new DateTime(2017, 3, 31));
        }
        [TestMethod]
        public void TwoMonths_NoEmpty_EmptyBudget()
        {
            Dictionary<string, int> budget = new Dictionary<string, int>
            {
                { "201702", 2800 }
            };
            intiBudget(budget);
            TotalAmountShouldBe(800, new DateTime(2017, 2, 21), new DateTime(2017, 3, 31));
        }

        [TestMethod]
        public void ThreeMonths_Empty_NoEmpty_EmptyBudget()
        {
            Dictionary<string, int> budget = new Dictionary<string, int>
            {
                { "201702", 2800 }
            };
            intiBudget(budget);
            TotalAmountShouldBe(2800, new DateTime(2017, 1, 1), new DateTime(2017, 3, 31));
        }

        [TestMethod]
        public void ThreeWholeMonths_NoEmpty_Empty_NoEmptyBudget()
        {
            Dictionary<string, int> budget = new Dictionary<string, int>
            {
                { "201702", 2800 },{"201704",300}
            };
            intiBudget(budget);
            TotalAmountShouldBe(3100, new DateTime(2017, 2, 1), new DateTime(2017, 4, 30));
        }

        [TestMethod]
        public void ThreeHalfMonths_NoEmpty_Empty_NoEmptyBudget()
        {
            Dictionary<string, int> budget = new Dictionary<string, int>
            {
                { "201702", 2800 },{"201704",300}
            };
            intiBudget(budget);
            TotalAmountShouldBe(250, new DateTime(2017, 2, 28), new DateTime(2017, 4, 15));
        }

        [TestMethod]
        public void FourHalfMonths_NoEmpty_Empty_NoEmpty_EmptyBudget()
        {
            Dictionary<string, int> budget = new Dictionary<string, int>
            {
                { "201702", 2800 },{"201704",300}
            };
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
            Dictionary<string, int> budget = new Dictionary<string, int>
            {
                { "201612", 3100 },{"201701",310},{"201702",28}
            };
            intiBudget(budget);
            TotalAmountShouldBe(3438, new DateTime(2016, 12, 1), new DateTime(2017, 2, 28));
        }

        [TestMethod]
        public void OnlyOneDayBudget()
        {
            Dictionary<string, int> budget = new Dictionary<string, int>
            {
                { "201701", 3100 }
            };
            intiBudget(budget);
            TotalAmountShouldBe(100, new DateTime(2017, 1, 1), new DateTime(2017,1, 1));
        }

        private void TotalAmountShouldBe(int expected, DateTime start, DateTime end)
        {
            Assert.AreEqual(expected, budgetSys.TotalAmount(start, end));
        }

        private void intiBudget(Dictionary<string, int> budgets)
        {
            repo.GetBudgets().Returns(budgets);
        }
    }
}