//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// 
	/// </summary>
	internal struct TextChunkId
	{
		public TextChunkId(int id)
		{
			this.value = id;
		}
		
		
		public static implicit operator int(TextChunkId id)
		{
			return id.value;
		}
		
		public static implicit operator TextChunkId(int id)
		{
			return new TextChunkId (id);
		}
		
		
		private int			value;
	}
}
