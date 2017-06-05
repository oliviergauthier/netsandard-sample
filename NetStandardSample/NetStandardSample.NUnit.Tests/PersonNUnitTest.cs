using System;
using NUnit.Framework;

namespace NetStandardSample.NUnit.Tests {
    public class PersonNUnitTest {

        [Test]
		public void Test_FullName_ReturnsFirstNameAndLastName_WithNUnit() {
			var p = new Person() { FirstName = "John", LastName = "Doe" };
            Assert.AreEqual(p.FullName, "John Doe");
		}

    }
}
