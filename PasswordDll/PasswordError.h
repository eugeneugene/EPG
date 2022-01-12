#pragma once

#include "../Common/WideHelp.h"
#include "../Common/Win32ErrorEx.h"

class CPasswordError
{
public:
	enum class PasswordErrorClass { PASSWORD_NOERROR, PASSWORD_WIN32ERROR, PASSWORD_STDERROR };

private:
	PasswordErrorClass error_class;
	long error_code;
	std::_tstring error_message;

public:
	CPasswordError() : error_class(PasswordErrorClass::PASSWORD_NOERROR), error_code(0L)
	{ }

	CPasswordError(CWin32ErrorT& win32error) : error_class(PasswordErrorClass::PASSWORD_WIN32ERROR), error_code(win32error.ErrorCode()), error_message(toTstring(win32error.Description()))
	{ }

	CPasswordError(std::exception& ex) : error_class(PasswordErrorClass::PASSWORD_STDERROR), error_code(0L), error_message(toTstring(ex.what()))
	{ }

	PasswordErrorClass get_error_class()
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
