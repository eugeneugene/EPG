#include "pch.h"
#include "Password.h"

bool Password::GenerateWord(size_t len)
{
	unsigned tries = 0;

	if (mode == 0 || len == 0)
		return false;

	unsigned Weight = GetWeight(mode, strIncludeSymbols);
	if (Weight == 0)
		return false;

#if !defined(_DEBUG)
	__timeb64 start;
	IO_Validate0(_ftime64_s(&start));
#endif

	Units.clear();
	ProperUnits.clear();
	Syllables.clear();
	do
	{
		unsigned o_mode = (mode & (ModeNO | ModeSO | ModeCO | ModeLO));
		/*
		** Find syllables until the entire word is constructed.
		*/
		while (GetLength() < len)
		{
#ifndef _DEBUG
			if (Timeout(start))
				return false;
#endif
			int p = GetRandomUINT(1, Weight);
			if (mode & (ModeCO | ModeLO))
			{
				p -= WeightCL;
				if (p <= 0)
				{
					bool Upper = false;
					/*
					** Get the syllable and find its length.
					*/
					Syllable syllable;
					if (!syllable.Generate(len - GetLength()))
					{
						tries = 0;
						Units.clear();
						ProperUnits.clear();
						Syllables.clear();
						continue;
					}
					std::string new_syllable = syllable.GetSyllable();
					size_t syllen = new_syllable.length();
#ifdef _DEBUG
					if (syllen > len - GetLength())
						throw std::runtime_error("Internal state error");
#endif
					if (mode & ModeCO)
					{
						Upper = true;
						if (mode & ModeLO)
							Upper = (bool)GetRandomUINT(0, 1);
						if (Upper)
							new_syllable = UpperString(new_syllable);
					}

					std::vector<pwd_unit> units;
					units.assign(ProperUnits.cbegin(), ProperUnits.cend());
					for (unsigned unit : syllable.UnitsInSyllable)
						units.push_back(pwd_unit(unit, Upper));

					/*
					** If the word has been improperly formed, throw out
					** the syllable.  The checks performed here are those
					** that must be formed on a word basis.  The other
					** tests are performed entirely within the syllable.
					** Otherwise, append the syllable to the word and
					** append the syllable to the hyphenated version of
					** the word.
					*/
					if (AreAllowedSymbolsIn(new_syllable, strExcludeSymbols) &&
						!ImproperWord(units) &&
						!(Units.empty() && HaveInitialY(syllable.UnitsInSyllable)) &&
						!((len == GetLength() + new_syllable.length()) && HaveFinalSplit(syllable.UnitsInSyllable)))
					{
						ProperUnits.assign(units.cbegin(), units.cend());
						for (unsigned unit : syllable.UnitsInSyllable)
							Units.push_back(pwd_unit(unit, Upper));
						Syllables.push_back(new_syllable);
						o_mode &= ~(ModeCO | ModeLO);
					}
				}
				if ((mode & ModeN) && p > 0)
				{
					p -= Numbers.size();
					if (p <= 0)
					{
						if (AddRandomSymbols(Numbers))
							o_mode &= ~ModeNO;
					}

				}
				if ((mode & ModeS) && p > 0)
				{
					p -= Symbols.size();
					if (p <= 0)
					{
						if (AddRandomSymbols(Symbols))
							o_mode &= ~ModeSO;
					}
				}
				if (p > 0)
					AddRandomSymbols(strIncludeSymbols);
			}
			/*
			** Keep track of the times we have tried to get
			** syllables.  If we have exceeded the threshold,
			** reinitialize the pwlen and word_size variables, clear
			** out the word arrays, and start from scratch.
			*/
			tries++;
			if (tries > MaxRetries(len))
			{
				tries = 0;
				Units.clear();
				ProperUnits.clear();
				Syllables.clear();
			}
		}

		/* if obligatory mode were left unused then try again */
		if (GetLength() == len)
		{
			if (o_mode)
			{
				tries = 0;
				Units.clear();
				ProperUnits.clear();
				Syllables.clear();
			}
			else
				break;
		}
	} while (true);

	return true;
}

