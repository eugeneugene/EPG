#pragma once

class bloom_exception : public std::exception
{
public:
	//! error types
	enum class _error {
		BLOOM_OK,
		BLOOM_ERROR_NOT_OPENED,
		BLOOM_ERROR_NOT_ALLOCATED,
		BLOOM_ERROR_ALREADY_OPENED,
		BLOOM_ERROR_ALREADY_ALLOCATED,
		BLOOM_ERROR_UNKNOWN_HEADER,
		BLOOM_ERROR_UNKNOWN_VERSION,
		BLOOM_INVALID_OPERATION,
		BLOOM_ERROR_CORRUPTED,
		BLOOM_INTERNAL_COMPATIBLE_DIGESTSIZE,
		BLOOM_INTERNAL_HASH_DIGESTSIZE,
	};
	bloom_exception(_error e) : _m_what(bloom_str_error(e)), _m_error(e) {}
	virtual ~bloom_exception() {};
	const char* what() const
	{
		return _m_what;
	}
	_error error() const
	{
		return _m_error;
	}
private:
	static const char* bloom_str_error(_error e);
	const char* _m_what;
	const _error _m_error;
};
