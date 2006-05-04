using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Replace gère les commandes chercher/remplacer.
	/// </summary>
	public class Replace : Abstract
	{
		public Replace() : base()
		{
			this.title.Text = Res.Strings.Action.ReplaceMain;

			this.buttonReplace = this.CreateIconButton("Replace", "Large");
			
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
				return 8 + 22*1.5*2;
			}
		}


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.buttonReplace == null )  return;

			double dx = this.buttonReplace.PreferredWidth;
			double dy = this.buttonReplace.PreferredHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5);
			this.buttonReplace.SetManualBounds(rect);
		}


		protected IconButton				buttonReplace;
	}
}
