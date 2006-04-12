using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Character permet de choisir la typographie des caractères.
	/// </summary>
	[SuppressBundleSupport]
	public class Character : Abstract
	{
		public Character() : base()
		{
			this.title.Text = Res.Strings.Ribbon.Section.Character;

			this.buttonBold       = this.CreateIconButton("FontBold");
			this.buttonItalic     = this.CreateIconButton("FontItalic");
			this.buttonUnderlined = this.CreateIconButton("FontUnderlined");
			this.buttonGlyphs     = this.CreateIconButton("Glyphs");
			
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
				return 8 + 22*4 + 5;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonBold == null )  return;

			double dx = this.buttonBold.DefaultWidth;
			double dy = this.buttonBold.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonBold.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonItalic.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUnderlined.Bounds = rect;
			rect.Offset(dx+5, 0);
			this.buttonGlyphs.Bounds = rect;
		}


		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected IconButton				buttonGlyphs;
	}
}
