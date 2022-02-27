#pragma once

#include "framework.h"

#include <Windows.h>
#include <tchar.h>
#include <shlwapi.h>

#include <CLI/CLI.hpp>

#include <string>
#include <vector>
#include <sstream>
#include <cstdio>
#include <iostream>
#include <format>
#include <stdexcept>
#include <utility>	

#include <boost/iostreams/concepts.hpp>
#include <boost/iostreams/filter/stdio.hpp>
#include <boost/iostreams/operations.hpp>
#include <boost/iostreams/filtering_stream.hpp>
#include <boost/locale/conversion.hpp>
#include <boost/locale/generator.hpp>

#if defined(_DEBUG)
#include <crtdbg.h>
#endif
