#pragma once

#include <Windows.h>
#include "PasswordError.h"

#ifdef PASSWORD_EXPORTS
#define PASSWORD_API __declspec(dllexport)
#else
#define PASSWORD_API __declspec(dllimport)
#endif

extern "C" PASSWORD_API PVOID __stdcall CreatePassword(int Mode, const WCHAR * pIncludeSymbols, const WCHAR * pExcludeSymbols);
extern "C" PASSWORD_API VOID __stdcall DestroyPassword(PVOID objptr);

extern "C" PASSWORD_API INT __stdcall GetErrorClass(PVOID objptr);
extern "C" PASSWORD_API LONG __stdcall GetErrorCode(PVOID objptr);
extern "C" PASSWORD_API INT __stdcall GetErrorMessage(PVOID objptr, WCHAR * buffer, UINT length);
extern "C" PASSWORD_API UINT __stdcall GetErrorMessageLength(PVOID objptr);

extern "C" PASSWORD_API INT __stdcall GenerateWord(PVOID objptr, UINT length);
extern "C" PASSWORD_API INT __stdcall GenerateRandomWord(PVOID objptr, UINT length);
extern "C" PASSWORD_API UINT __stdcall GetWordLength(PVOID objptr);
extern "C" PASSWORD_API INT __stdcall GetWord(PVOID objptr, WCHAR * buffer, UINT length);
extern "C" PASSWORD_API UINT __stdcall GetHyphenatedLength(PVOID objptr);
extern "C" PASSWORD_API INT __stdcall GetHyphenatedWord(PVOID objptr, WCHAR * buffer, UINT length);
