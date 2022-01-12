#ifndef PCH_H
#define PCH_H

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
