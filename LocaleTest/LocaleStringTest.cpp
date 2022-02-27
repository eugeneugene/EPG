#include "pch.h"
#include "CppUnitTest.h"
#include <clocale>
#include <stdlib.h>
#include <format>

#include "LocaleTWrapper.h"

using namespace Microsoft::VisualStudio::CppUnitTestFramework;

namespace LocaleTest
{
	TEST_CLASS(LocaleStringTest)
	{
	public:

		TEST_METHOD(ToUpperTest)
		{
			const std::string str = "AbCdÀáÂã";
			const std::string str_ref = "ABCDÀÁÂÃ";
			std::string dst;

			auto locale = _create_locale(LC_ALL, "");
			LocaleTWrapper wrapper(locale);

			dst = wrapper.toupper(str);

			_free_locale(locale);

			Logger::WriteMessage(std::format("Comparing {} and {}", dst, str_ref).c_str());
			Assert::AreEqual(0, dst.compare(str_ref));
		}

		TEST_METHOD(ToUpperWTest)
		{
			const std::wstring wstr = L"AbCdÀáÂã";
			const std::wstring wstr_ref = L"ABCDÀÁÂÃ";
			std::wstring wdst;

			auto locale = _wcreate_locale(LC_ALL, L"");
			LocaleTWrapper wrapper(locale);

			wdst = wrapper.toupper(wstr);

			_free_locale(locale);

			Logger::WriteMessage(std::format(L"Comparing {} and {}", wdst, wstr_ref).c_str());
			Assert::AreEqual(0, wdst.compare(wstr_ref));
		}

		TEST_METHOD(ToLowerTest)
		{
			const std::string str = "AbCdÀáÂã";
			const std::string str_ref = "abcdàáâã";
			std::string dst;

			auto locale = _create_locale(LC_ALL, "");
			LocaleTWrapper wrapper(locale);

			dst = wrapper.tolower(str);

			_free_locale(locale);

			Logger::WriteMessage(std::format("Comparing {} and {}", dst, str_ref).c_str());
			Assert::AreEqual(0, dst.compare(str_ref));
		}

		TEST_METHOD(ToLowerWTest)
		{
			const std::wstring wstr = L"AbCdÀáÂã";
			const std::wstring wstr_ref = L"abcdàáâã";
			std::wstring wdst;

			auto locale = _wcreate_locale(LC_ALL, L"");
			LocaleTWrapper wrapper(locale);

			wdst = wrapper.tolower(wstr);

			_free_locale(locale);

			Logger::WriteMessage(std::format(L"Comparing {} and {}", wdst, wstr_ref).c_str());
			Assert::AreEqual(0, wdst.compare(wstr_ref));
		}	
	};
}