bool Password::GenerateRandomWord(size_t len)
{
	if (mode == 0 || len == 0)
		return false;

	Units.clear();
	ProperUnits.clear();
	Syllables.clear();

	size_t Weight = GetWeightRandom(mode, strIncludeSymbols);
	if (Weight == 0)
		return false;

	do
	{
		int o_mode = (mode & (ModeNO | ModeSO | ModeCO | ModeLO));

		do
		{
			int p = GetRandomUINT(1, Weight);
			char ch = NULL;
			if (mode & ModeC)
			{
				p -= UpperChars.size();
				if (p <= 0)
				{
					ch = UpperChars[GetRandomUINT(0, UpperChars.size() - 1)];
					o_mode &= ~ModeCO;
				}
			}
			if ((mode & ModeL) && p > 0)
			{
				p -= LowerChars.size();
				if (p <= 0)
				{
					ch = LowerChars[GetRandomUINT(0, LowerChars.size() - 1)];
					o_mode &= ~ModeLO;
				}
			}
			if ((mode & ModeN) && p > 0)
			{
				p -= Numbers.size();
				if (p <= 0)
				{
					ch = Numbers[GetRandomUINT(0, Numbers.size() - 1)];
					o_mode &= ~ModeNO;
				}

			}
			if ((mode & ModeS) && p > 0)
			{
				p -= Symbols.size();
				if (p <= 0)
				{
					ch = Symbols[GetRandomUINT(0, Symbols.size() - 1)];
					o_mode &= ~ModeSO;
				}
			}
			if (p > 0)
				ch = strIncludeSymbols[GetRandomUINT(0, strIncludeSymbols.length() - 1)];

			std::string name;
			if (!GetSymbolName(ch, &name))
				name = ch;
			Units.push_back(pwd_unit(ch, name, false));
			Syllables.push_back(name);
		} while (GetLength() < len);
		if (o_mode)
		{
			Units.clear();
			Syllables.clear();
		}
		else
			return true;
	} while (true);
}

/*
 * Check that the word does not contain illegal combinations
 * that may span syllables.  Specifically, these are:
 *   1. An illegal pair of units between syllables.
 *   2. Three consecutive vowel units.
 *   3. Three consecutive consonant units.
 * The checks are made against units (1 or 2 letters), not against
 * the individual letters, so three consecutive units can have
 * the length of 6 at most.
 */
bool Password::ImproperWord(const std::vector<pwd_unit>& pwd_units)
{
	for (size_t unit_count = 0; unit_count < pwd_units.size(); unit_count++)
	{
		/*
		 * Check for ILLEGAL_PAIR.  This should have been caught
		 * for units within a syllable, but in some cases it
		 * would have gone unnoticed for units between syllables
		 * (e.g., when saved_unit's in get_syllable() were not
		 * used).
		 */
		if ((unit_count != 0) && (Digram[pwd_units[unit_count - 1].unit][pwd_units[unit_count].unit] & ILLEGAL_PAIR))
			return true;

		/*
		 * Check for consecutive vowels or consonants.  Because
		 * the initial y of a syllable is treated as a consonant
		 * rather than as a vowel, we exclude y from the first
		 * vowel in the vowel test.  The only problem comes when
		 * y ends a syllable and two other vowels start the next,
		 * like fly-oint.  Since such words are still
		 * pronounceable, we accept this.
		 */
		if (unit_count >= 2)
		{
			/*
			 * Vowel check.
			 */
			if ((((Rules[pwd_units[unit_count - 2].unit].flags & VOWEL) && !(Rules[pwd_units[unit_count - 2].unit].flags & ALTERNATE_VOWEL)) &&
				(Rules[pwd_units[unit_count - 1].unit].flags & VOWEL) && (Rules[pwd_units[unit_count].unit].flags & VOWEL)) ||
				/*
				 * Consonant check.
				 */
				 (!(Rules[pwd_units[unit_count - 2].unit].flags & VOWEL) &&
				  !(Rules[pwd_units[unit_count - 1].unit].flags & VOWEL) &&
				  !(Rules[pwd_units[unit_count].unit].flags & VOWEL)))
				return true;
		}
	}

	return false;
}

