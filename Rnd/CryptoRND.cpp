#include "pch.h"
#include "CryptoRND.h"
#include "MicrosoftCryptoProvider.h"
#include "..\Crypt\sha1.h"

void MSGenerateRandomBlock(BYTE* output, unsigned int size)
{
	static CMicrosoftCryptoProvider rng;		// Should be static
	rng.GenerateBlock(output, size);
}

inline void XorWords(WORD* r, const WORD* a, unsigned int n)
{
	for (unsigned int i = 0; i < n; i++)
		r[i] ^= a[i];
}

static void xorbuf(BYTE* buf, const BYTE* mask, unsigned int count)
{
	if (((size_t)buf | (size_t)mask | count) % sizeof(WORD) == 0)
		XorWords((WORD*)buf, (const WORD*)mask, count / sizeof(WORD));
	else
	{
		for (unsigned int i = 0; i < count; i++)
			buf[i] ^= mask[i];
	}
}

static unsigned int Parity(unsigned long value)
{
	for (unsigned int i = 8 * sizeof(value) / 2; i > 0; i /= 2)
		value ^= value >> i;
	return (unsigned int)value & 1;
}

static unsigned int BytePrecision(unsigned long value)
{
	unsigned int i;
	for (i = sizeof(value); i; --i)
		if (value >> (i - 1) * 8)
			break;

	return i;
}

static unsigned int BitPrecision(unsigned long value)
{
	if (!value)
		return 0;

	unsigned int l = 0, h = 8 * sizeof(value);

	while (h - l > 1)
	{
		unsigned int t = (l + h) / 2;
		if (value >> t)
			l = t;
		else
			h = t;
	}

	return h;
}

static unsigned long Crop(unsigned long value, unsigned int size)
{
	if (size < 8 * sizeof(value))
		return (value & ((1L << size) - 1));
	else
		return value;
}

#pragma warning(push)
#pragma warning(disable:6385)

// Generate New Seed From System-Depended Information
void CCryptoRND::CSeed::GetSeed(BYTE* pSeed, size_t N)
{
	const int seed_size = 96;
	BYTE seed[seed_size] = { 0 };
	size_t maxlen = seed_size;
	SYSTEMTIME st;
	FILETIME ft;
	DWORD dwRes;
	MEMORYSTATUSEX ms = { 0 };
	int ni = 0;

	GetSystemTime(&st);
	SystemTimeToFileTime(&st, &ft);

	_ASSERT(maxlen >= sizeof(FILETIME));
	std::memcpy(seed + ni, &ft, sizeof(FILETIME));
	ni += sizeof(FILETIME);
	maxlen -= sizeof(FILETIME);

	dwRes = GetCurrentProcessId();
	_ASSERT(maxlen >= sizeof(DWORD));
	std::memcpy(seed + ni, &dwRes, sizeof(dwRes));
	ni += sizeof(dwRes);
	maxlen -= sizeof(dwRes);

	dwRes = GetCurrentThreadId();
	_ASSERT(maxlen >= sizeof(DWORD));
	std::memcpy(seed + ni, &dwRes, sizeof(dwRes));
	ni += sizeof(dwRes);
	maxlen -= sizeof(dwRes);

	ULONGLONG ululRes = GetTickCount64();
	_ASSERT(maxlen >= sizeof(ULONGLONG));
	std::memcpy(seed + ni, &dwRes, sizeof(dwRes));
	ni += sizeof(dwRes);
	maxlen -= sizeof(dwRes);

	ms.dwLength = sizeof(MEMORYSTATUSEX);
	GlobalMemoryStatusEx(&ms);
	_ASSERT(maxlen >= sizeof(MEMORYSTATUSEX));
	std::memcpy(seed + ni, &ms, sizeof(MEMORYSTATUSEX));
	ni += sizeof(MEMORYSTATUSEX);
	maxlen -= sizeof(MEMORYSTATUSEX);

	++m_CC; // Roll over to zero if the value is incremented beyond ULONG_MAX
	_ASSERT(maxlen >= sizeof(m_CC));
	std::memcpy(seed + ni, &m_CC, sizeof(m_CC));
	ni += sizeof(m_CC);
	maxlen -= sizeof(m_CC);

	_ASSERT(maxlen >= S_size);
	std::memcpy(seed + ni, m_SD, S_size);
	ni += (int)S_size;
	maxlen -= S_size;

	_ASSERT(0 == maxlen);

	size_t R = N;
	size_t K = 0;
	BYTE hash[SHA1_DIGEST_SIZE];
	size_t size =  std::min(seed_size, SHA1_DIGEST_SIZE);
	while (R != 0)
	{
		sha1(hash, seed, (unsigned int)seed_size);
		std::memcpy(seed, hash, size);
		size_t B = std::min(R, (size_t)SHA1_DIGEST_SIZE);
		std::memcpy(pSeed + K, seed, B);
		K += B;
		R -= B;
	}
	memset(seed, 0, seed_size);
}

