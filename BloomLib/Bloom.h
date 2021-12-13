#pragma once

#include "BloomHeader.h"
#include <memory>

class CBloom
{
public:
	typedef std::unique_ptr<BYTE> BloomArray;

	CBloom() : Array(nullptr), ArraySize(0), m_bDirty(FALSE), File(0) { }
	~CBloom()
	{
		Close();
	}
	void Create(const TCHAR* FileName);
	void Open(const TCHAR* FileName);
	void Store();
	void Load();
	void Close();
	void Abort();
	void Allocate(unsigned Elements);
	void Put(const TCHAR* String);
	void Put(const BYTE* buffer, unsigned length);
	BOOL Check(const TCHAR* String) const;
	BOOL Check(const BYTE* buffer, unsigned length) const;
	BYTE operator[](unsigned pos) const;
	size_t GetSize() const
	{
		return ArraySize;
	}
	void CopyHeader(CBloomHeader* BloomHeaderDest) const
	{
		memcpy(BloomHeaderDest, &Header, sizeof CBloomHeader);
	}
	const CBloomHeader& GetHeader() const
	{
		return Header;
	}
	const BloomArray& GetArray() const
	{
		return Array;
	}

	static unsigned CountBits(unsigned Elements)
	{
		if (Elements == 0)
			return 0;
		double k = (double)BLOOM_DEFAULT_HASHSIZE;
		double n = (double)Elements;
		double mn = -k / log(1.0 - exp(log(BLOOM_PFP) / k));
		double m = mn * n;
		return unsigned(m);
	}
	static unsigned CountBytes(unsigned Elements)
	{
		return (CountBits(Elements) >> 3) + 1;
	}

private:
	int File;
	BloomArray Array;
	size_t ArraySize;
	CBloomHeader Header;
	BOOL m_bDirty;

private:
	static long IO_Validate(long result)
	{
		if (-1 == result)
			throw CWin32ErrorT();
		return result;
	}
	static long IO_Validate0(long result)
	{
		if (0 != result)
			throw CWin32ErrorT();
		return result;
	}
	static bool IsFileValid(int file)
	{
		return (file > 0);
	}
};
