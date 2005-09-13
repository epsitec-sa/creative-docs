//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Cursors
{
	/// <summary>
	/// La classe TempCursorDir joue le même rôle que TempCursor, mais prend en
	/// plus note de la direction du déplacement.
	/// </summary>
	public class TempCursorDir : TempCursor
	{
		public TempCursorDir()
		{
		}
		
		
		public override int						Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
			}
		}
		
		
		private int								direction;
	}
}
