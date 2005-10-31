using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Ribbons
{
	/// <summary>
	/// La classe Order permet de choisir l'ordre de la sélection.
	/// </summary>
	[SuppressBundleSupport]
	public class Order : Abstract
	{
		public Order() : base()
		{
			this.title.Text = Res.Strings.Action.OrderMain;

			this.buttonDownAll = this.CreateIconButton("OrderDownAll", Misc.Icon("OrderDownAll"), Res.Strings.Action.OrderDownAll);
			this.buttonDownOne = this.CreateIconButton("OrderDownOne", Misc.Icon("OrderDownOne"), Res.Strings.Action.OrderDownOne);
			this.buttonUpOne   = this.CreateIconButton("OrderUpOne",   Misc.Icon("OrderUpOne"),   Res.Strings.Action.OrderUpOne);
			this.buttonUpAll   = this.CreateIconButton("OrderUpAll",   Misc.Icon("OrderUpAll"),   Res.Strings.Action.OrderUpAll);
			
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
				return 8 + 22*2;
			}
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.buttonDownAll == null )  return;

			double dx = this.buttonDownAll.DefaultWidth;
			double dy = this.buttonDownAll.DefaultHeight;

			Rectangle rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			this.buttonDownAll.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonDownOne.Bounds = rect;

			rect = this.UsefulZone;
			rect.Width  = dx;
			rect.Height = dy;
			rect.Offset(0, dy+5);
			this.buttonUpAll.Bounds = rect;
			rect.Offset(dx, 0);
			this.buttonUpOne.Bounds = rect;
		}


		protected IconButton				buttonDownAll;
		protected IconButton				buttonDownOne;
		protected IconButton				buttonUpOne;
		protected IconButton				buttonUpAll;
	}
}
