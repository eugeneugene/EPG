#pragma once

#include <string>
#include <sstream>
#include <Windows.h>

// Wide character helpers
_STD_BEGIN
using _tstring = std::basic_string<TCHAR>;
_STD_END

#if defined(_UNICODE)
#define to_tstring(a) std::to_wstring(a)
#define to_tupper towupper
#define to_tlower towlower
using _tostringstream = std::wostringstream;
#else
#define to_tstring(a) std::to_string(a)
using _tostringstream = std::ostringstream;
#define to_tupper toupper
#define to_tlower tolower
#endif

wchar_t toWideChar(char Char);
std::wstring toWideString(const char* pStr, int len = -1);
inline std::wstring toWideString(const std::string& str)
{
	return toWideString(str.c_str(), (int)str.length());
}
inline std::wstring toWideString(const wchar_t* pStr, int len = -1)
{
	return (len < 0) ? pStr : std::wstring(pStr, len);
}
inline std::wstring toWideString(const std::wstring& str)
{
	return str;
}

char toNarrowChar(wchar_t WChar);
std::string toNarrowString(const wchar_t* pStr, int len = -1);
inline std::string toNarrowString(const std::wstring& str)
{
	return toNarrowString(str.c_str(), (int)str.length());
}
inline std::string toNarrowString(const char* pStr, int  len = -1)
{
	return (len < 0) ? pStr : std::string(pStr, len);
}
inline std::string toNarrowString(const std::string& str)
{
	return str;
}

#ifdef _UNICODE
inline TCHAR toTchar(char ch)
{
	return (wchar_t)ch;
}
inline TCHAR toTchar(wchar_t ch)
{
	return ch;
}
inline std::_tstring toTstring(const std::string& s)
{
	return toWideString(s);
}
inline std::_tstring toTstring(const char* p, int len = -1)
{
	return toWideString(p, (int)len);
}
inline std::_tstring toTstring(const std::wstring& s)
{
	return s;
}
inline std::_tstring toTstring(const wchar_t* p, int len = -1)
{
	return (len < 0) ? p : std::wstring(p, len);
}
#else 
inline TCHAR toTchar(char ch)
{
	return ch;
}
inline TCHAR toTchar(wchar_t ch)
{
	return (ch >= 0 && ch <= 0xFF) ? (char)ch : '?';
}
inline std::_tstring toTstring(const std::string& s)
{
	return s;
}
inline std::_tstring toTstring(const char* p, int len = -1)
{
	return (len < 0) ? p : std::string(p, len);
}
inline std::_tstring toTstring(const std::wstring& s)
{
	return toNarrowString(s);
}
inline std::_tstring toTstring(const wchar_t* p, int len = -1)
{
	return toNarrowString(p, (int)len);
}
#endif // _UNICODE

// Assigns the uID string resource to wsDest, returns length (0 if no resource)
inline size_t LoadString(std::wstring& wsDest, UINT uID, HINSTANCE hInstance = ::GetModuleHandle(NULL))
{
	PWCHAR wsBuf = nullptr;
	wsDest.clear();
	if (size_t len = ::LoadStringW(hInstance, uID, (PWCHAR)&wsBuf, 0))
		wsDest.assign(wsBuf, len);
	return wsDest.length();
}

// Assigns the uID string resource to sDest, returns length (0 if no resource)
inline size_t LoadString(std::string& sDest, UINT uID, HINSTANCE hInstance = ::GetModuleHandle(NULL))
{
	PCHAR sBuf = nullptr;
	sDest.clear();
	if (size_t len = ::LoadStringA(hInstance, uID, (PCHAR)&sBuf, 0))
		sDest.assign(sBuf, len);
	return sDest.length();
}

// Returns a StringType with uID string resource content (empty if no resource)
template <class StringType>
inline StringType LoadString_(UINT uID, HINSTANCE hInstance)
{
	StringType sDest = {};
	return LoadString(sDest, uID, hInstance) ? sDest : StringType();
}

// Returns a std::string with uID string resource content (empty if no resource)
inline std::string LoadString_S(UINT uID, HINSTANCE hInstance = ::GetModuleHandle(NULL))
{
	return LoadString_<std::string>(uID, hInstance);
}

// Returns a std::wstring with uID string resource content (empty if no resource)
inline std::wstring LoadString_W(UINT uID, HINSTANCE hInstance = ::GetModuleHandle(NULL))
{
	return LoadString_<std::wstring>(uID, hInstance);
}

// Returns a UNICODE depending std::wstring or std::string, with uID string resource content (empty if no resource)
inline std::_tstring LoadString_T(UINT uID, HINSTANCE hInstance = ::GetModuleHandle(NULL))
{
	return LoadString_<std::_tstring>(uID, hInstance);
}
