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
		
		public IconButton(string icon) : this()
		{
			this.IconName = icon;
		}
		
		public IconButton(string command, string icon) : this(icon)
		{
			this.Command = command;
		}
		
		public IconButton(string command, string icon, string name) : this(command, icon)
		{
			this.Name = name;
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
				return this.icon_name;
			}

			set
			{
				if (this.icon_name != value)
				{
					this.icon_name = value;
					
					if ((this.icon_name == null) ||
						(this.icon_name == ""))
					{
						this.Text = null;
					}
					else
					{
						this.Text = @"<img src=""" + this.icon_name + @"""/>";
					}
				}
			}
		}


		public override Drawing.Rectangle GetShapeBounds()
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;
			Drawing.Rectangle rect = this.Client.Bounds;
			rect.Inflate(adorner.GeometryToolShapeBounds);
			return rect;
		}


		protected string				icon_name;
	}
}
