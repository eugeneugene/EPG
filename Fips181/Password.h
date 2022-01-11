#pragma once
#include <string>
#include <limits.h>
#include "fips181_const.h"
#include "fips181.h"
#include "rand.h"
#include "Syllable.h"
#include "pwd_unit.h"
#include "..\Common\Win32ErrorEx.h"

class Password
{
	const size_t WeightCL = UpperChars.size() + LowerChars.size();

	std::vector<pwd_unit> Units;
	std::vector<pwd_unit> ProperUnits;
	std::vector<std::string> Syllables;
	int mode;
	std::string strIncludeSymbols;
	std::string strExcludeSymbols;

public:
	Password(int _mode, const std::string& _strIncludeSymbols, const std::string& _strExcludeSymbols):mode(_mode), strIncludeSymbols(_strIncludeSymbols), strExcludeSymbols(_strExcludeSymbols)
	{ }
	Password(int _mode, const std::string& _strIncludeSymbols) :mode(_mode), strIncludeSymbols(_strIncludeSymbols)
	{ }

	bool GenerateWord(size_t len);
	bool GenerateRandomWord(size_t len);

	std::string GetWord()
	{
		std::string str;
		for (auto &u : Units)
		{
			if (u.uppercase)
				str += UpperString(u.unit_code);
			else
				str += u.unit_code;
		}
		return str;
	}

	std::string GetHyphenatedWord()
	{
		std::string str;
		bool bFirst = true;
		for (auto &s : Syllables)
		{
			if (!bFirst)
				str += strHyphen;
			else
				bFirst = false;
			str += s;
		}
		return str;
	}


private:
	size_t GetWeight(int mode, const std::string& strIncludeSymbols);
	size_t GetWeightRandom(int mode, const std::string& strIncludeSymbols);

	static bool ImproperWord(const std::vector<pwd_unit>& units);
	static bool HaveInitialY(const std::vector<unsigned>& units);
	static bool HaveFinalSplit(const std::vector<unsigned>& units);

	static bool GetSymbolName(char symbol, std::string* name);
	static char UpperChar(char ch);
	static bool AreAllowedSymbolsIn(const std::string& strWord, const std::string& strExclude);

	size_t GetLength()
	{
		size_t len = 0;
		for (auto &p : Units)
			len += p.unit_code.length();
		return len;
	}

	static std::string UpperString(const std::string& str)
	{
		std::string out;
		for (auto c : str)
			out += UpperChar(c);
		return out;
	}

	bool IsAllowedSymbol(char ch)
	{
		return strExcludeSymbols.find(ch, 0) == std::string::npos;
	}

	template <typename T>
	bool AddRandomSymbols(const T& Symbols)
	{
		if (Symbols.empty())
			return false;
		char symbol = Symbols[GetRandomUINT(0, Symbols.size() - 1)];
		if (!IsAllowedSymbol(symbol))
			return false;
		std::string name;
		if (!GetSymbolName(symbol, &name))
			name = symbol;
		Units.push_back(pwd_unit(symbol, name, false));
		Syllables.push_back(name);
		return true;
	}

	bool AddRandomSymbols(const std::string& Symbols)
	{
		if (Symbols.empty())
			return false;
		char symbol = Symbols[GetRandomUINT(0, Symbols.length() - 1)];
		if (!IsAllowedSymbol(symbol))
			return false;
		std::string name;
		if (!GetSymbolName(symbol, &name))
			name = symbol;
		Units.push_back(pwd_unit(symbol, name, false));
		Syllables.push_back(name);
		return true;
	}

	inline unsigned MaxRetries(size_t len)
	{
		return 4 * len + Rules.size();
	}

	inline int Allowed(const unsigned* UnitsInSyllable, size_t nCurrentUnit, size_t nUnit, int nFlag)
	{
		return Digram[UnitsInSyllable[nCurrentUnit - 1]][nUnit] & nFlag;
	}

#if !defined(_DEBUG)
	static long IO_Validate0(long result);
	static bool Timeout(_timeb& start);
#endif
};

