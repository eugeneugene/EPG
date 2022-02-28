#include "pch.h"
#include "..\Fips181\Password.h"
#include "..\Rnd\CryptoRND.h"
#include "..\Common\char_to_oem_filters.h"
#include "..\Bloom\Bloom.h"
#include "PasswordResult.h"
#include "EPGCUtils.h"
#include "../Bloom/bloom_exception.h"


boost::iostreams::filtering_wostream fcerr;
boost::iostreams::filtering_wostream fcout;

int main(int argc, char* argv[], char* envp[])
{
	fcerr.push(char2oem_multichar_output_wfilter());
	fcerr.push(std::wcerr);
	fcout.push(char2oem_multichar_output_wfilter());
	fcout.push(std::wcout);

	int nRetCode = EXIT_SUCCESS;

	CLI::App app("Extended Password Generator", "EPGC.exe");

	try
	{
		app.allow_windows_style_options();

		int amount = 0;
		app.add_option("-A,--amount", amount, "Amount of passwords to generate")
			->required()
			->check(CLI::Range(1, 255), "(1..255)", "amount_range");


		int length = 0;
		app.add_option("-L,--length", length, "Length of generated passwords (min)")
			->required()
			->check(CLI::Range(1, 255), "(1..255)", "length_range");

		int max = -1;
		app.add_option("-m,--max", max, "Max length of passwords")
			->check(CLI::Range(length, 255), "(length..255)", "max_range");

		char mode = 0;
		app.add_option("-M,--mode", mode, "Mode: p - Pronounceable (default), h - Hyphenated, r - Random")
			->required()
			->check(CLI::IsMember({ 'p','h','H','r' }), "'p','h','H','r'");

		std::string set;
		app.add_option("-S,--set", set, "Character set: lL - Small, cC - Capital, nN - Numbers, sS - Symbols")
			->required()
			->check([](const std::string& str)
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
		app.add_option("-x,--exclude", exclude, "Symbols to exclude");

		std::string bloom;
		app.add_option("-b,--bloom", bloom, "Check in bloom")
			->check(CLI::ExistingFile);

		bool paranoid = false;
		app.add_flag("-p,--paranoid", paranoid, "Paranoid check");

		bool complexity = false;
		app.add_flag("-c,--complexity", complexity, "Calculate complexity");

		bool csv = false;
		app.add_flag("-v,--csv", csv, "Output in comma-separated values format");

		char separator = ',';
		app.add_option("-r,--separator", separator, "Use symbol as a CSV separator (default = ',')");

		if (argc == 1)
			throw CLI::CallForHelp();

		app.parse(argc, argv);

		Modes modes = get_modes(set);

		CCryptoRND rnd;
		CPassword password(modes, toWideString(include), toWideString(exclude));
		CBloom _bloom;

		if (!bloom.empty())
		{
			_bloom.Open(toWideString(bloom).c_str());
			_bloom.Load();
		}

		PasswordResult results(mode, _bloom.GetSize(), complexity, csv, toWideChar(separator));
		for (int i = 0; i < amount; i++)
		{
			unsigned _min = length;
			unsigned _max;
			unsigned _length;
			if (max > 0)
			{
				_max = max;
				if (_min > _max)
					throw std::exception("Minimum length of password cannot be greater than maximum length");
				unsigned _delta = _max - _min;
				if (_delta == 0)
					_length = _min;
				else
					_length = _min + rnd.GenerateByte() % (_delta + 1);
			}
			else
				_length = _min;

			PasswordItem item;
			switch (mode)
			{
			case 'r':
				if (password.GenerateRandomWord(_length))
				{
					std::wstring word;
					password.GetWord(word);
					item.word = word;
					item.bloom_result = check_bloom(_bloom, paranoid, word);
				}
				break;
			case 'p':
				if (password.GenerateWord(_length))
				{
					std::wstring word;
					password.GetWord(word);
					item.word = word;
					item.bloom_result = check_bloom(_bloom, paranoid, word);
				}
				break;
			case 'h':
				if (password.GenerateWord(_length))
				{
					std::wstring word;
					password.GetWord(word);
					std::wstring hword;
					password.GetHyphenatedWord(hword);
					item.word = word;
					item.hword = hword;
					item.bloom_result = check_bloom(_bloom, paranoid, word);
				}
				break;
			case 'H':
				if (password.GenerateWord(_length))
				{
					std::wstring word;
					password.GetWord(word);
					std::wstring hword;
					password.GetHyphenatedWord(hword);
					item.word = word;
					item.hword = hword;
					item.bloom_result = check_bloom(_bloom, paranoid, word);
				}
				break;
			}

			if (complexity)
			{
				auto q = PasswordBits(item.word);
				double qp = q * 100.0 / 128.0;
				item.complexity = static_cast<float>(qp);
			}

			results.Results.push_back(item);
		}

		fcout << results;
	}
	catch (const CLI::ParseError& e)
	{
		return app.exit(e);
	}
	catch (bloom_exception& e)
	{
		//ShowBuild(fcerr);
		fcerr << "Bloom Exception: "
			<< e.what()
			<< std::endl;
		nRetCode = EXIT_FAILURE;
	}
	catch (CWin32Error& e)
	{
		//ShowBuild(fcerr);
		fcerr << "Win32 Error: "
			<< e.Description()
			<< std::endl;
		nRetCode = EXIT_FAILURE;
	}
	catch (std::invalid_argument& e)
	{
		//ShowBuild(fcerr);
		fcerr << "Invalid Argument Exception: "
			<< e.what()
			<< std::endl;
		nRetCode = EXIT_FAILURE;
	}
	catch (std::runtime_error& e)
	{
		//ShowBuild(fcerr);
		fcerr << "Runtime Error Exception: "
			<< e.what()
			<< std::endl;
		nRetCode = EXIT_FAILURE;
	}
	catch (std::exception& e)
	{
		//ShowBuild(fcerr);
		fcerr << "C++ Exception: "
			<< e.what()
			<< std::endl;
		nRetCode = EXIT_FAILURE;
	}
	catch (...)
	{
		//ShowBuild(fcerr);
		fcerr << "Unhandled exception was caught"
			<< std::endl;
		nRetCode = EXIT_FAILURE;
	}

	return nRetCode;
}
