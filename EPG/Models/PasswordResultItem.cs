namespace EPG.Models
{
    public class PasswordResultItem
    {
        public PasswordResultItem(uint counter, string password, string? hyphenatedPassword, BloomFilterResult? bloomFilterResult, decimal? passwordQuality)
        {
            Counter = counter;
            Password = password;
            HyphenatedPassword = hyphenatedPassword;
            BloomFilterResult = bloomFilterResult;
            PasswordQuality = passwordQuality;
        }

        public uint Counter { get; }
        public string Password { get; }
        public string? HyphenatedPassword { get; }
        public BloomFilterResult? BloomFilterResult { get; }
        public decimal? PasswordQuality { get; }
    }
}
