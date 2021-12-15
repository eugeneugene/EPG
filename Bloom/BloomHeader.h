#pragma once

#include "../common/Win32ErrorEx.h"
#include "../Crypt/sha1.h"
#include "../Crypt/sha2.h"

constexpr USHORT BLOOM_VERSION = 0x0200;
constexpr double BLOOM_PFP = 0.001;		// Probability of a false positive = 1%;

constexpr auto  HASH_FUNC = sha256;
constexpr auto  HASH_DIGESTSIZE = SHA256_DIGEST_SIZE;
constexpr const TCHAR* HASH_NAME = _T("SHA-256");
constexpr BYTE  BLOOM_DEFAULT_HASHSIZE = 7;

#include <PSHPACK1.H>

struct CHead
{
	BYTE head[5];
	USHORT vers;
	ULONG size;			// size in bits
	BYTE Unused;
	BYTE hashfunc;
	ULONG Reserved[4];
};

#include <POPPACK.H>

class CBloomHeader
{
private:
	CHead Head;

protected:
	static void Default(CHead& Head);

public:
	CBloomHeader();
	virtual ~CBloomHeader() {};
	void Default();
	void Load(int file);
	void Save(int file);
	int Check() const;
	USHORT Version() const { return Head.vers; }
	USHORT Version(USHORT v) { return (Head.vers = v); }
	ULONG Size() const { return Head.size; }
	ULONG Size(ULONG s) { return (Head.size = s); }
	BYTE HashFunc() const { return Head.hashfunc; }
	BYTE HashFunc(BYTE h) { return (Head.hashfunc = h); }

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
