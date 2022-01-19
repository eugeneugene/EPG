namespace EPG.Models
{
    public class DataItem
    {
        public DataItem(string password, string? hyphenatedPassword, BloomFilterResult? bloomFilterResult, decimal? passwordQuality)
        {
            Password = password;
            HyphenatedPassword = hyphenatedPassword;
            BloomFilterResult = bloomFilterResult;
            PasswordQuality = passwordQuality;
        }

        public string Password { get; }
        public string? HyphenatedPassword { get; }
        public BloomFilterResult? BloomFilterResult { get; }
        public decimal? PasswordQuality { get; }
    }
}
