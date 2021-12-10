// Bloom.cpp: implementation of the CBloom class.
//
//////////////////////////////////////////////////////////////////////

#include "pch.h"
#include "BloomHeader.h"
#include "Bloom.h"
#include "bloom_exception.h"
#include "Hash.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

void CBloom::Create(const TCHAR* FileName)
{
	if (IsFileValid(File))
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_ALREADY_OPENED);

	Header.Default();

	static_assert(HASH_DIGESTSIZE >= BLOOM_DEFAULT_HASHSIZE * sizeof LONG, "Internal error: Insufficient HASH_DIGESTSIZE");
	Header.Version(BLOOM_VERSION);
	Header.HashFunc(BLOOM_DEFAULT_HASHSIZE);

	IO_Validate0(_tsopen_s(&File, FileName, _O_BINARY | _O_CREAT | _O_RDWR | _O_TRUNC, _SH_DENYNO, _S_IREAD | _S_IWRITE));
	Header.Save(File);
}

unsigned CBloom::CountBits(unsigned int Elements)
{
	if (Elements == 0)
		return 0;
	double k = (double)BLOOM_DEFAULT_HASHSIZE;
	double n = (double)Elements;
	double mn = -k / log(1.0 - exp(log(BLOOM_PFP) / k));
	double m = mn * n;
	return unsigned(m);
}

void CBloom::Allocate(unsigned Elements)
{
	unsigned bits;
	unsigned bytes;

	if (Elements == 0)
		return;

	if (Header.Version() == BLOOM_VERSION)
	{
		bits = CountBits(Elements);
		bytes = (bits >> 3) + 1;
	}
	else
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_UNKNOWN_VERSION);

	Header.Size(bits);
	Header.Save(File);

	m_bDirty = TRUE;
	Array.reset(new BYTE[ArraySize = bytes]);
}

void CBloom::Store()
{
	if (!IsFileValid(File))
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_NOT_OPENED);
	if (GetSize() == 0)				// Not allocated in memory
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_NOT_ALLOCATED);
	Header.Save(File);

	IO_Validate(_write(File, Array.get(), (unsigned)ArraySize));
}

void CBloom::Open(const TCHAR* FileName)
{
	if (IsFileValid(File))
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_ALREADY_OPENED);
	IO_Validate0(_tsopen_s(&File, FileName, _O_BINARY | _O_RDWR, _SH_DENYNO, _S_IREAD | _S_IWRITE));	// Does it need R/O mode for opening? R/O files cause exception #5

	Header.Load(File);
	if (!Header.Check())
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_UNKNOWN_HEADER);
	if (Header.Version() != BLOOM_VERSION)
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_UNKNOWN_VERSION);
}

void CBloom::Close()
{
	if (m_bDirty)
	{
		Store();
		m_bDirty = FALSE;
	}
	if (IsFileValid(File))
	{
		IO_Validate(_close(File));
		File = 0;
	}
	Array.reset();
}

void CBloom::Load()
{
	if (!IsFileValid(File))
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_NOT_OPENED);

	unsigned int len;
	long head;

	if (Header.Version() == BLOOM_VERSION)
		head = sizeof CHead;
	else
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_UNKNOWN_VERSION);

	IO_Validate(_lseek(File, head, SEEK_SET));
	len = IO_Validate(_filelength(File)) - head;
	if (len != (Header.Size() >> 3) + 1)
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_CORRUPTED);

	Array.reset(new BYTE[ArraySize = len]);
	IO_Validate(_read(File, Array.get(), (unsigned)ArraySize));
}

void CBloom::Abort()
{
	if (IsFileValid(File))
	{
		int res = _close(File);
		File = 0;
		IO_Validate(res);
	}
	Array.reset();
	Header.Default();
	Header.Version(BLOOM_VERSION);
	Header.HashFunc(BLOOM_DEFAULT_HASHSIZE);
}

void CBloom::Put(const BYTE* buffer, unsigned length)
{
	unsigned Count = Header.Size();
	unsigned HeaderSize;
	if (Count == 0)
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_NOT_ALLOCATED);
	if (GetSize() == 0)	// Not allocated in memory
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_NOT_ALLOCATED);

	CHash hash;
	if (hash.Size() != Header.HashFunc())
		hash.Size(Header.HashFunc());
	if (Header.Version() == BLOOM_VERSION)
	{
		BYTE h[HASH_DIGESTSIZE];
		HASH_FUNC(h, buffer, length);
		memcpy(hash.GetHash(), h, hash.Size() * sizeof(unsigned));
		HeaderSize = sizeof CHead;
	}
	else
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_UNKNOWN_VERSION);
	for (UINT i = 0; i < hash.Size(); i++)
	{
		unsigned bit = hash[i] % Count;
		unsigned byte = bit >> 3;
		BYTE b = BYTE(bit & 7);
		BYTE mask = 1 << b;

		auto array = Array.get();
		b = array[byte];
		b |= mask;
		array[byte] = b;
		m_bDirty = TRUE;
	}
}

void CBloom::Put(const TCHAR* String)
{
	Put((const BYTE*)String, (unsigned)_tcslen(String));
}

BOOL CBloom::Check(const BYTE* buffer, unsigned length) const
{
	unsigned Count = Header.Size();
	unsigned HeaderSize;
	if (Count == 0)
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_NOT_ALLOCATED);
	if (GetSize() == 0)	// Not allocated in memory
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_NOT_ALLOCATED);

	CHash hash;
	if (hash.Size() != Header.HashFunc())
		hash.Size(Header.HashFunc());
	if (Header.Version() == BLOOM_VERSION)
	{
		BYTE h[HASH_DIGESTSIZE];
		HASH_FUNC(h, buffer, length);
		memcpy(hash.GetHash(), h, hash.Size() * sizeof(unsigned));
		HeaderSize = sizeof CHead;
	}
	else
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_UNKNOWN_VERSION);
	for (UINT i = 0; i < hash.Size(); i++)
	{
		unsigned bit = hash[i] % Count;
		unsigned byte = bit >> 3;
		BYTE b = BYTE(bit & 7);
		BYTE mask = 1 << b;

		auto array = Array.get();
		b = array[byte];
		if (!(b & mask))
			return FALSE;
	}
	return TRUE;
}

BOOL CBloom::Check(const TCHAR* String) const
{
	return Check((const BYTE*)String, (unsigned)_tcslen(String));
}

BYTE CBloom::operator[](unsigned pos) const
{
	unsigned Count = Header.Size();
	if (Count == 0)
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_NOT_ALLOCATED);
	if (GetSize() == 0)	// Not allocated in memory
		throw bloom_exception(bloom_exception::_error::BLOOM_ERROR_NOT_ALLOCATED);

	if (GetSize() <= pos)
		throw std::out_of_range("Invalid array subscript");

	auto array = Array.get();
	return array[pos];
}
