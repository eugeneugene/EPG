#pragma once

#if !defined(_DEBUG)
#include <sys\timeb.h>
#endif

#include "fips181.h"
#include "rand.h"
#include "Syllable.h"
#include "PwdUnit.h"
#include "..\Common\Win32ErrorEx.h"
#include "..\Common\WideHelp.h"

class CPassword
{
private:
	const size_t WeightCL = UpperChars.size() + LowerChars.size();

	std::vector<PwdUnit> Units;
	std::vector<PwdUnit> ProperUnits;
	std::vector<std::_tstring> Syllables;
	int mode;
	std::_tstring strIncludeSymbols;
	std::_tstring strExcludeSymbols;

public:
	CPassword(int _mode, const std::_tstring&& _strIncludeSymbols, const std::_tstring&& _strExcludeSymbols) : mode(_mode), strIncludeSymbols(_strIncludeSymbols), strExcludeSymbols(_strExcludeSymbols)
	{ }
	CPassword(int _mode, const std::_tstring& _strIncludeSymbols, const std::_tstring& _strExcludeSymbols) : mode(_mode), strIncludeSymbols(_strIncludeSymbols), strExcludeSymbols(_strExcludeSymbols)
	{ }
	CPassword(int _mode, const TCHAR* pIncludeSymbols, const TCHAR* pExcludeSymbols) : mode(_mode), strIncludeSymbols(pIncludeSymbols), strExcludeSymbols(pExcludeSymbols)
	{ }

	bool GenerateWord(unsigned length);
	bool GenerateRandomWord(unsigned length);

	void GetWord(std::_tstring& out) const
	{
		out.clear();
		for (const auto& u : Units)
		{
			out += *u.UnitCode();
		}
	}

	void GetRandomWord(std::_tstring& out) const
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

	unsigned GetLength() const
	{
		unsigned len = 0;
		for (const auto& u : Units)
			len += static_cast<unsigned>((*u.UnitCode()).length());
		return len;
	}

	unsigned GetRandomLength() const
	{
		unsigned len = 0;
		for (const auto& s : Syllables)
			len += static_cast<unsigned>(s.length());
		return len;
	}

private:
	size_t GetWeight(int mode, const std::_tstring& strIncludeSymbols) const;
	size_t GetWeightRandom(int mode, const std::_tstring& strIncludeSymbols) const;

	static bool ImproperWord(const std::vector<PwdUnit>& units);
	static bool HaveInitialY(const std::vector<unsigned>& units);
	static bool HaveFinalSplit(const std::vector<unsigned>& units);

	static bool GetSymbolName(TCHAR symbol, std::_tstring& out);
	static bool AreAllowedSymbolsIn(const std::_tstring& strWord, const std::_tstring& strExclude);

	bool IsAllowedSymbol(TCHAR ch) const
	{
		return strExcludeSymbols.find(ch, 0) == std::_tstring::npos;
	}

	template <typename T>
	bool AddRandomSymbols(const T& Symbols)
	{
		if (Symbols.empty())
			return false;
		TCHAR symbol = Symbols[GetRandomUINT(0, (UINT)Symbols.size() - 1)];
		if (!IsAllowedSymbol(symbol))
			return false;
		std::_tstring name;
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

