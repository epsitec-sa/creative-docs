using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Select permet de gérer la sélection.
	/// </summary>
	public class Select : Abstract
	{
		public Select() : base()
		{
			this.Title = Res.Strings.Action.SelectMain;
			this.PreferredWidth = 8 + 22*1.5*2 + this.separatorWidth + 22*4;

			this.buttonDelete    = this.CreateIconButton("Delete", "Large");
			this.buttonDuplicate = this.CreateIconButton("Duplicate", "Large");

			this.separatorV = new IconSeparator(this);
			this.separatorH = new IconSeparator(this);
			this.separatorH.IsHorizontal = false;
			
			this.buttonDeselectAll  = this.CreateIconButton(Common.Widgets.Res.Commands.DeselectAll.CommandId);
			this.buttonSelectAll    = this.CreateIconButton(Common.Widgets.Res.Commands.SelectAll.CommandId);
			this.buttonSelectInvert = this.CreateIconButton("SelectInvert");
			this.buttonHideSel      = this.CreateIconButton("HideSel");
			this.buttonHideRest     = this.CreateIconButton("HideRest");
			this.buttonHideCancel   = this.CreateIconButton("HideCancel");
			this.buttonHideHalf     = this.CreateIconButton("HideHalf");
			
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

			if ( this.buttonDelete == null )  return;

			double dx = this.buttonDelete.PreferredWidth;
			double dy = this.buttonDelete.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Left += dx*1.5*2;
			rect.Width = this.separatorWidth;
			this.separatorV.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Left += dx*1.5*2+this.separatorWidth*0.5;
			rect.Bottom += dy;
			rect.Height = 5;
			this.separatorH.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonDelete.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonDuplicate.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+this.separatorWidth, dy+5);
			this.buttonDeselectAll.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonSelectAll.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonSelectInvert.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+this.separatorWidth, 0);
			this.buttonHideSel.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonHideRest.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonHideCancel.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonHideHalf.SetManualBounds(rect);
		}


		protected IconButton				buttonDelete;
		protected IconButton				buttonDuplicate;
		protected IconSeparator				separatorV;
		protected IconSeparator				separatorH;
		protected IconButton				buttonDeselectAll;
		protected IconButton				buttonSelectAll;
		protected IconButton				buttonSelectInvert;
		protected IconButton				buttonHideSel;
		protected IconButton				buttonHideRest;
		protected IconButton				buttonHideCancel;
		protected IconButton				buttonHideHalf;
	}
}
