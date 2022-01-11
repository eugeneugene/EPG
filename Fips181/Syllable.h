#pragma once
#include <string>
#include <vector>
#include "fips181_const.h"
#include "fips181.h"

class Syllable
{
	std::vector<unsigned> SavedUnits;

public:
	std::vector<unsigned> UnitsInSyllable;

	bool Generate(int len);

	std::string GetSyllable()
	{
		std::string s;
		for (auto unit : UnitsInSyllable)
			s += Rules[unit].unit_code;
		return s;
	}

private:
	bool IllegalPlacement(const std::vector<unsigned> units) const;
	static unsigned RandomRuler(flag_t type);

	inline unsigned MaxRetries(size_t len)
	{
		return static_cast<unsigned>(4 * len + Rules.size());
	}

	int Allowed(const unsigned* UnitsInSyllable, size_t nCurrentUnit, size_t nUnit, int nFlag)
	{
		return Digram[UnitsInSyllable[nCurrentUnit - 1]][nUnit] & nFlag;
	}
};