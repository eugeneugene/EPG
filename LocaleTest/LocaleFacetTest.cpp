#include "pch.h"
#include "CppUnitTest.h"
#include <clocale>
#include <stdlib.h>
#include <format>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace LocaleTest
{
	TEST_CLASS(LocaleFacetTest)
	{
		char toupper(char ch)
		{
			return std::use_facet<std::ctype<char>>(std::locale("")).toupper(ch);
		}
		wchar_t toupper(wchar_t ch)
		{
			return std::use_facet<std::ctype<wchar_t>>(std::locale("")).toupper(ch);
		}
		char tolower(char ch)
		{
			return std::use_facet<std::ctype<char>>(std::locale("")).tolower(ch);
		}
		wchar_t tolower(wchar_t ch)
		{
			return std::use_facet<std::ctype<wchar_t>>(std::locale("")).tolower(ch);
		}

	public:

		TEST_METHOD(ToUpperTest)
		{
			const char str[8] = { 'A','b','C','d','À','á','Â','ã' };
			const char str_ref[8] = { 'A','B','C','D','À','Á','Â','Ã' };
			char dst[8] = { 0 };

			for (int i = 0; i < _countof(str); i++)
				dst[i] = toupper(str[i]);

			for (int i = 0; i < _countof(str); i++)
			{
				Logger::WriteMessage(std::format("Comparing {} and {}", dst[i], str_ref[i]).c_str());
				Assert::AreEqual(str_ref[i], dst[i]);
			}
		}

		TEST_METHOD(ToUpperWTest)
		{
			const wchar_t str[8] = { L'A',L'b',L'C',L'd',L'À',L'á',L'Â',L'ã' };
			const wchar_t str_ref[8] = { 'A',L'B',L'C',L'D',L'À',L'Á',L'Â',L'Ã' };
			wchar_t dst[8] = { 0 };

			for (int i = 0; i < _countof(str); i++)
				dst[i] = toupper(str[i]);

			for (int i = 0; i < _countof(str); i++)
			{
				Logger::WriteMessage(std::format(L"Comparing {} and {}", dst[i], str_ref[i]).c_str());
				Assert::AreEqual(str_ref[i], dst[i]);
			}
		}

		TEST_METHOD(ToLowerTest)
		{
			const char str[8] = { 'A','b','C','d','À','á','Â','ã' };
			const char str_ref[8] = { 'a','b','c','d','à','á','â','ã' };
			char dst[8] = { 0 };

			for (int i = 0; i < _countof(str); i++)
				dst[i] = tolower(str[i]);

			for (int i = 0; i < _countof(str); i++)
			{
				Logger::WriteMessage(std::format("Comparing {} and {}", dst[i], str_ref[i]).c_str());
				Assert::AreEqual(str_ref[i], dst[i]);
			}
		}

		TEST_METHOD(ToLowerWTest)
		{
			const wchar_t str[8] = { L'A',L'b',L'C',L'd',L'À',L'á',L'Â',L'ã' };
			const wchar_t str_ref[8] = { L'a',L'b',L'c',L'd',L'à',L'á',L'â',L'ã' };
			wchar_t dst[8] = { 0 };

			for (int i = 0; i < _countof(str); i++)
				dst[i] = tolower(str[i]);

			for (int i = 0; i < _countof(str); i++)
			{
				Logger::WriteMessage(std::format(L"Comparing {} and {}", dst[i], str_ref[i]).c_str());
				Assert::AreEqual(str_ref[i], dst[i]);
			}
		}
	};
}
