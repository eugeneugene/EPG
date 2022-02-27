#include "pch.h"

#include "CppUnitTest.h"
#include <stdlib.h>
#include <Windows.h>

#include <algorithm>
#include <clocale>
#include <format>
#include <string>
#include <codecvt>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace LocaleTest
{
	TEST_CLASS(WideTest)
	{
		wchar_t mbtowc(char ch)
		{
			wchar_t out;
			auto locale = _create_locale(LC_ALL, "");
			auto converted = _mbtowc_l(&out, &ch, 1, locale);
			_free_locale(locale);
			if (converted != 1)
				throw std::runtime_error("Char conversion error");
			return out;
		}
		wchar_t mbtowc(char ch, _locale_t locale)
		{
			wchar_t out;
			auto converted = _mbtowc_l(&out, &ch, 1, locale);
			if (converted != 1)
				throw std::runtime_error("Char conversion error");
			return out;
		}

		char wtombc(wchar_t wch)
		{
			char out;
			auto locale = _create_locale(LC_ALL, "");
			int RetVal = 0;
			auto error = _wctomb_s_l(&RetVal, &out, 1, wch, locale);
			_free_locale(locale);
			if(error != 0)
				throw std::runtime_error("Char conversion error");
			return out;
		}
		char wtombc(wchar_t wch, _locale_t locale)
		{
			char out;
			int RetVal = 0;
			auto error = _wctomb_s_l(&RetVal, &out, 1, wch, locale);
			if(error != 0)
				throw std::runtime_error("Char conversion error");
			return out;
		}

		std::wstring toWideString(const char* pStr, int len)
		{
			// figure out how many wide characters we are going to get
			int nChars = MultiByteToWideChar(CP_ACP, 0, pStr, len, NULL, 0);
			if (len == -1)
				--nChars;
			if (nChars == 0)
				return L"";

			// convert the narrow string to a wide string 
			// nb: slightly naughty to write directly into the string like this
			std::wstring buf;
			buf.resize(nChars);
			MultiByteToWideChar(CP_ACP, 0, pStr, len,
				const_cast<wchar_t*>(buf.c_str()), nChars);

			return buf;
		}

		std::string toNarrowString(const wchar_t* pStr, int len)
		{
			// figure out how many narrow characters we are going to get 
			int nChars = WideCharToMultiByte(CP_ACP, 0, pStr, len, NULL, 0, NULL, NULL);
			if (len == -1)
				--nChars;
			if (nChars == 0)
				return "";

			// convert the wide string to a narrow string
			// nb: slightly naughty to write directly into the string like this
			std::string buf;
			buf.resize(nChars);
			WideCharToMultiByte(CP_ACP, 0, pStr, len, const_cast<char*>(buf.c_str()), nChars, NULL, NULL);

			return buf;
		}

	public:

		TEST_METHOD(CharToWide1)
		{
			const char str[8] = { 'A','b','C','d','À','á','Â','ã' };
			const wchar_t wstr_ref[8] = { L'A',L'b',L'C',L'd',L'À',L'á',L'Â',L'ã' };
			wchar_t wdst[8] = { 0 };

			for (int i = 0; i < _countof(str); i++)
				wdst[i] = mbtowc(str[i]);

			for (int i = 0; i < _countof(str); i++)
			{
				Logger::WriteMessage(std::format(L"Comparing {} and {}", wdst[i], wstr_ref[i]).c_str());
				Assert::AreEqual(wstr_ref[i], wdst[i]);
			}
		}

		TEST_METHOD(CharToWide2)
		{
			const char str[8] = { 'A','b','C','d','À','á','Â','ã' };
			const wchar_t wstr_ref[8] = { L'A',L'b',L'C',L'd',L'À',L'á',L'Â',L'ã' };
			wchar_t wdst[8] = { 0 };

			auto locale = _create_locale(LC_ALL, "");

			for (int i = 0; i < _countof(str); i++)
				wdst[i] = mbtowc(str[i], locale);

			_free_locale(locale);

			for (int i = 0; i < _countof(str); i++)
			{
				Logger::WriteMessage(std::format(L"Comparing {} and {}", wdst[i], wstr_ref[i]).c_str());
				Assert::AreEqual(wstr_ref[i], wdst[i]);
			}
		}

		TEST_METHOD(StringToWide)
		{
			const std::string str = "AbCdÀáÂã";
			const std::wstring wstr_ref = L"AbCdÀáÂã";
			std::wstring wdst;

			wdst = toWideString(str.c_str(), -1);

			Logger::WriteMessage(std::format(L"Comparing {} and {}", wdst, wstr_ref).c_str());
			Assert::AreEqual(0, wdst.compare(wstr_ref));
		}

		TEST_METHOD(WideToChar1)
		{
			const wchar_t wstr[8] = { L'A',L'b',L'C',L'd',L'À',L'á',L'Â',L'ã' };
			const char str_ref[8] = { 'A','b','C','d','À','á','Â','ã' };
			char dst[8] = { 0 };

			for (int i = 0; i < _countof(wstr); i++)
				dst[i] = wtombc(wstr[i]);

			for (int i = 0; i < _countof(wstr); i++)
			{
				Logger::WriteMessage(std::format(L"Comparing {} and {}", dst[i], str_ref[i]).c_str());
				Assert::AreEqual(str_ref[i], dst[i]);
			}
		}

		TEST_METHOD(WideToChar2)
		{
			const wchar_t wstr[8] = { L'A',L'b',L'C',L'd',L'À',L'á',L'Â',L'ã' };
			const char str_ref[8] = { 'A','b','C','d','À','á','Â','ã' };
			char dst[8] = { 0 };

			auto locale = _create_locale(LC_ALL, "");
			
			for (int i = 0; i < _countof(wstr); i++)
				dst[i] = wtombc(wstr[i], locale);

			_free_locale(locale);
			
			for (int i = 0; i < _countof(wstr); i++)
			{
				Logger::WriteMessage(std::format(L"Comparing {} and {}", dst[i], str_ref[i]).c_str());
				Assert::AreEqual(str_ref[i], dst[i]);
			}
		}

		TEST_METHOD(WideToString)
		{
			const std::wstring wstr = L"AbCdÀáÂã";
			const std::string str_ref = "AbCdÀáÂã";
			std::string dst;

			dst = toNarrowString(wstr.c_str(), -1);

			Logger::WriteMessage(std::format("Comparing {} and {}", dst, str_ref).c_str());
			Assert::AreEqual(0, dst.compare(str_ref));
		}
	};
}
