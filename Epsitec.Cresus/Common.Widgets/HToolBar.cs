namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe HToolBar permet de réaliser des tool bars horizontales.
	/// </summary>
	public class HToolBar : AbstractToolBar
	{
		public HToolBar()
		{
			this.direction     = Direction.Up;
			this.iconDockStyle = DockStyle.Left;
			
			double m = (this.DefaultHeight-this.defaultButtonHeight)/2;
			this.DockMargins = new Drawing.Margins(m, m, m, m);
		}
		
		public HToolBar(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public override double				DefaultHeight
		{
			// Retourne la hauteur standard d'une barre.
			get
			{
				return 28;
			}
		}
	}
}
