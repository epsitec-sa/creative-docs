//	Copyright � 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
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

		public IconButton(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}

		public IconButton(Command command)
			: this ()
		{
			this.CommandObject = command;
		}

		public IconButton(Command command, Drawing.Size size, DockStyle dock)
			: this (command)
		{
			this.PreferredSize = size;
			this.PreferredIconSize = size;
			this.Dock = dock;
		}

		public IconButton(string icon)
			: this ()
		{
			this.IconName = icon;
		}

		public IconButton(string command, string icon)
			: this (icon)
		{
			this.CommandObject = Command.Get (command);
		}

		public IconButton(string command, string icon, string name)
			: this (command, icon)
		{
			this.Name = name;
		}


		static IconButton()
		{
			Types.DependencyPropertyMetadata metadataText = Widget.TextProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();
			
			metadataText.MakeNotSerializable ();
			metadataDx.DefineDefaultValue (22.0);
			metadataDy.DefineDefaultValue (22.0);

			Widget.TextProperty.OverrideMetadata (typeof (IconButton), metadataText);
			Visual.PreferredWidthProperty.OverrideMetadata (typeof (IconButton), metadataDx);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (IconButton), metadataDy);
		}


		public Drawing.Size PreferredIconSize
		{
			get
			{
				return this.preferredIconSize;
			}

			set
			{
				if (this.preferredIconSize != value)
				{
					this.preferredIconSize = value;
					this.UpdateText (this.IconName);
				}
			}
		}

		public string PreferredIconLanguage
		{
			get
			{
				return this.preferredIconLanguage;
			}

			set
			{
				if (this.preferredIconLanguage != value)
				{
					this.preferredIconLanguage = value;
					this.UpdateText (this.IconName);
				}
			}
		}

		public string PreferredIconStyle
		{
			get
			{
				return this.preferredIconStyle;
			}

			set
			{
				if (this.preferredIconStyle != value)
				{
					this.preferredIconStyle = value;
					this.UpdateText (this.IconName);
				}
			}
		}

		public override bool HasTextLabel
		{
			get
			{
				return false;
			}
		}

		protected void UpdateText(string iconName)
		{
			//	Met � jour le texte du bouton, qui est un tag <img.../> contenant le nom de l'image
			//	suivi des diff�rentes pr�f�rences (taille et langue).
			if (string.IsNullOrEmpty (iconName))
			{
				this.Text = null;
			}
			else
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder ();

				builder.Append (@"<img src=""");
				builder.Append (iconName);
				builder.Append (@"""");

				if (this.preferredIconSize.Width != 0 && this.preferredIconSize.Height != 0)
				{
					builder.Append (@" dx=""");
					builder.Append (this.preferredIconSize.Width.ToString (System.Globalization.CultureInfo.InvariantCulture));
					builder.Append (@""" dy=""");
					builder.Append (this.preferredIconSize.Height.ToString (System.Globalization.CultureInfo.InvariantCulture));
					builder.Append (@"""");
				}

				if (string.IsNullOrEmpty (this.preferredIconLanguage) == false)
				{
					builder.Append (@" lang=""");
					builder.Append (this.preferredIconLanguage);
					builder.Append (@"""");
				}

				if (string.IsNullOrEmpty (this.preferredIconStyle) == false)
				{
					builder.Append (@" style=""");
					builder.Append (this.preferredIconStyle);
					builder.Append (@"""");
				}

				builder.Append (@"/>");

				this.Text = builder.ToString ();
			}
		}


		public override Drawing.Margins GetShapeMargins()
		{
			if ((this.PaintState&WidgetPaintState.ThreeState) == 0)
			{
				return Widgets.Adorners.Factory.Active.GeometryToolShapeMargins;
			}
			else
			{
				return Widgets.Adorners.Factory.Active.GeometryThreeStateShapeMargins;
			}
		}

		protected override void OnIconNameChanged(string oldIconName, string newIconName)
		{
			base.OnIconNameChanged (oldIconName, newIconName);

			if (string.IsNullOrEmpty (oldIconName) &&
				string.IsNullOrEmpty (newIconName))
			{
				//	Nothing to do. Change is not significant : the text remains
				//	empty if we swap "" for null.
			}
			else
			{
				this.UpdateText (newIconName);
			}
		}

		protected override void OnCommandObjectChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnCommandObjectChanged (e);

			Command command = e.NewValue as Command;

			if (command != null)
			{
				if (command.Statefull)
				{
					this.ButtonStyle = ButtonStyle.ActivableIcon;
				}
				else
				{
					//	TODO: d�finir clairement quelles valeurs DefaultParameter peut prendre

					switch (command.DefaultParameter)
					{
						case "GlyphButton":
							this.ButtonStyle = ButtonStyle.Icon;
							break;

						default:
							this.ButtonStyle = ButtonStyle.ToolItem;
							break;
					}
				}
				
				if ((!ToolTip.HasToolTipText (this)) &&
					(!ToolTip.HasToolTipWidget (this)))
				{
					string description = command.GetDescriptionWithShortcut ();

					if (!string.IsNullOrEmpty (description))
					{
						this.DefineToolTipFromCaption (description);
					}
				}
			}
		}

		protected override void DefineTextFromCaption(string text)
		{
			//	Do nothing - we don't want the text to be associated with the
			//	IconButton, as it is computed automatically anyway.
		}


		public static IconButton CreateSimple(string command, string icon)
		{
			IconButton button = new IconButton (command, icon);

			button.Name = command;

			return button;
		}

		public static IconButton CreateHidden(string command, string icon)
		{
			IconButton button = new IconButton (command, icon);

			button.Visibility = false;
			button.Name = command;

			return button;
		}

		public static IconButton CreateToggle(string command, string icon)
		{
			IconButton button = new IconButton (command, icon);

			button.AutoToggle = true;
			button.Name = command;

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


		protected Drawing.Size preferredIconSize;
		protected string preferredIconLanguage = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
		protected string preferredIconStyle;
	}
}
