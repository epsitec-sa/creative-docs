using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Ribbons
{
	/// <summary>
	/// La classe Character permet de choisir la typographie des caractères.
	/// </summary>
	public class CharacterRibbon : AbstractRibbon
	{
		public CharacterRibbon(DesignerApplication designerApplication) : base(designerApplication)
		{
			this.Title = Res.Strings.Ribbon.Section.Character;
			this.PreferredWidth = 8 + 22*3;

			this.buttonBold       = this.CreateIconButton("FontBold");
			this.buttonItalic     = this.CreateIconButton("FontItalic");
			this.buttonUnderline  = this.CreateIconButton("FontUnderline");
			this.buttonGlyphs     = this.CreateIconButton("DesignerGlyphs");
			
			this.UpdateClientGeometry();
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
			this.buttonUnderline.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonGlyphs.SetManualBounds(rect);
		}


		protected IconButton				buttonBold;
		protected IconButton				buttonItalic;
		protected IconButton				buttonUnderline;
		protected IconButton				buttonGlyphs;
	}
}
