#include "pch.h"
#include "Syllable.h"
#include "rand.h"

/*
 * Generate next unit to password, making sure that it follows
 * these rules:
 *   1. Each syllable must contain exactly 1 or 2 consecutive
 *      vowels, where y is considered a vowel.
 *   2. Syllable end is determined as follows:
 *        a. Vowel is generated and previous unit is a
 *           consonant and syllable already has a vowel.  In
 *           this case, new syllable is started and already
 *           contains a vowel.
 *        b. A pair determined to be a "break" pair is encountered.
 *           In this case new syllable is started with second unit
 *           of this pair.
 *        c. End of password is encountered.
 *        d. "begin" pair is encountered legally.  New syllable is
 *           started with this pair.
 *        e. "end" pair is legally encountered.  New syllable has
 *           nothing yet.
 *   3. Try generating another unit if:
 *        a. third consecutive vowel and not y.
 *        b. "break" pair generated but no vowel yet in current
 *           or previous 2 units are "not_end".
 *        c. "begin" pair generated but no vowel in syllable
 *           preceding begin pair, or both previous 2 pairs are
 *          designated "not_end".
 *        d. "end" pair generated but no vowel in current syllable
 *           or in "end" pair.
 *        e. "not_begin" pair generated but new syllable must
 *           begin (because previous syllable ended as defined in
 *           2 above).
 *        f. vowel is generated and 2a is satisfied, but no syllable
 *           break is possible in previous 3 pairs.
 *        g. Second and third units of syllable must begin, and
 *           first unit is "alternate_vowel".
 */
