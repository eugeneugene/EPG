#pragma once

constexpr int ModeLN = 0x01; // abc
constexpr int ModeCN = 0x02; // ABC
constexpr int ModeNN = 0x04; // 012
constexpr int ModeSN = 0x08; // !@#

constexpr int ModeLO = 0x10;
constexpr int ModeCO = 0x20;
constexpr int ModeNO = 0x40;
constexpr int ModeSO = 0x80;

constexpr int ModeL = ModeLO | ModeLN;
constexpr int ModeC = ModeCO | ModeCN;
constexpr int ModeN = ModeNO | ModeNN;
constexpr int ModeS = ModeSO | ModeSN;

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
extern const std::array<TCHAR, 25> Symbols;

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
	const int unit_len;
	const unsigned flags;
};

extern const std::array<unsigned, 210> rulers;
extern const std::array<unsigned, 12> vowel_rulers;

extern const std::array<SymbolName, 43> SymbolNames;
extern const std::array<Unit, 34> Rules;
extern const std::array<std::array<int, 34>, 34> Digram;
