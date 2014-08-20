using System;
using System.Collections.Generic;
using System.Linq;
using nofs.fuse.test;
using Nofs.Net.Fuse.Impl;
using Nofs.Net.nofs.metadata.interfaces;
using Nofs.Net.nofs.stocks;
using Nofs.Net.Utils;
using NUnit.Framework;

namespace nofs.test
{
    [TestFixture]
    [Category("Portfolio")]
    class PortfolioTest : BaseTest
    {
        public PortfolioTest()
            : base()
        {

        }

        [Test]
        public void TestLoadStocks()
        {
            Portfolio portfolio = new Portfolio();

            IAttributeAccessor accessor = new AttributeAccessor();

            accessor.SetContainerManagerIfAttributeExists(portfolio, Fixture.Manager);
            accessor.SetContainerIfAttributeExists(portfolio, Fixture.GetContainer(typeof(Stock)));

            Assert.IsTrue(portfolio.Stocks().Count() == 0);
            IEnumerable<Stock> stocks = portfolio.Stocks();
            portfolio.AddAStock("MSFT");
            Assert.AreEqual(stocks.Count(), 1);

            portfolio.AddAStock("IBM");
            Assert.AreEqual(stocks.Count(), 2);

            portfolio.AddAStock("MORN");
            Assert.AreEqual(stocks.Count(), 3);

            foreach (Stock s in portfolio.Stocks())
            {
                Console.WriteLine(StreamUtil.SerializeToString(s));
            }

        }


    }

}
