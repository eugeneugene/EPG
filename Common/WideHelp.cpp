#include "pch.h"
#include <cstdlib>
#include <stdexcept>
#include "WideHelp.h"

wchar_t toWideChar(char Char)
{
	wchar_t out;
	std::string curLocale = setlocale(LC_ALL, "");
	int SizeConverted = mbtowc(&out, &Char, 1);
	setlocale(LC_ALL, curLocale.c_str());
	if (SizeConverted != 1)
		throw std::runtime_error("Char conversion error");
	return out;
}

char toNarrowChar(wchar_t WChar)
{
	char out;
	int SizeConverted;
	std::string curLocale = setlocale(LC_ALL, "");
	wctomb_s(&SizeConverted, &out, 1, WChar);
	setlocale(LC_ALL, curLocale.c_str());
	if (SizeConverted != 1)
		throw std::runtime_error("Char conversion error");
	return out;
}

std::wstring toWideString(const char* pStr, int len)
{
	std::string curLocale = setlocale(LC_ALL, "");
	size_t PtNumOfCharConverted = 0;
	mbstowcs_s(&PtNumOfCharConverted, NULL, 0, pStr, 0);
	if (PtNumOfCharConverted == 0)
		return std::wstring();

	std::wstring buf;
	buf.resize(PtNumOfCharConverted);
	mbstowcs_s(&PtNumOfCharConverted, const_cast<wchar_t*>(buf.c_str()), buf.size(), pStr, _TRUNCATE);
	setlocale(LC_ALL, curLocale.c_str());

	return buf;
}

std::string toNarrowString(const wchar_t* pStr, int len)
{
	std::string curLocale = setlocale(LC_ALL, "");
	size_t PtNumOfCharConverted = 0;
	wcstombs_s(&PtNumOfCharConverted, NULL, 0, pStr, 0);
	if (PtNumOfCharConverted == 0)
		return std::string();

	std::string buf;
	buf.resize(PtNumOfCharConverted);
	wcstombs_s(&PtNumOfCharConverted, const_cast<char*>(buf.c_str()), buf.size(), pStr, _TRUNCATE);
	setlocale(LC_ALL, curLocale.c_str());

	return buf;
}
