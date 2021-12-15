#pragma once

#include "../BloomLib/Bloom.h"
#include "BloomError.h"

class CBloomContainer
{
private:
	std::unique_ptr<CBloom> m_bloom;
	std::unique_ptr<CBloomError> m_bloom_error;

public:
	CBloomContainer() : m_bloom(new CBloom()), m_bloom_error(nullptr)
	{ }

	CBloom* GetBloom()
	{
		return m_bloom.get();
	}
	CBloomError* GetBloomError()
	{
		return m_bloom_error.get();
	}
	void process_exception(CWin32ErrorT& win32error)
	{
		m_bloom_error.reset(new CBloomError(win32error));
	}

	void process_exception(bloom_exception& ex)
	{
		m_bloom_error.reset(new CBloomError(ex));
	}

	void process_exception(std::exception& ex)
	{
		m_bloom_error.reset(new CBloomError(ex));
	}

	void reset_exception()
	{
		m_bloom_error.release();
	}
};

