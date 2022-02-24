#include "pch.h"
#include "EPGCUtils.h"
#include "ModuleVersion.h"

std::wstring check_bloom(const CBloom& bloom, bool paranoid, const std::wstring &word)
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
