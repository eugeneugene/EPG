#pragma once

#include "BloomError.h"

#ifdef BLOOM_EXPORTS
#define BLOOM_API __declspec(dllexport)
#else
#define BLOOM_API __declspec(dllimport)
#endif

extern "C" BLOOM_API PVOID __stdcall CreateBloom();
extern "C" BLOOM_API VOID __stdcall DestroyBloom(PVOID objptr);

extern "C" BLOOM_API INT __stdcall GetErrorClass(PVOID objptr);
extern "C" BLOOM_API LONG __stdcall GetErrorCode(PVOID objptr);
extern "C" BLOOM_API INT __stdcall GetErrorMessage(PVOID objptr, WCHAR * buffer, UINT length);
extern "C" BLOOM_API UINT __stdcall GetErrorMessageLength(PVOID objptr);

extern "C" BLOOM_API INT __stdcall Create(PVOID objptr, const WCHAR * filename);
extern "C" BLOOM_API INT __stdcall Open(PVOID objptr, const WCHAR * filename);

extern "C" BLOOM_API INT __stdcall Store(PVOID objptr);
extern "C" BLOOM_API INT __stdcall Load(PVOID objptr);
extern "C" BLOOM_API INT __stdcall Close(PVOID objptr);
extern "C" BLOOM_API INT __stdcall Abort(PVOID objptr);
extern "C" BLOOM_API INT __stdcall Allocate(PVOID objptr, UINT elements);

extern "C" BLOOM_API INT __stdcall PutString(PVOID objptr, const WCHAR * string);
extern "C" BLOOM_API INT __stdcall PutArray(PVOID objptr, const BYTE * buffer, UINT length);
extern "C" BLOOM_API INT __stdcall CheckString(PVOID objptr, const WCHAR * string);
extern "C" BLOOM_API INT __stdcall CheckArray(PVOID objptr, const BYTE * buffer, UINT length);

extern "C" BLOOM_API USHORT __stdcall HeaderVersion(PVOID objptr);
extern "C" BLOOM_API UINT __stdcall HeaderSize(PVOID objptr);
extern "C" BLOOM_API BYTE __stdcall HeaderHashFunc(PVOID objptr);
