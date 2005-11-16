using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Order permet de choisir l'ordre de la s�lection.
	/// </summary>
	[SuppressBundleSupport]
	public class Order : Abstract
	{
		public Order() : base()
		{
			this.title.Text = Res.Strings.Action.OrderMain;

			this.buttonUpAll   = this.CreateIconButton("OrderUpAll",   Misc.Icon("OrderUpAll2"),   Res.Strings.Action.OrderUpAll);
			this.buttonDownAll = this.CreateIconButton("OrderDownAll", Misc.Icon("OrderDownAll2"), Res.Strings.Action.OrderDownAll);
			this.buttonUpOne   = this.CreateIconButton("OrderUpOne",   Misc.Icon("OrderUpOne"),    Res.Strings.Action.OrderUpOne);
			this.buttonDownOne = this.CreateIconButton("OrderDownOne", Misc.Icon("OrderDownOne"),  Res.Strings.Action.OrderDownOne);
			
			this.UpdateClientGeometry();
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
				return 8 + 22*1.5*2 + 4 + 22*1;
			}
		}


		// Met � jour la g�om�trie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonDownAll == null )  return;

			double dx = this.buttonDownAll.DefaultWidth;
			double dy = this.buttonDownAll.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx*1.5;
			rect.Height = dy*1.5;
			rect.Offset(0, dy*0.5+5);
			this.buttonUpAll.Bounds = rect;
			rect.Offset(dx*1.5, -dy*0.5-5);
			this.buttonDownAll.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(dx*1.5*2+4, dy+5);
			this.buttonUpOne.Bounds = rect;
			rect.Offset(0, -dy-5);
			this.buttonDownOne.Bounds = rect;
		}


		protected IconButton				buttonDownAll;
		protected IconButton				buttonDownOne;
		protected IconButton				buttonUpOne;
		protected IconButton				buttonUpAll;
	}
}
