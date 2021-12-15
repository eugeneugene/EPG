#include "pch.h"
#include "BloomDll.h"
#include "BloomContainer.h"

extern "C" BLOOM_API void* __stdcall CreateBloom()
{
	return new CBloomContainer();
}

extern "C" BLOOM_API void __stdcall DestroyBloom(void* objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	delete bc;
}

extern "C" BLOOM_API INT __stdcall GetErrorClass(void* objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	auto bloom_error = bc->GetBloomError();
	if (!bloom_error)
		return -1;
	return static_cast<int>(bloom_error->get_error_class());
}

extern "C" BLOOM_API LONG __stdcall GetErrorCode(void* objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	auto bloom_error = bc->GetBloomError();
	if (!bloom_error)
		return -1;
	return bloom_error->get_error_code();
}

/// <summary>
/// Get Error Message
/// </summary>
/// <param name="objptr">Pointer to CBloomContainer</param>
/// <param name="buffer">The memory area where the message shall be copied. 'length' symbols must be allocated ahead</param>
/// <param name="length">The buffer size in symbols (not bytes). If *length is less than required, method returns 0 and *length gets the required amount of symbols.
/// It is safe to allocate length+1 symbols for terminating '\0'</param>
/// <returns>0 if *length gets required size of buffer, 1 if buffer gets the error message, -1 if there is no error information available</returns>
extern "C" BLOOM_API INT __stdcall GetErrorMessage(void* objptr, TCHAR * buffer, DWORD * length)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	auto bloom_error = bc->GetBloomError();
	if (!bloom_error)
		return -1;
	if (bloom_error->get_error_message_len() > *length)
	{
		*length = (DWORD)bloom_error->get_error_message_len();
		return 0;
	}
	_tcscpy_s(buffer, *length, bloom_error->get_error_message());
	*length = (DWORD)bloom_error->get_error_message_len();
	return 1;
}

extern "C" BLOOM_API INT __stdcall Create(void* objptr, const TCHAR * filename)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		bc->GetBloom()->Create(filename);
		return TRUE;
	}
	catch (CWin32ErrorT& ex)
	{
		bc->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->process_exception(ex);
	}
	return -1;
}

extern "C" BLOOM_API INT __stdcall Open(void* objptr, const TCHAR * filename)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		bc->GetBloom()->Open(filename);
		return TRUE;
	}
	catch (CWin32ErrorT& ex)
	{
		bc->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->process_exception(ex);
	}
	return -1;
}

extern "C" BLOOM_API INT __stdcall Store(void* objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		bc->GetBloom()->Store();
		return TRUE;
	}
	catch (CWin32ErrorT& ex)
	{
		bc->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->process_exception(ex);
	}
	return -1;
}

extern "C" BLOOM_API INT __stdcall Load(void* objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		bc->GetBloom()->Load();
		return TRUE;
	}
	catch (CWin32ErrorT& ex)
	{
		bc->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->process_exception(ex);
	}
	return -1;
}

extern "C" BLOOM_API INT __stdcall Close(void* objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		bc->GetBloom()->Close();
		return TRUE;
	}
	catch (CWin32ErrorT& ex)
	{
		bc->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->process_exception(ex);
	}
	return -1;
}

extern "C" BLOOM_API INT __stdcall Abort(void* objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		bc->GetBloom()->Abort();
		return TRUE;
	}
	catch (CWin32ErrorT& ex)
	{
		bc->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->process_exception(ex);
	}
	return -1;
}

extern "C" BLOOM_API INT __stdcall Allocate(void* objptr, unsigned elements)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		bc->GetBloom()->Allocate(elements);
		return TRUE;
	}
	catch (CWin32ErrorT& ex)
	{
		bc->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->process_exception(ex);
	}
	return -1;
}

extern "C" BLOOM_API INT __stdcall PutString(void* objptr, const TCHAR * string)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		bc->GetBloom()->Put(string);
		return TRUE;
	}
	catch (CWin32ErrorT& ex)
	{
		bc->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->process_exception(ex);
	}
	return -1;
}

extern "C" BLOOM_API INT __stdcall PutArray(void* objptr, const BYTE * buffer, unsigned length)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		bc->GetBloom()->Put(buffer, length);
		return TRUE;
	}
	catch (CWin32ErrorT& ex)
	{
		bc->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->process_exception(ex);
	}
	return -1;
}

extern "C" BLOOM_API BOOL __stdcall CheckString(void* objptr, const TCHAR * string)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		return bc->GetBloom()->Check(string);
	}
	catch (CWin32ErrorT& ex)
	{
		bc->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->process_exception(ex);
	}
	return -1;
}

extern "C" BLOOM_API BOOL __stdcall CheckArray(void* objptr, const BYTE * buffer, unsigned length)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	try
	{
		return bc->GetBloom()->Check(buffer, length);
	}
	catch (CWin32ErrorT& ex)
	{
		bc->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->process_exception(ex);
	}
	return -1;
}

extern "C" BLOOM_API USHORT __stdcall HeaderVersion(void* objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	return bc->GetBloom()->GetHeader().Version();
}

extern "C" BLOOM_API ULONG __stdcall HeaderSize(void* objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	return bc->GetBloom()->GetHeader().Size();
}

extern "C" BLOOM_API BYTE __stdcall HeaderHashFunc(void* objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	return bc->GetBloom()->GetHeader().HashFunc();
}
