#pragma once

#include <string>
#include <stdexcept>
#include <algorithm>
#include <iterator>
#include "fips181_const.h"

class PwdUnit
{
	unsigned unit;
	std::string unit_code;
	std::string symbol_name;

public:
	PwdUnit(std::string _unit_code, std::string _symbol_name, bool _uppercase, unsigned _unit = UINT_MAX) : symbol_name(_symbol_name), unit(_unit)
	{
		if (_uppercase)
			std::transform(_unit_code.cbegin(), _unit_code.cend(), std::back_inserter(unit_code), ::toupper);
		else
			unit_code = _unit_code;
	}

	PwdUnit(char _unit_code, std::string _symbol_name, bool _uppercase, unsigned _unit = UINT_MAX) : symbol_name(_symbol_name), unit(_unit)
	{
		if (_uppercase)
			unit_code += ::toupper(_unit_code);
		else
			unit_code += _unit_code;
	}

	PwdUnit(unsigned _unit, bool _uppercase) : unit(_unit)
	{
		if (_unit < Rules.size())
		{
			if (_uppercase)
				std::transform(std::cbegin(Rules[_unit].unit_code), std::cend(Rules[_unit].unit_code), std::back_inserter(unit_code), ::toupper);
			else
				unit_code = Rules[_unit].unit_code;
			symbol_name = Rules[_unit].unit_code;
		}
		else
			throw std::runtime_error("Invalid unit index");
	}

	unsigned Unit() const
	{
		return unit;
	}

	const std::string* UnitCode() const
	{
		return &unit_code;
	}

	const std::string* SymbolName() const
	{
		return &symbol_name;
	}
};
