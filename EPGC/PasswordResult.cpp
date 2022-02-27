#include "pch.h"
#include "PasswordResult.h"

std::wostream& operator<<(std::wostream& os, const PasswordResult& result)
{
	int wordlen = 0;
	int hwordlen = 0;

	if (result.GetCsv())
	{
		for (const PasswordItem& item : result.Results)
		{
			switch (result.GetMode())
			{
			case 'r':
			case 'p':
				os << item.word;
				break;
			case 'h':
				os << item.word << result.GetSeparator() << item.hword;
				break;
			case 'H':
				os << item.hword;
				break;
			}

			if (result.GetBloom())
			{
				os << result.GetSeparator();
				switch (item.bloom_result)
				{
				case BloomResult::UNKNOWN:
					os << L"UNKNOWN";
					break;
				case BloomResult::FOUND:
					os << L"FOUND";
					break;
				case BloomResult::NOTFOUND:
					os << L"NOTFOUND";
					break;
				case BloomResult::NOTSAFE:
					os << L"NOTSAFE";
					break;
				}
			}

			if (result.GetComplexity())
				os << result.GetSeparator() << std::format(L"{:.1f}%", item.complexity);
			os << std::endl;
		}
	}
	else
	{
		switch (result.GetMode())
		{
		case 'r':
		case 'p':
			for (const PasswordItem& item : result.Results)
				wordlen = std::max(wordlen, static_cast<int>(item.word.length()));
			break;
		case 'h':
			for (const PasswordItem& item : result.Results)
			{
				wordlen = std::max(wordlen, static_cast<int>(item.word.length()));
				hwordlen = std::max(hwordlen, static_cast<int>(item.hword.length()));
			}
			break;
		case 'H':
			for (const PasswordItem& item : result.Results)
				hwordlen = std::max(hwordlen, static_cast<int>(item.hword.length()));
			break;
		}

		for (const PasswordItem& item : result.Results)
		{
			switch (result.GetMode())
			{
			case 'r':
			case 'p':
				os << item.word;
				for (int i = static_cast<int>(item.word.length()); i <= wordlen; i++)
					os << L' ';
				break;
			case 'h':
				os << item.word;
				for (int i = static_cast<int>(item.word.length()); i <= wordlen; i++)
					os << L' ';
				[[fallthrough]];
			case 'H':
				os << item.hword;
				for (int i = static_cast<int>(item.hword.length()); i <= hwordlen; i++)
					os << L' ';
				break;
			}

			if (result.GetBloom())
			{
				switch (item.bloom_result)
				{
				case BloomResult::UNKNOWN:
					os << L"UNKNOWN   ";
					break;
				case BloomResult::FOUND:
					os << L"FOUND     ";
					break;
				case BloomResult::NOTFOUND:
					os << L"NOTFOUND  ";
					break;
				case BloomResult::NOTSAFE:
					os << L"NOTSAFE   ";
					break;
				}
			}

			if (result.GetComplexity())
				os << std::format(L"{:.1f}%", item.complexity);
			os << std::endl;
		}
	}
	return os;
}
