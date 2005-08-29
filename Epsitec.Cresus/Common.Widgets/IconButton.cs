//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		
		
		public override double					DefaultWidth
		{
			// Retourne la largeur standard d'une icône.
			get
			{
				return 22;
			}
		}

		public override double					DefaultHeight
		{
			// Retourne la hauteur standard d'une icône.
			get
			{
				return 22;
			}
		}

		
		[Bundle ("Icon")] public string			IconName
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
					
					if ( this.iconName == null ||
						 this.iconName == ""   )
					{
						this.Text = null;
					}
					else
					{
						this.Text = @"<img src=""" + this.iconName + @"""/>";
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
		
		
		public static IconButton CreateSimple(string command, string icon)
		{
			IconButton button = new IconButton (command, icon);
			
			button.Name = Support.CommandDispatcher.ExtractCommandName (command);
			
			return button;
		}
		
		public static IconButton CreateHidden(string command, string icon)
		{
			IconButton button = new IconButton (command, icon);
			
			button.SetVisible (false);
			button.Name = Support.CommandDispatcher.ExtractCommandName (command);
			
			return button;
		}
		
		public static IconButton CreateToggle(string command, string icon)
		{
			IconButton button = new IconButton (command, icon);
			
			button.InternalState |= InternalState.AutoToggle;
			button.Name = Support.CommandDispatcher.ExtractCommandName (command);
			
			return button;
		}
		
		
		public static IconButton CreateSimple(System.Enum command, string icon)
		{
			return IconButton.CreateSimple (command.ToString (), icon);
		}
		
		public static IconButton CreateHidden(System.Enum command, string icon)
		{
			return IconButton.CreateHidden (command.ToString (), icon);
		}
		
		public static IconButton CreateToggle(System.Enum command, string icon)
		{
			return IconButton.CreateToggle (command.ToString (), icon);
		}
		
		
		protected string						iconName;
	}
}
