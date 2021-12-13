#include "pch.h"
#include "BloomExport.h"
#include "../BloomLib/Bloom.h"


extern "C" BLOOM_API void* __stdcall CreateBloom()
{
	return new CBloom();
}

extern "C" BLOOM_API INT __stdcall DestroyBloom(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	delete bloom;
	return TRUE;
}

extern "C" BLOOM_API INT __stdcall Create(void* objptr, const TCHAR * FileName)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	try
	{
		bloom->Create(FileName);
		return TRUE;
	}
	catch(std::exception &ex)
	{
		return -1;
	}
}

extern "C" BLOOM_API void __stdcall Open(void* objptr, const TCHAR * FileName)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	bloom->Open(FileName);
}

extern "C" BLOOM_API void __stdcall Store(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	bloom->Store();
}

extern "C" BLOOM_API void __stdcall Load(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	bloom->Load();
}

extern "C" BLOOM_API void __stdcall Close(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	bloom->Close();
}

extern "C" BLOOM_API void __stdcall Abort(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	bloom->Abort();
}

extern "C" BLOOM_API void __stdcall Allocate(void* objptr, unsigned Elements)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	bloom->Allocate(Elements);
}

extern "C" BLOOM_API void __stdcall PutString(void* objptr, const TCHAR * String)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	bloom->Put(String);
}

extern "C" BLOOM_API void __stdcall PutArray(void* objptr, const BYTE * buffer, unsigned length)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	bloom->Put(buffer, length);
}

extern "C" BLOOM_API BOOL __stdcall CheckString(void* objptr, const TCHAR * String)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	return bloom->Check(String);
}

extern "C" BLOOM_API BOOL __stdcall CheckArray(void* objptr, const BYTE * buffer, unsigned length)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	return bloom->Check(buffer, length);
}

extern "C" BLOOM_API USHORT __stdcall HeaderVersion(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	return bloom->GetHeader().Version();
}

extern "C" BLOOM_API ULONG __stdcall HeaderSize(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	return bloom->GetHeader().Size();
}

extern "C" BLOOM_API BYTE __stdcall HeaderHashFunc(void* objptr)
{
	CBloom* bloom = (CBloom*)objptr;
	if (!bloom)
		throw std::invalid_argument("Bloom Filter pointer cannot be null");
	return bloom->GetHeader().HashFunc();
}
