using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Zoom permet de gérer les groupes.
	/// </summary>
	[SuppressBundleSupport]
	public class Zoom : Abstract
	{
		public Zoom(Document document) : base(document)
		{
			this.title.Text = "Zoom";

			this.buttonZoomMin = this.CreateIconButton("ZoomMin", Misc.Icon("ZoomMin"), Res.Strings.Action.ZoomMin);
			this.buttonZoomPage = this.CreateIconButton("ZoomPage", Misc.Icon("ZoomPage"), Res.Strings.Action.ZoomPage);
			this.buttonZoomPageWidth = this.CreateIconButton("ZoomPageWidth", Misc.Icon("ZoomPageWidth"), Res.Strings.Action.ZoomPageWidth);
			this.buttonZoomDefault = this.CreateIconButton("ZoomDefault", Misc.Icon("ZoomDefault"), Res.Strings.Action.ZoomDefault);
			this.buttonZoomSel = this.CreateIconButton("ZoomSel", Misc.Icon("ZoomSel"), Res.Strings.Action.ZoomSel);
			this.buttonZoomSelWidth = this.CreateIconButton("ZoomSelWidth", Misc.Icon("ZoomSelWidth"), Res.Strings.Action.ZoomSelWidth);
			this.buttonZoomPrev = this.CreateIconButton("ZoomPrev", Misc.Icon("ZoomPrev"), Res.Strings.Action.ZoomPrev);
			this.CreateSeparator(ref this.separator);
			this.buttonZoomAdd = this.CreateIconButton("ZoomAdd", Misc.Icon("ZoomAdd"), Res.Strings.Action.ZoomAdd);
			this.buttonZoomSub = this.CreateIconButton("ZoomSub", Misc.Icon("ZoomSub"), Res.Strings.Action.ZoomSub);

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
				return 8 + 22*4 + this.separatorWidth + 22*1;
			}
		}

		// Retourne la largeur étendue.
		public override double ExtendWidth
		{
			get
			{
				return 8 + 22*4 + this.separatorWidth + 22*1;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonZoomMin == null )  return;

			double dx = this.buttonZoomMin.DefaultWidth;
			double dy = this.buttonZoomMin.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*4;
			rect.Width = this.separatorWidth;
			this.separator.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonZoomMin.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonZoomPage.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonZoomPageWidth.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonZoomDefault.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonZoomAdd.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonZoomSel.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonZoomSelWidth.Bounds = rect;
			rect.Offset(dx*2, 0);
			this.buttonZoomPrev.Bounds = rect;
			rect.Offset(dx+this.separatorWidth, 0);
			this.buttonZoomSub.Bounds = rect;
		}


		protected IconButton				buttonZoomMin;
		protected IconButton				buttonZoomPage;
		protected IconButton				buttonZoomPageWidth;
		protected IconButton				buttonZoomDefault;
		protected IconButton				buttonZoomSel;
		protected IconButton				buttonZoomSelWidth;
		protected IconButton				buttonZoomPrev;
		protected IconSeparator				separator;
		protected IconButton				buttonZoomAdd;
		protected IconButton				buttonZoomSub;
	}
}
