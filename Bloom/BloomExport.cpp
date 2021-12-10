#include "pch.h"
#include "BloomExport.h"
#include "../BloomLib/Bloom.h"

extern "C" BLOOM_API void* __stdcall CreateBloom()
{
	return new CBloom();
}

extern "C" BLOOM_API void __stdcall DestroyBloom(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		delete bloom;
}

extern "C" BLOOM_API void __stdcall Create(void* objptr, const TCHAR * FileName)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		bloom->Create(FileName);
}

extern "C" BLOOM_API void __stdcall Open(void* objptr, const TCHAR * FileName)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		bloom->Open(FileName);
}

extern "C" BLOOM_API void __stdcall Store(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		bloom->Store();
}

extern "C" BLOOM_API void __stdcall Load(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		bloom->Load();
}

extern "C" BLOOM_API void __stdcall Close(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		bloom->Close();
}

extern "C" BLOOM_API void __stdcall Abort(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		bloom->Abort();
}

extern "C" BLOOM_API void __stdcall Allocate(void* objptr, unsigned Elements)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		bloom->Allocate(Elements);
}

extern "C" BLOOM_API void __stdcall PutString(void* objptr, const TCHAR * String)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		bloom->Put(String);
}

extern "C" BLOOM_API void __stdcall PutArray(void* objptr, const BYTE * buffer, unsigned length)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		bloom->Put(buffer, length);
}

extern "C" BLOOM_API BOOL __stdcall CheckString(void* objptr, const TCHAR * String)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		return bloom->Check(String);
	return FALSE;
}

extern "C" BLOOM_API BOOL __stdcall CheckArray(void* objptr, const BYTE * buffer, unsigned length)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		bloom->Check(buffer, length);
	return FALSE;
}

extern "C" BLOOM_API USHORT __stdcall HeaderVersion(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		return bloom->GetHeader().Version();
	return 0;
}

extern "C" BLOOM_API ULONG __stdcall HeaderSize(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		return bloom->GetHeader().Size();
	return 0;
}

extern "C" BLOOM_API BYTE __stdcall HeaderHashFunc(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (bloom)
		return bloom->GetHeader().HashFunc();
	return 0;
}
