#pragma once

#include <string>
#include <stdexcept>
#include <algorithm>
#include <iterator>
#include "..\Common\WideHelp.h"

class PwdUnit
{
	unsigned unit;
	unsigned unit_len;
	std::_tstring unit_code;
	std::_tstring symbol_name;

public:
	PwdUnit(TCHAR _unit_code, std::_tstring _symbol_name, bool _uppercase) : symbol_name(_symbol_name), unit(0), unit_len(1)
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
			unit_len = Rules[_unit].unit_len;
		}
		else
			throw std::runtime_error("Invalid unit index");
	}

	unsigned Unit() const
	{
		return unit;
	}

	unsigned UnitLen() const
	{
		return unit_len;
	}

	const std::_tstring* UnitCode() const
	{
		return &unit_code;
	}

	const std::_tstring* SymbolName() const
	{
		return &symbol_name;
	}
};
