//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.Common
{
	/// <summary>
	/// The <c>ParishAddressRepository</c> class implements the repository used to resolve an
	/// address to a parish name. The address should be compatible with the MAT[CH] street
	/// data base and the street should have been normalized beforehand (see method <see
	/// cref="SwissPostStreet.NormalizeStreetName"/>).
	/// </summary>
	public sealed class ParishAddressRepository
	{
		public ParishAddressRepository(IEnumerable<string> lines)
		{
			this.addresses = new Dictionary<string, ParishAddresses> ();
			this.infos     = new Dictionary<string, ParishAddressInformation> ();

			foreach (var info in ParishAddressRepository.GetParishInformations (lines))
			{
				var key  = ParishAddressRepository.GetKey (info.ZipCode, info.TownNameOfficial);
				var addr = this.AddParishAddress (key, info);

				if (info.TownNameOfficial != info.TownName)
				{
					//	If there is more than one possible spelling for the town name, add the
					//	variant into the dictionary, too, but do not insert the info twice !

					key = ParishAddressRepository.GetKey (info.ZipCode, info.TownName);
					this.addresses[key] = addr;
				}

				this.infos[info.ParishName] = info;
			}
		}


		/// <summary>
		/// The <see cref="ParishAddressRepository"/> singleton.
		/// </summary>
		public static readonly ParishAddressRepository Current = ParishAddressRepository.GetDefault ();


		/// <summary>
		/// Finds the parish addresses for a given zip code and town name.
		/// </summary>
		/// <param name="zipCode">The zip code.</param>
		/// <param name="townName">The town name.</param>
		/// <returns>The parish addresses or <c>null</c> if no information is available.</returns>
		public ParishAddresses FindAddresses(int zipCode, string townName)
		{
			return this.FindAddresses (ParishAddressRepository.GetKey (zipCode, townName));
		}

		/// <summary>
		/// Finds all address informations (without duplicates).
		/// </summary>
		/// <returns>The collection of address informations.</returns>
		public IEnumerable<ParishAddressInformation> FindAllAddressInformations()
		{
			return this.addresses.Values.SelectMany (x => x.FindAll ()).Distinct ();
		}

		/// <summary>
		/// Finds the name of the parish, based on the zip code, the town name and a normalized
		/// street address.
		/// </summary>
		/// <param name="zipCode">The zip code.</param>
		/// <param name="townName">The name of the town.</param>
		/// <param name="normalizedStreetName">The normalized street name.</param>
		/// <param name="houseNumber">The house number.</param>
		/// <returns>The name of the parish or <c>null</c>.</returns>
		public string FindParishName(int zipCode, string townName, string normalizedStreetName, int houseNumber)
		{
			var name = this.FindParishName (ParishAddressRepository.GetKey (zipCode, townName), normalizedStreetName, houseNumber);

			if (name == null)
			{
				foreach (var similar in Epsitec.Data.Platform.SwissPostZipCodeFoldingRepository.FindSimilar (zipCode))
				{
					name = this.FindParishName (ParishAddressRepository.GetKey (similar.ZipCode, townName), normalizedStreetName, houseNumber);

					if (name != null)
					{
						break;
					}
				}
			}
			
			return name;
		}

		/// <summary>
		/// Gets the details about the specified parish.
		/// </summary>
		/// <param name="parishName">Name of the parish.</param>
		/// <returns>The detailes information about the parish.</returns>
		public ParishAddressInformation GetDetails(string parishName)
		{
			ParishAddressInformation info;

			this.infos.TryGetValue (parishName, out info);

			return info;
		}

		private string FindParishName(string key, string normalizedStreetName, int houseNumber)
		{
			var info = this.FindParishAddressInformation (key, normalizedStreetName, houseNumber);

			if (info == null)
			{
				return null;
			}
			else
			{
				return info.ParishName;
			}

		}
		private ParishAddressInformation FindParishAddressInformation(string key, string normalizedStreetName, int houseNumber)
		{
			var addresses = this.FindAddresses (key);

			if (addresses == null)
			{
				return null;
			}
			
			var parish = addresses.FindSpecific (normalizedStreetName, houseNumber)
					  ?? addresses.FindDefault (normalizedStreetName)
					  ?? addresses.FindDefault ();

			return parish;
		}


		private ParishAddresses FindAddresses(string key)
		{
			ParishAddresses addresses;

			this.addresses.TryGetValue (key, out addresses);

			return addresses;
		}

		private ParishAddresses AddParishAddress(string key, ParishAddressInformation info)
		{
			ParishAddresses addresses;
			if (this.addresses.TryGetValue (key, out addresses) == false)
			{
				addresses = new ParishAddresses ();
				this.addresses[key] = addresses;
			}

			addresses.Add (info);
			return addresses;
		}

		
		private static string GetKey(int zipCode, string townName)
		{
			return string.Format ("{0:0000} {1}", zipCode, townName);
		}

		private static IEnumerable<ParishAddressInformation> GetParishInformations(IEnumerable<string> lines)
		{
			// We skip the entries within region 0 as we know that they are invalid.

			return lines
				.Select (x => new ParishAddressInformation (x))
				.Where (x => x.RegionCode != 0);
		}

		private static IEnumerable<string> GetLines()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			var resource = "Epsitec.Aider.DataFiles.ParishAddresses.zip";
			var source   = Epsitec.Common.IO.ZipFile.DecompressTextFile (assembly, resource);

			return Epsitec.Common.IO.StringLineExtractor.GetLines (source);
		}

		private static ParishAddressRepository GetDefault()
		{
			var lines = ParishAddressRepository.GetLines ();

			return new ParishAddressRepository (lines);
		}

		
		private readonly Dictionary<string, ParishAddresses>	addresses;
		private readonly Dictionary<string, ParishAddressInformation> infos;
	}
}
