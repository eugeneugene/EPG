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
	bool auto_free_locale;
public:
	LocaleTWrapper(const _locale_t _locale, bool _auto_free_locale = false) : locale(_locale), auto_free_locale(_auto_free_locale)
	{ }

	~LocaleTWrapper()
	{
		if (auto_free_locale)
			_free_locale(locale);
	}

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

	std::string toupper(const std::string& str)
	{
		std::string result;
		result.reserve(str.size());
		std::transform(str.cbegin(), str.cend(), std::back_inserter(result), [this](char c) { return this->toupper(c); });
		return result;
	}

	std::wstring toupper(const std::wstring& wstr)
	{
		std::wstring result;
		result.reserve(wstr.size());
		std::transform(wstr.cbegin(), wstr.cend(), std::back_inserter(result), [this](wchar_t c) { return this->towupper(c); });
		return result;
	}

	inline std::string tolower(const std::string& str)
	{
		std::string result;
		result.reserve(str.size());
		std::transform(str.cbegin(), str.cend(), std::back_inserter(result), [this](char c) { return this->tolower(c); });
		return result;
	}

	inline std::wstring tolower(const std::wstring& wstr)
	{
		std::wstring result;
		result.reserve(wstr.size());
		std::transform(wstr.cbegin(), wstr.cend(), std::back_inserter(result), [this](wchar_t c) { return this->towlower(c); });
		return result;
	}
};
