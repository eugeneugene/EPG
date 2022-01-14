using CSAdapter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace BloomCS_Test
{
    [DoNotParallelize]
    [TestClass]
    public class PasswordTest
    {
        [TestMethod]
        public void Words1000Excluded()
        {
            string excluded = "lIO01";
            Password password = new(Password.Mode.ModeLO, string.Empty, excluded);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                var word = password.GenerateWord(10);
                Assert.IsFalse(string.IsNullOrEmpty(word));
                Debug.WriteLine($"Words1000Excluded: {word}");
                Assert.IsFalse(word.Contains(excluded));
            }
        }

        [TestMethod]
        public void Words1000Included()
        {
            string included = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Password password = new(Password.Mode.ModeLN | Password.Mode.ModeCO, string.Empty, string.Empty);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                var word = password.GenerateWord(10);
                Assert.IsFalse(string.IsNullOrEmpty(word));
                Debug.WriteLine($"Words1000Included: {word}");
                Assert.IsTrue(word.Contains(included));
            }
        }
    }
}