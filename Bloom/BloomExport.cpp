#include "pch.h"
#include "BloomExport.h"
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
	return static_cast<int>(bc->GetBloomError()->GetErrorClass());
}

extern "C" BLOOM_API LONG __stdcall GetErrorCode(void* objptr)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	return bc->GetBloomError()->GetErrorCode();
}

/// <summary>
/// Получить сообщение об ошибке
/// </summary>
/// <param name="objptr">Указатель на CBloomContainer</param>
/// <param name="buffer">Буфер, куда будет скопировано сообщение об ошибке. Должно быть выделено length символов </param>
/// <param name="length">Размер буфера. Если *length меньше требуемого размера, то метод возвращает 0, а в *length копируется требуемый размер буфера.
/// Выделять нужно на 1 символ больше под финальный '\0'</param>
/// <returns>Если возвращается 0, то *length содержит требуемый размер буфера,
/// Если возвращается 1, то *buffer содержит сообщение об ошибке</returns>
extern "C" BLOOM_API INT __stdcall GetErrorMessage(void* objptr, TCHAR * buffer, DWORD * length)
{
	CBloomContainer* bc = (CBloomContainer*)objptr;
	if (!bc)
		throw std::invalid_argument("Bloom Filter Container pointer cannot be null");
	if (bc->GetBloomError()->GetErrorMessageLen() > *length)
	{
		*length = (DWORD)bc->GetBloomError()->GetErrorMessageLen();
		return 0;
	}
	_tcscpy_s(buffer, *length, bc->GetBloomError()->GetErrorMessage());
	*length = (DWORD)bc->GetBloomError()->GetErrorMessageLen();
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
		bc->GetBloomError()->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
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
		bc->GetBloomError()->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
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
		bc->GetBloomError()->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
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
		bc->GetBloomError()->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
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
		bc->GetBloomError()->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
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
		bc->GetBloomError()->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
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
		bc->GetBloomError()->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
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
		bc->GetBloomError()->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
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
		bc->GetBloomError()->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
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
		bc->GetBloomError()->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
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
		bc->GetBloomError()->process_exception(ex);
	}
	catch (bloom_exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
	}
	catch (std::exception& ex)
	{
		bc->GetBloomError()->process_exception(ex);
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
