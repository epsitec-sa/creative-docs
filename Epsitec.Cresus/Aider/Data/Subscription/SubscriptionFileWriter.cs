using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Text;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;

using Epsitec.Data.Platform.MatchSort;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.IO;

using System.Linq;


namespace Epsitec.Aider.Data.Subscription
{


	internal sealed class SubscriptionFileWriter
	{


		public SubscriptionFileWriter
		(
			CoreData coreData,
			FileInfo outputFile,
			FileInfo errorFile,
			bool skipLinesWithPostmanNumberError
		)
		{
			this.coreData = coreData;
			this.outputFile = outputFile;
			this.errorFile = errorFile;
			this.skipLinesWithPostmanNumberError = skipLinesWithPostmanNumberError;
			this.startTime = System.DateTime.UtcNow;

			this.errors = new List<Tuple<string, string>> ();
			this.postmanErrors = new List<Tuple<string, string>> ();
			this.editionStats = new int[16];
			this.countries = new HashSet<string> ();
		}


		public void Write()
		{
			this.errors.Clear ();

			SubscriptionFileLine.Write (this.GetLines (), this.outputFile);

			this.LogErrors ();
			this.LogStats ();
		}


		private void LogErrors()
		{
			if ((this.errors.Count > 0 || this.postmanErrors.Count > 0) && this.errorFile != null)
			{
				using (var stream = this.errorFile.Open (FileMode.Create, FileAccess.Write))
				using (var streamWriter = new StreamWriter (stream))
				{
					foreach (var error in this.errors.Concat (this.postmanErrors))
					{
						streamWriter.WriteLine (error.Item1 + " => " + error.Item2);
					}
				}
			}
		}

		private void LogStats()
		{
			var lines = new List<string> ();
			var time  = (int)((System.DateTime.UtcNow - this.startTime).TotalMinutes);
			
			lines.Add (string.Format ("Export du {0} à {1}, {2} minutes", System.DateTime.Now.ToShortDateString (), System.DateTime.Now.ToShortTimeString (), time));
			lines.Add (string.Format ("Total: {0}", this.totalCount));
			lines.Add (string.Format ("Vaud: {0}", this.totalVaud));
			lines.Add (string.Format ("Suisse: {0}", this.totalSwiss));
			lines.Add (string.Format ("Étranger: {0}", this.totalForeign));
			lines.Add ("");
			lines.Add ("Pays:");
			lines.AddRange (this.countries.OrderBy (x => x));
			lines.Add ("");

			for (int i = 0; i < this.editionStats.Length; i++)
			{
				if (this.editionStats[i] != 0)
				{
					lines.Add (string.Format ("Edition {0} (N{0:X});{1}", i, this.editionStats[i]));
				}
			}

			System.IO.File.WriteAllLines (this.outputFile.FullName + ".log", lines);
		}


		private IEnumerable<SubscriptionFileLine> GetLines()
		{
			var lines = new List<SubscriptionFileLine> ();

			var encodingHelper = new EncodingHelper (SubscriptionFileLine.GetEncoding ());

			using (var etl = new MatchSortEtl (MatchSortEtl.MatchSortCsvPath))
			{
				AiderEnumerator.Execute (this.coreData, (b, subscriptions) =>
				{
					lines.AddRange (this.GetLines (subscriptions, etl, encodingHelper));
				});
			}

			return lines;
		}


		private IEnumerable<SubscriptionFileLine> GetLines
		(
			IEnumerable<AiderSubscriptionEntity> subscriptions,
			MatchSortEtl etl,
			EncodingHelper encodingHelper
		)
		{
			foreach (var subscription in subscriptions)
			{
				SubscriptionFileLine line;

				try
				{
					line = this.GetLine (subscription, etl, encodingHelper);
				}
				catch (Exception e)
				{
					line = null;

					this.errors.Add (Tuple.Create (subscription.Id, e.Message));
				}

				if (this.skipLinesWithPostmanNumberError)
				{
					if (line != null && !line.PostmanNumber.HasValue)
					{
						line = null;
					}
				}

				if (line != null)
				{
					int id = line.EditionId[1];

					if (id >= '0' && id <= '9')
					{
						id = id - '0';
					}
					else if (id >= 'A' && id <= 'F')
					{
						id = id - 'A' + 10;
					}
					else
					{
						throw new System.ArgumentException ();
					}

					int  count = line.CopiesCount;
					bool swiss = line.Country == "Suisse";

					this.editionStats[id] += count;
					this.countries.Add (line.Country);
					
					this.totalCount   += count;
					this.totalVaud    += (line.Canton == "VD") ? count : 0;
					this.totalSwiss   += swiss ? count : 0;
					this.totalForeign += swiss ? 0 : count;

					yield return line;
				}
			}
		}


