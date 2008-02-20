//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La structure CursorId encapsule un identificateur lié à une structure
	/// de type Internal.Cursor.
	/// </summary>
	internal struct CursorId
	{
		public CursorId(int id)
		{
			this.value = id;
		}
		
		
		public bool								IsValid
		{
			//	Un curseur est déclaré comme étant valide s'il a un identificateur
			//	strictement positif.
			
			//	CursorId = 0 n'est donc pas valide.
			
			get
			{
				return this.value > 0;
			}
		}
		
		
		public static implicit operator int(CursorId id)
		{
			return id.value;
		}
		
		public static implicit operator CursorId(int id)
		{
			return new CursorId (id);
		}
		
		
		private int								value;
	}
}