bool Syllable::Generate(int len)
{
	unsigned unit;
	size_t current_unit;
	size_t vowel_count;
	bool rule_broken = false;
	bool want_vowel;
	bool want_another_unit;
	size_t tries1, tries2 = 0;
	size_t last_unit = 0;
	int length_left;

	/*
	 * This is needed if the saved_unit is tries and the syllable then
	 * discarded because of the retry limit. Since the saved_unit is OK and
	 * fits in nicely with the preceding syllable, we will always use it.
	 */
	std::vector<unsigned> HoldSavedUnits(SavedUnits);

	/*
	 * Loop until valid syllable is found.
	 */
	do
	{
		/*
		 * Try for a new syllable.  Initialize all pertinent
		 * syllable variables.
		 */
		tries1 = 0;
		SavedUnits = HoldSavedUnits;
		vowel_count = 0;
		current_unit = 0;
		length_left = len;
		want_another_unit = true;
		UnitsInSyllable.clear();
		/*
		 * This loop finds all the units for the syllable.
		 */
		do
		{
			want_vowel = false;

			/*
			 * This loop continues until a valid unit is found for the
			 * current position within the syllable.
			 */
			do
			{
				/*
				 * If there are saved_unit's from the previous
				 * syllable, use them up first.
				 */
				if (SavedUnits.size() != 0)
				{
					/*
					 * If there were two saved units, the first is
					 * guaranteed (by checks performed in the previous
					 * syllable) to be valid.  We ignore the checks
					 * and place it in this syllable manually.
					 */
					if (SavedUnits.size() == 2)
					{
						UnitsInSyllable.clear();
						UnitsInSyllable.push_back(SavedUnits[1]);
						if (Rules[SavedUnits[1]].flags & VOWEL)
							vowel_count++;
						current_unit++;
						//syl = Rules[SavedUnits[1]].unit_code;
						length_left -= strlen(Rules[SavedUnits[1]].unit_code);
					}
					/*
					 * The unit becomes the last unit checked in the
					 * previous syllable.
					 */
					unit = SavedUnits[0];

					/*
					 * The saved units have been used.  Do not try to
					 * reuse them in this syllable (unless this particular
					 * syllable is rejected at which point we start to rebuild
					 * it with these same saved units.
					 */
					SavedUnits.clear();
				}
				else
				{
					/*
					** If we don't have to scoff the saved units,
					** we generate a random one.  If we know it has
					** to be a vowel, we get one rather than looping
					** through until one shows up.
					*/
					if (want_vowel)
						unit = RandomRuler(VOWEL);
					else
						unit = RandomRuler(NO_SPECIAL_RULE);
				}
				length_left -= strlen(Rules[unit].unit_code);

				/*
				 * Prevent having a word longer than expected.
				 */
				if (length_left < 0)
					rule_broken = true;
				else
					rule_broken = false;

				/*
				 * First unit of syllable.  This is special because the
				 * digram tests require 2 units and we don't have that yet.
				 * Nevertheless, we can perform some checks.
				 */
				if (current_unit == 0)
				{
					/*
					 * If the shouldn't begin a syllable, don't
					 * use it.
					 */
					if (Rules[unit].flags & NOT_BEGIN_SYLLABLE)
						rule_broken = true;
					else
					{
						/*
						** If this is the last unit of a word,
						** we have a one unit syllable.  Since each
						** syllable must have a vowel, we make sure
						** the unit is a vowel.  Otherwise, we
						** discard it.
						*/
						if (length_left == 0)
						{
							if (Rules[unit].flags & VOWEL)
								want_another_unit = false;
							else
								rule_broken = true;
						}
					}
				}
				else
				{
					/*
					 * There are some digram tests that are
					 * universally true.  We test them out.
					 */

					 /*
					  * Reject ILLEGAL_PAIRS of units.
					  */
					if ((Allowed(UnitsInSyllable.data(), current_unit, unit, ILLEGAL_PAIR)) ||

						/*
						 * Reject units that will be split between syllables
						 * when the syllable has no vowels in it.
						 */
						(Allowed(UnitsInSyllable.data(), current_unit, unit, BREAK) && (vowel_count == 0)) ||

						/*
						 * Reject a unit that will end a syllable when no
						 * previous unit was a vowel and neither is this one.
						 */
						(Allowed(UnitsInSyllable.data(), current_unit, unit, END) && (vowel_count == 0) && !(Rules[unit].flags & VOWEL)))
						rule_broken = true;

					if (current_unit == 1)
					{
						/*
						 * Reject the unit if we are at the starting digram of
						 * a syllable and it does not fit.
						 */
						if (Allowed(UnitsInSyllable.data(), current_unit, unit, NOT_BEGIN))
							rule_broken = true;
					}
					else
					{
						/*
						 * We are not at the start of a syllable.
						 * Save the previous unit for later tests.
						 */
						last_unit = UnitsInSyllable[current_unit - 1];

						/*
						 * Do not allow syllables where the first letter is y
						 * and the next pair can begin a syllable.  This may
						 * lead to splits where y is left alone in a syllable.
						 * Also, the combination does not sound to good even
						 * if not split.
						 */
						if (((current_unit == 2) && (Allowed(UnitsInSyllable.data(), current_unit, unit, BEGIN))
							&& (Rules[UnitsInSyllable[0]].flags & ALTERNATE_VOWEL)) ||

							/*
							 * If this is the last unit of a word, we should
							 * reject any digram that cannot end a syllable.
							 */
							(Allowed(UnitsInSyllable.data(), current_unit, unit, NOT_END) && (length_left == 0)) ||

							/*
							 * Reject the unit if the digram it forms wants
							 * to break the syllable, but the resulting
							 * digram that would end the syllable is not
							 * allowed to end a syllable.
							 */
							(Allowed(UnitsInSyllable.data(), current_unit, unit, BREAK) &&
								(Digram[UnitsInSyllable[current_unit - 2]][last_unit] & NOT_END)) ||

							/*
							 * Reject the unit if the digram it forms
							 * expects a vowel preceding it and there is
							 * none.
							 */
							(Allowed(UnitsInSyllable.data(), current_unit, unit, PREFIX) &&
								!(Rules[UnitsInSyllable[current_unit - 2]].flags & VOWEL)))
							rule_broken = true;

						/*
						 * The following checks occur when the current unit
						 * is a vowel and we are not looking at a word ending
						 * with an e.
						 */
						if (!rule_broken && (Rules[unit].flags & VOWEL) && ((length_left > 0) ||
							!(Rules[last_unit].flags & NO_FINAL_SPLIT)))

						{
							/*
							** Don't allow 3 consecutive vowels in a
							** syllable.  Although some words formed like this
							** are OK, like beau, most are not.
							*/
							if ((vowel_count > 1) && (Rules[last_unit].flags & VOWEL))
								rule_broken = true;
							else
							{
								/*
								** Check for the case of
								** vowels-consonants-vowel, which is only
								** legal if the last vowel is an e and we are
								** the end of the word (wich is not
								** happening here due to a previous check.
								*/
								if ((vowel_count != 0) && !(Rules[last_unit].flags & VOWEL))
								{
									/*
									** Try to save the vowel for the next
									** syllable, but if the syllable left here
									** is not proper (i.e., the resulting last
									** digram cannot legally end it), just
									** discard it and try for another.
									*/
									if (Digram[UnitsInSyllable[current_unit - 2]][last_unit] & NOT_END)
										rule_broken = true;
									else
									{
										SavedUnits.clear();
										SavedUnits.push_back(unit);
										want_another_unit = false;
									}
								}
							}
						}
					}

					/*
					 * The unit picked and the digram formed are legal.
					 * We now determine if we can end the syllable.  It may,
					 * in some cases, mean the last unit(s) may be deferred to
					 * the next syllable.  We also check here to see if the
					 * digram formed expects a vowel to follow.
					 */
					if (!rule_broken && want_another_unit)
					{
						/*
						 * This word ends in a silent e.
						 */
						if (((vowel_count != 0) && (Rules[unit].flags & NO_FINAL_SPLIT) &&
							(length_left == 0) && !(Rules[last_unit].flags & VOWEL)) ||

							/*
							 * This syllable ends either because the digram
							 * is an END pair or we would otherwise exceed
							 * the length of the word.
							 */
							(Allowed(UnitsInSyllable.data(), current_unit, unit, END) || (length_left == 0)))
							want_another_unit = false;
						else
						{
							/*
							** Since we have a vowel in the syllable
							** already, if the digram calls for the end of the
							** syllable, we can legally split it off. We also
							** make sure that we are not at the end of the
							** dangerous because that syllable may not have
							** vowels, or it may not be a legal syllable end,
							** and the retrying mechanism will loop infinitely
							** with the same digram.
							*/
							if ((vowel_count != 0) && (length_left > 0))
							{
								/*
								** If we must begin a syllable, we do so if
								** the only vowel in THIS syllable is not part
								** of the digram we are pushing to the next
								** syllable.
								*/
								if (Allowed(UnitsInSyllable.data(), current_unit, unit, BEGIN) && (current_unit > 1) &&
									!((vowel_count == 1) && (Rules[last_unit].flags & VOWEL)))
								{
									SavedUnits.clear();
									SavedUnits.push_back(last_unit);
									SavedUnits.push_back(unit);
									want_another_unit = false;
								}
								else
								{
									if (Allowed(UnitsInSyllable.data(), current_unit, unit, BREAK))
									{
										SavedUnits.clear();
										SavedUnits.push_back(unit);
										want_another_unit = false;
									}
								}
							}
							else
							{
								if (Allowed(UnitsInSyllable.data(), current_unit, unit, SUFFIX))
									want_vowel = true;
							}
						}
					}
				}
				tries1++;

				/*
				 * If this unit was illegal, redetermine the amount of
				 * letters left to go in the word.
				 */
				if (rule_broken)
					length_left += strlen(Rules[unit].unit_code);
			} while (rule_broken && (tries1 <= MaxRetries(len)));
			/*
			 * The unit fit OK.
			 */
			if (tries1 <= MaxRetries(len))
			{
				/*
				 * If the unit were a vowel, count it in.
				 * However, if the unit were a y and appear
				 * at the start of the syllable, treat it
				 * like a constant (so that words like year can
				 * appear and not conflict with the 3 consecutive
				 * vowel rule.
				 */
				if ((Rules[unit].flags & VOWEL) && ((current_unit > 0) ||
					!(Rules[unit].flags & ALTERNATE_VOWEL)))
					vowel_count++;

				/*
				 * If a unit or units were to be saved, we must
				 * adjust the syllable formed.  Otherwise, we
				 * append the current unit to the syllable.
				 */
				switch (SavedUnits.size())
				{
				case 0:
					UnitsInSyllable.push_back(unit);
					//syl += Rules[unit].unit_code;
					break;
				case 1:
					current_unit--;
					break;
				case 2:
					//syl.erase(syl.length() - strlen(Rules[last_unit].unit_code));
					UnitsInSyllable.pop_back();
					length_left += strlen(Rules[last_unit].unit_code);
					current_unit -= 2;
					break;
				}
			}
			else
			{
				/*
				** Whoops!  Too many tries.  We set rule_broken so we can
				** loop in the outer loop and try another syllable.
				*/
				rule_broken = true;
			}
			/*
			 * ...and the syllable length grows.
			 */
			current_unit++;
		} while ((tries1 <= MaxRetries(len)) && want_another_unit);
		if (++tries2 >= MaxRetries(len))
		{
			SavedUnits.clear();
			return false;
		}
	} while (rule_broken || IllegalPlacement(UnitsInSyllable));

	return true;
}

