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
            using Password password = new(Password.Modes.LowersForced, string.Empty, excluded);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                bool res = password.GenerateWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                var hword = password.GetHyphenatedWord();
                Assert.IsFalse(string.IsNullOrEmpty(hword));
                Debug.WriteLine($"Words1000Excluded: {word} ({hword})");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsFalse(word.Contains(excluded));
            }
        }

        [TestMethod]
        public void Words1000IncludedC()
        {
            string included = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            using Password password = new(Password.Modes.LowersForced | Password.Modes.CapitalsForced, string.Empty, string.Empty);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                bool res = password.GenerateWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                var hword = password.GetHyphenatedWord();
                Assert.IsFalse(string.IsNullOrEmpty(hword));
                Debug.WriteLine($"Words1000Included: {word} ({hword})");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.IndexOfAny(included.ToCharArray()) >= 0);
            }
        }

        [TestMethod]
        public void Words1000IncludedN()
        {
            string included = "0123456789";
            using Password password = new(Password.Modes.Lowers | Password.Modes.Capitals | Password.Modes.NumeralsForced, string.Empty, string.Empty);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                bool res = password.GenerateWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                var hword = password.GetHyphenatedWord();
                Assert.IsFalse(string.IsNullOrEmpty(hword));
                Debug.WriteLine($"Words1000Included: {word} ({hword})");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.IndexOfAny(included.ToCharArray()) >= 0);
            }
        }

        [TestMethod]
        public void Words1000IncludedNS()
        {
            string includedN = "0123456789";
            string includedS = "!\"#$%&\'()*+.-,/:;<=>?@[\\]^_`{|}~";

            using Password password = new(Password.Modes.Lowers | Password.Modes.Capitals | Password.Modes.NumeralsForced| Password.Modes.SymbolsForced, string.Empty, string.Empty);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                bool res = password.GenerateWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                var hword = password.GetHyphenatedWord();
                Assert.IsFalse(string.IsNullOrEmpty(hword));
                Debug.WriteLine($"Words1000Included: {word} ({hword})");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.IndexOfAny(includedN.ToCharArray()) >= 0);
                Assert.IsTrue(word.IndexOfAny(includedS.ToCharArray()) >= 0);
            }
        }
    }
}