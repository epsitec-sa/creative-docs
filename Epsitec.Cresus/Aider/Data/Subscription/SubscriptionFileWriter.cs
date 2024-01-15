//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Text;
using Epsitec.Common.Types;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Data.Platform.MatchSort;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;


namespace Epsitec.Aider.Data.Subscription
{
	internal sealed class SubscriptionFileWriter
	{
		public SubscriptionFileWriter(CoreData coreData, FileInfo outputFile, FileInfo errorFile, bool skipLinesWithDistrictNumberError)
		{
			this.coreData                         = coreData;
			this.outputFile                       = outputFile;
			this.errorFile                        = errorFile;
			this.skipLinesWithDistrictNumberError = skipLinesWithDistrictNumberError;
			this.startTime                        = System.DateTime.UtcNow;

			this.errors               = new List<System.Tuple<string, string>> ();
			this.districtNumberErrors = new List<System.Tuple<string, string>> ();
			this.editionStats         = new int[16];
			this.countries            = new HashSet<string> ();

			using (var bc = new BusinessContext (this.coreData, enableReload: false))
			{
				var parishExample  = new AiderGroupEntity
				{
					GroupDef = SubscriptionFileWriter.GetParishGroupDef (bc)
				};

				this.parishes = bc.GetByExample (parishExample)
					.ToDictionary (x => x.Path, x => x.Name);

				this.parishes["NOPA."] = "Sans paroisse";
			}
		}


		private static AiderGroupDefEntity GetParishGroupDef(BusinessContext context)
		{
			var ex = new AiderGroupDefEntity
						{
							Level = 1,
							PathTemplate = AiderGroupIds.ParishTemplatePath,
						};

			return context.GetByExample (ex).Single ();
		}
		public void Write()
		{
			this.errors.Clear ();

			var lines    = this.GetLines ();
			var multiple = new List<SubscriptionFileLine> ();

			var filteredLines = SubscriptionFileWriter.Filter (lines, multiple);

			this.ComputeStats (filteredLines);

			SubscriptionFileLine.Write (filteredLines, this.outputFile);

			this.LogErrors (multiple);
			this.LogStats ();
		}


		private void LogErrors(List<SubscriptionFileLine> multiple)
		{
			if ((this.errors.Count > 0 || this.districtNumberErrors.Count > 0 || multiple.Count > 0) && this.errorFile != null)
			{
				using (var stream = this.errorFile.Open (FileMode.Create, FileAccess.Write))
				using (var streamWriter = new StreamWriter (stream))
				{
					foreach (var error in this.errors.Concat (this.districtNumberErrors))
					{
						streamWriter.WriteLine (error.Item1 + " => " + error.Item2);
					}
					foreach (var line in multiple)
					{
						streamWriter.WriteLine (line.SubscriptionNumber + " => multiple " + line.Firstname + " " + line.Lastname + ", " + line.Street + " " + line.HouseNumber + ", " + line.ZipCode + " " + line.Town);
					}
				}
			}
		}

		private void LogStats()
		{
			var lines = new List<string> ();
			var time  = (int) ((System.DateTime.UtcNow - this.startTime).TotalMinutes);

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
			var lines          = new List<SubscriptionFileLine> ();
			var encodingHelper = new EncodingHelper (SubscriptionFileLine.DefaultEncoding);

			using (var etl = new MatchSortEtl (MatchSortEtl.MatchSortCsvPath))
			{
				AiderEnumerator.Execute (this.coreData,
					(b, subscriptions) =>
					{
						lines.AddRange (this.GetLines (subscriptions, etl, encodingHelper));
					});
			}

			return lines;
		}

		public static IEnumerable<SubscriptionFileLine> Filter(IEnumerable<SubscriptionFileLine> lines, List<SubscriptionFileLine> multiple)
		{
			var sorted = (from line in lines
						  orderby line.SubscriptionNumber
						  select line).ToList ();

			var distinct = new HashSet<SubscriptionFileLine> (new LineComparer ());

			foreach (var line in sorted)
			{
				if (distinct.Add (line))
				{
					yield return line;
				}
				else
				{
					multiple.Add (line);
				}
			}

			System.Diagnostics.Debug.WriteLine ("Sorted: {0} items, Distinct: {1} items", sorted.Count, distinct.Count);
		}

