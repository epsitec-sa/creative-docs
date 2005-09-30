using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Clipboard permet de gérer les groupes.
	/// </summary>
	[SuppressBundleSupport]
	public class Clipboard : Abstract
	{
		public Clipboard(Document document) : base(document)
		{
			this.title.Text = "Clipboard";

			this.buttonCut   = this.CreateIconButton("Cut",   Misc.Icon("Cut"),   Res.Strings.Action.Cut);
			this.buttonCopy  = this.CreateIconButton("Copy",  Misc.Icon("Copy"),  Res.Strings.Action.Copy);
			this.buttonPaste = this.CreateIconButton("Paste", Misc.Icon("Paste"), Res.Strings.Action.Paste);

			this.buttonPaste.SetClientZoom(2);

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
				return 8+22+4+44;
			}
		}

		// Retourne la largeur étendue.
		public override double ExtendWidth
		{
			get
			{
				return 8+22+4+44;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonCut == null )  return;

			double dx = this.buttonCut.DefaultWidth;
			double dy = this.buttonCut.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonCut.Bounds = rect;
			rect.Offset(0, -dy);
			this.buttonCopy.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx*2;
			rect.Height = dy*2;
			rect.Offset(dx+4, 0);
			this.buttonPaste.Bounds = rect;
		}


		protected IconButton				buttonCut;
		protected IconButton				buttonCopy;
		protected IconButton				buttonPaste;
	}
}
