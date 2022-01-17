#include "pch.h"
#include "Password.h"
#include "..\Common\WideHelp.h"

bool CPassword::GenerateWord(unsigned length)
{
	unsigned tries = 0;

	if (length == 0)
		return false;

	if (mode.IsClear())
		return false;

	unsigned Weight = (unsigned)GetWeight();
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
		auto forced = forcedmodes;
		/*
		** Find syllables until the entire word is constructed.
		*/
		while (GetLength() < length)
		{
#ifndef _DEBUG
			if (Timeout(start))
				return false;
#endif
			int p = GetRandomUINT(1, Weight);
			if (mode.IsSet(Modes::Capitals) || mode.IsSet(Modes::Lowers))
			{
				p -= (int)WeightCL;
				if (p <= 0)
				{
					bool Upper = false;
					/*
					** Get the syllable and find its length.
					*/
					Syllable syllable;
					if (!syllable.Generate((int)(length - GetLength())))
					{
						tries = 0;
						Units.clear();
						ProperUnits.clear();
						Syllables.clear();
						continue;
					}
					std::_tstring syllable_str;
					syllable.GetSyllable(syllable_str);
					size_t syllen = syllable_str.length();
#ifdef _DEBUG
					if (syllen > length - GetLength())
						throw std::runtime_error("Internal state error");
#endif
					if (mode.IsSet(Modes::CapitalsForced))
					{
						if (mode.IsSet(Modes::LowersForced))
							Upper = (bool)GetRandomUINT(0, 1);
						else
							Upper = true;
					}
					if (Upper)
						std::transform(syllable_str.cbegin(), syllable_str.cend(), syllable_str.begin(), ::UpperChar);

					std::vector<PwdUnit> units(ProperUnits);
					for (unsigned unit : *syllable.UnitsInSyllable())
						units.push_back(PwdUnit(unit, Upper));

					/*
					** If the word has been improperly formed, throw out
					** the syllable.  The checks performed here are those
					** that must be formed on a word basis.  The other
					** tests are performed entirely within the syllable.
					** Otherwise, append the syllable to the word and
					** append the syllable to the hyphenated version of
					** the word.
					*/
					if (AreAllowedSymbolsIn(syllable_str, strExcludeSymbols) &&
						!ImproperWord(units) &&
						!(Units.empty() && HaveInitialY(*syllable.UnitsInSyllable())) &&
						!((length == GetLength() + syllable_str.length()) && HaveFinalSplit(*syllable.UnitsInSyllable())))
					{
						ProperUnits = units;
						for (unsigned unit : *syllable.UnitsInSyllable())
							Units.push_back(PwdUnit(unit, Upper));
						Syllables.push_back(syllable_str);
						if (Upper)
							forced -= Modes::CapitalsForced;
						else
							forced -= Modes::LowersForced;
					}
				}
				if (mode.IsSet(Modes::Numerals) && p > 0)
				{
					p -= (int)Numbers.size();
					if (p <= 0)
					{
						if (AddRandomSymbols(Numbers))
							forced -= Modes::NumeralsForced;
					}

				}
				if (mode.IsSet(Modes::Symbols) && p > 0)
				{
					p -= (int)Symbols.size();
					if (p <= 0)
					{
						if (AddRandomSymbols(Symbols))
							forced -= Modes::SymbolsForced;
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
			if (tries > MaxRetries(length))
			{
				tries = 0;
				Units.clear();
				ProperUnits.clear();
				Syllables.clear();
			}
		}

		/* if obligatory mode were left unused then try again */
		if (GetLength() >= length)
		{
			if (!forced.IsClear())
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

bool CPassword::GenerateRandomWord(unsigned length)
{
	if (length == 0)
		return false;

	if (mode.IsClear())
		return false;

	Units.clear();
	ProperUnits.clear();
	Syllables.clear();

	size_t Weight = GetWeightRandom();
	if (Weight == 0)
		return false;

	do
	{
		auto forced = forcedmodes;

		do
		{
			int p = GetRandomUINT(1, (UINT)Weight);
			TCHAR ch = NULL;
			if (mode.IsSet(Modes::Capitals))
			{
				p -= (int)UpperChars.size();
				if (p <= 0)
				{
					ch = UpperChars[GetRandomUINT(0, (UINT)UpperChars.size() - 1)];
					forced -= Modes::CapitalsForced;
				}
			}
			if (mode.IsSet(Modes::Lowers) && p > 0)
			{
				p -= (int)LowerChars.size();
				if (p <= 0)
				{
					ch = LowerChars[GetRandomUINT(0, (UINT)LowerChars.size() - 1)];
					forced -= Modes::LowersForced;
				}
			}
			if (mode.IsSet(Modes::Numerals) && p > 0)
			{
				p -= (int)Numbers.size();
				if (p <= 0)
				{
					ch = Numbers[GetRandomUINT(0, (UINT)Numbers.size() - 1)];
					forced -= Modes::NumeralsForced;
				}

			}
			if (mode.IsSet(Modes::Symbols) && p > 0)
			{
				p -= (int)Symbols.size();
				if (p <= 0)
				{
					ch = Symbols[GetRandomUINT(0, (UINT)Symbols.size() - 1)];
					forced -= Modes::SymbolsForced;
				}
			}
			if (p > 0)
				ch = strIncludeSymbols[GetRandomUINT(0, (UINT)strIncludeSymbols.length() - 1)];

			std::_tstring name;
			if (!GetSymbolName(ch, name))
				name = ch;
			Units.push_back(PwdUnit(ch, name, false));
			Syllables.push_back(name);
		} while (GetLength() < length);
		if (!forced.IsClear())
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
bool CPassword::ImproperWord(const std::vector<PwdUnit>& pwd_units)
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
		if ((unit_count != 0) && (Digram[pwd_units[unit_count - 1].Unit()][pwd_units[unit_count].Unit()] & ILLEGAL_PAIR))
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
			if ((((Rules[pwd_units[unit_count - 2].Unit()].flags & VOWEL) && !(Rules[pwd_units[unit_count - 2].Unit()].flags & ALTERNATE_VOWEL)) &&
				(Rules[pwd_units[unit_count - 1].Unit()].flags & VOWEL) && (Rules[pwd_units[unit_count].Unit()].flags & VOWEL)) ||
				/*
				 * Consonant check.
				 */
				(!(Rules[pwd_units[unit_count - 2].Unit()].flags & VOWEL) &&
					!(Rules[pwd_units[unit_count - 1].Unit()].flags & VOWEL) &&
					!(Rules[pwd_units[unit_count].Unit()].flags & VOWEL)))
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
bool CPassword::HaveInitialY(const std::vector<unsigned>& units)
{
	unsigned vowel_count = 0;
	unsigned normal_vowel_count = 0;

	for (auto uit = units.cbegin(); uit != units.cend(); uit++)
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
			if (!(Rules[*uit].flags & ALTERNATE_VOWEL) || (uit != units.cbegin()))
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
bool CPassword::HaveFinalSplit(const std::vector<unsigned>& units)
{
	size_t vowel_count = 0;

	/*
	 *    Count all the vowels in the word.
	 */
	 //for (size_t unit_count = 0; unit_count < units.size(); unit_count++)
	for (auto uit = units.cbegin(); uit != units.cend(); uit++)
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

bool CPassword::GetSymbolName(TCHAR symbol, std::_tstring& out)
{
	auto p = std::find_if(SymbolNames.cbegin(), SymbolNames.cend(), [&, symbol](auto& s) { return (s.symbol == symbol); });

	if (p == SymbolNames.cend())
		return false;
	out = p->name;
	return true;
}

bool CPassword::AreAllowedSymbolsIn(const std::_tstring& strWord, const std::_tstring& strExclude)
{
	return (strWord.find_first_of(strExclude) == std::string::npos);
}

size_t CPassword::GetWeight() const
{
	size_t Weight = 0;

	if (mode.IsSet(Modes::Lowers) || mode.IsSet(Modes::Capitals))
		Weight += WeightCL;
	if (mode.IsSet(Modes::Numerals))
		Weight += Numbers.size();
	if (mode.IsSet(Modes::Symbols))
		Weight += Symbols.size();
	Weight += strIncludeSymbols.length();
	return Weight;
}

size_t CPassword::GetWeightRandom() const
{
	size_t Weight = 0;

	if (mode.IsSet(Modes::Capitals))
		Weight += UpperChars.size();
	if (mode.IsSet(Modes::Lowers))
		Weight += LowerChars.size();
	if (mode.IsSet(Modes::Numerals))
		Weight += Numbers.size();
	if (mode.IsSet(Modes::Symbols))
		Weight += Symbols.size();
	Weight += strIncludeSymbols.length();
	return Weight;
}

#if !defined(_DEBUG)
long CPassword::IO_Validate0(long result)
{
	if (0 != result)
		throw CWin32Error();
	return result;
}

bool CPassword::Timeout(_timeb& start)
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
