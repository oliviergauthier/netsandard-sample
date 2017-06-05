using System;
using Xunit;

namespace NetStandardSample.XUnit.Tests {
    public class PersonXUnitTest {
        
        [Fact]
        public void Test_FullName_ReturnsFirstNameAndLastName_WithXUnit() {
            var p = new Person() { FirstName = "John", LastName = "Doe" };
            Assert.Equal(p.FullName, "John Doe" );
        }
    }
}
