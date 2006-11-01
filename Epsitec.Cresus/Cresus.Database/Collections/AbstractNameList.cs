//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Collections
{
	/// <summary>
	/// La classe Collection.SqlColumns encapsule une collection d'instances de type SqlColumn.
	/// </summary>
	public class AbstractNameList<T> : AbstractList<T> where T : class, Epsitec.Common.Types.IName
	{
		public AbstractNameList()
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
