#include "pch.h"
#include "bloom_exception.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

// **** Error messages **** //
const char* bloom_exception::bloom_str_error(_error e)
{
	switch (e)
	{
	case _error::BLOOM_OK:
		return "No error";
	case _error::BLOOM_ERROR_NOT_OPENED:
		return "Filter not opened";
	case _error::BLOOM_ERROR_NOT_ALLOCATED:
		return "Filter not loaded into memory";
	case _error::BLOOM_ERROR_ALREADY_OPENED:
		return "Filter already opened";
	case _error::BLOOM_ERROR_ALREADY_ALLOCATED:
		return "Filter already loaded into memory";
	case _error::BLOOM_ERROR_UNKNOWN_HEADER:
		return "Unknown filter format";
	case _error::BLOOM_ERROR_UNKNOWN_VERSION:
		return "Unsupported version";
	case _error::BLOOM_INVALID_OPERATION:
		return "Illegal operation";
	case _error::BLOOM_ERROR_CORRUPTED:
		return "Bloom filter corrupted";
	case _error::BLOOM_INTERNAL_COMPATIBLE_DIGESTSIZE:
		return "Internal error: Insufficient COMPATIBLE_DIGESTSIZE";
	case _error::BLOOM_INTERNAL_HASH_DIGESTSIZE:
		return "Internal error: Insufficient HASH_DIGESTSIZE";
	}
	return "Unknown error";
};
