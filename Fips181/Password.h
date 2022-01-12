#pragma once
#include <string>
#include <limits.h>
#include "fips181_const.h"
#include "fips181.h"
#include "rand.h"
#include "Syllable.h"
#include "PwdUnit.h"
#include "..\Common\Win32ErrorEx.h"

class CPassword
{
	const size_t WeightCL = UpperChars.size() + LowerChars.size();

	std::vector<PwdUnit> Units;
	std::vector<PwdUnit> ProperUnits;
	std::vector<std::string> Syllables;
	int mode;
	std::string strIncludeSymbols;
	std::string strExcludeSymbols;

public:
	CPassword(int _mode, const std::string& _strIncludeSymbols, const std::string& _strExcludeSymbols) : mode(_mode), strIncludeSymbols(_strIncludeSymbols), strExcludeSymbols(_strExcludeSymbols)
	{ }
	CPassword(int _mode, const std::string& _strIncludeSymbols) : mode(_mode), strIncludeSymbols(_strIncludeSymbols)
	{ }

	bool GenerateWord(size_t len);
	bool GenerateRandomWord(size_t len);

	void GetWord(std::string& out) const
	{
		out.clear();
		for (const auto& u : Units)
		{
			out += *u.UnitCode();
		}
	}

	void GetHyphenatedWord(std::string& out) const
	{
		bool bFirst = true;
		for (const auto& s : Syllables)
		{
			if (!bFirst)
				out += chHyphen;
			else
				bFirst = false;
			out += s;
		}
	}


private:
	size_t GetWeight(int mode, const std::string& strIncludeSymbols) const;
	size_t GetWeightRandom(int mode, const std::string& strIncludeSymbols) const;

	static bool ImproperWord(const std::vector<PwdUnit>& units);
	static bool HaveInitialY(const std::vector<unsigned>& units);
	static bool HaveFinalSplit(const std::vector<unsigned>& units);

	static bool GetSymbolName(char symbol, std::string& out);
	static bool AreAllowedSymbolsIn(const std::string& strWord, const std::string& strExclude);

	size_t GetLength() const
	{
		size_t len = 0;
		for (const auto& p : Units)
			len += (*p.UnitCode()).length();
		return len;
	}

	bool IsAllowedSymbol(char ch) const
	{
		return strExcludeSymbols.find(ch, 0) == std::string::npos;
	}

	template <typename T>
	bool AddRandomSymbols(const T& Symbols)
	{
		if (Symbols.empty())
			return false;
		char symbol = Symbols[GetRandomUINT(0, (UINT)Symbols.size() - 1)];
		if (!IsAllowedSymbol(symbol))
			return false;
		std::string name;
		if (!GetSymbolName(symbol, name))
			name = symbol;
		Units.push_back(PwdUnit(symbol, name, false));
		Syllables.push_back(name);
		return true;
	}

	static inline unsigned MaxRetries(size_t len)
	{
		return (unsigned)(4 * len + Rules.size());
	}

#if !defined(_DEBUG)
	static long IO_Validate0(long result);
	static bool Timeout(_timeb& start);
#endif
};

