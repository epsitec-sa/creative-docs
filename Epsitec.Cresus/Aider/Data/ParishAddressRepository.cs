﻿//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data
{
	/// <summary>
	/// The <c>ParishAddressRepository</c> class implements the repository used to resolve an
	/// address to a parish name. The address should be compatible with the MAT[CH] street
	/// data base and the street should have been normalized beforehand (see method <see
	/// cref="SwissPostStreet.NormalizeStreetName"/>).
	/// </summary>
	public sealed class ParishAddressRepository
	{
		private ParishAddressRepository()
		{
			this.addresses = new Dictionary<string, ParishAddresses> ();

			foreach (var info in ParishAddressRepository.GetParishInformations ())
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
			}
		}


		/// <summary>
		/// The <see cref="ParishAddressRepository"/> singleton.
		/// </summary>
		public static readonly ParishAddressRepository Current = new ParishAddressRepository ();


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

		private ParishAddresses FindAddresses(string key)
		{
			ParishAddresses addresses;

			this.addresses.TryGetValue (key, out addresses);

			return addresses;
		}

		public string FindParishName(int zipCode, string townName, string normalizedStreetName, int streetNumber)
		{
			return this.FindParishName (ParishAddressRepository.GetKey (zipCode, townName), normalizedStreetName, streetNumber);
		}

		public string FindParishName(string zipCode, string townName, string normalizedStreetName, int streetNumber)
		{
			return this.FindParishName (string.Concat (zipCode, " ", townName), normalizedStreetName, streetNumber);
		}

		private string FindParishName(string key, string normalizedStreetName, int streetNumber)
		{
			var addresses = this.FindAddresses (key);

			if (addresses == null)
			{
				return null;
			}
			
			var parish = addresses.FindSpecific (normalizedStreetName, streetNumber)
					  ?? addresses.FindDefault (normalizedStreetName)
					  ?? addresses.FindDefault ();

			if (parish == null)
			{
				return null;
			}
			else
			{
				return parish.ParishName;
			}
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

		private static IEnumerable<ParishAddressInformation> GetParishInformations()
		{
			return ParishAddressRepository.GetSourceFile ().Select (x => new ParishAddressInformation (x));
		}

		private static IEnumerable<string> GetSourceFile()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			var resource = "Epsitec.Aider.DataFiles.ParishAddresses.zip";
			var source   = Epsitec.Common.IO.ZipFile.DecompressTextFile (assembly, resource);

			return Epsitec.Common.IO.StringLineExtractor.GetLines (source);
		}

		
		private readonly Dictionary<string, ParishAddresses>	addresses;
	}
}
