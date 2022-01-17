#pragma once

enum class Modes { NoMode = 0, Lowers = 0x01, LowersForced = 0x11, Capitals = 0x02, CapitalsForced = 0x22, Numerals = 0x04, NumeralsForced = 0x44, Symbols = 0x08, SymbolsForced = 0x88 };

inline Modes operator+(Modes modes1, Modes modes2)
{
	return static_cast<Modes>(static_cast<int>(modes1) | static_cast<int>(modes2));
}

inline Modes operator+=(Modes modes1, Modes modes2)
{
	return static_cast<Modes>(static_cast<int>(modes1) | static_cast<int>(modes2));
}

inline Modes operator&(Modes modes1, Modes modes2)
{
	return static_cast<Modes>(static_cast<int>(modes1) & static_cast<int>(modes2));
}

inline Modes operator&=(Modes modes1, Modes modes2)
{
	return static_cast<Modes>(static_cast<int>(modes1) & static_cast<int>(modes2));
}

inline Modes operator-(Modes modes1, Modes modes2)
{
	return static_cast<Modes>(static_cast<int>(modes1) & ~static_cast<int>(modes2));
}

inline Modes operator-=(Modes modes1, Modes modes2)
{
	return static_cast<Modes>(static_cast<int>(modes1) & ~static_cast<int>(modes2));
}

inline Modes operator~(Modes modes)
{
	return static_cast<Modes>(~static_cast<int>(modes));
}

inline bool operator==(Modes modes1, Modes modes2)
{
	return static_cast<int>(modes1) == static_cast<int>(modes2);
}

inline bool operator!=(Modes modes1, Modes modes2)
{
	return static_cast<int>(modes1) != static_cast<int>(modes2);
}

class CPasswordMode
{
private:
	Modes modes;

public:
	CPasswordMode(const Modes& _modes) : modes(_modes)
	{}

	bool IsSet(Modes _modes) const
	{
		return (modes & _modes) == _modes;
	}

	bool IsNotSet(Modes _modes) const
	{
		return (modes + ~_modes) == _modes;
	}

	bool IsClear() const
	{
		return modes == Modes::NoMode;
	}

	CPasswordMode& operator=(const CPasswordMode& passwordMode) const
	{
		CPasswordMode mode(passwordMode.modes);
		return mode;
	}
	CPasswordMode& operator=(Modes _modes) const
	{
		CPasswordMode mode(_modes);
		return mode;
	}
	CPasswordMode& operator += (Modes _modes)
	{
		//modes += _modes;
		modes = modes + _modes;
		return *this;
	}
	CPasswordMode& operator &= (Modes _modes)
	{
		//modes &= _modes;
		modes = modes & _modes;
		return *this;
	}
	CPasswordMode& operator -= (Modes _modes)
	{
		//modes -= _modes;
		modes = modes - _modes;
		return *this;
	}

	friend CPasswordMode operator+(const CPasswordMode& passwordMode1, const CPasswordMode& passwordMode2);
	friend CPasswordMode operator&(const CPasswordMode& passwordMode1, const CPasswordMode& passwordMode2);
	friend CPasswordMode operator-(const CPasswordMode& passwordMode1, const CPasswordMode& passwordMode2);
	friend bool operator==(const CPasswordMode& passwordMode1, const CPasswordMode& passwordMode2);
	friend bool operator!=(const CPasswordMode& passwordMode1, const CPasswordMode& passwordMode2);
};

inline CPasswordMode operator+(const CPasswordMode& passwordMode1, const CPasswordMode& passwordMode2)
{
	CPasswordMode mode(passwordMode1.modes + passwordMode2.modes);
	return mode;
}

inline CPasswordMode operator&(const CPasswordMode& passwordMode1, const CPasswordMode& passwordMode2)
{
	CPasswordMode mode(passwordMode1.modes & passwordMode2.modes);
	return mode;
}

inline CPasswordMode operator-(const CPasswordMode& passwordMode1, const CPasswordMode& passwordMode2)
{
	CPasswordMode mode(passwordMode1.modes - passwordMode2.modes);
	return mode;
}

inline bool operator==(const CPasswordMode& passwordMode1, const CPasswordMode& passwordMode2)
{
	return passwordMode1.modes == passwordMode2.modes;
}

inline bool operator!=(const CPasswordMode& passwordMode1, const CPasswordMode& passwordMode2)
{
	return passwordMode1.modes != passwordMode2.modes;
}
