#pragma once

#include <string>
#include <stdexcept>
#include <algorithm>
#include <iterator>
#include "..\Common\WideHelp.h"
#include "Password.h"

class PwdUnit
{
	unsigned unit;
	unsigned unit_len;
	TCHAR unit_code[3];
	std::_tstring symbol_name;

public:
	PwdUnit(TCHAR _unit_code, const std::_tstring& _symbol_name, bool _uppercase) : symbol_name(_symbol_name), unit(0), unit_len(1)
	{
		ZeroMemory(unit_code, sizeof(unit_code));
		if (_uppercase)
			unit_code[0] = ::UpperChar(_unit_code);
		else
			unit_code[0] = _unit_code;
	}

	PwdUnit(unsigned _unit, bool _uppercase) : unit(_unit)
	{
		if (_unit < Rules.size())
		{
			ZeroMemory(unit_code, sizeof(unit_code));
			for (unsigned i = 0; i < Rules[_unit].unit_len; i++)
			{
				if (_uppercase)
					unit_code[i] = ::UpperChar(Rules[_unit].unit_code[i]);
				else
					unit_code[i] = Rules[_unit].unit_code[i];
			}
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

	const TCHAR* UnitCode() const
	{
		return unit_code;
	}

	const std::_tstring* SymbolName() const
	{
		return &symbol_name;
	}
};
