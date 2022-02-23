#include "pch.h"
#include "..\Bloom\Bloom.h"

std::wstring CheckBloom(const CBloom& bloom, bool paranoid, const std::wstring(word))
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
