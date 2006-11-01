//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// The <c>Collections.NameList</c> encapsulates a list of named items.
	/// </summary>
	public class NameList<T> : GenericList<T> where T : IName
	{
		public NameList()
		{
		}

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
