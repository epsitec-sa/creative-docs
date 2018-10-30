//	Copyright © 2018, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIDER.ParishPreprocessor
{
	public class ParishAddress
	{
		public ParishAddress(string id, int zip, string town, string zipTown,
			string streetName, string streetPrefix, string range,
			string parishName, string parishRegion, string parishPrefix, string parishRealName)
		{
			streetPrefix = streetPrefix.Replace ("ch.", "chemin");
			streetPrefix = streetPrefix.Replace ("av.", "avenue");
			streetPrefix = streetPrefix.Replace ("rte", "route");
			streetPrefix = streetPrefix.Replace ("pl.", "place");
			streetPrefix = streetPrefix.Replace ("quart.", "quartier");
			streetPrefix = streetPrefix.Replace ("prom.", "promenade");
			streetPrefix = streetPrefix.Trim ();

			this.id = id;
			this.zip = zip;
			this.zipCode = zip.ToString (System.Globalization.CultureInfo.InvariantCulture);
			this.town = town;
			this.zipTown = zipTown;
			this.streetName = streetName;
			this.streetPrefix = streetPrefix;
			this.range = range;
			this.parishName = parishName;
			this.parishRegion = parishRegion;
			this.parishPrefix = parishPrefix;
			this.parishRealName = parishRealName;

			this.streetFullName = this.streetName + ", " + streetPrefix;
		}


		public int Zip => this.zip;
		public string StreetName => this.streetName;
		public string StreetPrefix => this.streetPrefix;
		public string StreetFullName => this.streetFullName;


		public override string ToString()
		{
			return string.Concat (
				this.id, "\t",
				this.zipCode, "\t",
				this.town, "\t",
				this.zipTown, "\t",
				this.streetName, "\t",
				this.streetPrefix, "\t",
				this.range, "\t",
				this.parishName, "\t",
				this.parishRegion, "\t",
				this.parishPrefix, "\t",
				this.parishRealName);
		}

		private readonly string id;
		private readonly int zip;
		private readonly string zipCode;
		private readonly string town;
		private readonly string zipTown;
		private readonly string streetName;
		private readonly string streetPrefix;
		private readonly string streetFullName;
		private readonly string range;
		private readonly string parishName;
		private readonly string parishRegion;
		private readonly string parishPrefix;
		private readonly string parishRealName;
	}
}
