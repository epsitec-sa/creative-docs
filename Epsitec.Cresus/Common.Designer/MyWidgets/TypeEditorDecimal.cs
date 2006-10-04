using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorDecimal : AbstractTypeEditor
	{
		public TypeEditorDecimal()
		{
			StaticText s = new StaticText(this);
			s.PreferredHeight = 100;
			s.Text = @"<font size=""200%""><b>Decimal</b> (à faire)</font>";
			s.Dock = DockStyle.Fill;
		}
		
		public TypeEditorDecimal(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


	}
}
