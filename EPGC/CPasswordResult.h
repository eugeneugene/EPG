#pragma once
#include <string>

struct CPasswordResult
{
public:
	std::wstring word;
	std::wstring bloom_result;
	std::wstring complexity;

	CPasswordResult()
	{ }

	CPasswordResult(const std::wstring& _word, const std::wstring& _bloom_result, const std::wstring& _complexity) :
		word(_word),
		bloom_result(_bloom_result),
		complexity(_complexity)
	{ }

	CPasswordResult(const CPasswordResult& other) :
		word(other.word),
		bloom_result(other.bloom_result),
		complexity(other.complexity)
	{ }

	CPasswordResult(CPasswordResult&& other) noexcept :
		word(std::move(other.word)),
		bloom_result(std::move(other.bloom_result)),
		complexity(std::move(other.complexity))
	{ }

	CPasswordResult& operator=(const CPasswordResult& other) const
	{
		CPasswordResult result(other);
		return result;
	}
};
