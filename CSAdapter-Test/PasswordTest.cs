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
            using Password password = new(Password.Modes.Lowers | Password.Modes.Capitals, string.Empty, excluded);
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
        public void Words1000ForceC()
        {
            string includedL = "abcdefghijklmnopqrstuvwxyz";
            string includedC = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            using Password password = new(Password.Modes.Lowers | Password.Modes.CapitalsForced, string.Empty, string.Empty);
            Assert.IsNotNull(password);

            int amount = 1000;
            decimal l = 0.0m;
            for (int i = 0; i < amount; i++)
            {
                bool res = password.GenerateWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                var hword = password.GetHyphenatedWord();
                Assert.IsFalse(string.IsNullOrEmpty(hword));
                Debug.WriteLine($"Words1000ForceC: {word} ({hword})");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.IndexOfAny(includedC.ToCharArray()) >= 0);

                if (word.IndexOfAny(includedL.ToCharArray()) >= 0)
                    l++;
            }

            decimal p = l / amount;
            Debug.WriteLine("Fraction of Passwords with small letters included = {0:F5}", p);
        }

        [TestMethod]
        public void Words1000ForceL()
        {
            string includedL = "abcdefghijklmnopqrstuvwxyz";
            string includedC = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            using Password password = new(Password.Modes.LowersForced | Password.Modes.Capitals, string.Empty, string.Empty);
            Assert.IsNotNull(password);

            int amount = 1000;
            decimal c = 0.0m;
            for (int i = 0; i < amount; i++)
            {
                bool res = password.GenerateWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                var hword = password.GetHyphenatedWord();
                Assert.IsFalse(string.IsNullOrEmpty(hword));
                Debug.WriteLine($"Words1000ForceSC: {word} ({hword})");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.IndexOfAny(includedL.ToCharArray()) >= 0);

                if (word.IndexOfAny(includedC.ToCharArray()) >= 0)
                    c++;
            }

            decimal p = c / amount;
            Debug.WriteLine("Fraction of Passwords with capital letters included = {0:F5}", p);
        }

        [TestMethod]
        public void Words1000ForceN()
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
                Debug.WriteLine($"Words1000ForceN: {word} ({hword})");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.IndexOfAny(included.ToCharArray()) >= 0);
            }
        }

        [TestMethod]
        public void Words1000ForceNS()
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
                Debug.WriteLine($"Words1000ForceNS: {word} ({hword})");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.IndexOfAny(includedN.ToCharArray()) >= 0);
                Assert.IsTrue(word.IndexOfAny(includedS.ToCharArray()) >= 0);
            }
        }

        [TestMethod]
        public void RandomWords1000Excluded()
        {
            string excluded = "lIO01";
            using Password password = new(Password.Modes.Lowers | Password.Modes.Capitals, string.Empty, excluded);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                bool res = password.GenerateRandomWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                Debug.WriteLine($"RandomWords1000Excluded: {word}");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsFalse(word.Contains(excluded));
            }
        }

        [TestMethod]
        public void RandomWords1000ForceC()
        {
            string included = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            using Password password = new(Password.Modes.LowersForced | Password.Modes.CapitalsForced, string.Empty, string.Empty);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                bool res = password.GenerateRandomWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                Debug.WriteLine($"RandomWords1000ForceC: {word}");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.IndexOfAny(included.ToCharArray()) >= 0);
            }
        }

        [TestMethod]
        public void RandomWords1000ForceLC()
        {
            string includedL = "abcdefghijklmnopqrstuvwxyz";
            string includedC = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            using Password password = new(Password.Modes.LowersForced | Password.Modes.CapitalsForced, string.Empty, string.Empty);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                bool res = password.GenerateRandomWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                Debug.WriteLine($"RandomWords1000ForceSC: {word}");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.IndexOfAny(includedL.ToCharArray()) >= 0);
                Assert.IsTrue(word.IndexOfAny(includedC.ToCharArray()) >= 0);
            }
        }

        [TestMethod]
        public void RandomWords1000ForceN()
        {
            string included = "0123456789";
            using Password password = new(Password.Modes.Lowers | Password.Modes.Capitals | Password.Modes.NumeralsForced, string.Empty, string.Empty);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                bool res = password.GenerateRandomWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                Debug.WriteLine($"RandomWords1000ForceN: {word}");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.IndexOfAny(included.ToCharArray()) >= 0);
            }
        }

        [TestMethod]
        public void RandomWords1000ForceNS()
        {
            string includedN = "0123456789";
            string includedS = "!\"#$%&\'()*+.-,/:;<=>?@[\\]^_`{|}~";

            using Password password = new(Password.Modes.Lowers | Password.Modes.Capitals | Password.Modes.NumeralsForced | Password.Modes.SymbolsForced, string.Empty, string.Empty);
            Assert.IsNotNull(password);

            for (int i = 0; i < 1000; i++)
            {
                bool res = password.GenerateRandomWord(10);
                Assert.IsTrue(res);
                var word = password.GetWord();
                Assert.IsFalse(string.IsNullOrEmpty(word));
                Debug.WriteLine($"RandomWords1000ForceNS: {word}");
                Assert.AreEqual(10, word.Length);
                uint l1 = password.GetWordLength();
                Assert.AreEqual(10u, l1);
                Assert.IsTrue(word.IndexOfAny(includedN.ToCharArray()) >= 0);
                Assert.IsTrue(word.IndexOfAny(includedS.ToCharArray()) >= 0);
            }
        }
    }
}