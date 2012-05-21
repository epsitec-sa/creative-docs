//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// The <c>Collections.NameList</c> encapsulates a list of named items.
	/// </summary>
	public class NameList<T> : GenericList<T> where T : IName
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NameList&lt;T&gt;"/> class.
		/// </summary>
		public NameList()
		{
		}

		/// <summary>
		/// Returns the index of the first item with the specified name, starting at
		/// the specified start index.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="start">The start index.</param>
		/// <returns>The index of the item or <c>-1</c>.</returns>
		public override int IndexOf(string name, int start)
		{
			if (start >= 0)
			{
				for (int i = start; i < this.Count; i++)
				{
					if (this[i].Name == name)
					{
						return i;
					}
				}
			}

			return -1;
		}
	}
}