#pragma warning(pop)

CCryptoRND::CCryptoRND()
{
	std::memset(m_FirstBlock, 0, DES_KEY_SZ);
	std::memset(m_LastBlock, 0, DES_KEY_SZ);
	Reseed();
	m_bHasBlock = false;
	m_Pointer = 0;
	for (int i = 0; i < 8; i++)
		DiscardByte();
}

CCryptoRND::~CCryptoRND()
{
	std::memset(m_KeySeed, 0, K_size + S_size);
	std::memset(m_FirstBlock, 0, DES_KEY_SZ);
	std::memset(m_LastBlock, 0, DES_KEY_SZ);
	std::memset(m_Block, 0, DES_KEY_SZ);
	std::memset(ks1, 0, DES_SCHEDULE_SZ);
	std::memset(ks2, 0, DES_SCHEDULE_SZ);
	std::memset(ks3, 0, DES_SCHEDULE_SZ);
}

#pragma warning(push)
#pragma warning(disable:6305)

void CCryptoRND::Reseed()
{
	static CSeed s_seed;
	s_seed.GetSeed(m_KeySeed, K_size + S_size);

	// For use in Gen64
	des_cblock* key1 = (des_cblock*)&m_Key;
	des_cblock* key2 = (des_cblock*)&m_Key + DES_KEY_SZ;
	des_cblock* key3 = (des_cblock*)&m_Key + 2 * DES_KEY_SZ;
	des_set_key(key1, ks1);
	des_set_key(key2, ks2);
	des_set_key(key3, ks3);
}

#pragma warning(pop)

void CCryptoRND::GenerateBlock0()
{
	SYSTEMTIME st;
	FILETIME ft;
	GetSystemTime(&st);
	SystemTimeToFileTime(&st, &ft);

	BYTE* S = m_Seed;
	BYTE* D = (BYTE*)&ft;

	Gen64(m_Block, D, S);
	if (!std::memcmp(m_Block, m_FirstBlock, DES_KEY_SZ) || !std::memcmp(m_Block, m_LastBlock, DES_KEY_SZ))
		ThrowError();
	std::memcpy(m_FirstBlock, m_Block, DES_KEY_SZ);
	std::memcpy(m_LastBlock, m_Block, DES_KEY_SZ);
	std::memset(D, 0, sizeof(FILETIME));
	m_bHasBlock = true;
}

void CCryptoRND::GenerateBlock()
{
	if (!m_bHasBlock)
		return GenerateBlock0();

	SYSTEMTIME st;
	FILETIME ft;
	GetSystemTime(&st);
	SystemTimeToFileTime(&st, &ft);

	BYTE* S = m_Seed;
	BYTE* D = (BYTE*)&ft;

	Gen64(m_Block, D, S);
	if (!std::memcmp(m_Block, m_FirstBlock, DES_KEY_SZ) || !std::memcmp(m_Block, m_LastBlock, DES_KEY_SZ))
		ThrowError();
	std::memcpy(m_LastBlock, m_Block, DES_KEY_SZ);
	std::memset(D, 0, sizeof(FILETIME));
}

void CCryptoRND::Gen64(/* Out */ BYTE* X, /* In */ BYTE* D, /* In&Out */ BYTE* S)
{
	des_cblock I;

	des_ecb3_encrypt((des_cblock*)D, &I, ks1, ks2, ks3, true);
	xorbuf(I, S, DES_KEY_SZ);
	des_ecb3_encrypt(&I, (des_cblock*)X, ks1, ks2, ks3, true);
	xorbuf(X, I, DES_KEY_SZ);
	des_ecb3_encrypt((des_cblock*)X, (des_cblock*)S, ks1, ks2, ks3, true);
}

BYTE CCryptoRND::GenerateByte()
{
	if (0 == m_Pointer)
	{
		GenerateBlock();
		m_Pointer = DES_KEY_SZ;
	}
	return m_Block[--m_Pointer];
}

DWORD CCryptoRND::GenerateDWORD(DWORD min, DWORD max)
{
	DWORD range = max - min;
	const int maxBytes = BytePrecision(range);
	const int maxBits = BitPrecision(range);

	DWORD value = 0;

	do
	{
		value = 0;
		for (int i = 0; i < maxBytes; i++)
			value = (value << 8) | GenerateByte();

		value = Crop(value, maxBits);
	} while (value > range);

	return value + min;
}

void CCryptoRND::GenerateBlock(BYTE* output, unsigned int size)
{
	while (size--)
		*output++ = GenerateByte();
}

bool CCryptoRND::GenerateBit()
{
	return Parity(GenerateByte()) ? true : false;
}