/*
 * Treating y as a vowel is sometimes a problem.  Some words
 * get formed that look irregular.  One special group is when
 * y starts a word and is the only vowel in the first syllable.
 * The word ycl is one example.  We discard words like these.
 */
bool Password::HaveInitialY(const std::vector<unsigned>& units)
{
	size_t vowel_count = 0;
	size_t normal_vowel_count = 0;

	for (auto uit = units.begin(); uit != units.end(); uit++)
	{
		/*
		 * Count vowels.
		 */
		if (Rules[*uit].flags & VOWEL)
		{
			vowel_count++;

			/*
			 * Count the vowels that are not: 1. y, 2. at the start of
			 * the word.
			 */
			if (!(Rules[*uit].flags & ALTERNATE_VOWEL) || (uit != units.begin()))
				normal_vowel_count++;
		}
	}
	return ((vowel_count <= 1) && (normal_vowel_count == 0));
}

/*
 * Besides the problem with the letter y, there is one with
 * a silent e at the end of words, like face or nice.  We
 * allow this silent e, but we do not allow it as the only
 * vowel at the end of the word or syllables like ble will
 * be generated.
 */
bool Password::HaveFinalSplit(const std::vector<unsigned>& units)
{
	size_t vowel_count = 0;

	/*
	 *    Count all the vowels in the word.
	 */
	 //for (size_t unit_count = 0; unit_count < units.size(); unit_count++)
	for (auto uit = units.begin(); uit != units.end(); uit++)
	{
		if (Rules[*uit].flags & VOWEL)
			vowel_count++;
	}

	/*
	 * Return true if the only vowel was e, found at the end if the
	 * word.
	 */
	return ((vowel_count == 1) && (Rules[units.back()].flags & NO_FINAL_SPLIT));
}

bool Password::GetSymbolName(char symbol, std::string* name)
{
	auto p = std::find_if(SymbolNames.cbegin(), SymbolNames.cend(), [&, symbol](auto& s) { return (s.symbol == symbol); });

	if (p == SymbolNames.cend())
		return false;
	*name = p->name;
	return true;
}

char Password::UpperChar(char ch)
{
	auto a = std::find(LowerChars.cbegin(), LowerChars.cend(), ch);
	if (LowerChars.cend() == a)
		return ch;
	return *((a - LowerChars.cbegin()) + UpperChars.cbegin());
}

bool Password::AreAllowedSymbolsIn(const std::string& strWord, const std::string& strExclude)
{
	return (strWord.find_first_of(strExclude) == std::string::npos);
}

size_t Password::GetWeight(int mode, const std::string& strIncludeSymbols)
{
	size_t Weight = 0;

	if (mode & (ModeCO | ModeLO))
		Weight += WeightCL;
	if (mode & ModeN)
		Weight += Numbers.size();
	if (mode & ModeS)
		Weight += Symbols.size();
	Weight += strIncludeSymbols.length();
	return Weight;
}

size_t Password::GetWeightRandom(int mode, const std::string& strIncludeSymbols)
{
	size_t Weight = 0;

	if (mode & ModeC)
		Weight += UpperChars.size();
	if (mode & ModeL)
		Weight += LowerChars.size();
	if (mode & ModeN)
		Weight += Numbers.size();
	if (mode & ModeS)
		Weight += Symbols.size();
	Weight += strIncludeSymbols.length();
	return Weight;
}

#if !defined(_DEBUG)
long Password::IO_Validate0(long result)
{
	if (0 != result)
		throw CWin32Error();
	return result;
}

bool Password::Timeout(_timeb& start)
{
	__timeb64 stop;
	IO_Validate0(_ftime64_s(&stop));
	__time64_t d = stop.time - start.time;
	d *= 1000;
	d += (stop.millitm - start.millitm);
	if (d >= TIMEOUT)
		return true;
	return false;
}
#endif
