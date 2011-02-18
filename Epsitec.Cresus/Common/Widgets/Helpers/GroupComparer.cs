//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// La classe GroupComparer permet de comparer/trier des widgets selon leur
	/// groupe, puis selon leur index.
	/// </summary>
	public class GroupComparer : System.Collections.IComparer
	{
		public GroupComparer(int dir)
		{
			System.Diagnostics.Debug.Assert ((dir == -1) || (dir == 1));
			
			this.dir = dir;
		}
		
		
		public int Compare(object x, object y)
		{
			Widget bx = x as Widget;
			Widget by = y as Widget;
			
			if (bx == by) return 0;
			
			if (bx == null) return -this.dir;
			if (by == null) return  this.dir;
			
			int compare = string.Compare (bx.Group, by.Group) * this.dir;
			
			return (compare == 0) ? (bx.Index - by.Index) * this.dir : compare;
		}
		
		
		protected int						dir;
	}
}
