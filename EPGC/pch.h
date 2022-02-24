#ifndef PCH_H
#define PCH_H

#include "framework.h"

#include <Windows.h>
#include <tchar.h>

#include <vector>
#include <iostream>
#include <stdexcept>
#include <format>
#include <string>
#include <cstdio>	

#include <CLI/CLI.hpp>

#include <boost/iostreams/filtering_stream.hpp>
#include <boost/iostreams/concepts.hpp>
#include <boost/iostreams/filter/stdio.hpp>
#include <boost/iostreams/operations.hpp>
#include <boost/locale/conversion.hpp>
#include <boost/locale/generator.hpp>

#if defined(_DEBUG)
#include <crtdbg.h>
#endif

#endif //PCH_H
