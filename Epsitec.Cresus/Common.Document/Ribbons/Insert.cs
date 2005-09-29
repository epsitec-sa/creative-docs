using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Insert permet de choisir un élément à insérer dans le texte.
	/// </summary>
	[SuppressBundleSupport]
	public class Insert : Abstract
	{
		public Insert(Document document) : base(document)
		{
			this.title.Text = "Insert";

			this.buttonGlyphs = this.CreateIconButton("Glyphs", Misc.Icon("Glyphs"), Res.Strings.Action.Glyphs);

			this.isNormalAndExtended = false;
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		// Retourne la largeur compacte.
		public override double CompactWidth
		{
			get
			{
				return 8+22+22;
			}
		}

		// Retourne la largeur étendue.
		public override double ExtendWidth
		{
			get
			{
				return 8+22+22;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonGlyphs == null )  return;

			double dx = this.buttonGlyphs.DefaultWidth;
			double dy = this.buttonGlyphs.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonGlyphs.Bounds = rect;
		}


		protected IconButton				buttonGlyphs;
	}
}
