namespace EPG.Models
{
    public class PasswordResultItem
    {
        public PasswordResultItem(uint counter, string password, string? hyphenatedPassword, BloomFilterResult? bloomFilterResult, decimal? passwordQuality, bool manuallyEnterred)
        {
            Counter = counter;
            Password = password;
            HyphenatedPassword = hyphenatedPassword;
            BloomFilterResult = bloomFilterResult;
            PasswordQuality = passwordQuality;
            ManuallyEnterred = manuallyEnterred;
        }

        public uint Counter { get; set; }
        public string Password { get; set; }
        public string? HyphenatedPassword { get; set; }
        public BloomFilterResult? BloomFilterResult { get; set; }
        public decimal? PasswordQuality { get; set; }
        public bool ManuallyEnterred { get; set; }

        public override string ToString()
        {
            return $"{Counter}. {Password}";
        }
    }
}
