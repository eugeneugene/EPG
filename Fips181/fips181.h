#pragma once
#include <Windows.h>
#include <tchar.h>
#include <array>

constexpr int TIMEOUT = 1000;

constexpr int MAXSYMBOLLEN = 17;
constexpr int MAXINCLUDE = 256;
constexpr int MAXEXCLUDE = 256;
constexpr int MAXLEN = 128;
constexpr int MINLEN = 4;
constexpr int MAXWORDS = 65536;

constexpr TCHAR chHyphen = '-';
constexpr TCHAR chNewLine = '\n';
constexpr TCHAR chSpace = ' ';
constexpr TCHAR chVLine = '|';
constexpr TCHAR chHLine = '-';
constexpr TCHAR chCross = '+';

extern const std::array<TCHAR, 26> LowerChars;
extern const std::array<TCHAR, 26> UpperChars;
extern const std::array<TCHAR, 6> Vowels;
extern const std::array<TCHAR, 20> Consonants;
extern const std::array<TCHAR, 10> Numbers;
extern const std::array<TCHAR, 32> Symbols;

constexpr auto MAX_UNACCEPTABLE = 20;

enum flag_t { NO_SPECIAL_RULE = 0, ALTERNATE_VOWEL = 1, VOWEL = 2, NO_FINAL_SPLIT = 4, NOT_BEGIN_SYLLABLE = 8 };
enum digramflag_t { ANY_COMBINATION = 0, NOT_END = 1, END = 2, SUFFIX = 4, ILLEGAL_PAIR = 8, PREFIX = 16, BREAK = 32, NOT_BEGIN = 64, BEGIN = 128 };

struct SymbolName
{
	const TCHAR symbol;
	const TCHAR* name;
};

struct Unit
{
	const TCHAR unit_code[3];
	const unsigned unit_len;
	const unsigned flags;
};

extern const std::array<unsigned, 210> rulers;
extern const std::array<unsigned, 12> vowel_rulers;

extern const std::array<SymbolName, 43> SymbolNames;
extern const std::array<Unit, 34> Rules;
extern const std::array<std::array<int, 34>, 34> Digram;

UINT GetRandomUINT(UINT min, UINT max);
TCHAR UpperChar(TCHAR ch);
