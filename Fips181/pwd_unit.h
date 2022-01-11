#pragma once
#include <string>
#include "fips181_const.h"

struct pwd_unit
{
	unsigned unit;
	std::string unit_code;
	std::string symbol_name;
	bool uppercase;

	pwd_unit(std::string _unit_code, std::string _symbol_name, bool _uppercase, unsigned _unit = UINT_MAX) : unit_code(_unit_code), symbol_name(_symbol_name), uppercase(_uppercase), unit(_unit)
	{ }

	pwd_unit(char _unit_code, std::string _symbol_name, bool _uppercase, unsigned _unit = UINT_MAX) : symbol_name(_symbol_name), uppercase(_uppercase), unit(_unit)
	{
		unit_code += _unit_code;
	}

	pwd_unit(unsigned _unit, bool _uppercase) : uppercase(_uppercase), unit(_unit)
	{
		if (_unit < Rules.size())
		{
			unit_code = Rules[_unit].unit_code;
			symbol_name = Rules[_unit].unit_code;
		}
	}
};
