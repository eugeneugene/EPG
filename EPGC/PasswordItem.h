#pragma once
#include <string>
#include <iostream>
#include "EPGCUtils.h"

struct PasswordItem
{
	std::wstring word;
	std::wstring hword;
	BloomResult bloom_result;
	float complexity;

	PasswordItem() : bloom_result(BloomResult::UNKNOWN), complexity(0.0)
	{ }

	PasswordItem(const std::wstring& _word, const std::wstring& _hword, BloomResult _bloom_result, float _complexity) :
		word(_word),
		hword(_hword),
		bloom_result(_bloom_result),
		complexity(_complexity)
	{ }

	PasswordItem(const PasswordItem& other) :
		word(other.word),
		hword(other.hword),
		bloom_result(other.bloom_result),
		complexity(other.complexity)
	{ }

	PasswordItem(PasswordItem&& other) noexcept :
		word(std::move(other.word)),
		hword(std::move(other.hword)),
		bloom_result(std::exchange(other.bloom_result, BloomResult::UNKNOWN)),
		complexity(std::exchange(other.complexity, 0.0f))
	{ }

	PasswordItem& operator=(const PasswordItem& other) const
	{
		PasswordItem result(other);
		return result;
	}

	PasswordItem& operator=(PasswordItem&& other) noexcept
	{
		if (this != &other)
		{
			word = std::move(other.word);
			hword = std::move(other.hword);
			bloom_result = std::exchange(other.bloom_result, BloomResult::UNKNOWN);
			complexity = std::exchange(other.complexity, 0.0f);
		}
		return *this;
	}
};