		private SubscriptionFileLine GetLine
		(
			AiderSubscriptionEntity subscription,
			MatchSortEtl etl,
			EncodingHelper encodingHelper
		)
		{
			switch (subscription.SubscriptionType)
			{
				case SubscriptionType.Household:
					return this.GetLine
					(
						subscription, etl, encodingHelper, this.GetHouseholdReceiverData
					);

				case SubscriptionType.LegalPerson:
					return this.GetLine
					(
						subscription, etl, encodingHelper, this.GetLegalPersonReceiverData
					);

				default:
					throw new NotImplementedException ();
			}
		}


		private SubscriptionFileLine GetLine
		(
			AiderSubscriptionEntity subscription,
			MatchSortEtl etl,
			EncodingHelper encodingHelper,
			ReceiverDataGetter receiverDataGetter
		)
		{
			var address = subscription.GetAddress ();
			var town = address.Town;
			var country = town.Country;
			var canton = town.SwissCantonCode;

			var subscriptionNumber = subscription.Id;
			var copiesCount = subscription.Count;
			var editionId = subscription.GetEditionId ();
			var postmanNumber = this.GetPostmanNumber (subscriptionNumber, address, etl);

			string title;
			string lastname;
			string firstname;

			receiverDataGetter
			(
				subscription, encodingHelper, out title, out lastname, out firstname
			);

			string addressComplement;
			string street;
			string houseNumber;

			SubscriptionFileWriter.GetAddressData
			(
				address, encodingHelper, out addressComplement, out street, out houseNumber
			);

			var zipCode = SubscriptionFileWriter.Process
			(
				SubscriptionFileWriter.GetZipCode (town),
				SubscriptionFileLine.ZipCodeLength,
				encodingHelper
			);

			var townName = SubscriptionFileWriter.Process
			(
				town.Name,
				SubscriptionFileLine.TownLength,
				encodingHelper
			);

			var countryName = SubscriptionFileWriter.Process
			(
				country.Name,
				SubscriptionFileLine.CountryLength,
				encodingHelper
			);

			var isSwitzerland = country.IsSwitzerland ();

			return new SubscriptionFileLine
			(
				subscriptionNumber, copiesCount, editionId, title, lastname, firstname,
				addressComplement, street, houseNumber, postmanNumber, zipCode, townName,
				countryName, DistributionMode.Surface, isSwitzerland, canton
			);
		}


		private void GetHouseholdReceiverData
		(
			AiderSubscriptionEntity subscription,
			EncodingHelper encodingHelper,
			out string title,
			out string lastname,
			out string firstname
		)
		{
			var household = subscription.Household;

			var firstnames = household.GetFirstnames ();
			var lastnames = household.GetLastnames ();

			title = SubscriptionFileWriter.Process
			(
				household.GetHonorific (false),
				SubscriptionFileLine.TitleLength,
				encodingHelper
			);

			if (firstnames.Count == 1)
			{
				SubscriptionFileWriter.GetHouseholdName
				(
					firstnames[0],
					lastnames[0],
					encodingHelper,
					out firstname,
					out lastname
				);
			}
			else if (firstnames.Count == 2 && lastnames.Count == 1)
			{
				SubscriptionFileWriter.GetHouseholdName
				(
					firstnames,
					lastnames[0],
					encodingHelper,
					out firstname,
					out lastname
				);
			}
			else if (firstnames.Count == 2 && lastnames.Count == 2)
			{
				string titleOverride;

				SubscriptionFileWriter.GetHouseholdName
				(
					firstnames,
					lastnames,
					encodingHelper,
					out titleOverride,
					out firstname,
					out lastname
				);

				title = titleOverride ?? title;
			}
			else
			{
				throw new NotSupportedException ();
			}
		}