/*
 * This routine goes through an individual syllable and checks
 * for illegal combinations of letters that go beyond looking
 * at digrams.  We look at things like 3 consecutive vowels or
 * consonants, or syllables with consonants between vowels (unless
 * one of them is the final silent e).
 */
bool Syllable::IllegalPlacement(const std::vector<unsigned> units) const
{
	size_t vowel_count = 0;
	size_t unit_count;
	auto len = units.size();

	for (unit_count = 0; unit_count < len; unit_count++)
	{
		if (unit_count) // > 0
		{
			/*
			 * Don't allow vowels to be split with consonants in
			 * a single syllable.  If we find such a combination
			 * (except for the silent e) we have to discard the
			 * syllable).
			 */
			if ((!(Rules[units[unit_count - 1]].flags & VOWEL) &&
				(Rules[units[unit_count]].flags & VOWEL) &&
				!((Rules[units[unit_count]].flags & NO_FINAL_SPLIT) &&
					(unit_count == len)) && (vowel_count != 0)) ||

				/*
				 * Perform these checks when we have at least 3 units.
				 */
				((unit_count >= 2) &&

					/*
					 * Disallow 3 consecutive consonants.
					 */
					((!(Rules[units[unit_count - 2]].flags & VOWEL) &&
						!(Rules[units[unit_count - 1]].flags & VOWEL) &&
						!(Rules[units[unit_count]].flags & VOWEL)) ||

						/*
						 * Disallow 3 consecutive vowels, where the first is
						 * not a y.
						 */
						(((Rules[units[unit_count - 2]].flags & VOWEL) &&
							!((Rules[units[0]].flags & ALTERNATE_VOWEL) &&
								(unit_count == 2))) && (Rules[units[unit_count - 1]].flags & VOWEL) &&
							(Rules[units[unit_count]].flags & VOWEL)))))
				return true;
		}

		/*
		 * Count the vowels in the syllable.  As mentioned somewhere
		 * above, exclude the initial y of a syllable.  Instead,
		 * treat it as a consonant.
		 */
		if ((Rules[units[unit_count]].flags & VOWEL) &&
			!((Rules[units[0]].flags & ALTERNATE_VOWEL) && unit_count == 0 && len != 0))
			vowel_count++;
	}

	return false;
}

unsigned Syllable::RandomRuler(flag_t type)
{
	unsigned i;
	/*
	 * Sometimes, we are asked to explicitly get a vowel (i.e., if
	 * a digram pair expects one following it).  This is a shortcut
	 * to do that and avoid looping with rejected consonants.
	 */
	if (type & VOWEL)
		i = vowel_rulers[GetRandomUINT(0, vowel_rulers.size() - 1)];
	else
		// Get any letter according to the English distribution.
		i = rulers[GetRandomUINT(0, rulers.size() - 1)];
	return i;
}
