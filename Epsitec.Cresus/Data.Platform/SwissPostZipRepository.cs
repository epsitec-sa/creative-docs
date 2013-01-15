//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>SwissPostZipRepository</c> class gives access to the ZIP information, as
	/// provided by the Swiss Post (see MAT[CH]zip).
	/// </summary>
	public sealed class SwissPostZipRepository
	{
		private SwissPostZipRepository()
		{
			IEnumerable<SwissPostZipInformation> zips = SwissPostZip.GetZips ();

			this.nameByZip  = new Dictionary<int, List<SwissPostZipInformation>> ();
			this.nameByOnrp = new Dictionary<int, SwissPostZipInformation> ();

			foreach (var zip in zips)
			{
				List<SwissPostZipInformation> list;

				if (this.nameByZip.TryGetValue (zip.ZipCode, out list) == false)
				{
					list = new List<SwissPostZipInformation> ();
					this.nameByZip[zip.ZipCode] = list;
				}

				list.Add (zip);

				this.nameByOnrp[zip.OnrpCode] = zip;
			}

			foreach (var list in this.nameByZip.Values)
			{
				list.Sort (SwissPostZipRepository.Sorter);
			}
		}


		public static readonly SwissPostZipRepository Current = new SwissPostZipRepository ();

		
		public IEnumerable<SwissPostZipInformation> FindAll()
		{
			return this.nameByZip.SelectMany (x => x.Value);
		}

		public SwissPostZipInformation FindByOnrpCode(int onrpCode)
		{
			SwissPostZipInformation zip;

			this.nameByOnrp.TryGetValue (onrpCode, out zip);

			return zip;
		}

		public IEnumerable<SwissPostZipInformation> FindZips(int zipCode)
		{
			List<SwissPostZipInformation> list;

			if (this.nameByZip.TryGetValue (zipCode, out list))
			{
				return list;
			}
			else
			{
				return EmptyList<SwissPostZipInformation>.Instance;
			}
		}

		public IEnumerable<SwissPostZipInformation> FindZips(string name)
		{
			var altName = SwissPostZipInformation.ConvertToAlternateName (name);
			return this.FindAll ().Where (x => x.MatchAlternateName (altName)|| string.Equals (name, x.LongName, System.StringComparison.OrdinalIgnoreCase));
		}

		public IEnumerable<SwissPostZipInformation> FindZips(int zipCode, int zipComplement)
		{
			List<SwissPostZipInformation> list;

			if (this.nameByZip.TryGetValue (zipCode, out list))
			{
				return list.Where (x => x.ZipComplement == zipComplement);
			}
			else
			{
				return EmptyList<SwissPostZipInformation>.Instance;
			}
		}

		public IEnumerable<SwissPostZipInformation> FindZips(int zipCode, string name)
		{
			List<SwissPostZipInformation> list;
			
			if (this.nameByZip.TryGetValue (zipCode, out list))
			{
				return list.Where (x => x.MatchName (name));
			}
			else
			{
				return EmptyList<SwissPostZipInformation>.Instance;
			}
		}

		public int FindOnrp(int onrpCode, int zipCode, string name)
		{
			if (this.nameByOnrp.ContainsKey (onrpCode))
			{
				return onrpCode;
			}
			else
			{
				var info = this.FindZips (zipCode, name).Concat (this.FindZips (name)).FirstOrDefault ();

				if (info != null)
				{
					return info.OnrpCode;
				}
				else
				{
					System.Diagnostics.Debug.WriteLine ("ONRP code not found for {0} {1} (old ONRP was {2})", zipCode, name, onrpCode);
					return 0;
				}
			}
		}

		/// <summary>
		/// Sorts the zip records of a same zip code. The domiciles/mixed come before PO boxes,
		/// companies and internal zip codes.
		/// </summary>
		/// <param name="a">Zip record.</param>
		/// <param name="b">Zip record.</param>
		/// <returns>Comparison result.</returns>
		private static int Sorter(SwissPostZipInformation a, SwissPostZipInformation b)
		{
			if (a == b)
			{
				return 0;
			}

			SwissPostZipType aZipType = a.ZipType;
			SwissPostZipType bZipType = b.ZipType;

			if (aZipType == SwissPostZipType.DomicileOnly)
			{
				aZipType = SwissPostZipType.Mixed;
			}
			if (bZipType == SwissPostZipType.DomicileOnly)
			{
				bZipType = SwissPostZipType.Mixed;
			}

			if (aZipType < bZipType)
			{
				return -1;
			}
			if (aZipType > bZipType)
			{
				return 1;
			}
			if (a.ZipComplement < b.ZipComplement)
			{
				return -1;
			}
			if (a.ZipComplement > b.ZipComplement)
			{
				return 1;
			}
			if (a.OnrpCode < b.OnrpCode)
			{
				return -1;
			}
			if (a.OnrpCode > b.OnrpCode)
			{
				return 1;
			}
			return 0;
		}


		private readonly Dictionary<int, List<SwissPostZipInformation>> nameByZip;
		private readonly Dictionary<int, SwissPostZipInformation> nameByOnrp;
	}
}