		private static void GetHouseholdName
		(
			string rawFirstname,
			string rawLastname,
			EncodingHelper encodingHelper,
			out string firstname,
			out string lastname
		)
		{
			// This case is simple, we have a single person, so firstname and lastname are its
			// firstname and lastname.

			SubscriptionFileWriter.GetFirstAndLastname
			(
				() => rawFirstname,
				() => NameProcessor.GetAbbreviatedFirstname (rawFirstname),
				() => rawLastname,
				() => NameProcessor.GetShortenedLastname (rawLastname),
				encodingHelper,
				SubscriptionFileLine.FirstnameLength,
				SubscriptionFileLine.LastnameLength,
				SubscriptionFileLine.NameLengthMax,
				false,
				false,
				out firstname,
				out lastname
			);
		}


		private static void GetHouseholdName
		(
			List<string> rawFirstnames,
			string rawLastname,
			EncodingHelper encodingHelper,
			out string firstname,
			out string lastname
		)
		{
			// Here we have two persons with a common lastname. So we want to put both firstnames
			// in firstname and the lastname in lastname. Both firstnames are supposed to be joined
			// by a "et".

			SubscriptionFileWriter.GetFirstAndLastname
			(
				() => string.Join (" et ", rawFirstnames),
				() => string.Join
					(
						" et ",
						rawFirstnames.Select (n => NameProcessor.GetAbbreviatedFirstname (n))
					),
				() => rawLastname,
				() => NameProcessor.GetShortenedLastname (rawLastname),
				encodingHelper,
				SubscriptionFileLine.FirstnameLength,
				SubscriptionFileLine.LastnameLength,
				SubscriptionFileLine.NameLengthMax,
				false,
				false,
				out firstname,
				out lastname
			);
		}


		private static void GetHouseholdName
		(
			List<string> rawFirstnames,
			List<string> rawLastnames,
			EncodingHelper encodingHelper,
			out string titleOverride,
			out string firstname,
			out string lastname
		)
		{
			// Here we have to persons with different lastnames. So we want to put the firstname and
			// the lastname of the first person in lastname and the firstname and the lastname of
			// the second person in the firstname. Both fields are supposed to be joined by a "et",
			// either at the end of the lastname or at the start of the firstname.
			// We use this order because the order of the two fields on the printed address will be
			// lastname and then firstname.

			titleOverride = null;

			for (int step = 0; step < 3; step++)
			{
				bool forceShortFirstname = step > 0;
				bool forceShortLastname = step > 1;

				var firstnames = new string[2];
				var lastnames = new string[2];

				for (int i = 0; i < 2; i++)
				{
					string fn;
					string ln;

					// We substract 5, so we are sure that we leave a place for the firstname and
					// the lastname, because both of the must be smaller that the maximum length.
					var maxNamePartLength = SubscriptionFileLine.FirstnameLength - 5;

					// We substract 1, because later on we will join both names by a space.
					var maxFullNameLength = SubscriptionFileLine.FirstnameLength - 1;

					SubscriptionFileWriter.GetFirstAndLastname
					(
						() => rawFirstnames[i],
						() => NameProcessor.GetAbbreviatedFirstname (rawFirstnames[i]),
						() => rawLastnames[i],
						() => NameProcessor.GetShortenedLastname (rawLastnames[i]),
						encodingHelper,
						maxNamePartLength,
						maxNamePartLength,
						maxFullNameLength,
						forceShortFirstname,
						forceShortLastname,
						out fn,
						out ln
					);

					firstnames[i] = fn;
					lastnames[i] = ln;
				}

				lastname = firstnames[0] + " " + lastnames[0];
				firstname = firstnames[1] + " " + lastnames[1];

				if (lastname.Length < SubscriptionFileLine.LastnameLength - 3)
				{
					lastname += " et";
				}
				else if (firstname.Length > SubscriptionFileLine.FirstnameLength - 3)
				{
					firstname = "et " + firstname;
				}
				else
				{
					// If we can't place the separator, try with shorter names.
					continue;
				}

				var nameLength = SubscriptionFileLine.GetNameLength (firstname, lastname);
				if (nameLength > SubscriptionFileLine.NameLengthMax)
				{
					// If the names are too long, try with shorter names.
					continue;
				}

				// The names where short enough to fit on the fields and to satisfy the constraints,
				// so we return thew now.
				return;
			}

			// At this point, we tried to shorten the names, but we could not do it enough. So we
			// fallback by overriding the title and using only a single member of the familiy.

			titleOverride = HouseholdMrMrs.Famille.GetLongText ();

			SubscriptionFileWriter.GetHouseholdName
			(
				rawFirstnames[0],
				rawLastnames[0],
				encodingHelper,
				out firstname,
				out lastname
			);
		}


