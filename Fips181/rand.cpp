#include "pch.h"
#include "..\Rnd\CryptoRND.h"

UINT GetRandomUINT(UINT min, UINT max)
{
	if (min == max)
		return min;
	static CCryptoRND pool;
	return pool.GenerateDWORD(min, max);
}
