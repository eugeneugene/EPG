#ifndef PCH_H
#define PCH_H

#include "framework.h"

#include <Windows.h>
#include <tchar.h>

#if !defined(_DEBUG)
#include <sys\timeb.h>
#endif

#include <array>
#include <string>
#include <vector>
#include <algorithm>
#include <iterator>
#include <stdexcept>

#if defined(_DEBUG)
#include <crtdbg.h>
#endif

#endif //PCH_H
