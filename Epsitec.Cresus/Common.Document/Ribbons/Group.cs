using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Group permet de gérer les groupes.
	/// </summary>
	[SuppressBundleSupport]
	public class Group : Abstract
	{
		public Group() : base()
		{
			this.title.Text = Res.Strings.Action.GroupMain;

			this.buttonGroup   = this.CreateIconButton("Group");
			this.buttonUngroup = this.CreateIconButton("Ungroup");
			this.buttonMerge   = this.CreateIconButton("Merge");
			this.buttonExtract = this.CreateIconButton("Extract");
			this.buttonInside  = this.CreateIconButton("Inside", "Large");
			this.buttonOutside = this.CreateIconButton("Outside", "Large");
			
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
				return 8 + 22*2 + 4 + 22*1.5*2;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonGroup == null )  return;

			double dx = this.buttonGroup.DefaultWidth;
			double dy = this.buttonGroup.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonGroup.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUngroup.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonMerge.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonExtract.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(dx*2+4, dy*0.5);
			this.buttonInside.Bounds = rect;
			rect.Offset(dx*1.5, 0);
			this.buttonOutside.Bounds = rect;
		}


		protected IconButton				buttonGroup;
		protected IconButton				buttonMerge;
		protected IconButton				buttonExtract;
		protected IconButton				buttonUngroup;
		protected IconButton				buttonInside;
		protected IconButton				buttonOutside;
	}
}