		private class LineComparer : IEqualityComparer<SubscriptionFileLine>
		{
			#region IEqualityComparer<SubscriptionFileLine> Members

			public bool Equals(SubscriptionFileLine x, SubscriptionFileLine y)
			{
				if ((x.ZipCode == y.ZipCode) &&
					(x.Town == y.Town) &&
					(x.Street == y.Street) &&
					(x.Lastname == y.Lastname))
				{
					//	Multiple subscriptions for different editions at same address is OK
					if (x.EditionId != y.EditionId)
					{
						return false;
					}

					if ((x.HouseNumber == y.HouseNumber) ||
						((x.HouseNumber.Length >= 1) && (y.HouseNumber.Length == 0)) ||
						((x.HouseNumber.Length == 0) && (y.HouseNumber.Length >= 1)))
					{
						return x.Firstname == y.Firstname;
					}
				}

				return false;
			}

			public int GetHashCode(SubscriptionFileLine obj)
			{
				return obj.ZipCode.GetHashCode () ^ obj.Town.GetHashCode () ^ obj.Street.GetHashCode () ^ obj.Lastname.GetHashCode ();
			}

			#endregion
		}


		private IEnumerable<SubscriptionFileLine> GetLines(IEnumerable<AiderSubscriptionEntity> subscriptions, MatchSortEtl etl, EncodingHelper encodingHelper)
		{
			foreach (var subscription in subscriptions)
			{
				if ((subscription.SubscriptionFlag == SubscriptionFlag.VerificationRequired) ||
					(subscription.SubscriptionFlag == SubscriptionFlag.Suspended) ||
					(subscription.Count < 1))
				{
					continue;
				}

				SubscriptionFileLine line;

				try
				{
					line = this.GetLine (subscription, etl, encodingHelper);
				}
				catch (System.Exception e)
				{
					this.RecordError (subscription, e.Message);
					continue;
				}

				if (line == null)
				{
					continue;
				}

				if (line.DistrictNumber == null)
				{
					if ((this.skipLinesWithDistrictNumberError) ||
						(string.IsNullOrEmpty (line.Street) && string.IsNullOrEmpty (line.AddressComplement)) ||
						(string.IsNullOrEmpty (line.Town)) ||
						(string.IsNullOrEmpty (line.Firstname)) ||
						(string.IsNullOrEmpty (line.Lastname)))
					{
						this.RecordError (subscription, "Address not found in MAT[CH]sort");
						continue;
					}
				}

				yield return line;
			}
		}

		private void RecordError(AiderSubscriptionEntity subscription, string error)
		{
			var parishPath   = subscription.ParishGroupPathCache;
			var regionNumber = AiderGroupIds.GetRegionNumber (parishPath);
			var parishName   = this.GetParishSafely (parishPath);
			var message      = string.Format ("{0}\t{1}\t{2}", error, regionNumber.GetValueOrDefault (0), parishName);

			this.errors.Add (System.Tuple.Create (subscription.Id, message));
		}

		private string GetParishSafely(string parishPath)
		{
			if (parishPath == null)
			{
				return "<null>";
			}

			string parishName;
			
			if (this.parishes.TryGetValue (parishPath, out parishName))
			{
				return parishName;
			}

			return "<path=" + parishPath + ">";
		}

		private void ComputeStats(IEnumerable<SubscriptionFileLine> lines)
		{
			foreach (var line in lines)
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
			}
		}


		private SubscriptionFileLine GetLine(AiderSubscriptionEntity subscription, MatchSortEtl etl, EncodingHelper encodingHelper)
		{
			switch (subscription.SubscriptionType)
			{
				case SubscriptionType.Household:
					return this.GetHouseholdLine (subscription, etl, encodingHelper);

				case SubscriptionType.LegalPerson:
					return this.GetLegalPersonLine (subscription, etl, encodingHelper);

				default:
					throw new System.NotImplementedException ();
			}
		}

		private SubscriptionFileLine GetHouseholdLine(AiderSubscriptionEntity subscription, MatchSortEtl etl, EncodingHelper encodingHelper)
		{
			if (subscription.Household.IsNull ())
			{
				throw new System.NotSupportedException ("No household");
			}

			if (subscription.Household.Members.Any (x => (x.Age == null) || (x.Age > 17)) == false)
			{
				throw new System.NotSupportedException ("No adult in household");
			}

			return this.GetLine (subscription, etl, encodingHelper, this.GetHouseholdReceiverData);
		}

