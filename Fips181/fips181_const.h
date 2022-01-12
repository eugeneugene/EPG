#pragma once

constexpr auto MAX_UNACCEPTABLE = 20;

enum flag_t { NO_SPECIAL_RULE = 0, ALTERNATE_VOWEL = 1, VOWEL = 2, NO_FINAL_SPLIT = 4, NOT_BEGIN_SYLLABLE = 8 };
enum digramflag_t { ANY_COMBINATION = 0, NOT_END = 1, END = 2, SUFFIX = 4, ILLEGAL_PAIR = 8, PREFIX = 16, BREAK = 32, NOT_BEGIN = 64, BEGIN = 128 };

struct SymbolName
{
	const char symbol;
	const char* name;
};

struct Unit
{
	const char unit_code[3];
	const int unit_len;
	const unsigned flags;
};

extern const std::array<char, 26> LowerChars;
extern const std::array<char, 26> UpperChars;
extern const std::array<char, 6> Vowels;
extern const std::array<char, 20> Consonants;
extern const std::array<char, 10> Numbers;
extern const std::array<char, 25> Symbols;

extern const std::array<unsigned, 210> rulers;
extern const std::array<unsigned, 12> vowel_rulers;

extern const std::array<SymbolName, 43> SymbolNames;
extern const std::array<Unit, 34> Rules;
extern const std::array<std::array<int, 34>, 34> Digram;
