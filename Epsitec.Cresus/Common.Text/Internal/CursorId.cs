//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La structure CursorId encapsule un identificateur pointant sur
	/// une structure Cursor.
	/// </summary>
	internal struct CursorId
	{
		public CursorId(int id)
		{
			this.value = id;
		}
		
		
		public static implicit operator int(CursorId id)
		{
			return id.value;
		}
		
		public static implicit operator CursorId(int id)
		{
			return new CursorId (id);
		}
		
		
		private int			value;
	}
}