		private SubscriptionFileLine GetLegalPersonLine(AiderSubscriptionEntity subscription, MatchSortEtl etl, EncodingHelper encodingHelper)
		{
			if (subscription.LegalPersonContact.IsNull ())
			{
				return null;
			}

			return this.GetLine	(subscription, etl, encodingHelper, this.GetLegalPersonReceiverData);
		}

		private SubscriptionFileLine GetLine(AiderSubscriptionEntity subscription, MatchSortEtl etl, EncodingHelper encodingHelper, ReceiverDataGetter receiverDataGetter)
		{
			var address = subscription.GetAddress ();
			var town    = address.Town;
			var country = town.Country;
			var canton  = town.SwissCantonCode;

			var subscriptionNumber = subscription.Id;
			
			var copiesCount    = subscription.Count;
			var editionId      = subscription.GetEditionId ();
			var districtNumber = this.GetDistrictNumber (subscriptionNumber, address, etl);

			string title;
			string lastname;
			string firstname;

			receiverDataGetter (subscription, encodingHelper, out title, out lastname, out firstname);

			string addressComplement;
			string street;
			string houseNumber;



			SubscriptionFileWriter.GetAddressData (address, encodingHelper, out addressComplement, out street, out houseNumber);

			//Avoid long address+housenumber lines
			var addressLineLength = $"{street} {houseNumber}".Length;
			if (addressLineLength > 30)
			{
				street = address.GetBestStreetName(true);
				addressLineLength = $"{street} {houseNumber}".Length;
				if(addressLineLength > 30)
                {
					var maxLength = 30 - houseNumber.Length - 1;
					street = street.SubstringEnd(maxLength);
                }
			}

			var zipCode     = SubscriptionFileWriter.Format (SubscriptionFileWriter.GetZipCode (town), SubscriptionFileLine.ZipCodeLength, encodingHelper);
			var townName    = SubscriptionFileWriter.Format (town.Name, SubscriptionFileLine.TownLength, encodingHelper);
			var countryName = SubscriptionFileWriter.Format (country.Name, SubscriptionFileLine.CountryLength, encodingHelper);

			var isSwitzerland = country.IsSwitzerland ();

			return new SubscriptionFileLine (subscriptionNumber, copiesCount, editionId, title, lastname, firstname,
											 addressComplement, street, houseNumber, districtNumber, zipCode, townName,
											 countryName, DistributionMode.Surface, isSwitzerland, canton);
		}


		private void GetHouseholdReceiverData(AiderSubscriptionEntity subscription, EncodingHelper encodingHelper, out string title, out string lastname, out string firstname)
		{
			var household = subscription.Household;

			var names = household.GetHeadNames (compact: false);
			
			var firstnames = names.Item1;
			var lastnames  = names.Item2;

			title = SubscriptionFileWriter.Format (household.GetHonorificTitles (), SubscriptionFileLine.TitleLength, encodingHelper);

			if (firstnames.Count == 1)
			{
				SubscriptionFileWriter.GetHouseholdName (firstnames[0], lastnames[0], encodingHelper,
														 out firstname, out lastname);
			}
			else if (firstnames.Count == 2 && lastnames.Count == 1)
			{
				SubscriptionFileWriter.GetHouseholdName (firstnames, lastnames[0], encodingHelper,
														 out firstname, out lastname);
			}
			else if (firstnames.Count == 2 && lastnames.Count == 2)
			{
				string titleOverride;

				SubscriptionFileWriter.GetHouseholdName (firstnames, lastnames, encodingHelper,
														 out titleOverride, out firstname, out lastname);

				title = titleOverride ?? title;
			}
			else
			{
				throw new System.NotSupportedException ();
			}
		}

