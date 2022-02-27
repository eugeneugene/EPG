#pragma once
#include <string>
#include <vector>
#include "PasswordItem.h"

class PasswordResult
{
	char mode;
	bool bloom;
	bool complexity;
	bool csv;
	wchar_t separator;

public:
	std::vector<PasswordItem> Results;
	PasswordResult(char _mode, bool _bloom, bool _complexity, bool _csv, wchar_t _separator) :
		mode(_mode),
		bloom(_bloom),
		complexity(_complexity),
		csv(_csv),
		separator(_separator)
	{}

	char GetMode() const
	{
		return mode;
	}
	bool GetBloom() const
	{
		return bloom;
	}
	bool GetComplexity() const
	{
		return complexity;
	}
	bool GetCsv() const
	{
		return csv;
	}
	wchar_t GetSeparator() const
	{
		return separator;
	}
	friend std::wostream& operator<<(std::wostream& os, const PasswordResult& result);
};
