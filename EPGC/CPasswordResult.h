#pragma once
#include <string>

struct CPasswordResult
{
	std::wstring word;
	std::wstring bloom_result;
	std::wstring complexity;
};

std::wstring CheckBloom(const CBloom& bloom, bool paranoid, const std::wstring(word));
