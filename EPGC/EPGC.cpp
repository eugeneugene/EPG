#include "pch.h"
#include "..\Fips181\Password.h"

int main(int argc, char* argv[], char* envp[])
{
	CLI::App app("Extended Password Generator");
	app.allow_windows_style_options();

	int amount = 0;
	app.add_option("-a,--amount", amount, "Amount of passwords to generate")
		->required()
		->check(CLI::Range(1, 1000), "(1..1000)", "amount_range");


	int length = 0;
	app.add_option("-l,--length", length, "Length of generated passwords (min)")
		->required()
		->check(CLI::Range(1, 100), "(1..100)", "length_range");

	int max = -1;
	app.add_option("-m,--max", max, "Max length of passwords")
		->check(CLI::Range(length, 100), "(length..100)", "max_range");

	char mode = 0;
	app.add_option("-d,--mode", mode, "Mode: p - Pronounceable (default), h - Hyphenated, r - Random")
		->required()
		->check(CLI::IsMember({ 'p','h','r' }), "'p','h','r'");

	std::string set;
	app.add_option("-s,--set", set, "Character set: lL - Small, cC - Capital, nN - Numbers, sS - Symbols")
		->required()
		->check([](const std::string str)
			{
				for (char c : str)
				{
					switch (c)
					{
					case 'l':
					case 'L':
					case 'c':
					case 'C':
					case 'n':
					case 'N':
					case 's':
					case 'S':
						continue;
					default:
						return std::string("Invalid set");
					}
				}
				return std::string();
			}, "lLcCnNsS", "set_collection");

	std::string include;
	app.add_option("-i,--include", include, "Symbols to include");

	std::string exclude;
	app.add_option("-e,--exclude", include, "Symbols to exclude");

	std::string bloom;
	app.add_option("-b,--bloom", bloom, "Check in bloom")
		->check(CLI::ExistingFile);

	bool paranoid = false;
	app.add_flag("-p,--paranoid", paranoid, "Paranoid check");

	bool complexity = false;
	app.add_flag("-c,--complexity", complexity, "Calculate complexity");

	bool csv = false;
	app.add_flag("-v,--csv", csv, "Output in comma-separated values format");

	CLI11_PARSE(app, argc, argv);

	Modes modes = Modes::NoMode;
	for (char c : set)
	{
		switch (c)
		{
		case 'l':
			modes += Modes::Lowers;
			break;
		case 'L':
			modes += Modes::LowersForced;
			break;
		case 'c':
			modes += Modes::Capitals;
			break;
		case 'C':
			modes += Modes::CapitalsForced;
			break;
		case 'n':
			modes += Modes::Numerals;
			break;
		case 'N':
			modes += Modes::NumeralsForced;
			break;
		case 's':
			modes += Modes::Symbols;
			break;
		case 'S':
			modes += Modes::SymbolsForced;
			break;
		default:
			throw std::exception(std::format("Invalid mode '{}'", c).c_str());
		}
	}

	CPassword password(modes, toTstring(include), toTstring(exclude));
	if (password.GenerateRandomWord(length))
	{
		std::_tstring word;
		password.GetWord(word);
		std::wcout << word << std::endl;
	}
	return EXIT_SUCCESS;
}