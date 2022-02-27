#include "pch.h"
#include "CppUnitTest.h"
#include <clocale>
#include <stdlib.h>
#include <format>

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace LocaleTest
{
	TEST_CLASS(LocaleTest)
	{
	public:

		TEST_METHOD(ToUpperTest)
		{
			const char str[8] = { 'A','b','C','d','�','�','�','�' };
			const char str_ref[8] = { 'A','B','C','D','�','�','�','�' };
			char dst[8] = { 0 };

			auto locale = _create_locale(LC_ALL, "");

			for (int i = 0; i < _countof(str); i++)
				dst[i] = _toupper_l(str[i], locale);

			_free_locale(locale);

			for (int i = 0; i < _countof(str); i++)
			{
				Logger::WriteMessage(std::format("Comparing {} and {}", dst[i], str_ref[i]).c_str());
				Assert::AreEqual(str_ref[i], dst[i]);
			}
		}

		TEST_METHOD(ToUpperWTest)
		{
			const wchar_t str[8] = { L'A',L'b',L'C',L'd',L'�',L'�',L'�',L'�' };
			const wchar_t str_ref[8] = { 'A',L'B',L'C',L'D',L'�',L'�',L'�',L'�' };
			wchar_t dst[8] = { 0 };

			auto locale = _wcreate_locale(LC_ALL, L"");

			for (int i = 0; i < _countof(str); i++)
				dst[i] = _towupper_l(str[i], locale);

			_free_locale(locale);

			for (int i = 0; i < _countof(str); i++)
			{
				Logger::WriteMessage(std::format(L"Comparing {} and {}", dst[i], str_ref[i]).c_str());
				Assert::AreEqual(str_ref[i], dst[i]);
			}
		}

		TEST_METHOD(ToLowerTest)
		{
			const char str[8] = { 'A','b','C','d','�','�','�','�' };
			const char str_ref[8] = { 'a','b','c','d','�','�','�','�' };
			char dst[8] = { 0 };

			auto locale = _create_locale(LC_ALL, "");

			for (int i = 0; i < _countof(str); i++)
				dst[i] = _tolower_l(str[i], locale);

			_free_locale(locale);

			for (int i = 0; i < _countof(str); i++)
			{
				Logger::WriteMessage(std::format("Comparing {} and {}", dst[i], str_ref[i]).c_str());
				Assert::AreEqual(str_ref[i], dst[i]);
			}
		}

		TEST_METHOD(ToLowerWTest)
		{
			const wchar_t str[8] = { L'A',L'b',L'C',L'd',L'�',L'�',L'�',L'�' };
			const wchar_t str_ref[8] = { L'a',L'b',L'c',L'd',L'�',L'�',L'�',L'�' };
			wchar_t dst[8] = { 0 };

			auto locale = _wcreate_locale(LC_ALL, L"");

			for (int i = 0; i < _countof(str); i++)
				dst[i] = _towlower_l(str[i], locale);

			_free_locale(locale);

			for (int i = 0; i < _countof(str); i++)
			{
				Logger::WriteMessage(std::format(L"Comparing {} and {}", dst[i], str_ref[i]).c_str());
				Assert::AreEqual(str_ref[i], dst[i]);
			}
		}
	};
}
