#pragma once

#include "../Common/WideHelp.h"
#include "../Common/Win32ErrorEx.h"
#include "../BloomLib/bloom_exception.h"

class CBloomError
{
public:
	enum class BloomErrorClass { BLOOM_NOERROR, BLOOM_WIN32ERROR, BLOOM_LIBERROR, BLOOM_STDERROR };

private:
	BloomErrorClass error_class;
	long error_code;
	std::_tstring error_message;

public:
	CBloomError() : error_class(BloomErrorClass::BLOOM_NOERROR), error_code(0L)
	{}

	void process_exception(CWin32ErrorT& win32error);
	void process_exception(bloom_exception& ex);
	void process_exception(std::exception& ex);
	void reset();

	BloomErrorClass ErrorClass()
	{
		return error_class;
	}
	long ErrorCode()
	{
		return error_code;
	}
	const TCHAR* GetErrorMesssage()
	{
		return error_message.c_str();
	}
};

