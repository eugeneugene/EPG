#include "pch.h"
#include "BloomError.h"
#include "../Common/WideHelp.h"

void CBloomError::process_exception(CWin32ErrorT& win32error)
{
	error_class = BloomErrorClass::BLOOM_WIN32ERROR;
	error_code = win32error.ErrorCode();
	error_message = toTstring(win32error.Description());
}

void CBloomError::process_exception(bloom_exception& ex)
{
	error_class = BloomErrorClass::BLOOM_LIBERROR;
	error_code = static_cast<long>(ex.error());
	error_message = toTstring(ex.what());
}

void CBloomError::process_exception(std::exception& ex)
{
	error_class = BloomErrorClass::BLOOM_LIBERROR;
	error_code = 0L;
	error_message = toTstring(ex.what());
}

void CBloomError::reset()
{
	error_class = BloomErrorClass::BLOOM_NOERROR;
	error_code = 0L;
	error_message.clear();
}
