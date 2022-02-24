#pragma once
#include <string>
#include "..\Bloom\Bloom.h"
#include "../Fips181/PasswordMode.h"

std::wstring check_bloom(const CBloom& bloom, bool paranoid, const std::wstring &word);
Modes get_modes(const std::string& set);
void ShowBuild(boost::iostreams::filtering_wostream& fout/* = fcout*/);