		private static void GetHouseholdName(string rawFirstname, string rawLastname, EncodingHelper encodingHelper, out string firstname, out string lastname)
		{
			//	This case is simple, we have a single person, so firstname and lastname are its
			//	firstname and lastname.

			SubscriptionFileWriter.GetFirstAndLastname (
				() => rawFirstname,
				() => NameProcessor.GetAbbreviatedFirstName (rawFirstname),
				() => rawLastname,
				() => NameProcessor.GetShortenedLastName (rawLastname),
				encodingHelper,
				SubscriptionFileLine.FirstnameLength,
				SubscriptionFileLine.LastnameLength,
				SubscriptionFileLine.NameLengthMax,
				forceShortFirstname: false,
				forceShortLastname: false,
				firstname: out firstname,
				lastname: out lastname);
		}

		private static void GetHouseholdName(List<string> rawFirstnames, string rawLastname, EncodingHelper encodingHelper, out string firstname, out string lastname)
		{
			//	Here we have two persons with a common lastname. So we want to put both firstnames
			//	in firstname and the lastname in lastname. Both firstnames are supposed to be joined
			//	by a "et".

			SubscriptionFileWriter.GetFirstAndLastname
			(
				() => string.Join (" et ", rawFirstnames),
				() => string.Join
					(
						" et ",
						rawFirstnames.Select (n => NameProcessor.GetAbbreviatedFirstName (n))
					),
				() => rawLastname,
				() => NameProcessor.GetShortenedLastName (rawLastname),
				encodingHelper,
				SubscriptionFileLine.FirstnameLength,
				SubscriptionFileLine.LastnameLength,
				SubscriptionFileLine.NameLengthMax,
				forceShortFirstname: false,
				forceShortLastname: false,
				firstname: out firstname,
				lastname: out lastname);
		}

		private static void GetHouseholdName(List<string> rawFirstnames, List<string> rawLastnames, EncodingHelper encodingHelper, out string titleOverride, out string firstname, out string lastname)
		{
			//	Here we have to persons with different lastnames. So we want to put the firstname and
			//	the lastname of the first person in lastname and the firstname and the lastname of
			//	the second person in the firstname. Both fields are supposed to be joined by a "et",
			//	either at the end of the lastname or at the start of the firstname.
			//	We use this order because the order of the two fields on the printed address will be
			//	lastname and then firstname.

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

					// We subtract 5, so we are sure that we leave a place for the firstname and
					// the lastname, because both of the must be smaller that the maximum length.
					var maxNamePartLength = SubscriptionFileLine.FirstnameLength - 5;

					// We subtract 1, because later on we will join both names by a space.
					var maxFullNameLength = SubscriptionFileLine.FirstnameLength - 1;

					SubscriptionFileWriter.GetFirstAndLastname
					(
						() => rawFirstnames[i],
						() => NameProcessor.GetAbbreviatedFirstName (rawFirstnames[i]),
						() => rawLastnames[i],
						() => NameProcessor.GetShortenedLastName (rawLastnames[i]),
						encodingHelper,
						maxNamePartLength,
						maxNamePartLength,
						maxFullNameLength,
						forceShortFirstname,
						forceShortLastname,
						out fn,
						out ln);

					firstnames[i] = fn;
					lastnames[i] = ln;
				}

				lastname  = firstnames[0] + " " + lastnames[0];
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

