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

constexpr char chHyphen = '-';
constexpr char chNewLine = '\n';
constexpr char chSpace = ' ';
constexpr char chVLine = '|';
constexpr char chHLine = '-';
constexpr char chCross = '+';

extern const std::array<char, 26> LowerChars;
extern const std::array<char, 26> UpperChars;
extern const std::array<char, 6> Vowels;
extern const std::array<char, 20> Consonants;
extern const std::array<char, 10> Numbers;
extern const std::array<char, 25> Symbols;
