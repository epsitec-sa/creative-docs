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
		
		public IconButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		public IconButton(string name) : this()
		{
			this.IconName = name;
		}
		
		public IconButton(string name, string icon) : this(icon)
		{
			this.Name = name;
			this.IsCommand = true;
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


		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			rect.Inflate(adorner.GeometryToolShapeBounds);
			return rect;
		}


		protected string				iconName;
	}
}
