#pragma once

#include "PasswordError.h"

#ifdef PASSWORD_EXPORTS
#define PASSWORD_API __declspec(dllexport)
#else
#define PASSWORD_API __declspec(dllimport)
#endif

extern "C" PASSWORD_API void* __stdcall CreatePassword(int Mode, const char* pIncludeSymbols, const char* pExcludeSymbols);
extern "C" PASSWORD_API void __stdcall DestroyPassword(void* objptr);

extern "C" PASSWORD_API INT __stdcall GetErrorClass(void* objptr);
extern "C" PASSWORD_API LONG __stdcall GetErrorCode(void* objptr);
extern "C" PASSWORD_API INT __stdcall GetErrorMessage(void* objptr, TCHAR * buffer, INT64 length);
extern "C" PASSWORD_API INT64 __stdcall GetErrorMessageLength(void* objptr);

extern "C" PASSWORD_API INT __stdcall GenerateWord(void* objptr, UINT length);
extern "C" PASSWORD_API INT __stdcall GenerateRandomWord(void* objptr, UINT length);
