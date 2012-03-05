//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data
{
	/// <summary>
	/// The <c>ParishAddresses</c> class groups similar <see cref="ParishAddressInformation"/>
	/// records, i.e. all those which belong to the same zip code and town name.
	/// </summary>
	public sealed class ParishAddresses : IEnumerable<ParishAddressInformation>
	{
		public ParishAddresses()
		{
			//	The list of parish address infos is only allocated when there is
			//	more than one record needed for this group. We want to optimize
			//	the memory consumption.
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


		/// <summary>
		/// Finds the default information record for this town.
		/// </summary>
		/// <returns>The default information or <c>null</c>.</returns>
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

		/// <summary>
		/// Finds the default information record for this town, matching the specified normalized
		/// street name (see method <see cref="SwissPostStreet.NormalizeStreetName"/>). This will
		/// never return information for an explicit house number range.
		/// </summary>
		/// <param name="normalizedStreetName">The normalized street name.</param>
		/// <returns>The default information or <c>null</c>.</returns>
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

		/// <summary>
		/// Finds the information for a specific street name and house number. This will only
		/// return a result if the house number lies within a specific house number range, as
		/// defined by the information record.
		/// </summary>
		/// <param name="normalizedStreetName">The normalized street name.</param>
		/// <param name="houseNumber">The house number.</param>
		/// <returns>The information or <c>null</c> if there is no specific information for the given house number.</returns>
		public ParishAddressInformation FindSpecific(string normalizedStreetName, int houseNumber)
		{
			foreach (var info in this)
			{
				if (info.NormalizedStreetName == normalizedStreetName)
				{
					if (info.StreetNumberSubset != null)
					{
						if (info.StreetNumberSubset.Contains (houseNumber))
						{
							return info;
						}
					}
				}
			}

			return null;
		}


		public IEnumerable<ParishAddressInformation> FindAll()
		{
			return this.infos.AsReadOnly ();
		}

		
		internal void Add(ParishAddressInformation info)
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