		private static void GetFirstAndLastname
		(
			Func<string> firstnameGetter,
			Func<string> shortFirstnameGetter,
			Func<string> lastnameGetter,
			Func<string> shortLastnameGetter,
			EncodingHelper encodingHelper,
			int maxFirstnameLength,
			int maxLastnameLength,
			int maxFullnameLength,
			bool forceShortFirstname,
			bool forceShortLastname,
			out string firstname,
			out string lastname
		)
		{
			bool truncatedLastname;
			bool shortenedLastname = false;

			lastname = SubscriptionFileWriter.Process
			(
				lastnameGetter (),
				maxLastnameLength,
				encodingHelper,
				out truncatedLastname
			);

			// If the lastname has been truncated, we shorten it.
			if (truncatedLastname || forceShortLastname)
			{
				lastname = SubscriptionFileWriter.Process
				(
					shortLastnameGetter (),
					maxLastnameLength,
					encodingHelper
				);

				shortenedLastname = true;
			}

			bool truncatedFirstname;
			bool shortenedFirstname = false;

			firstname = SubscriptionFileWriter.Process
			(
				firstnameGetter (),
				maxFirstnameLength,
				encodingHelper,
				out truncatedFirstname
			);

			// If the first name has been truncated, we shorten it.
			if (truncatedFirstname || forceShortFirstname)
			{
				firstname = SubscriptionFileWriter.Process
				(
					shortFirstnameGetter (),
					maxFirstnameLength,
					encodingHelper
				);

				shortenedFirstname = true;
			}

			// If the lastname and the firstname are short enough, we return.
			var nameLength = SubscriptionFileLine.GetNameLength (lastname, firstname);
			if (nameLength <= maxFullnameLength)
			{
				if (shortenedFirstname || shortenedLastname)
				{
					var message = "Shortened name (Level 1): "
						+ firstnameGetter () + " " + lastnameGetter ()
						+ " => "
						+ firstname + " " + lastname;

					Debug.WriteLine (message);
				}

				return;
			}

			// The lastname and the first name are too long when put together. We try to shorten
			// them.

			// Shorten the firstname if we haven't done it already.
			if (!shortenedFirstname)
			{
				firstname = SubscriptionFileWriter.Process
				(
					shortFirstnameGetter (),
					maxFirstnameLength,
					encodingHelper
				);

				// If the lastname and the firstname are short enough, we return.
				nameLength = SubscriptionFileLine.GetNameLength (lastname, firstname);
				if (nameLength <= maxFullnameLength)
				{
					var message = "Shortened name (Level 2): "
						+ firstnameGetter () + " " + lastnameGetter ()
						+ " => "
						+ firstname + " " + lastname;

					Debug.WriteLine (message);

					return;
				}
			}

			// Shorten the lastname if we haven't done it already.
			if (!shortenedLastname)
			{
				lastname = SubscriptionFileWriter.Process
				(
					shortLastnameGetter (),
					maxLastnameLength,
					encodingHelper,
					out shortenedLastname
				);

				// If the lastname and the firstname are short enough, we return.
				nameLength = SubscriptionFileLine.GetNameLength (lastname, firstname);
				if (nameLength <= maxFullnameLength)
				{
					var message = "Shortened name (Level 3): "
						+ firstnameGetter () + " " + lastnameGetter ()
						+ " => "
						+ firstname + " " + lastname;

					Debug.WriteLine (message);

					return;
				}
			}

			// We could not shorten the first name and last name enough. We shall let our fury fall
			// upon them like Conan the Barbarian! Which means, we truncate them at their maximum
			// length.
			// The truncation algorithm assumes that the max length of the first name and that the
			// max length of the last name are both smaller than the max length of the full name.

			var maxLength = maxFullnameLength - lastname.Length - 1;
			firstname = firstname.Truncate (maxLength);

			if (shortenedFirstname || shortenedLastname)
			{
				var message = "Shortened name (Level 4): "
					+ firstnameGetter () + " " + lastnameGetter ()
					+ " => "
					+ firstname + " " + lastname;

				Debug.WriteLine (message);
			}
		}


