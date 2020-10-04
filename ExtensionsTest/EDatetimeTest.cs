using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtensionsTest
{
    [TestClass]
    public class EDatetimeTest : MainTest
    {
        [TestMethod]
        public void IsDateTimeTest()
        {
            Assert.IsTrue("1995-01-27".IsDateTime());
        }
    }
}
