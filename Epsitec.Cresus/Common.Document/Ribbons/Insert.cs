using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Insert permet de choisir un élément à insérer dans le texte.
	/// </summary>
	public class Insert : Abstract
	{
		public Insert() : base()
		{
			this.Title = Res.Strings.Action.Text.Insert;
			this.PreferredWidth = 8 + 22*2;

			this.buttonNewFrame = this.CreateIconButton("TextInsertNewFrame");
			this.buttonNewPage  = this.CreateIconButton("TextInsertNewPage");
			this.buttonQuad     = this.CreateIconButton("TextInsertQuad");
			this.buttonGlyphs   = this.CreateIconButton("Glyphs");
			
//			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonGlyphs == null )  return;

			double dx = this.buttonGlyphs.PreferredWidth;
			double dy = this.buttonGlyphs.PreferredHeight;

			Rectangle rect = this.UsefulZone;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonNewFrame.SetManualBounds(rect);
			rect.Offset(20, 0);
			this.buttonNewPage.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonQuad.SetManualBounds(rect);
			rect.Offset(20, 0);
			this.buttonGlyphs.SetManualBounds(rect);
		}


		protected IconButton				buttonNewFrame;
		protected IconButton				buttonNewPage;
		protected IconButton				buttonQuad;
		protected IconButton				buttonGlyphs;
	}
}
