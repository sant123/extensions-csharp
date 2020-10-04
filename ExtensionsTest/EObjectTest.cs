using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ExtensionsTest
{
    [TestClass]
    public class EObjectTest : MainTest
    {
        [TestMethod]
        public void ConvertObjToTest()
        {
            var person1 = new Person { Id = 774, Name = "Mauricio", LastName = "Martinez", Age = 48 };
            var person2 = person1.ConvertObjTo<Person>();

            Assert.AreEqual(person1.Id, person2.Id);
        }

        [TestMethod]
        public void GetPropertiesNamesTest()
        {
            Assert.IsTrue(new Person().GetPropertiesNames().Any(c => c == "Age"));
        }

        [TestMethod]
        public void GetValueTest()
        {
            var person = new Person { Id = 774, Name = "Mauricio", LastName = "Martinez", Age = 48 };

            Assert.AreEqual("Martinez", person.GetValue("LastName"));
        }

        [TestMethod]
        public void IsPrimitiveTest()
        {
            var obj1 = string.Empty;
            var obj2 = DateTime.Now;
            const decimal obj3 = 5847;

            Assert.IsTrue(obj1.IsPrimitive());
            Assert.IsTrue(obj2.IsPrimitive());
            Assert.IsTrue(obj3.IsPrimitive());
        }
    }
}
