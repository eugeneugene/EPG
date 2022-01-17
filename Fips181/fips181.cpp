#include "pch.h"
#include "..\Rnd\CryptoRND.h"
#include "fips181.h"

UINT GetRandomUINT(UINT min, UINT max)
{
	if (min == max)
		return min;
	static CCryptoRND pool;
	return pool.GenerateDWORD(min, max);
}

TCHAR UpperChar(TCHAR ch)
{
	auto a = std::find(LowerChars.cbegin(), LowerChars.cend(), ch);
	if (LowerChars.cend() == a)
		return ch;
	return *(a - LowerChars.cbegin() + UpperChars.cbegin());
}
