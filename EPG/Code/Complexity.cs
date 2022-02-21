using System;

namespace EPG.Code
{
    internal static class Complexity
    {
        const int CHARSPACE_ESCAPE = 60;
        const int CHARSPACE_ALPHA = 26;
        const int CHARSPACE_NUMBER = 10;
        const int CHARSPACE_SIMPSPECIAL = 16;
        const int CHARSPACE_EXTSPECIAL = 17;
        const int CHARSPACE_HIGH = 112;

        public static int CalculateComplexity(string password)
        {
            bool chLower = false;
            bool chUpper = false;
            bool chNumber = false;
            bool chSimpleSpecial = false;
            bool chExtSpecial = false;
            bool chHigh = false;
            bool chEscape = false;

            int charSpace = 0;
            foreach (char c in password)
            {
                if (c < ' ')
                    chEscape = true;
                else if (c >= 'A' && c <= 'Z')
                    chUpper = true;
                else if (c >= 'a' && c <= 'z')
                    chLower = true;
                else if (c >= 'a' && c <= 'z')
                    chLower = true;
                else if (c >= '0' && c <= '9')
                    chNumber = true;
                else if (c >= ' ' && c <= '/')
                    chSimpleSpecial = true;
                else if (c >= ':' && c <= '@')
                    chExtSpecial = true;
                else if (c >= '[' && c <= '`')
                    chExtSpecial = true;
                else if (c >= '{' && c <= '~')
                    chExtSpecial = true;
                else if (c > '~')
                    chHigh = true;
            }

            if (chEscape)
                charSpace += CHARSPACE_ESCAPE;
            if (chUpper)
                charSpace += CHARSPACE_ALPHA;
            if (chLower)
                charSpace += CHARSPACE_ALPHA;
            if (chNumber)
                charSpace += CHARSPACE_NUMBER;
            if (chSimpleSpecial)
                charSpace += CHARSPACE_SIMPSPECIAL;
            if (chExtSpecial)
                charSpace += CHARSPACE_EXTSPECIAL;
            if (chHigh)
                charSpace += CHARSPACE_HIGH;

            if (charSpace == 0)
                return 0;

            var bitsPerChar = Math.Log(charSpace) / Math.Log(2.0);
            var bits = (int)Math.Ceiling(bitsPerChar * password.Length);
            return bits;
        }
    }
}
