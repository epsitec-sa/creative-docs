namespace Epsitec.Common.Widgets
{
	using BundleAttribute = Epsitec.Common.Support.BundleAttribute;
	
	/// <summary>
	/// La classe IconButton permet de dessiner de petits pictogrammes, en
	/// particulier pour remplir une ToolBar.
	/// </summary>
	public class IconButton : Button
	{
		public IconButton()
		{
			this.ButtonStyle = ButtonStyle.ToolItem;
		}
		
		public IconButton(string name) : this()
		{
			this.IconName = name;
		}
		
		
		public override double DefaultWidth
		{
			// Retourne la largeur standard d'une icône.
			get
			{
				return 22;
			}
		}

		public override double DefaultHeight
		{
			// Retourne la hauteur standard d'une icône.
			get
			{
				return 22;
			}
		}

		
		[ Bundle("icon") ] public string IconName
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
					this.Text = @"<img src=""" + this.iconName + @"""/>";
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
