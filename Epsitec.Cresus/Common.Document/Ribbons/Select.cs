using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Select permet de gérer les groupes.
	/// </summary>
	[SuppressBundleSupport]
	public class Select : Abstract
	{
		public Select() : base()
		{
			this.title.Text = "Selection";

			this.buttonDelete = this.CreateIconButton("Delete", Misc.Icon("Delete2"), Res.Strings.Action.Delete);
			this.buttonDuplicate = this.CreateIconButton("Duplicate", Misc.Icon("Duplicate2"), Res.Strings.Action.Duplicate);
			this.CreateSeparator(ref this.separator);
			this.buttonDeselectAll = this.CreateIconButton("DeselectAll", Misc.Icon("DeselectAll"), Res.Strings.Action.DeselectAll);
			this.buttonSelectAll = this.CreateIconButton("SelectAll", Misc.Icon("SelectAll"), Res.Strings.Action.SelectAll);
			this.buttonSelectInvert = this.CreateIconButton("SelectInvert", Misc.Icon("SelectInvert"), Res.Strings.Action.SelectInvert);
			this.buttonHideSel = this.CreateIconButton("HideSel", Misc.Icon("HideSel"), Res.Strings.Action.HideSel);
			this.buttonHideRest = this.CreateIconButton("HideRest", Misc.Icon("HideRest"), Res.Strings.Action.HideRest);
			this.buttonHideCancel = this.CreateIconButton("HideCancel", Misc.Icon("HideCancel"), Res.Strings.Action.HideCancel);
			this.buttonHideHalf = this.CreateIconButton("HideHalf", Misc.Icon("HideHalf"), Res.Strings.Action.HideHalf);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
			}
			
			base.Dispose(disposing);
		}

		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 8 + 22*1.5*2 + this.separatorWidth + 22*4;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonDelete == null )  return;

			double dx = this.buttonDelete.DefaultWidth;
			double dy = this.buttonDelete.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*1.5*2;
			rect.Width = this.separatorWidth;
			this.separator.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonDelete.Bounds = rect;
			rect.Offset(dx*1.5, 0);
			this.buttonDuplicate.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+this.separatorWidth, dy);
			this.buttonDeselectAll.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonSelectAll.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonSelectInvert.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+this.separatorWidth, 0);
			this.buttonHideSel.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonHideRest.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonHideCancel.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonHideHalf.Bounds = rect;
		}


		protected IconButton				buttonDelete;
		protected IconButton				buttonDuplicate;
		protected IconSeparator				separator;
		protected IconButton				buttonDeselectAll;
		protected IconButton				buttonSelectAll;
		protected IconButton				buttonSelectInvert;
		protected IconButton				buttonHideSel;
		protected IconButton				buttonHideRest;
		protected IconButton				buttonHideCancel;
		protected IconButton				buttonHideHalf;
	}
}
