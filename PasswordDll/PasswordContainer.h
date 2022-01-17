#pragma once

#include <memory>
#include <string>
#include "..\Fips181\Password.h"
#include "PasswordError.h"

class CPasswordContainer
{
private:
	std::unique_ptr<CPassword> m_password;
	std::unique_ptr<CPasswordError> m_password_error;

public:
	CPasswordContainer(Modes Mode, const WCHAR* pIncludeSymbols, const WCHAR* pExcludeSymbols) : m_password(new CPassword(Mode, pIncludeSymbols, pExcludeSymbols)), m_password_error(nullptr)
	{ }

	CPassword* GetPassword()
	{
		return m_password.get();
	}

	CPasswordError* GetPasswordError()
	{
		return m_password_error.get();
	}

	void process_exception(CWin32ErrorT& win32error)
	{
		m_password_error.reset(new CPasswordError(win32error));
	}

	void process_exception(std::exception& ex)
	{
		m_password_error.reset(new CPasswordError(ex));
	}

	void reset_exception()
	{
		m_password_error.release();
	}
};
