// pch.h: This is a precompiled header file.
// Files listed below are compiled only once, improving build performance for future builds.
// This also affects IntelliSense performance, including code completion and many code browsing features.
// However, files listed here are ALL re-compiled if any one of them is updated between builds.
// Do not add files here that you will be updating frequently as this negates the performance advantage.

#ifndef PCH_H
#define PCH_H

// add headers that you want to pre-compile here
#include "framework.h"

#include <Windows.h>

#include <io.h>
#include <fcntl.h>
#include <stdexcept>
#include <tchar.h>
#include <memory>
#include <stdio.h>
#include <string>
#include <iostream>		// cin, cout.
#include <sstream>
#include <cstdio>		// EOF.
#include <boost/iostreams/concepts.hpp>
#include <boost/iostreams/filter/stdio.hpp>
#include <boost/iostreams/operations.hpp>
#include <boost/iostreams/filtering_stream.hpp>

#if defined(_DEBUG)
#include <crtdbg.h>
#endif

#endif //PCH_H
