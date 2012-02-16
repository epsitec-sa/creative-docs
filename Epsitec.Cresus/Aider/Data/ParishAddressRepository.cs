﻿//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data
{
	public sealed class ParishAddressRepository
	{
		public ParishAddressRepository()
		{
			this.addresses = new Dictionary<string, ParishAddresses> ();

			foreach (var info in ParishAddressRepository.GetParishInformations ())
			{
				this.AddParishAddress (ParishAddressRepository.GetKey (info.ZipCode, info.TownNameOfficial), info);

				if (info.TownNameOfficial != info.TownName)
				{
					this.AddParishAddress (ParishAddressRepository.GetKey (info.ZipCode, info.TownName), info);
				}
			}
		}


		public ParishAddresses FindAddresses(int zipCode, string townName)
		{
			return this.FindAddresses (ParishAddressRepository.GetKey (zipCode, townName));
		}

		public ParishAddresses FindAddresses(string key)
		{
			ParishAddresses addresses;

			this.addresses.TryGetValue (key, out addresses);

			return addresses;
		}

		public string FindParishName(int zipCode, string townName, string normalizedStreetName, int streetNumber)
		{
			return this.FindParishName (ParishAddressRepository.GetKey (zipCode, townName), normalizedStreetName, streetNumber);
		}

		public string FindParishName(string key, string normalizedStreetName, int streetNumber)
		{
			var addresses = this.FindAddresses (key);
			
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


		private void AddParishAddress(string key, ParishAddressInformation info)
		{
			ParishAddresses addresses;
			if (this.addresses.TryGetValue (key, out addresses) == false)
			{
				addresses = new ParishAddresses ();
				this.addresses[key] = addresses;
			}

			addresses.Add (info);
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
