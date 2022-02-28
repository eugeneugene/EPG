#include "pch.h"
#include "EPGCUtils.h"
#include "ModuleVersion.h"
#include "LocaleTWrapper.h"

BloomResult check_bloom(const CBloom& bloom, bool paranoid, const std::wstring& word)
{
	if (bloom.GetSize() == 0)
		return BloomResult::UNKNOWN;
	if (bloom.Check(word.c_str()))
		return BloomResult::FOUND;
	if (paranoid)
	{
		auto locale = _wcreate_locale(LC_ALL, L"");
		LocaleTWrapper wrapper(locale);

		std::wstring upper = wrapper.toupper(word);
		if (bloom.Check(upper.c_str()))
			return BloomResult::NOTSAFE;
		
		std::wstring lower = wrapper.tolower(word);
		if (bloom.Check(lower.c_str()))
			return BloomResult::NOTSAFE;

		lower[0] = upper[0];
		if (bloom.Check(lower.c_str()))
			return BloomResult::NOTSAFE;
		
		_free_locale(locale);
	}
	return BloomResult::NOTFOUND;
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
	if (version.GetValue(L"LegalTradeMarks", &buf))
		strTrademark = buf;
	std::wstring strVersion;
	if (version.GetValue(L"FileVersion", &buf))
		strVersion = buf;
	std::wstring strCopyright;
	if (version.GetValue(L"LegalCopyright", &buf))
		strCopyright = buf;
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

unsigned PasswordBits(const std::wstring& Password)
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

	double dblBitsPerChar = log((double)dwCharSpace) / log(2.0);
	unsigned dwBits = static_cast<unsigned>(ceil(dblBitsPerChar * Password.length()));
	if (dwBits > 128)
		dwBits = 128;

	return dwBits;
}
