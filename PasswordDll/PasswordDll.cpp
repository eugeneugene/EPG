#include "pch.h"
#include "PasswordDll.h"
#include "PasswordContainer.h"

extern "C" PASSWORD_API PVOID __stdcall CreatePassword(int Mode, const WCHAR* pIncludeSymbols, const WCHAR* pExcludeSymbols)
{
	return new CPasswordContainer(Mode, pIncludeSymbols, pExcludeSymbols);
}

extern "C" PASSWORD_API VOID __stdcall DestroyPassword(PVOID objptr)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Password Container pointer cannot be null");
	delete pc;
}

extern "C" PASSWORD_API INT __stdcall GetErrorClass(PVOID objptr)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Password Container pointer cannot be null");
	auto password_error = pc->GetPasswordError();
	if (!password_error)
		return -1;
	return static_cast<int>(password_error->get_error_class());
}

extern "C" PASSWORD_API LONG __stdcall GetErrorCode(PVOID objptr)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Password Container pointer cannot be null");
	auto password_error = pc->GetPasswordError();
	if (!password_error)
		return -1;
	return password_error->get_error_code();
}

extern "C" PASSWORD_API INT __stdcall GetErrorMessage(PVOID objptr, WCHAR * buffer, UINT length)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Password Container pointer cannot be null");
	auto password_error = pc->GetPasswordError();
	if (!password_error)
		return -1;
	_tcscpy_s(buffer, length, password_error->get_error_message());
	return 1;
}

extern "C" PASSWORD_API UINT __stdcall GetErrorMessageLength(PVOID objptr)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Password Container pointer cannot be null");
	auto password_error = pc->GetPasswordError();
	if (!password_error)
		return -1;
	return password_error->get_error_message_len();
}

extern "C" PASSWORD_API INT __stdcall GenerateWord(PVOID objptr, UINT length)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		if (pc->GetPassword()->GenerateWord(length))
			return pc->GetPassword()->GetLength();
		return 0;
	}
	catch (CWin32ErrorT& ex)
	{
		pc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		pc->process_exception(ex);
	}
	return -1;
}

extern "C" PASSWORD_API INT __stdcall GenerateRandomWord(PVOID objptr, UINT length)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		if (pc->GetPassword()->GenerateRandomWord(length))
			return pc->GetPassword()->GetRandomLength();
		return 0;
	}
	catch (CWin32ErrorT& ex)
	{
		pc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		pc->process_exception(ex);
	}
	return -1;
}

extern "C" PASSWORD_API UINT __stdcall GetWordLength(PVOID objptr)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Password Container pointer cannot be null");
	return pc->GetPassword()->GetLength();
}

extern "C" PASSWORD_API INT __stdcall GetWord(PVOID objptr, WCHAR* buffer, UINT length)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		std::_tstring str;
		pc->GetPassword()->GetWord(str);
		if (_tcscpy_s(buffer, length, str.c_str()))
			throw CWin32ErrorT();
		return 1;
	}
	catch (CWin32ErrorT& ex)
	{
		pc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		pc->process_exception(ex);
	}
	return -1;
}

extern "C" PASSWORD_API UINT __stdcall GetRandomWordLength(PVOID objptr)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Password Container pointer cannot be null");
	return pc->GetPassword()->GetRandomLength();
}

extern "C" PASSWORD_API INT __stdcall GetRandomWord(PVOID objptr, WCHAR* buffer, UINT length)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		std::_tstring str;
		pc->GetPassword()->GetRandomWord(str);
		if (_tcscpy_s(buffer, length, str.c_str()))
			throw CWin32ErrorT();
		return 1;
	}
	catch (CWin32ErrorT& ex)
	{
		pc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		pc->process_exception(ex);
	}
	return -1;
}
