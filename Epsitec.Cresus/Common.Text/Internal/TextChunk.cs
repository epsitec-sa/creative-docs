//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// Summary description for TextChunk.
	/// </summary>
	internal class TextChunk
	{
		public TextChunk()
		{
			this.cursors = new CursorIdArray ();
		}
		
		
		private CursorIdArray				cursors;
	}
}
