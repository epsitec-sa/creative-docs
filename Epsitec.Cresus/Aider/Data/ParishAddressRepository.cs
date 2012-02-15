//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data
{
	public sealed class ParishAddressRepository
	{
		public ParishAddressRepository()
		{
			this.addresses = new Dictionary<int, ParishAddresses> ();

			foreach (var info in ParishAddressRepository.GetParishInformations ())
			{
				ParishAddresses addresses;

				if (this.addresses.TryGetValue (info.ZipCode, out addresses) == false)
				{
					addresses = new ParishAddresses ();
					this.addresses[info.ZipCode] = addresses;
				}

				addresses.Add (info);
			}
		}

		
		public ParishAddresses FindAddresses(int zipCode)
		{
			ParishAddresses addresses;

			this.addresses.TryGetValue (zipCode, out addresses);

			return addresses;
		}

		public string FindParishName(int zipCode, string streetName, string streetPrefix, int streetNumber)
		{
			var addresses = this.FindAddresses (zipCode);
			var parish    = addresses.FindSpecific (streetName, streetPrefix, streetNumber) ?? addresses.FindDefault (streetName, streetPrefix);

			if (parish == null)
			{
				return null;
			}
			else
			{
				return parish.ParishName;
			}
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

		
		private readonly Dictionary<int, ParishAddresses>	addresses;
	}
}