			SubscriptionFileWriter.GetHouseholdName (rawFirstnames[0], rawLastnames[0], encodingHelper, out firstname, out lastname);
		}


		private static void GetFirstAndLastname(System.Func<string> firstnameGetter, System.Func<string> shortFirstnameGetter, System.Func<string> lastnameGetter, System.Func<string> shortLastnameGetter,
												EncodingHelper encodingHelper,
												int maxFirstnameLength, int maxLastnameLength, int maxFullnameLength, bool forceShortFirstname, bool forceShortLastname,
												out string firstname, out string lastname)
		{
			bool truncatedLastname;
			bool shortenedLastname = false;

			lastname = SubscriptionFileWriter.Format (lastnameGetter (), maxLastnameLength, encodingHelper, out truncatedLastname);

			// If the lastname has been truncated, we shorten it.
			if (truncatedLastname || forceShortLastname)
			{
				lastname = SubscriptionFileWriter.Format (shortLastnameGetter (), maxLastnameLength, encodingHelper);

				shortenedLastname = true;
			}

			bool truncatedFirstname;
			bool shortenedFirstname = false;

			firstname = SubscriptionFileWriter.Format (firstnameGetter (), maxFirstnameLength, encodingHelper, out truncatedFirstname);

			// If the first name has been truncated, we shorten it.
			if (truncatedFirstname || forceShortFirstname)
			{
				firstname = SubscriptionFileWriter.Format (shortFirstnameGetter (), maxFirstnameLength, encodingHelper);

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
				firstname = SubscriptionFileWriter.Format
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
				lastname = SubscriptionFileWriter.Format
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

            //  Keep at least one character in the first name.
            if (maxLength < 1)
            {
                maxLength = 1;
            }

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


		private static void GetAddressData(AiderAddressEntity address, EncodingHelper encodingHelper,
										   out string addressComplement, out string street, out string houseNumber)
		{
			if (string.IsNullOrEmpty (address.PostBox))
			{
				SubscriptionFileWriter.GetAddressDataRegular (address, encodingHelper, out addressComplement, out street, out houseNumber);
			}
			else
			{
				SubscriptionFileWriter.GetAddressDataPostBox (address, encodingHelper, out addressComplement, out street, out houseNumber);
			}
		}

		private static void GetAddressDataRegular(AiderAddressEntity address, EncodingHelper encodingHelper,
												  out string addressComplement, out string streetAddress, out string houseNumber)
		{
			addressComplement = address.AddressLine1;
			streetAddress     = address.StreetUserFriendly;
			houseNumber       = address.GetCleanHouseNumberAndComplement ().Replace (" ", "");

			addressComplement = SubscriptionFileWriter.Format (addressComplement, SubscriptionFileLine.AddressComplementLength, encodingHelper);
			streetAddress     = SubscriptionFileWriter.Format (streetAddress, SubscriptionFileLine.StreetLength, encodingHelper);
			houseNumber       = SubscriptionFileWriter.Format (houseNumber, SubscriptionFileLine.HouseNumberLength, encodingHelper);

			if (!SubscriptionFileLine.SwissHouseNumberRegex.IsMatch (houseNumber))
			{
				var message = "Invalid house number: " + houseNumber;

				Debug.WriteLine (message);
				throw new System.NotSupportedException (message);
			}
		}

		private static void GetAddressDataPostBox(AiderAddressEntity address, EncodingHelper encodingHelper,
												  out string addressComplement, out string streetAddress, out string houseNumber)
		{
			// If we have a post box, we put it in place of the street, we put the street (with
			// house number and complement) in place of the complement and we drop the complement.

			addressComplement = SubscriptionFileWriter.Format (address.StreetHouseNumberAndComplement, SubscriptionFileLine.AddressComplementLength, encodingHelper);
			streetAddress     = SubscriptionFileWriter.Format (address.PostBox, SubscriptionFileLine.StreetLength, encodingHelper);
			houseNumber       = "";
		}

		private void GetLegalPersonReceiverData(AiderSubscriptionEntity subscription, EncodingHelper encodingHelper,
												out string title, out string lastname, out string firstname)
		{
			var contact = subscription.LegalPersonContact;
			var legalPerson = contact.LegalPerson;

			var corporateName = legalPerson.Name;
			var personTitle = contact.LegalPersonContactMrMrs;
			var personName = contact.LegalPersonContactFullName;

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


		private void GetLegalPersonReceiverData(EncodingHelper encodingHelper, string corporateName,
												out string title, out string lastname, out string firstname)
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

		private void GetLegalPersonReceiverData(EncodingHelper encodingHelper, string corporateName, PersonMrMrs? personTitle, string personName,
												out string title, out string lastname, out string firstname)
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

				title = SubscriptionFileWriter.Format
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


		private static bool FitTextInFirstnameAndLastname(EncodingHelper encodingHelper, string text,
														  out string lastname, out string firstname)
		{
			firstname = "";
			lastname = "";

			// We try to fit the text on the lastname field.

			bool truncated;

			lastname = SubscriptionFileWriter.Format
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

		private static bool FitTextInFirstnameAndLastname(EncodingHelper encodingHelper, string corporateName, int part1Count, int part2Start,
														  out string lastname, out string firstname)
		{
			var part1 = corporateName.Substring (0, part1Count);
			var part2 = corporateName.Substring (part2Start);

			lastname = SubscriptionFileWriter.Format
			(
				part1,
				SubscriptionFileLine.LastnameLength,
				encodingHelper
			);

			var maxFirstnameLength = System.Math.Min
			(
				SubscriptionFileLine.FirstnameLength,
				SubscriptionFileLine.NameLengthMax - lastname.Length - 1
			);

			bool truncated;

			firstname = SubscriptionFileWriter.Format
			(
				part2,
				maxFirstnameLength,
				encodingHelper,
				out truncated
			);

			return truncated;
		}


		private static string Format(string value, int maxLength, EncodingHelper encodingHelper)
		{
			bool truncated;

			var result = SubscriptionFileWriter.Format (value, maxLength, encodingHelper, out truncated);

			if (truncated)
			{
				Debug.WriteLine ("Value has been truncated: " + value + " => " + result);
			}

			return result;
		}

		private static string Format(string value, int maxLength, EncodingHelper encodingHelper, out bool truncated)
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
			if (town.Country.IsSwitzerland ())
			{
				return Epsitec.Data.Platform.SwissPostFullZip.GetZipCode (town.SwissZipCode, town.SwissZipCodeAddOn);
			}
			else
			{
				return town.ZipCode;
			}
		}


		private int? GetDistrictNumber(string subscriptionId, AiderAddressEntity address, MatchSortEtl etl)
		{
			// The specs requires 0 for an addresses outside Switzerland.
			if (!address.Town.Country.IsSwitzerland ())
			{
				return SubscriptionFileLine.ForeignDistrictNumber;
			}

			// The specs requires 999 for adresses with a post box.
			if (!string.IsNullOrEmpty (address.PostBox))
			{
				return SubscriptionFileLine.SwissDistrictNumberPostbox;
			}

			var zipCode  = address.Town.SwissZipCode.ToString ();
			var zipAddOn = Epsitec.Data.Platform.SwissPostFullZip.GetZipCodeAddOn (address.Town.SwissZipCodeAddOn);
			var street   = address.StreetUserFriendly;
			var number   = address.HouseNumber.HasValue
				? InvariantConverter.ToString (address.HouseNumber.Value)
				: null;
			var complement = address.HouseNumberComplement;

			var districtNumber = etl.GetDistrictNumber
			(
				zipCode ?? "",
				zipAddOn ?? "",
				street ?? "",
				number ?? "",
				complement ?? ""
			);

			if (districtNumber.HasValue)
			{
				return districtNumber.Value;
			}

			// We have not found the district number. We try to find one by using an empty house
			// number complement if the address has a house number complement. This is probably a
			// wrong address.

			if (!string.IsNullOrEmpty (complement))
			{
				districtNumber = etl.GetDistrictNumber
				(
					zipCode ?? "",
					zipAddOn ?? "",
					street ?? "",
					number ?? "",
					""
				);

				if (districtNumber.HasValue)
				{
					return districtNumber.Value;
				}
			}

			// We still have found no district number. In this case, the spec requires an empty
			// district number.

			var message = SubscriptionFileWriter.ErrorMessage
				+ zipCode + ", " + zipAddOn + ", "
				+ street + ", "
				+ number + ", " + complement;

			Debug.WriteLine (message);
			System.Console.WriteLine (message);

			this.districtNumberErrors.Add (System.Tuple.Create (subscriptionId, message));

			return null;
		}


		private readonly CoreData				coreData;
		private readonly FileInfo				outputFile;
		private readonly FileInfo				errorFile;
		private readonly bool					skipLinesWithDistrictNumberError;
		private readonly List<System.Tuple<string, string>> errors;
		private readonly List<System.Tuple<string, string>> districtNumberErrors;
		private readonly Dictionary<string, string> parishes;

		private readonly int[]					editionStats;
		private readonly System.DateTime		startTime;
		private readonly HashSet<string>		countries;
		private int								totalCount;
		private int								totalVaud;
		private int								totalSwiss;
		private int								totalForeign;

		internal const string					ErrorMessage = "District number not found for address: ";


		//	We need a delegate to define this, as the System.Action type cannot use arguments with
		//	the out keyword. We need an old school delegate for this kind of stuff.

		private delegate void ReceiverDataGetter(AiderSubscriptionEntity subscription, EncodingHelper encodingHelper, out string title, out string lastname, out string firstname);
	}
}
