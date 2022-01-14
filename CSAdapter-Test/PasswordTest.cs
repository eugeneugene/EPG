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
            using Password password = new(Password.Mode.ModeLO, string.Empty, excluded);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                bool res = password.GenerateWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                Debug.WriteLine($"Words1000Excluded: {word}");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsFalse(word.Contains(excluded));
            }
        }

        [TestMethod]
        public void Words1000Included()
        {
            string included = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            using Password password = new(Password.Mode.ModeL | Password.Mode.ModeCO, string.Empty, string.Empty);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                bool res = password.GenerateWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                Debug.WriteLine($"Words1000Included: {word}");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.Contains(included));
            }
        }
    }
}