		private static void GetAddressData
		(
			AiderAddressEntity address,
			EncodingHelper encodingHelper,
			out string addressComplement,
			out string street,
			out string houseNumber
		)
		{
			if (string.IsNullOrEmpty (address.PostBox))
			{
				SubscriptionFileWriter.GetAddressDataRegular
				(
					address,
					encodingHelper,
					out addressComplement,
					out street,
					out houseNumber
				);
			}
			else
			{
				SubscriptionFileWriter.GetAddressDataPostBox
				(
					address,
					encodingHelper,
					out addressComplement,
					out street,
					out houseNumber
				);
			}
		}


		private static void GetAddressDataRegular
		(
			AiderAddressEntity address,
			EncodingHelper encodingHelper,
			out string addressComplement,
			out string street,
			out string houseNumber)
		{
			addressComplement = SubscriptionFileWriter.Process
			(
				address.AddressLine1,
				SubscriptionFileLine.AddressComplementLength,
				encodingHelper
			);

			street = SubscriptionFileWriter.Process
			(
				address.StreetUserFriendly,
				SubscriptionFileLine.StreetLength,
				encodingHelper
			);

			houseNumber = SubscriptionFileWriter.Process
			(
				address.HouseNumberAndComplement.Replace (" ", ""),
				SubscriptionFileLine.HouseNumberLength,
				encodingHelper
			);

			if (!SubscriptionFileLine.SwissHouseNumberRegex.IsMatch (houseNumber))
			{
				var message = "Invalid house number: " + houseNumber;

				Debug.WriteLine (message);
				throw new NotSupportedException (message);
			}
		}


		private static void GetAddressDataPostBox
		(
			AiderAddressEntity address,
			EncodingHelper encodingHelper,
			out string addressComplement,
			out string street,
			out string houseNumber
		)
		{
			// If we have a post box, we put it in place of the street, we put the street (with
			// house number and complement) in place of the complement and we drop the complement.

			addressComplement = SubscriptionFileWriter.Process
			(
				address.StreetHouseNumberAndComplement,
				SubscriptionFileLine.AddressComplementLength,
				encodingHelper
			);

			street = SubscriptionFileWriter.Process
			(
				address.PostBox,
				SubscriptionFileLine.StreetLength,
				encodingHelper
			);

			houseNumber = "";
		}


		private void GetLegalPersonReceiverData
		(
			AiderSubscriptionEntity subscription,
			EncodingHelper encodingHelper,
			out string title,
			out string lastname,
			out string firstname
		)
		{
			var contact = subscription.LegalPersonContact;
			var legalPerson = contact.LegalPerson;

			var corporateName = legalPerson.Name;
			var personTitle = contact.PersonMrMrs;
			var personName = contact.PersonFullName;

			if (string.IsNullOrEmpty (personName))
			{
				this.GetLegalPersonReceiverData
				(
					encodingHelper, corporateName, out title, out lastname, out firstname
				);
			}
			else
			{
				this.GetLegalPersonReceiverData
				(
					encodingHelper, corporateName, personTitle, personName, out title, out lastname,
					out firstname
				);
			}
		}


