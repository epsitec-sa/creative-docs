using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Group permet de gérer les groupes.
	/// </summary>
	public class Group : Abstract
	{
		public Group() : base()
		{
			this.Title = Res.Strings.Action.GroupMain;
			this.PreferredWidth = 8 + 22*2 + 4 + 22*1.5*2;

			this.buttonGroup   = this.CreateIconButton("Group");
			this.buttonUngroup = this.CreateIconButton("Ungroup");
			this.buttonMerge   = this.CreateIconButton("Merge");
			this.buttonExtract = this.CreateIconButton("Extract");
			this.buttonInside  = this.CreateIconButton("Inside", "Large");
			this.buttonOutside = this.CreateIconButton("Outside", "Large");
			
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

			if ( this.buttonGroup == null )  return;

			double dx = this.buttonGroup.PreferredWidth;
			double dy = this.buttonGroup.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonGroup.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonUngroup.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonMerge.SetManualBounds(rect);
			rect.Offset(dx, 0);
			this.buttonExtract.SetManualBounds(rect);

			rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(dx*2+4, dy*0.5);
			this.buttonInside.SetManualBounds(rect);
			rect.Offset(dx*1.5, 0);
			this.buttonOutside.SetManualBounds(rect);
		}


		protected IconButton				buttonGroup;
		protected IconButton				buttonMerge;
		protected IconButton				buttonExtract;
		protected IconButton				buttonUngroup;
		protected IconButton				buttonInside;
		protected IconButton				buttonOutside;
	}
}
