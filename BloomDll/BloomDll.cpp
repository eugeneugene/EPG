#include "pch.h"
#include "BloomDll.h"
#include "BloomContainer.h"

extern "C" BLOOM_API PVOID __stdcall CreateBloom()
{
	return new CBloomContainer();
}

extern "C" BLOOM_API VOID __stdcall DestroyBloom(PVOID objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	delete bc;
}

extern "C" BLOOM_API INT __stdcall GetErrorClass(PVOID objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	auto bloom_error = bc->GetBloomError();
	if (!bloom_error)
		return -1;
	return static_cast<int>(bloom_error->get_error_class());
}

extern "C" BLOOM_API LONG __stdcall GetErrorCode(PVOID objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	auto bloom_error = bc->GetBloomError();
	if (!bloom_error)
		return -1;
	return bloom_error->get_error_code();
}

extern "C" BLOOM_API INT __stdcall GetErrorMessage(PVOID objptr, WCHAR * buffer, UINT length)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	auto bloom_error = bc->GetBloomError();
	if (!bloom_error)
		return -1;
	_tcscpy_s(buffer, length, bloom_error->get_error_message());
	return 1;
}

extern "C" BLOOM_API UINT __stdcall GetErrorMessageLength(PVOID objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	auto bloom_error = bc->GetBloomError();
	if (!bloom_error)
		return -1;
	return bloom_error->get_error_message_len();
}

extern "C" BLOOM_API INT __stdcall Create(PVOID objptr, const WCHAR * filename)
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

extern "C" BLOOM_API INT __stdcall Open(PVOID objptr, const WCHAR * filename)
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

extern "C" BLOOM_API INT __stdcall Store(PVOID objptr)
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

extern "C" BLOOM_API INT __stdcall Load(PVOID objptr)
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

extern "C" BLOOM_API INT __stdcall Close(PVOID objptr)
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

extern "C" BLOOM_API INT __stdcall Abort(PVOID objptr)
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

extern "C" BLOOM_API INT __stdcall Allocate(PVOID objptr, UINT elements)
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

extern "C" BLOOM_API INT __stdcall PutString(PVOID objptr, const WCHAR * string)
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

extern "C" BLOOM_API INT __stdcall PutArray(PVOID objptr, const BYTE * buffer, UINT length)
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

extern "C" BLOOM_API BOOL __stdcall CheckString(PVOID objptr, const WCHAR * string)
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

extern "C" BLOOM_API BOOL __stdcall CheckArray(PVOID objptr, const BYTE * buffer, UINT length)
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

extern "C" BLOOM_API USHORT __stdcall HeaderVersion(PVOID objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	return bc->GetBloom()->GetHeader().Version();
}

extern "C" BLOOM_API UINT __stdcall HeaderSize(PVOID objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	return bc->GetBloom()->GetHeader().Size();
}

extern "C" BLOOM_API BYTE __stdcall HeaderHashFunc(PVOID objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	return bc->GetBloom()->GetHeader().HashFunc();
}