		private void GetLegalPersonReceiverData
		(
			EncodingHelper encodingHelper,
			string corporateName,
			out string title,
			out string lastname,
			out string firstname
		)
		{
			// Here we simply try to fit the corporate name in the lastname and firstname fields.

			title = "";

			bool truncated = SubscriptionFileWriter.FitTextInFirstnameAndLastname
			(
				encodingHelper, corporateName, out lastname, out firstname
			);

			if (truncated)
			{
				var message = "Shortened corporate name: "
					+ corporateName
					+ " => "
					+ lastname + " " + firstname;

				Debug.WriteLine (message);
			}
		}


		private void GetLegalPersonReceiverData
		(
			EncodingHelper encodingHelper,
			string corporateName,
			PersonMrMrs? personTitle,
			string personName,
			out string title,
			out string lastname,
			out string firstname
		)
		{
			title = "";
			lastname = "";
			firstname = "";

			// We try to fit the name of the contact person and the corporate name each on a line,
			// the shorter one on the shortest line (title) and the longest one on the longest line
			// (lastname + firstname).

			// The for loop is there, so that first we try with long texts and we shorten them only
			// if necessary. We have three steps
			// - Use the full title + the person name for the contact name
			// - Use the abbreviated title + the person name for the contact name
			// - Use only the person name for the contact name
			// Only the contact name can be reduced. We do not try to reduce the corporate name.

			// We exit the loop as soon as both texts fit on their line or if there is no way to
			// better fit the texts.

			for (int step = 0; step < 3; step++)
			{
				// If the person doesn't have a title, we skip directly to the last step.
				if (!personTitle.HasValue || personTitle.Value == PersonMrMrs.None)
				{
					step = 2;
				}

				bool isLastStep = step == 2;

				// Build the contact name based on the current step.
				bool useTitle = step < 2;
				bool useAbbreviatedTitle = step > 0;

				var contactName =  useTitle
					? personTitle.Value.GetText (useAbbreviatedTitle) + " " + personName
					: personName;

				// We decide which text goes on line 1 and which goes on line 2 based on their
				// length. If one of the line can be shortened in a further step, we set the
				// corresponding bool to true. Later on we migth use these bools to decide whether
				// we should attempt a try with shorter texts or not.
				bool contactNameOnLine1 = contactName.Length <= corporateName.Length;

				string line1;
				string line2;
				bool line1CanBeShortened;
				bool line2CanBeShortened;

				if (contactNameOnLine1)
				{
					line1 = contactName;
					line1CanBeShortened = !isLastStep;

					line2 = corporateName;
					line2CanBeShortened = false;
				}
				else
				{
					line1 = corporateName;
					line1CanBeShortened = false;

					line2 = contactName;
					line2CanBeShortened = !isLastStep;
				}

				// Determine if the lines will be swaped on a further step. They will be only if
				// there is a further step and currently the contact name is on the longest line and
				// will be on the shortest one later on. We migth use this bool to decide whether
				// we should attempt a try with shorter texts or not later on.
				var linesWillBeSwapedOnFurtherStep = !isLastStep
					&& !contactNameOnLine1
					&& personName.Length <= corporateName.Length;

				// We try to fit the texts on the lines.
				bool line1Truncated;

				title = SubscriptionFileWriter.Process
				(
					line1,
					SubscriptionFileLine.TitleLength,
					encodingHelper,
					out line1Truncated
				);

				bool line2Truncated = SubscriptionFileWriter.FitTextInFirstnameAndLastname
				(
					encodingHelper, line2, out lastname, out firstname
				);

				// If both text fit, we are happy and we return.
				if (!line1Truncated && !line2Truncated)
				{
					return;
				}

				// If the text on line 1 did not fit and can be shortened, we try again with a
				// shorter text.
				if (line1Truncated && line1CanBeShortened)
				{
					continue;
				}

				// If the text on line 2 did not fit and can be shortened, we try again with a
				// shorter text.
				if (line2Truncated && line2CanBeShortened)
				{
					continue;
				}

				// If the lines will be swaped on a further step, we try again as this will probably
				// result in less truncation, if the first line has been truncated.
				if (line1Truncated && linesWillBeSwapedOnFurtherStep)
				{
					continue;
				}

				// At this point, at least one text did not fit and cannot be shortened anymore,
				// therefore we exit and leave the text truncated.
				break;
			}

			// Here we might want to add a special treatment, like maybe inverse the text on the
			// line 1 and the text on the line 2, based on what combination would result in the
			// minimum number of truncated characters. For now we don't do it, as I don't think that
			// such a method would give better results.

			// Display a message since we truncated a text.
			var message = "Shortened contact or corporate name: "
				+ personName + ", " + corporateName
				+ " => "
				+ title + ", " + lastname + " " + firstname;

			Debug.WriteLine (message);
		}


