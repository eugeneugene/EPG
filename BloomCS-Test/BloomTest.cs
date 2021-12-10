using BloomCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BloomCS_Test
{
    [TestClass]
    public class BloomTest
    {
        [TestMethod]
        public void StringTest1()
        {
            var bloom = new Bloom();
            Assert.IsNotNull(bloom);

            bloom.Allocate(5);
            bloom.PutString("Hello");
            bloom.PutString("World");
            bloom.PutString("12345");
            bloom.PutString("Mickey");
            bloom.PutString("Mouse");

            Assert.IsTrue(bloom.CheckString("Hello"));
            Assert.IsTrue(bloom.CheckString("World"));
            Assert.IsTrue(bloom.CheckString("12345"));
            Assert.IsTrue(bloom.CheckString("Mickey"));
            Assert.IsTrue(bloom.CheckString("Mouse"));

            Assert.IsFalse(bloom.CheckString("Hello!"));
            Assert.IsFalse(bloom.CheckString("world"));
            Assert.IsFalse(bloom.CheckString("123456"));
            Assert.IsFalse(bloom.CheckString("Tom"));
            Assert.IsFalse(bloom.CheckString("Jerry"));
        }
    }
}