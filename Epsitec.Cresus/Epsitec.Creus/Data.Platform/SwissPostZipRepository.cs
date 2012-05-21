//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

			this.nameByZip = new Dictionary<int, List<SwissPostZipInformation>> ();

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

			foreach (var list in this.nameByZip.Values)
			{
				list.Sort (SwissPostZipRepository.Sorter);
			}
		}


		public static readonly SwissPostZipRepository Current = new SwissPostZipRepository ();

		
		public IEnumerable<SwissPostZipInformation> FindAll()
		{
			return nameByZip.SelectMany (x => x.Value);
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

		public IEnumerable<SwissPostZipInformation> FindZips(int zipCode, string longName)
		{
			List<SwissPostZipInformation> list;
			
			if (this.nameByZip.TryGetValue (zipCode, out list))
			{
				return list.Where (x => x.LongName == longName);
			}
			else
			{
				return EmptyList<SwissPostZipInformation>.Instance;
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
	}
}
