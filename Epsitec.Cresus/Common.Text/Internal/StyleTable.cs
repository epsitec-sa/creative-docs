//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
{
	/// <summary>
	/// Summary description for StyleTable.
	/// </summary>
	internal sealed class StyleTable
	{
		public StyleTable()
		{
			this.font_styles      = null;
			this.paragraph_styles = null;
			this.special_styles   = null;
		}
		
		
		private System.Collections.ArrayList	font_styles;
		private System.Collections.ArrayList	paragraph_styles;
		private System.Collections.ArrayList	special_styles;
	}
}
