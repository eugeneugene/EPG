#pragma once
#include <string>
#include <vector>
#include "fips181_const.h"
#include "fips181.h"

/// <summary>
/// Слог
/// </summary>
class Syllable
{
private:
	std::vector<unsigned> saved_units;
	std::vector<unsigned> units_in_syllable;

public:
	bool Generate(int len);

	const std::vector<unsigned>* UnitsInSyllable() const
	{
		return &units_in_syllable;
	}

	void GetSyllable(std::string& out) const
	{
		out.clear();
		for (auto unit : units_in_syllable)
			out += Rules[unit].unit_code;
	}

private:
	bool IllegalPlacement(const std::vector<unsigned>& units) const;
	static unsigned RandomRuler(flag_t type);

	inline unsigned MaxRetries(size_t len) const
	{
		return static_cast<unsigned>(4 * len + Rules.size());
	}

	int Allowed(size_t nCurrentUnit, size_t nUnit, int nFlag) const
	{
		return Digram[units_in_syllable[nCurrentUnit - 1]][nUnit] & nFlag;
	}
};