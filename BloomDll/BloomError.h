#pragma once

#include "../Common/WideHelp.h"
#include "../Common/Win32ErrorEx.h"
#include "../Bloom/bloom_exception.h"

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
	{ }

	CBloomError(CWin32ErrorT& win32error) : error_class(BloomErrorClass::BLOOM_WIN32ERROR), error_code(win32error.ErrorCode()), error_message(toTstring(win32error.Description()))
	{ }

	CBloomError(bloom_exception& ex) : error_class(BloomErrorClass::BLOOM_LIBERROR), error_code(static_cast<long>(ex.error())), error_message(toTstring(ex.what()))
	{ }

	CBloomError(std::exception& ex) : error_class(BloomErrorClass::BLOOM_LIBERROR), error_code(0L), error_message(toTstring(ex.what()))
	{ }

	BloomErrorClass get_error_class()
	{
		return error_class;
	}
	long get_error_code()
	{
		return error_code;
	}
	const TCHAR* get_error_message()
	{
		return error_message.c_str();
	}
	const size_t get_error_message_len()
	{
		return error_message.length();
	}
};

