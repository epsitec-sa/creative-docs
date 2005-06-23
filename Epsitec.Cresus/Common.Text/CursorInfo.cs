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
		public CursorInfo(int id, int position, int direction)
		{
			this.id        = id;
			this.position  = position;
			this.direction = direction;
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
		
		public int							Direction
		{
			get
			{
				return this.direction;
			}
		}
		
		
		public static readonly IComparer	PositionComparer = new PrivatePositionComparer ();
		public static readonly IComparer	CursorIdComparer = new PrivateCursorIdComparer ();
		
		public delegate bool Filter(ICursor cursor, int position);
		
		#region Private Comparer Classes
		private class PrivatePositionComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				CursorInfo info_x = (CursorInfo) x;
				CursorInfo info_y = (CursorInfo) y;
				
				if (info_x.position == info_y.position)
				{
					return info_x.direction - info_y.direction;
				}
				
				return info_x.position - info_y.position;
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
				
				return info_x.id - info_y.id;
			}
			#endregion
		}
		#endregion
		
		private int							id;
		private int							position;
		private int							direction;
	}
}
