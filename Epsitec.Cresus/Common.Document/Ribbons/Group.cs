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
		public Group(Document document) : base(document)
		{
			this.title.Text = Res.Strings.Action.GroupMain;

			this.buttonGroup = this.CreateIconButton("Group", Misc.Icon("Group"), Res.Strings.Action.Group);
			this.buttonMerge = this.CreateIconButton("Merge", Misc.Icon("Merge"), Res.Strings.Action.Merge);
			this.buttonExtract = this.CreateIconButton("Extract", Misc.Icon("Extract"), Res.Strings.Action.Extract);
			this.buttonUngroup = this.CreateIconButton("Ungroup", Misc.Icon("Ungroup"), Res.Strings.Action.Ungroup);
			this.buttonInside = this.CreateIconButton("Inside", Misc.Icon("Inside"), Res.Strings.Action.Inside);
			this.buttonOutside = this.CreateIconButton("Outside", Misc.Icon("Outside"), Res.Strings.Action.Outside);

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
				return 8+22+22+22;
			}
		}

		// Retourne la largeur étendue.
		public override double ExtendWidth
		{
			get
			{
				return 8+22+22+22;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonGroup == null )  return;

			double dx = this.buttonGroup.DefaultWidth;
			double dy = this.buttonGroup.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy);
			this.buttonGroup.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonMerge.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonExtract.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonUngroup.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonInside.Bounds = rect;
			rect.Offset(dx, 0);
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
