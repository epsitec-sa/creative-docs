using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// La classe TextFieldExName est un TextFieldEx qui ne sélectionne que la dernière
	/// partie du texte lors d'un SelectAll.
	/// </summary>
	public class TextFieldExName : TextFieldEx
	{
		public TextFieldExName()
		{
		}
		
		public TextFieldExName(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		protected override void SelectAll(bool silent)
		{
			int i = this.Text.LastIndexOf('.');
			if (i != -1)
			{
				this.CursorFrom  = this.TextLayout.FindIndexFromOffset(i+1);
				this.CursorTo    = 10000;  // à la fin
				this.CursorAfter = false;
			}
			else
			{
				this.TextNavigator.TextLayout.SelectAll(this.TextNavigator.Context);
			}

			this.OnCursorChanged(silent);
		}
	}
}
