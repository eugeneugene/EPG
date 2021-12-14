#pragma once

#include "../BloomLib/Bloom.h"
#include "BloomError.h"

class CBloomContainer
{
private:
	CBloom* m_pBloom;
	CBloomError* m_pBloomError;

public:
	CBloomContainer() : m_pBloom(new CBloom()), m_pBloomError(new CBloomError())
	{}

	~CBloomContainer()
	{
		delete m_pBloomError;
		delete m_pBloom;
	}

	CBloom* GetBloom()
	{
		return m_pBloom;
	}
	CBloomError* GetBloomError()
	{
		return m_pBloomError;
	}
};

