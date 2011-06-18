//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	/// <summary>
	/// The <c>BrickPropertyCollection</c> class provides sequential access to a filtered collection
	/// of <see cref="BrickProperty"/> items, including <see cref="PeekBefore"/> and <see cref="PeekAfter"/>
	/// methods, which can be used to search for additional, related properties.
	/// </summary>
	public sealed class BrickPropertyCollection : IEnumerable<BrickProperty>
	{
		internal BrickPropertyCollection(IList<BrickProperty> properties, BrickPropertyKey[] filter)
		{
			this.properties = properties;
			this.filter = new HashSet<BrickPropertyKey> (filter);
			this.index = -1;
		}

		public BrickProperty? PeekAfter(BrickPropertyKey adjacentPropertyKey, int offset = 0)
		{
			int index = this.index + offset;

			while ((++index >= 0) && (index < this.properties.Count))
			{
				var property = this.properties[index];

				if (property.Key == adjacentPropertyKey)
				{
					return property;
				}

				if (this.filter.Contains (property.Key))
				{
					break;
				}
			}

			return null;
		}


		public BrickProperty? PeekBefore(BrickPropertyKey adjacentPropertyKey, int offset = 0)
		{
			int index = this.index + offset;

			while ((--index >= 0) && (index < this.properties.Count))
			{
				var property = this.properties[index];

				if (property.Key == adjacentPropertyKey)
				{
					return property;
				}

				if (this.filter.Contains (property.Key))
				{
					break;
				}
			}

			return null;
		}

		private IEnumerable<BrickProperty> GetProperties()
		{
			while (this.index < this.properties.Count)
			{
				var property = this.properties[this.index++];

				if ((this.filter.Count == 0) ||
							(this.filter.Contains (property.Key)))
				{
					yield return property;
				}
			}

			this.index = -1;
		}

		#region IEnumerable<BrickProperty> Members

		public IEnumerator<BrickProperty> GetEnumerator()
		{
			this.index = 0;
			return this.GetProperties ().GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		private readonly IList<BrickProperty> properties;
		private readonly HashSet<BrickPropertyKey> filter;
		private int index;
	}
}
