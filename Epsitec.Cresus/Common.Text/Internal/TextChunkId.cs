//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// La structure TextChunkId encapsule un identificateur lié à une struc-
	/// ture de type Internal.TextChunk.
	/// </summary>
	internal struct TextChunkId
	{
		public TextChunkId(int id)
		{
			this.value = id;
		}
		
		
		public bool								IsValid
		{
			//	Un morceau de texte est déclaré comme étant valide s'il a un
			//	identificateur strictement positif.
			
			//	TextChunkId = 0 n'est donc pas valide.
			
			get
			{
				return this.value > 0;
			}
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
