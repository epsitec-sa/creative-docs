//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data
{
	public sealed class ParishAddresses : IEnumerable<ParishAddressInformation>
	{
		public ParishAddresses()
		{
		}

		public int								Count
		{
			get
			{
				if (this.infos != null)
				{
					return this.infos.Count;
				}
				else
				{
					return this.info == null ? 0 : 1;
				}
			}
		}

		public ParishAddressInformation FindDefault()
		{
			foreach (var info in this)
			{
				if (string.IsNullOrEmpty (info.NormalizedStreetName))
				{
					return info;
				}
			}

			return null;
		}

		public ParishAddressInformation FindDefault(string normalizedStreetName)
		{
			foreach (var info in this)
			{
				if (info.NormalizedStreetName == normalizedStreetName)
				{
					if (info.StreetNumberSubset == null)
					{
						return info;
					}
				}
			}

			return null;
		}

		public ParishAddressInformation FindSpecific(string normalizedStreetName, int streetNumber)
		{
			foreach (var info in this)
			{
				if (info.NormalizedStreetName == normalizedStreetName)
				{
					if (info.StreetNumberSubset != null)
					{
						if (info.StreetNumberSubset.Contains (streetNumber))
						{
							return info;
						}
					}
				}
			}

			return null;
		}

		public void Add(ParishAddressInformation info)
		{
			if ((this.info == null) &&
				(this.infos == null))
			{
				this.info = info;
			}
			else if (this.info != null)
			{
				this.infos = new List<ParishAddressInformation> ();
				this.infos.Add (this.info);
				this.infos.Add (info);
				this.info = null;
			}
			else
			{
				this.infos.Add (info);
			}
		}


		#region IEnumerable<ParishAddressInformation> Members

		public IEnumerator<ParishAddressInformation> GetEnumerator()
		{
			return this.infos == null ? this.GetItems ().GetEnumerator () : this.infos.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.infos == null ? this.GetItems ().GetEnumerator () : this.infos.GetEnumerator ();
		}

		#endregion

		private IEnumerable<ParishAddressInformation> GetItems()
		{
			if (this.info == null)
			{
				yield break;
			}
			else
			{
				yield return this.info;
			}
		}
		
		private ParishAddressInformation		info;
		private List<ParishAddressInformation>	infos;
	}
}
