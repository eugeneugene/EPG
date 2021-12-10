using BloomCS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BloomCS_Test
{
    [TestClass]
    public class BloomTest
    {
        [TestMethod]
        public void Test1()
        {
            CreateTestString();
            OpenTestString();
        }

        [TestMethod]
        public void Test2()
        {
            CreateTestArray();
            OpenTestArray();
        }

        private static void CreateTestString()
        {
            Bloom bloom = new();
            Assert.IsNotNull(bloom);

            bloom.Create("BloomTest1.bf");

            bloom.Allocate(5);
            bloom.PutString("Hello");
            bloom.PutString("World");
            bloom.PutString("12345");
            bloom.PutString("Mickey");
            bloom.PutString("Mouse");

            bloom.Store();

            Assert.IsTrue(bloom.CheckString("Hello"));
            Assert.IsTrue(bloom.CheckString("World"));
            Assert.IsTrue(bloom.CheckString("12345"));
            Assert.IsTrue(bloom.CheckString("Mickey"));
            Assert.IsTrue(bloom.CheckString("Mouse"));

            Assert.IsFalse(bloom.CheckString("Hello!"));
            Assert.IsFalse(bloom.CheckString("world"));
            Assert.IsFalse(bloom.CheckString("123456"));
            Assert.IsFalse(bloom.CheckString("1234"));
            Assert.IsFalse(bloom.CheckString("M"));

            bloom.Close();
        }

        private static void OpenTestString()
        {
            Bloom bloom = new();
            Assert.IsNotNull(bloom);

            bloom.Open("BloomTest1.bf");
            Assert.AreEqual(0x200, bloom.HeaderVersion());
            Assert.AreNotEqual(75L, bloom.HeaderSize());
            Assert.AreEqual(7, bloom.HeaderHashFunc());

            bloom.Load();

            Assert.IsTrue(bloom.CheckString("Hello"));
            Assert.IsTrue(bloom.CheckString("World"));
            Assert.IsTrue(bloom.CheckString("12345"));
            Assert.IsTrue(bloom.CheckString("Mickey"));
            Assert.IsTrue(bloom.CheckString("Mouse"));

            Assert.IsFalse(bloom.CheckString("Hello!"));
            Assert.IsFalse(bloom.CheckString("world"));
            Assert.IsFalse(bloom.CheckString("123456"));
            Assert.IsFalse(bloom.CheckString("1234"));
            Assert.IsFalse(bloom.CheckString("M"));

            bloom.Close();
        }

        private static void CreateTestArray()
        {
            Bloom bloom = new();
            Assert.IsNotNull(bloom);

            bloom.Create("BloomTest2.bf");

            bloom.Allocate(3);
            bloom.PutArray(new byte[] { 0x00, 0x00, 0x00 });
            bloom.PutArray(new byte[] { 0x10, 0x11, 0x12 });
            bloom.PutArray(new byte[] { 0x22, 0x21, 0x20 });

            bloom.Store();

            Assert.IsTrue(bloom.CheckArray(new byte[] { 0x00, 0x00, 0x00 }));
            Assert.IsTrue(bloom.CheckArray(new byte[] { 0x10, 0x11, 0x12 }));
            Assert.IsTrue(bloom.CheckArray(new byte[] { 0x22, 0x21, 0x20 }));

            Assert.IsFalse(bloom.CheckArray(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
            Assert.IsFalse(bloom.CheckArray(new byte[] { 0x00, 0x00 }));
            Assert.IsFalse(bloom.CheckArray(new byte[] { 0x10, 0x11, 0x11 }));
            Assert.IsFalse(bloom.CheckArray(new byte[] { 0x22, 0x21, 0x20, 0x22, 0x21, 0x20 }));

            bloom.Close();
        }

        private static void OpenTestArray()
        {
            Bloom bloom = new();
            Assert.IsNotNull(bloom);

            bloom.Open("BloomTest2.bf");
            Assert.AreEqual(0x200, bloom.HeaderVersion());
            Assert.AreNotEqual(75UL, bloom.HeaderSize());
            Assert.AreEqual(7, bloom.HeaderHashFunc());

            bloom.Load();

            Assert.IsTrue(bloom.CheckArray(new byte[] { 0x00, 0x00, 0x00 }));
            Assert.IsTrue(bloom.CheckArray(new byte[] { 0x10, 0x11, 0x12 }));
            Assert.IsTrue(bloom.CheckArray(new byte[] { 0x22, 0x21, 0x20 }));

            Assert.IsFalse(bloom.CheckArray(new byte[] { 0x00, 0x00, 0x00, 0x00 }));
            Assert.IsFalse(bloom.CheckArray(new byte[] { 0x00, 0x00 }));
            Assert.IsFalse(bloom.CheckArray(new byte[] { 0x10, 0x11, 0x11 }));
            Assert.IsFalse(bloom.CheckArray(new byte[] { 0x22, 0x21, 0x20, 0x22, 0x21, 0x20 }));

            bloom.Close();
        }
    }
}