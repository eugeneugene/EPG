#pragma once

#include <cstdio>		// EOF.
#include <iostream>		// cin, cout.
#include <Windows.h>	// CharToOemBuff
#include <boost/iostreams/concepts.hpp>
#include <boost/iostreams/filter/stdio.hpp>
#include <boost/iostreams/operations.hpp>
#include <boost/iostreams/filtering_stream.hpp>

inline char Char2OEM(char c)
{
	char strIn[] = { '\0', '\0' };
	char strOut[] = { '\0', '\0' };
	strIn[0] = (char)c;
	CharToOemBuffA(strIn, strOut, 1);
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

class char2oem_input_filter : public boost::iostreams::input_filter {
public:
	template<typename Source> int get(Source& src)
	{
		int c = boost::iostreams::get(src);
		if (EOF != c && WOULD_BLOCK != c)
			return Char2OEM(c);
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

class char2oem_dual_use_filter : public boost::iostreams::dual_use_filter {
public:
	template<typename Source> int get(Source& src)
	{
		int c = boost::iostreams::get(src);
		if (EOF != c && WOULD_BLOCK != c)
			return Char2OEM(c);
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
			else if (c == WOULD_BLOCK)
			{
				retval = z;
				break;
			}
			s[z] = Char2OEM(c);
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
			else if (c == WOULD_BLOCK)
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
			if (!iostreams::put(dest, c))
				break;
		}
		return z;
	}
};
