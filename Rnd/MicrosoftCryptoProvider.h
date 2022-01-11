#pragma once
#include <wincrypt.h>

class CMicrosoftCryptoProvider
{
public:
	CMicrosoftCryptoProvider()
	{
		if (!CryptAcquireContext(&m_hProvider, 0, 0, PROV_RSA_FULL, CRYPT_VERIFYCONTEXT))
			throw std::runtime_error("CryptAcquireContext returns FALSE");
	}

	~CMicrosoftCryptoProvider()
	{
		CryptReleaseContext(m_hProvider, 0);
	}

	HCRYPTPROV GetProviderHandle() const { return m_hProvider; }

	BYTE GenerateByte();
	void GenerateBlock(BYTE* output, unsigned int size);

private:
	HCRYPTPROV m_hProvider;
};

BYTE CMicrosoftCryptoProvider::GenerateByte()
{
	BYTE b;
	GenerateBlock(&b, 1);
	return b;
}

void CMicrosoftCryptoProvider::GenerateBlock(BYTE* output, unsigned int size)
{
	if (!CryptGenRandom(m_hProvider, size, output))
		throw std::runtime_error("CryptGenRandom");
}