		private static bool FitTextInFirstnameAndLastname
		(
			EncodingHelper encodingHelper,
			string text,
			out string lastname,
			out string firstname
		)
		{
			firstname = "";
			lastname = "";

			// We try to fit the text on the lastname field.

			bool truncated;

			lastname = SubscriptionFileWriter.Process
			(
				text,
				SubscriptionFileLine.LastnameLength,
				encodingHelper,
				out truncated
			);

			// The text did fit on the lastname field, so we return.
			if (!truncated)
			{
				return false;
			}

			// The text has been truncated. We try to make if fit on the firstname and the lastname
			// field. For that, we split the text at the last space possible before the maximum
			// length authorized for the lastname field.

			var maxSplitIndex = SubscriptionFileLine.LastnameLength - 1;
			var splitIndex = text.LastIndexOf (' ', maxSplitIndex);

			int part1Count;
			int part2Start;

			if (splitIndex >= 0)
			{
				// Were we want to skip the space between the two parts at it will be added back
				// when printed on the publication.
				part1Count = splitIndex;
				part2Start = splitIndex + 1;
			}
			else
			{
				// Here we don't want to skip anything, as we don't split on a space but on a
				// regular character.
				part1Count = SubscriptionFileLine.LastnameLength;
				part2Start = SubscriptionFileLine.LastnameLength;
			}

			truncated = SubscriptionFileWriter.FitTextInFirstnameAndLastname
			(
				encodingHelper, text, part1Count, part2Start, out lastname, out firstname
			);

			// The text did fit on the lastname and the firstname fields, so we return.
			if (!truncated)
			{
				return false;
			}

			// Here we are in a corner case where we could not use all the space at our disposal. It
			// might be because there is a space early in the text and a very long part after. For
			// instance "Paroisse Payerne-Corcelles-Ressudens (PACORE)". In such a case, we would
			// have put "Paroisse" in the lastname and the firstname field is not long enough to
			// accomodate the remainder of the text. If we are in such a case, we treat it specially
			// by splitting the text without taking care of the space.
			var nameLength = SubscriptionFileLine.GetNameLength (firstname, lastname);
			if (nameLength < SubscriptionFileLine.NameLengthMax)
			{
				part1Count = SubscriptionFileLine.LastnameLength;
				part2Start = SubscriptionFileLine.LastnameLength;

				truncated = SubscriptionFileWriter.FitTextInFirstnameAndLastname
				(
					encodingHelper, text, part1Count, part2Start, out lastname,
					out firstname
				);

				if (!truncated)
				{
					return false;
				}
			}

			// The text has been truncated.
			return true;
		}


		private static bool FitTextInFirstnameAndLastname
		(
			EncodingHelper encodingHelper,
			string corporateName,
			int part1Count,
			int part2Start,
			out string lastname,
			out string firstname
		)
		{
			var part1 = corporateName.Substring (0, part1Count);
			var part2 = corporateName.Substring (part2Start);

			lastname = SubscriptionFileWriter.Process
			(
				part1,
				SubscriptionFileLine.LastnameLength,
				encodingHelper
			);

			var maxFirstnameLength = Math.Min
			(
				SubscriptionFileLine.FirstnameLength,
				SubscriptionFileLine.NameLengthMax - lastname.Length - 1
			);

			bool truncated;

			firstname = SubscriptionFileWriter.Process
			(
				part2,
				maxFirstnameLength,
				encodingHelper,
				out truncated
			);

			return truncated;
		}


