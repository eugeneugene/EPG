#pragma once

#include <cstdio>		// EOF.
#include <iostream>		// cin, cout.
#include <Windows.h>	// CharToOemBuff
#include <tchar.h>
#include <boost/iostreams/concepts.hpp>
#include <boost/iostreams/filter/stdio.hpp>
#include <boost/iostreams/operations.hpp>
#include <boost/iostreams/filtering_stream.hpp>

inline char Char2OEM(char c)
{
	char strIn[1] = { 0 };
	char strOut[1] = { 0 };
	strIn[0] = (char)c;
	CharToOemBuffA(strIn, strOut, 1);
	return strOut[0];
}

inline wchar_t WChar2OEM(wchar_t c)
{
	wchar_t strIn[1] = { 0 };
	wchar_t strOut[1] = { 0 };
	strIn[0] = (wchar_t)c;
	CharToOemBuffW(strIn,(LPSTR) strOut, 1);
	return strOut[0];
}

class char2oem_stdio_filter : public boost::iostreams::stdio_filter
{
private:
	void do_filter()
	{
		int c;
		while ((c = std::cin.get()) != EOF)
		{
			std::cout.put(Char2OEM(c));
		}
	}
};

class char2oem_wstdio_wfilter : public boost::iostreams::wstdio_wfilter
{
private:
	void do_filter()
	{
		int c;
		while ((c = std::wcin.get()) != EOF)
		{
			std::wcout.put(Char2OEM(c));
		}
	}
};

class char2oem_input_filter : public boost::iostreams::input_filter {
public:
	template<typename Source> int get(Source& src)
	{
		int c = boost::iostreams::get(src);
		if (EOF != c && boost::iostreams::WOULD_BLOCK != c)
			return Char2OEM(c);
		return c;
	}
};

class char2oem_input_wfilter : public boost::iostreams::input_wfilter {
public:
	template<typename Source> int get(Source& src)
	{
		int c = boost::iostreams::get(src);
		if (EOF != c && boost::iostreams::WOULD_BLOCK != c)
			return WChar2OEM(c);
		return c;
	}
};

class char2oem_output_filter : public boost::iostreams::output_filter {
public:
	template<typename Sink> bool put(Sink& dest, int c)
	{
		return boost::iostreams::put(dest, Char2OEM(c));
	}
};

class char2oem_output_wfilter : public boost::iostreams::output_wfilter {
public:
	template<typename Sink> bool put(Sink& dest, int c)
	{
		return boost::iostreams::put(dest, WChar2OEM(c));
	}
};

class char2oem_dual_use_filter : public boost::iostreams::dual_use_filter {
public:
	template<typename Source> int get(Source& src)
	{
		int c = boost::iostreams::get(src);
		if (EOF != c && boost::iostreams::WOULD_BLOCK != c)
			return Char2OEM(c);
		return c;
	}
	template<typename Sink> bool put(Sink& dest, int c)
	{
		return boost::iostreams::put(dest, Char2OEM(c));
	}
};

class char2oem_dual_use_wfilter : public boost::iostreams::dual_use_wfilter {
public:
	template<typename Source> int get(Source& src)
	{
		int c = boost::iostreams::get(src);
		if (EOF != c && boost::iostreams::WOULD_BLOCK != c)
			return WChar2OEM(c);
		return c;
	}
	template<typename Sink> bool put(Sink& dest, int c)
	{
		return boost::iostreams::put(dest, Char2OEM(c));
	}
};

class char2oem_multichar_input_filter : public boost::iostreams::multichar_input_filter {
public:
	template<typename Source> std::streamsize read(Source& src, char* s, std::streamsize n)
	{
		std::streamsize retval = n;
		for (std::streamsize z = 0; z < n; ++z)
		{
			int c;
			if ((c = boost::iostreams::get(src)) == EOF)
			{
				retval = (0 != z ? z : EOF);
				break;
			}
			else if (c == boost::iostreams::WOULD_BLOCK)
			{
				retval = z;
				break;
			}
			s[z] = Char2OEM(c);
		}
		return retval;
	}
};

class char2oem_multichar_input_wfilter : public boost::iostreams::multichar_input_wfilter {
public:
	template<typename Source> std::streamsize read(Source& src, wchar_t* s, std::streamsize n)
	{
		std::streamsize retval = n;
		for (std::streamsize z = 0; z < n; ++z)
		{
			int c;
			if ((c = boost::iostreams::get(src)) == EOF)
			{
				retval = (0 != z ? z : EOF);
				break;
			}
			else if (c == boost::iostreams::WOULD_BLOCK)
			{
				retval = z;
				break;
			}
			s[z] = WChar2OEM(c);
		}
		return retval;
	}
};

class char2oem_multichar_output_filter : public boost::iostreams::multichar_output_filter {
public:
	template<typename Sink> std::streamsize write(Sink& dest, const char* s, std::streamsize n)
	{
		std::streamsize z;
		for (z = 0; z < n; ++z)
		{
			int c = Char2OEM(s[z]);
			if (!boost::iostreams::put(dest, c))
				break;
		}
		return z;
	}
};

class char2oem_multichar_output_wfilter : public boost::iostreams::multichar_output_wfilter {
public:
	template<typename Sink> std::streamsize write(Sink& dest, const wchar_t* s, std::streamsize n)
	{
		std::streamsize z;
		for (z = 0; z < n; ++z)
		{
			int c = WChar2OEM(s[z]);
			if (!boost::iostreams::put(dest, c))
				break;
		}
		return z;
	}
};

class char2oem_multichar_idual_use_filter : public boost::iostreams::multichar_dual_use_filter {
public:
	template<typename Source> std::streamsize read(Source& src, char* s, std::streamsize n)
	{
		std::streamsize retval = n;
		for (std::streamsize z = 0; z < n; ++z)
		{
			int c;
			if ((c = boost::iostreams::get(src)) == EOF)
			{
				retval = (0 != z ? z : EOF);
				break;
			}
			else if (c == boost::iostreams::WOULD_BLOCK)
			{
				retval = z;
				break;
			}
			s[z] = Char2OEM(c);
		}
		return retval;
	}
	template<typename Sink> std::streamsize write(Sink& dest, const char* s, std::streamsize n)
	{
		std::streamsize z;
		for (z = 0; z < n; ++z)
		{
			int c = Char2OEM(s[z]);
			if (!boost::iostreams::put(dest, c))
				break;
		}
		return z;
	}
};

class char2oem_multichar_idual_use_wfilter : public boost::iostreams::multichar_dual_use_wfilter {
public:
	template<typename Source> std::streamsize read(Source& src, wchar_t* s, std::streamsize n)
	{
		std::streamsize retval = n;
		for (std::streamsize z = 0; z < n; ++z)
		{
			int c;
			if ((c = boost::iostreams::get(src)) == EOF)
			{
				retval = (0 != z ? z : EOF);
				break;
			}
			else if (c == boost::iostreams::WOULD_BLOCK)
			{
				retval = z;
				break;
			}
			s[z] = WChar2OEM(c);
		}
		return retval;
	}
	template<typename Sink> std::streamsize write(Sink& dest, const wchar_t* s, std::streamsize n)
	{
		std::streamsize z;
		for (z = 0; z < n; ++z)
		{
			int c = WChar2OEM(s[z]);
			if (!boost::iostreams::put(dest, c))
				break;
		}
		return z;
	}
};
