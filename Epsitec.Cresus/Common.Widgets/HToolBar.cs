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
			
			double m = (this.DefaultHeight-this.defaultButtonHeight)/2;
			this.Padding = new Drawing.Margins(m, m, m, m);
		}
		
		public HToolBar(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public override double				DefaultHeight
		{
			//	Retourne la hauteur standard d'une barre.
			get
			{
				return 28;
			}
		}
		
		public override DockStyle			DefaultIconDockStyle
		{
			get
			{
				return DockStyle.Left;
			}
		}
	}
}