		private static string Process(string value, int maxLength, EncodingHelper encodingHelper)
		{
			bool truncated;

			var result = SubscriptionFileWriter.Process
			(
				value,
				maxLength,
				encodingHelper,
				out truncated
			);

			if (truncated)
			{
				Debug.WriteLine ("Value has been truncated: " + value + " => " + result);
			}

			return result;
		}


		private static string Process
		(
			string value,
			int maxLength,
			EncodingHelper encodingHelper,
			out bool truncated
		)
		{
			truncated = false;

			if (value == null)
			{
				return "";
			}

			var converted = encodingHelper.ConvertToEncoding (value);

			if (converted.Length > maxLength)
			{
				truncated = true;
				converted = converted.Truncate (maxLength);
			}

			return converted;
		}


		private static string GetZipCode(AiderTownEntity town)
		{
			return town.Country.IsSwitzerland ()
				? Epsitec.Data.Platform.SwissPostFullZip.GetZipCode (town.SwissZipCode, town.SwissZipCodeAddOn)
				: town.ZipCode;
		}


		private int? GetPostmanNumber
		(
			string subscriptionId,
			AiderAddressEntity address,
			MatchSortEtl etl
		)
		{
			// The specs requires 0 for an addresses outside Switzerland.
			if (!address.Town.Country.IsSwitzerland ())
			{
				return SubscriptionFileLine.ForeignPostmanNumber;
			}

			// The specs requires 999 for adresses with a post box.
			if (!String.IsNullOrEmpty (address.PostBox))
			{
				return SubscriptionFileLine.SwissPostmanNumberPostbox;
			}

			var zipCode  = address.Town.SwissZipCode.ToString ();
			var zipAddOn = Epsitec.Data.Platform.SwissPostFullZip.GetZipCodeAddOn (address.Town.SwissZipCodeAddOn);
			var street   = address.StreetUserFriendly;
			var number   = address.HouseNumber.HasValue
				? InvariantConverter.ToString (address.HouseNumber.Value)
				: null;
			var complement = address.HouseNumberComplement;

			var postmanNumber = etl.GetMessenger
			(
				zipCode ?? "",
				zipAddOn ?? "",
				street ?? "",
				number ?? "",
				complement ?? ""
			);

			if (postmanNumber.HasValue)
			{
				return postmanNumber.Value;
			}

			// We have not found the postman number. We try to find one by using an empty house
			// number complement if the address has a house number complement. This is probably a
			// wrong address.

			if (!string.IsNullOrEmpty (complement))
			{
				postmanNumber = etl.GetMessenger
				(
					zipCode ?? "",
					zipAddOn ?? "",
					street ?? "",
					number ?? "",
					""
				);

				if (postmanNumber.HasValue)
				{
					return postmanNumber.Value;
				}
			}

			// We still have found no postman number. In this case, the spec requires an empty
			// postman number.

			var message = SubscriptionFileWriter.ErrorMessage
				+ zipCode + ", " + zipAddOn + ", "
				+ street + ", "
				+ number + ", " + complement;

			Debug.WriteLine (message);
			System.Console.WriteLine (message);

			this.postmanErrors.Add (Tuple.Create (subscriptionId, message));

			return null;
		}


		private readonly CoreData coreData;
		private readonly FileInfo outputFile;
		private readonly FileInfo errorFile;
		private readonly bool skipLinesWithPostmanNumberError;
		private readonly List<Tuple<string, string>> errors;
		private readonly List<Tuple<string, string>> postmanErrors;

		private readonly int[] editionStats;
		private readonly System.DateTime startTime;
		private readonly HashSet<string> countries;
		private int totalCount;
		private int totalVaud;
		private int totalSwiss;
		private int totalForeign;

		internal const string ErrorMessage = "Postman number not found for address: ";


		// We need a delegate to define this, as the System.Action type cannot use arguments with
		// the out keyword. We need an old school delegate for this kind of stuff.
		private delegate void ReceiverDataGetter
		(
			AiderSubscriptionEntity subscription,
			EncodingHelper encodingHelper,
			out string title,
			out string lastname,
			out string firstname
		);


	}


}
