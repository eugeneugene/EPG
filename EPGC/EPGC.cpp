#include "pch.h"
#include "..\Fips181\Password.h"
#include "..\Rnd\CryptoRND.h"
#include "..\Common\char_to_oem_filters.h"
#include "..\Bloom\Bloom.h"
#include "CPasswordResult.h"
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
	try
	{
		CLI::App app("Extended Password Generator");
		app.allow_windows_style_options();

		int amount = 0;
		app.add_option("-a,--amount", amount, "Amount of passwords to generate")
			->required()
			->check(CLI::Range(1, 255), "(1..255)", "amount_range");


		int length = 0;
		app.add_option("-l,--length", length, "Length of generated passwords (min)")
			->required()
			->check(CLI::Range(1, 255), "(1..255)", "length_range");

		int max = -1;
		app.add_option("-m,--max", max, "Max length of passwords")
			->check(CLI::Range(length, 255), "(length..255)", "max_range");

		char mode = 0;
		app.add_option("-M,--mode", mode, "Mode: p - Pronounceable (default), h - Hyphenated, r - Random")
			->required()
			->check(CLI::IsMember({ 'p','h','H','r'}), "'p','h','H','r'");

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

		Modes modes = get_modes(set);

		CCryptoRND rnd;
		CPassword password(modes, toTstring(include), toTstring(exclude));
		CBloom  _bloom;

		if (!bloom.empty())
		{
			_bloom.Open(toTstring(bloom).c_str());
			_bloom.Load();
		}

		std::vector<CPasswordResult> results;
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

			CPasswordResult result;
			if (mode == 'r')
			{
				if (password.GenerateRandomWord(_length))
				{
					std::_tstring word;
					password.GetWord(word);
					result.word = word;
					result.bloom_result = check_bloom(_bloom, paranoid, word);
				}
			}
			else if (mode == 'p')
			{
				if (password.GenerateWord(_length))
				{
					std::_tstring word;
					password.GetWord(word);
					result.word = word;
					result.bloom_result = check_bloom(_bloom, paranoid, word);
				}
			}
			else if (mode == 'h')
			{
				if (password.GenerateWord(_length))
				{
					std::_tstring word;
					password.GetWord(word);
					std::_tstring hword;
					password.GetHyphenatedWord(hword);
					result.word = hword;
					result.bloom_result = check_bloom(_bloom, paranoid, word);
				}
			}
			else
				throw std::exception("Invalid mode");

			if (complexity)
			{
				DWORD q = PasswordBits(result.word);
				double qp = q * 100.0 / 128.0;
				result.complexity = std::format(L"{:.1f}%", qp);
			}

			results.push_back(result);
		}

		for (auto i : results)
		{
			fcout << i.word << L' ' << i.bloom_result << L' ' << i.complexity << std::endl;
		}
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
