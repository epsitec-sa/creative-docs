//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	public class SwissPostZipRepository
	{
		public SwissPostZipRepository(IEnumerable<SwissPostZipInformation> zips)
		{
			this.nameByZip = new Dictionary<string, List<SwissPostZipInformation>> ();

			foreach (var zip in zips)
			{
				List<SwissPostZipInformation> list;

				if (this.nameByZip.TryGetValue (zip.ZipCode, out list) == false)
				{
					list = new List<SwissPostZipInformation> ();
					this.nameByZip[zip.ZipCode] = list;
				}

				list.Add (zip);
			}
		}

		public IEnumerable<SwissPostZipInformation> FindZips(int zipCode)
		{
			List<SwissPostZipInformation> list;
			string key = zipCode.ToString ("####");

			if (this.nameByZip.TryGetValue (key, out list))
			{
				return list;
			}
			else
			{
				return EmptyList<SwissPostZipInformation>.Instance;
			}
		}

		public IEnumerable<SwissPostZipInformation> FindZips(int zipCode, string shortName)
		{
			List<SwissPostZipInformation> list;
			string key = zipCode.ToString ("####");

			if (this.nameByZip.TryGetValue (key, out list))
			{
				return list.Where (x => x.ShortName == shortName);
			}
			else
			{
				return EmptyList<SwissPostZipInformation>.Instance;
			}
		}


		private readonly Dictionary<string, List<SwissPostZipInformation>> nameByZip;

		public IEnumerable<SwissPostZipInformation> FindAll()
		{
			return nameByZip.SelectMany (x => x.Value);
		}
	}
}
