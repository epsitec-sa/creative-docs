//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	using IComparer = System.Collections.IComparer;
	
	/// <summary>
	/// La structure CursorInfo permet de stocker à la fois l'identificateur
	/// d'un curseur et sa position dans le texte.
	/// </summary>
	public struct CursorInfo
	{
		public CursorInfo(int id, int position)
		{
			this.id       = id;
			this.position = position;
		}
		
		
		public int							CursorId
		{
			get
			{
				return this.id;
			}
		}
		
		public int							Position
		{
			get
			{
				return this.position;
			}
		}
		
		
		public static readonly IComparer	PositionComparer = new PrivatePositionComparer ();
		public static readonly IComparer	CursorIdComparer = new PrivateCursorIdComparer ();
		
		#region Private Comparer Classes
		private class PrivatePositionComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				CursorInfo info_x = (CursorInfo) x;
				CursorInfo info_y = (CursorInfo) y;
				
				return info_y.position - info_x.position;
			}
			#endregion
		}
		
		private class PrivateCursorIdComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				CursorInfo info_x = (CursorInfo) x;
				CursorInfo info_y = (CursorInfo) y;
				
				return info_y.id - info_x.id;
			}
			#endregion
		}
		#endregion
		
		private int							id;
		private int							position;
	}
}
