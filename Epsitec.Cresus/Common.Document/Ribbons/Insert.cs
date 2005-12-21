using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Insert permet de choisir un �l�ment � ins�rer dans le texte.
	/// </summary>
	[SuppressBundleSupport]
	public class Insert : Abstract
	{
		public Insert() : base()
		{
			this.title.Text = Res.Strings.Action.Text.Insert;

			this.buttonNewFrame = this.CreateIconButton("TextInsertNewFrame");
			this.buttonNewPage  = this.CreateIconButton("TextInsertNewPage");
			this.buttonQuad     = this.CreateIconButton("TextInsertQuad");
			this.buttonGlyphs   = this.CreateIconButton("Glyphs");
			
			this.UpdateClientGeometry();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		public override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 8 + 22*2;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.buttonGlyphs == null )  return;

			double dx = this.buttonGlyphs.DefaultWidth;
			double dy = this.buttonGlyphs.DefaultHeight;

			Rectangle rect = this.UsefulZone;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonNewFrame.Bounds = rect;
			rect.Offset(20, 0);
			this.buttonNewPage.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonQuad.Bounds = rect;
			rect.Offset(20, 0);
			this.buttonGlyphs.Bounds = rect;
		}


		protected IconButton				buttonNewFrame;
		protected IconButton				buttonNewPage;
		protected IconButton				buttonQuad;
		protected IconButton				buttonGlyphs;
	}
}
