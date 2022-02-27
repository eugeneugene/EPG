#pragma once
#include <stdlib.h>
#include <ctype.h>
#include <string>
#include <algorithm>
#include <locale>
#include <functional>

class LocaleTWrapper
{
	_locale_t locale;
public:
	LocaleTWrapper(const _locale_t _locale) : locale(_locale)
	{ }

	char toupper(char c)
	{
		return ::_toupper_l(c, locale);
	}

	char tolower(char c)
	{
		return ::_tolower_l(c, locale);
	}

	wchar_t towupper(wchar_t c)
	{
		return ::_towupper_l(c, locale);
	}

	wchar_t towlower(wchar_t c)
	{
		return ::_towlower_l(c, locale);
	}

	std::string toupperstr(const std::string& str)
	{
		std::string result;
		result.reserve(str.size());
		std::transform(str.cbegin(), str.cend(), std::back_inserter(result), std::mem_fn(&LocaleTWrapper::toupper));
		return result;
	}

	std::wstring toupperstr(const std::wstring& str)
	{
		std::wstring result;
		result.reserve(str.size());
		std::transform(str.cbegin(), str.cend(), std::back_inserter(result), std::mem_fn(&LocaleTWrapper::towupper));
		return result;
	}


	inline std::string tolowerstr(const std::string& str) 
	{
		std::string result;
		result.reserve(str.size()); 
		std::transform(str.cbegin(), str.cend(), std::back_inserter(result), std::mem_fn(&LocaleTWrapper::tolower));
		return result;
	}

	inline std::wstring tolowerstr(const std::wstring& wstr) 
	{
		std::wstring result;
		result.reserve(wstr.size()); 
		std::transform(wstr.cbegin(), wstr.cend(), std::back_inserter(result), std::mem_fn(&LocaleTWrapper::towlower));
		return result;
	}
};
