using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtensionsTest
{
    [TestClass]
    public class ECultureTest : MainTest
    {
        [TestMethod]
        public void GetMonthsFromCultureTest()
        {
            var months = ECulture.GetMonthsFromCulture("es-CO");

            Assert.AreEqual("Agosto", months.ElementAt(7));
        }
    }
}
