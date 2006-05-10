using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Character permet de choisir la typographie des caract�res.
	/// </summary>
	public class Character : Abstract
	{
		public Character() : base()
		{
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

		protected override double DefaultWidth
		{
			//	Retourne la largeur standard.
			get
			{
				return 8 + 22*4 + 5;
			}
		}

		protected override string DefaultTitle
		{
			//	Retourne le titre standard.
			get
			{
				return Res.Strings.Ribbon.Section.Character;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.buttonBold == null )  return;

			double dx = this.buttonBold.PreferredWidth;
			double dy = this.buttonBold.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonBold.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonItalic.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonUnderlined.SetManualBounds(rect);
			rect.Offset(dx+5, 0);
			this.buttonGlyphs.SetManualBounds(rect);
		}


		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderlined;
		protected IconButton				buttonGlyphs;
	}
}
