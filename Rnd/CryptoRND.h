#pragma once

#include "..\Crypt\des_ecb3_enc.h"

void MSGenerateRandomBlock(BYTE* output, unsigned int size);

class CCryptoRND
{
private:
	class CSeed
	{
	public:
		CSeed()
		{
			memset(m_SD, 0, S_size);
			MSGenerateRandomBlock((BYTE*)&m_CC, sizeof(m_CC));
		}
		~CSeed()
		{
			memset(m_SD, 0, S_size);
		}

		void GetSeed(BYTE* pSeed, size_t N);

	private:
		enum { S_size = 8 };

		DWORD m_CC;
		BYTE m_SD[S_size];
	};

public:
	CCryptoRND();
	~CCryptoRND();

	void Reseed();
	BYTE GenerateByte();
	DWORD GenerateDWORD(DWORD min = 0, DWORD max = 0xFFFFFFFFL);
	void GenerateBlock(BYTE* output, unsigned int size);
	bool GenerateBit();
	void DiscardByte()
	{
		GenerateByte();
	}

private:
	void GenerateBlock();
	void GenerateBlock0();
	virtual void ThrowError()
	{
		throw std::runtime_error("CCryptoRND: Internal test failed!");
	}

	void Gen64(/* Out */ BYTE* X, /* In */ BYTE* D, /* In&Out */ BYTE* S);

	enum { K_size = 24, S_size = 8 };
	union
	{
		struct
		{
			BYTE m_Key[K_size];
			BYTE m_Seed[S_size];
		};
		BYTE m_KeySeed[K_size + S_size];
	};

	bool m_bHasBlock;
	des_cblock m_FirstBlock, m_LastBlock;
	des_cblock m_Block;
	size_t m_Pointer;
	des_key_schedule ks1, ks2, ks3;
};
