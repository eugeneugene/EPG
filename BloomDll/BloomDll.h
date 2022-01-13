#pragma once

#include "BloomError.h"

#ifdef BLOOM_EXPORTS
#define BLOOM_API __declspec(dllexport)
#else
#define BLOOM_API __declspec(dllimport)
#endif

extern "C" BLOOM_API void* __stdcall CreateBloom();
extern "C" BLOOM_API void __stdcall DestroyBloom(void* objptr);

extern "C" BLOOM_API INT __stdcall GetErrorClass(void* objptr);
extern "C" BLOOM_API LONG __stdcall GetErrorCode(void* objptr);
extern "C" BLOOM_API INT __stdcall GetErrorMessage(void* objptr, WCHAR * buffer, INT64 length);
extern "C" BLOOM_API INT64 __stdcall GetErrorMessageLength(void* objptr);

extern "C" BLOOM_API INT __stdcall Create(void* objptr, const WCHAR * filename);
extern "C" BLOOM_API INT __stdcall Open(void* objptr, const WCHAR * filename);

extern "C" BLOOM_API INT __stdcall Store(void* objptr);
extern "C" BLOOM_API INT __stdcall Load(void* objptr);
extern "C" BLOOM_API INT __stdcall Close(void* objptr);
extern "C" BLOOM_API INT __stdcall Abort(void* objptr);
extern "C" BLOOM_API INT __stdcall Allocate(void* objptr, unsigned elements);

extern "C" BLOOM_API INT __stdcall PutString(void* objptr, const WCHAR * string);
extern "C" BLOOM_API INT __stdcall PutArray(void* objptr, const BYTE * buffer, unsigned length);
extern "C" BLOOM_API INT __stdcall CheckString(void* objptr, const WCHAR * string);
extern "C" BLOOM_API INT __stdcall CheckArray(void* objptr, const BYTE * buffer, unsigned length);

extern "C" BLOOM_API USHORT __stdcall HeaderVersion(void* objptr);
extern "C" BLOOM_API ULONG __stdcall HeaderSize(void* objptr);
extern "C" BLOOM_API BYTE __stdcall HeaderHashFunc(void* objptr);
