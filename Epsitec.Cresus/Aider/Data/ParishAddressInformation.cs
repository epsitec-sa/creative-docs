//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Data.Platform;

namespace Epsitec.Aider.Data
{
	/// <summary>
	/// The <c>ParishAddressInformation</c> class describes a parish address, as provided
	/// by the EERV. Every town should have at least one associated information record.
	/// </summary>
	public sealed class ParishAddressInformation
	{
		public ParishAddressInformation(string line)
		{
			string[] cols = line.Split ('\t');

			System.Diagnostics.Debug.Assert (cols.Length == 8);

			this.Row     = int.Parse (cols[0], System.Globalization.CultureInfo.InvariantCulture);
			this.ZipCode = int.Parse (cols[1], System.Globalization.CultureInfo.InvariantCulture);

			this.TownName         = cols[2];
			this.TownNameOfficial = cols[3];
			this.StreetName       = cols[4];
			this.StreetPrefix     = cols[5];
			this.ParishName       = cols[7];

			this.NormalizedStreetName = SwissPostStreet.NormalizeStreetName (this.StreetName + ", " + this.StreetPrefix);

			if (string.IsNullOrEmpty (cols[6]))
			{
				this.StreetNumberSubset = null;
			}
			else
			{
				this.StreetNumberSubset = new HashSet<int> ();

				foreach (var range in cols[6].Split ('/'))
				{
					this.AddStreetNumberRange (range);
				}
			}
		}

		
		public readonly int						Row;

		public readonly int						ZipCode;

		public readonly string					TownName;

		public readonly string					TownNameOfficial;

		public readonly string					StreetName;

		public readonly string					StreetPrefix;

		public readonly string					ParishName;

		public readonly string					NormalizedStreetName;

		public readonly HashSet<int>			StreetNumberSubset;


		public bool CheckStreetNumber(int number)
		{
			if (this.StreetNumberSubset == null)
			{
				return true;
			}
			else
			{
				return this.StreetNumberSubset.Contains (number);
			}
		}


		private void AddStreetNumberRange(string range)
		{
			var args = range.Split ('-');

			if (args.Length == 1)
			{
				this.AddStreetNumber (int.Parse (range, System.Globalization.CultureInfo.InvariantCulture));
			}
			else
			{
				int start = int.Parse (args[0], System.Globalization.CultureInfo.InvariantCulture);
				int stop  = string.IsNullOrEmpty (args[1]) ? ((start & 0x01) + 400) : int.Parse (args[1], System.Globalization.CultureInfo.InvariantCulture);

				if ((start & 0x01) != (stop & 0x01))
				{
					throw new System.ArgumentException ("Even/odd mismatch in range " + range, "range");
				}

				while (start <= stop)
				{
					this.AddStreetNumber (start);
					start += 2;
				}
			}
		}

		private void AddStreetNumber(int number)
		{
			this.StreetNumberSubset.Add (number);
		}
	}
}
