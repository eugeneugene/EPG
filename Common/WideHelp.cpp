#include "pch.h"
#include "WideHelp.h"

std::wstring toWideString(const char* pStr, int len)
{
	// figure out how many wide characters we are going to get
	int nChars = MultiByteToWideChar(CP_ACP, 0, pStr, len, NULL, 0);
	if (len == -1)
		--nChars;
	if (nChars == 0)
		return L"";

	// convert the narrow string to a wide string 
	// nb: slightly naughty to write directly into the string like this
	std::wstring buf;
	buf.resize(nChars);
	MultiByteToWideChar(CP_ACP, 0, pStr, len, const_cast<wchar_t*>(buf.c_str()), nChars);

	return buf;
}

std::string toNarrowString(const wchar_t* pStr, int len)
{
	// figure out how many narrow characters we are going to get 
	int nChars = WideCharToMultiByte(CP_ACP, 0, pStr, len, NULL, 0, NULL, NULL);
	if (len == -1)
		--nChars;
	if (nChars == 0)
		return "";

	// convert the wide string to a narrow string
	// nb: slightly naughty to write directly into the string like this
	std::string buf;
	buf.resize(nChars);
	WideCharToMultiByte(CP_ACP, 0, pStr, len, const_cast<char*>(buf.c_str()), nChars, NULL, NULL);

	return buf;
}
