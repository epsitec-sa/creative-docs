namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe IconButton permet de dessiner de petits pictogrammes.
	/// </summary>
	public class IconButton : Button
	{
		public IconButton()
		{
			this.ButtonStyle = ButtonStyle.ToolItem;
		}
		
		// Retourne la largeur standard d'une icône.
		public override double DefaultWidth
		{
			get
			{
				return 22;
			}
		}

		// Retourne la hauteur standard d'une icône.
		public override double DefaultHeight
		{
			get
			{
				return 22;
			}
		}

		public string IconName
		{
			get
			{
				return this.iconName;
			}

			set
			{
				if ( this.iconName != value )
				{
					this.iconName = value;
					this.Text = "<img src=\"..\\..\\" + this.iconName + ".png\"/>";
				}
			}
		}
		
		public override Drawing.Rectangle GetPaintBounds()
		{
			return new Drawing.Rectangle(0, 0, this.clientInfo.width+1, this.clientInfo.height);
		}

		
		protected string				iconName;
	}
}
