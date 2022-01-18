#pragma once

#if !defined(_DEBUG)
#include <sys\timeb.h>
#endif

#include "fips181.h"
#include "PasswordMode.h"
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
	CPasswordMode mode;
	CPasswordMode forcedmode;
	std::_tstring strIncludeSymbols;
	std::_tstring strExcludeSymbols;

public:
	CPassword(Modes modes, const std::_tstring&& _strIncludeSymbols, const std::_tstring&& _strExcludeSymbols) : mode(modes), forcedmode(modes& Modes::AllForced),
		strIncludeSymbols(_strIncludeSymbols), strExcludeSymbols(_strExcludeSymbols)
	{
		FixMode();
	}
	CPassword(Modes modes, const std::_tstring& _strIncludeSymbols, const std::_tstring& _strExcludeSymbols) : mode(modes), forcedmode(modes& Modes::AllForced),
		strIncludeSymbols(_strIncludeSymbols), strExcludeSymbols(_strExcludeSymbols)
	{
		FixMode();
	}
	CPassword(Modes modes, const TCHAR* pIncludeSymbols, const TCHAR* pExcludeSymbols) : mode(modes), forcedmode(modes& Modes::AllForced),
		strIncludeSymbols(pIncludeSymbols), strExcludeSymbols(pExcludeSymbols)
	{
		FixMode();
	}

	bool GenerateWord(unsigned length);
	bool GenerateRandomWord(unsigned length);

	void GetWord(std::_tstring& out) const;
	void GetHyphenatedWord(std::_tstring& out) const;
	unsigned GetLength() const;
	unsigned GetHyphenatedLength() const;

private:
	void FixMode();
	
	size_t GetWeight() const;
	size_t GetWeightRandom() const;
	inline bool IsAllowedSymbol(TCHAR ch) const;
	inline bool GetUpper() const;

	template <typename T>
	bool AddRandomSymbols(const T& Symbols);

	static bool ImproperWord(const std::vector<PwdUnit>& units);
	static bool HaveInitialY(const std::vector<unsigned>& units);
	static bool HaveFinalSplit(const std::vector<unsigned>& units);
	static bool GetSymbolName(TCHAR symbol, std::_tstring& out);
	static bool AreAllowedSymbolsIn(const std::_tstring& strWord, const std::_tstring& strExclude);
	static inline unsigned MaxRetries(size_t len);

#if !defined(_DEBUG)
	static long IO_Validate0(long result);
	static bool Timeout(_timeb& start);
#endif
};
