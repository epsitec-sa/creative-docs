namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe VToolBar permet de réaliser des tool bars verticales.
	/// </summary>
	public class VToolBar : AbstractToolBar
	{
		public VToolBar()
		{
			this.direction = Direction.Left;
			
			double m = (this.DefaultWidth-this.defaultButtonWidth)/2;
			this.Padding = new Drawing.Margins(m, m, m, m);
		}
		
		public VToolBar(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public override double				DefaultWidth
		{
			//	Retourne la largeur standard d'une barre.
			get
			{
				return 28;
			}
		}
		
		public override DockStyle			DefaultIconDockStyle
		{
			get
			{
				return DockStyle.Top;
			}
		}
	}
}
