//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
				CursorInfo infoX = (CursorInfo) x;
				CursorInfo infoY = (CursorInfo) y;
				
				if (infoX.position == infoY.position)
				{
					return infoX.direction - infoY.direction;
				}
				
				return infoX.position - infoY.position;
			}
			#endregion
		}
		
		private class PrivateCursorIdComparer : System.Collections.IComparer
		{
			#region IComparer Members
			public int Compare(object x, object y)
			{
				CursorInfo infoX = (CursorInfo) x;
				CursorInfo infoY = (CursorInfo) y;
				
				return infoX.id - infoY.id;
			}
			#endregion
		}
		#endregion
		
		private int							id;
		private int							position;
		private int							direction;
	}
}
