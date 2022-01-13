#include "pch.h"
#include "PasswordDll.h"
#include "PasswordContainer.h"

extern "C" PASSWORD_API void* __stdcall CreatePassword(int Mode, const char* pIncludeSymbols, const char* pExcludeSymbols)
{
	return new CPasswordContainer(Mode, pIncludeSymbols, pExcludeSymbols);
}

extern "C" PASSWORD_API void __stdcall DestroyPassword(void* objptr)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Password Container pointer cannot be null");
	delete pc;
}

extern "C" PASSWORD_API INT __stdcall GetErrorClass(void* objptr)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Password Container pointer cannot be null");
	auto password_error = pc->GetPasswordError();
	if (!password_error)
		return -1;
	return static_cast<int>(password_error->get_error_class());
}

extern "C" PASSWORD_API LONG __stdcall GetErrorCode(void* objptr)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Password Container pointer cannot be null");
	auto password_error = pc->GetPasswordError();
	if (!password_error)
		return -1;
	return password_error->get_error_code();
}

extern "C" PASSWORD_API INT __stdcall GetErrorMessage(void* objptr, TCHAR * buffer, INT64 length)
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

extern "C" PASSWORD_API INT64 __stdcall GetErrorMessageLength(void* objptr)
{
	CPasswordContainer* pc = (CPasswordContainer*)objptr;
	if (!pc)
		throw std::invalid_argument("Password Container pointer cannot be null");
	auto password_error = pc->GetPasswordError();
	if (!password_error)
		return -1;
	return password_error->get_error_message_len();
}

extern "C" PASSWORD_API INT __stdcall GenerateWord(void* objptr, UINT length)
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

extern "C" PASSWORD_API INT __stdcall GenerateRandomWord(void* objptr, UINT length)
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

void GetWord(std::string& out);

void GetHyphenatedWord(std::string& out);
