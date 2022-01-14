#pragma once

#include "../Common/WideHelp.h"
#include "../Common/Win32ErrorEx.h"

class CPasswordError
{
public:
	enum class PasswordErrorClass { PASSWORD_NOERROR, PASSWORD_WIN32ERROR, PASSWORD_STDERROR, PASSWORD_LIBERROR };

private:
	PasswordErrorClass error_class;
	LONG error_code;
	std::_tstring error_message;

public:
	CPasswordError() : error_class(PasswordErrorClass::PASSWORD_NOERROR), error_code(0)
	{ }

	CPasswordError(const CWin32ErrorT& win32error) : error_class(PasswordErrorClass::PASSWORD_WIN32ERROR), error_code(win32error.ErrorCode()), error_message(toTstring(win32error.Description()))
	{ }

	CPasswordError(const std::exception& ex) : error_class(PasswordErrorClass::PASSWORD_STDERROR), error_code(0), error_message(toTstring(ex.what()))
	{ }

	PasswordErrorClass get_error_class()
	{
		return error_class;
	}
	LONG get_error_code()
	{
		return error_code;
	}
	const WCHAR* get_error_message()
	{
		return error_message.c_str();
	}
	const UINT get_error_message_len()
	{
		return static_cast<UINT>(error_message.length());
	}
};
