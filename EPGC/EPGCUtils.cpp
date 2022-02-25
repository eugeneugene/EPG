#include "pch.h"
#include "EPGCUtils.h"
#include "ModuleVersion.h"

std::wstring check_bloom(const CBloom& bloom, bool paranoid, const std::wstring& word)
{
	if (bloom.GetSize() == 0)
		return std::wstring();
	if (bloom.Check(word.c_str()))
		return std::wstring(L"FOUND");
	if (paranoid)
	{
		boost::locale::generator gen;
		std::locale loc = gen.generate("");

		std::wstring upper = boost::locale::to_upper(word, loc);
		if (bloom.Check(upper.c_str()))
			return std::wstring(L"NOTSAFE");

		std::wstring lower = boost::locale::to_lower(word, loc);
		if (bloom.Check(lower.c_str()))
			return std::wstring(L"NOTSAFE");

		std::wstring title = boost::locale::to_title(lower, loc);
		if (bloom.Check(title.c_str()))
			return std::wstring(L"NOTSAFE");
	}
	return std::wstring(L"NOTFOUND");
}

Modes get_modes(const std::string& set)
{
	Modes modes = Modes::NoMode;
	for (char c : set)
	{
		switch (c)
		{
		case 'l':
			modes += Modes::Lowers;
			break;
		case 'L':
			modes += Modes::LowersForced;
			break;
		case 'c':
			modes += Modes::Capitals;
			break;
		case 'C':
			modes += Modes::CapitalsForced;
			break;
		case 'n':
			modes += Modes::Numerals;
			break;
		case 'N':
			modes += Modes::NumeralsForced;
			break;
		case 's':
			modes += Modes::Symbols;
			break;
		case 'S':
			modes += Modes::SymbolsForced;
			break;
		default:
			throw std::exception(std::format("Invalid mode '{}'", c).c_str());
		}
	}

	return modes;
}

void ShowBuild(boost::iostreams::filtering_wostream& fout/* = fcout*/)
{
	CModuleVersion version;
	wchar_t szModule[MAX_PATH];
	if (GetModuleFileName(GetModuleHandle(nullptr), szModule, MAX_PATH) == 0)
		throw CWin32Error();
	version.GetFileVersionInfo(szModule);

	wchar_t* buf;
	std::wstring strTrademark;
	if (version.GetValue(L"LegalTradeMarks", &buf)) strTrademark = buf;
	std::wstring strVersion;
	if (version.GetValue(L"FileVersion", &buf)) strVersion = buf;
	std::wstring strCopyright;
	if (version.GetValue(L"LegalCopyright", &buf)) strCopyright = buf;
	std::wstring strUsage;

	fout << strTrademark << L' '
		<< strVersion << std::endl
		<< strCopyright << std::endl;
#ifdef _DEBUG
	fout << L"Debug build" << std::endl;
#endif
	fout << std::endl;
}

const int nCHARSPACE_ESCAPE = 60;
const int nCHARSPACE_ALPHA = 26;
const int nCHARSPACE_NUMBER = 10;
const int nCHARSPACE_SIMPSPECIAL = 16;
const int nCHARSPACE_EXTSPECIAL = 17;
const int nCHARSPACE_HIGH = 112;

DWORD PasswordBits(const std::wstring& Password)
{
	BOOL bChLower = FALSE, bChUpper = FALSE, bChNumber = FALSE;
	BOOL bChSimpleSpecial = FALSE, bChExtSpecial = FALSE, bChHigh = FALSE;
	BOOL bChEscape = FALSE;

	if (!Password.length())
		return 0; // Zero bits of information :)

	for (auto ch : Password) // Get character types
	{
		if (ch < L' ')
			bChEscape = TRUE;
		if (ch >= L'A' && ch <= L'Z')
			bChUpper = TRUE;
		if (ch >= L'a' && ch <= 'z')
			bChLower = TRUE;
		if (ch >= L'0' && ch <= L'9')
			bChNumber = TRUE;
		if (ch >= L' ' && ch <= L'/')
			bChSimpleSpecial = TRUE;
		if (ch >= L':' && ch <= L'@')
			bChExtSpecial = TRUE;
		if (ch >= L'[' && ch <= L'`')
			bChExtSpecial = TRUE;
		if (ch >= L'{' && ch <= L'~')
			bChExtSpecial = TRUE;
		if (ch > L'~')
			bChHigh = TRUE;
	}

	auto dwCharSpace = 0;
	if (bChEscape == TRUE)
		dwCharSpace += nCHARSPACE_ESCAPE;
	if (bChUpper == TRUE)
		dwCharSpace += nCHARSPACE_ALPHA;
	if (bChLower == TRUE)
		dwCharSpace += nCHARSPACE_ALPHA;
	if (bChNumber == TRUE)
		dwCharSpace += nCHARSPACE_NUMBER;
	if (bChSimpleSpecial == TRUE)
		dwCharSpace += nCHARSPACE_SIMPSPECIAL;
	if (bChExtSpecial == TRUE)
		dwCharSpace += nCHARSPACE_EXTSPECIAL;
	if (bChHigh == TRUE)
		dwCharSpace += nCHARSPACE_HIGH;

	if (dwCharSpace == 0)
		return 0;

	auto dblBitsPerChar = log((double)dwCharSpace) / log(2.00);
	auto dwBits = (DWORD)ceil(dblBitsPerChar * static_cast<double>(Password.length()));
	if (dwBits > 128)
		dwBits = 128;

	return dwBits;
}
