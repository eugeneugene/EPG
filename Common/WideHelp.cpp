#include "pch.h"
#include <cstdlib>
#include <stdexcept>
#include "WideHelp.h"

wchar_t toWideChar(char Char)
{
	wchar_t out;
	auto locale = _create_locale(LC_ALL, "");
	auto converted = _mbtowc_l(&out, &Char, 1, locale);
	_free_locale(locale);
	if (converted != 1)
		throw std::runtime_error("Char conversion error");
	return out;
}

char toNarrowChar(wchar_t WChar)
{
	char out;
	auto locale = _create_locale(LC_ALL, "");
	int RetVal = 0;
	auto error = _wctomb_s_l(&RetVal, &out, 1, WChar, locale);
	_free_locale(locale);
	if (error != 0)
		throw std::runtime_error("Char conversion error");
	return out;
}

std::wstring toWideString(const char* pStr, int len)
{
	int nChars = MultiByteToWideChar(CP_ACP, 0, pStr, len, NULL, 0);
	if (len == -1)
		--nChars;
	if (nChars == 0)
		return L"";

	std::wstring buf;
	buf.resize(nChars);
	MultiByteToWideChar(CP_ACP, 0, pStr, len,
		const_cast<wchar_t*>(buf.c_str()), nChars);

	return buf;
}

std::string toNarrowString(const wchar_t* pStr, int len)
{
	int nChars = WideCharToMultiByte(CP_ACP, 0, pStr, len, NULL, 0, NULL, NULL);
	if (len == -1)
		--nChars;
	if (nChars == 0)
		return "";

	std::string buf;
	buf.resize(nChars);
	WideCharToMultiByte(CP_ACP, 0, pStr, len, const_cast<char*>(buf.c_str()), nChars, NULL, NULL);

	return buf;
}
