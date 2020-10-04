using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtensionsTest
{
    [TestClass]
    public class ECollectionTest : MainTest
    {
        [TestMethod]
        public void DistinctByTest()
        {
            var people = GetPeople();

            var distinctValue = people.DistinctBy(c => c.Id);
            var theCopy = distinctValue.FirstOrDefault(c => c.Name == "The Copy");

            Assert.IsNull(theCopy);
        }

        [TestMethod]
        public void GroupByIndexTest()
        {
            var people = GetPeople();

            var groupByValue = people.GroupByIndex(3);

            Assert.AreEqual(2, groupByValue.Count());
        }

        [TestMethod]
        public void LeftJoinTest()
        {
            var people = GetPeople();
            var products = GetProducts();

            var leftJoinValue = people.LeftJoin(products, c => c.Id, c => c.IdPerson, (person, product) => new { Name = person.Name, HasProducts = product != null });

            Assert.IsTrue(leftJoinValue.Any(c => !c.HasProducts));
        }

        [TestMethod]
        public void OrderByPropertyNameTest()
        {
            var people = GetPeople();

            var orderByPropertyName = people.OrderByPropertyName("Name");

            Assert.IsTrue(orderByPropertyName.First().Name == "Gina");

            orderByPropertyName = people.OrderByPropertyName("Name", ECollection.SortDirection.Descending);

            Assert.IsTrue(orderByPropertyName.First().Name == "The Copy");
        }

        [TestMethod]
        public void RightJoinTest()
        {
            var people = GetPeople();
            var products = GetProducts();

            var leftJoinValue = people.RightJoin(products, c => c.Id, c => c.IdPerson, (person, product) => new { Name = product.Name, HasPeople = person != null });

            Assert.IsTrue(leftJoinValue.Any(c => !c.HasPeople));
        }

        [TestMethod]
        public void SliceTest()
        {
            var people = GetPeople();

            const int removeFrom = 2;
            const int removeTo = 3;

            var sliceValue = people.Slice(removeFrom);

            Assert.IsFalse(sliceValue.Any(c => c.Name == "Santiago" || c.Name == "Paola"));

            sliceValue = people.Slice(removeFrom, removeTo);

            Assert.IsNotNull(sliceValue.FirstOrDefault(c => c.Name == "Javier"));
        }

        [TestMethod]
        public void XorTest()
        {
            var people1 = GetPeople();
            var people2 = GetPeople();

            people1.Add(new Person { Id = 211, Name = "Oscar", LastName = "Cardenas", Age = 28 });

            var xorValue = people1.Xor(people2, c => c.Id);

            Assert.IsTrue(xorValue.Count() == 1);

            Assert.AreEqual(211, xorValue.First().Id);
        }
    }
}
