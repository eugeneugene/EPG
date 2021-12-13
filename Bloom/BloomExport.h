#pragma once

#include "BloomError.h"

#ifdef BLOOM_EXPORTS
#define BLOOM_API __declspec(dllexport)
#else
#define BLOOM_API __declspec(dllimport)
#endif

class CBloomExpContainer
{
private:
	CBloom m_bloom;
	CBloomError m_bloomError;

public :
	CBloomExpContainer()
	{

	}
};

extern "C" BLOOM_API void* __stdcall CreateBloom();
extern "C" BLOOM_API INT __stdcall DestroyBloom(void* objptr);

extern "C" BLOOM_API INT __stdcall Create(void* objptr, const TCHAR * FileName);
extern "C" BLOOM_API void __stdcall Open(void* objptr, const TCHAR * FileName);

extern "C" BLOOM_API void __stdcall Store(void* objptr);
extern "C" BLOOM_API void __stdcall Load(void* objptr);
extern "C" BLOOM_API void __stdcall Close(void* objptr);
extern "C" BLOOM_API void __stdcall Abort(void* objptr);
extern "C" BLOOM_API void __stdcall Allocate(void* objptr, unsigned Elements);

extern "C" BLOOM_API void __stdcall PutString(void* objptr, const TCHAR * String);
extern "C" BLOOM_API void __stdcall PutArray(void* objptr, const BYTE * buffer, unsigned length);
extern "C" BLOOM_API BOOL __stdcall CheckString(void* objptr, const TCHAR * String);
extern "C" BLOOM_API BOOL __stdcall CheckArray(void* objptr, const BYTE * buffer, unsigned length);

extern "C" BLOOM_API USHORT __stdcall HeaderVersion(void* objptr);
extern "C" BLOOM_API ULONG __stdcall HeaderSize(void* objptr);
extern "C" BLOOM_API BYTE __stdcall HeaderHashFunc(void* objptr);
