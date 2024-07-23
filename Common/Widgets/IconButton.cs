/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


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
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public IconButton(Command command)
            : this()
        {
            this.CommandObject = command;
        }

        public IconButton(Command command, Drawing.Size size, DockStyle dock)
            : this(command)
        {
            this.PreferredSize = size;
            this.PreferredIconSize = size;
            this.Dock = dock;
        }

        public IconButton(string icon)
            : this()
        {
            this.IconUri = icon;
        }

        public IconButton(string command, string icon)
            : this(icon)
        {
            this.CommandObject = Command.Get(command);
        }

        public IconButton(string command, string icon, string name)
            : this(command, icon)
        {
            this.Name = name;
        }

        static IconButton()
        {
            Types.DependencyPropertyMetadata metadataText =
                Widget.TextProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDx =
                Visual.PreferredWidthProperty.DefaultMetadata.Clone();
            Types.DependencyPropertyMetadata metadataDy =
                Visual.PreferredHeightProperty.DefaultMetadata.Clone();

            metadataText.MakeNotSerializable();
            metadataDx.DefineDefaultValue(22.0);
            metadataDy.DefineDefaultValue(22.0);

            Widget.TextProperty.OverrideMetadata(typeof(IconButton), metadataText);
            Visual.PreferredWidthProperty.OverrideMetadata(typeof(IconButton), metadataDx);
            Visual.PreferredHeightProperty.OverrideMetadata(typeof(IconButton), metadataDy);
        }

        public Drawing.Size PreferredIconSize
        {
            get { return this.preferredIconSize; }
            set
            {
                if (this.preferredIconSize != value)
                {
                    this.preferredIconSize = value;
                    this.UpdateText(this.IconUri);
                }
            }
        }

        public string PreferredIconLanguage
        {
            get { return this.preferredIconLanguage; }
            set
            {
                if (this.preferredIconLanguage != value)
                {
                    this.preferredIconLanguage = value;
                    this.UpdateText(this.IconUri);
                }
            }
        }

        public string PreferredIconStyle
        {
            get { return this.preferredIconStyle; }
            set
            {
                if (this.preferredIconStyle != value)
                {
                    this.preferredIconStyle = value;
                    this.UpdateText(this.IconUri);
                }
            }
        }

        public override bool HasTextLabel
        {
            get { return false; }
        }

        protected void UpdateText(string iconUri)
        {
            //	Met à jour le texte du bouton, qui est un tag <img.../> contenant le nom de l'image
            //	suivi des différentes préférences (taille et langue).

            this.Text = IconButton.GetSourceForIconText(
                iconUri,
                this.PreferredIconSize,
                this.PreferredIconLanguage,
                this.PreferredIconStyle
            );
        }

        public static string GetSourceForIconText(
            string iconUri,
            Drawing.Size preferredIconSize,
            string preferredIconLanguage,
            string preferredIconStyle
        )
        {
            if (string.IsNullOrEmpty(iconUri))
            {
                return null;
            }
            else
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();

                builder.Append(@"<img src=""");
                builder.Append(iconUri);
                builder.Append(@"""");

                if (preferredIconSize.Width != 0 && preferredIconSize.Height != 0)
                {
                    builder.Append(@" dx=""");
                    builder.Append(
                        preferredIconSize.Width.ToString(
                            System.Globalization.CultureInfo.InvariantCulture
                        )
                    );
                    builder.Append(@""" dy=""");
                    builder.Append(
                        preferredIconSize.Height.ToString(
                            System.Globalization.CultureInfo.InvariantCulture
                        )
                    );
                    builder.Append(@"""");
                }

                if (string.IsNullOrEmpty(preferredIconLanguage) == false)
                {
                    builder.Append(@" lang=""");
                    builder.Append(preferredIconLanguage);
                    builder.Append(@"""");
                }

                if (string.IsNullOrEmpty(preferredIconStyle) == false)
                {
                    builder.Append(@" style=""");
                    builder.Append(preferredIconStyle);
                    builder.Append(@"""");
                }

                builder.Append(@"/>");

                return builder.ToString();
            }
        }

        public override Drawing.Margins GetShapeMargins()
        {
            if ((this.PaintState & WidgetPaintState.ThreeState) == 0)
            {
                return Widgets.Adorners.Factory.Active.GeometryToolShapeMargins;
            }
            else
            {
                return Widgets.Adorners.Factory.Active.GeometryThreeStateShapeMargins;
            }
        }

        protected override void OnIconUriChanged(string oldiconUri, string newiconUri)
        {
            base.OnIconUriChanged(oldiconUri, newiconUri);

            if (string.IsNullOrEmpty(oldiconUri) && string.IsNullOrEmpty(newiconUri))
            {
                //	Nothing to do. Change is not significant : the text remains
                //	empty if we swap "" for null.
            }
            else
            {
                this.UpdateText(newiconUri);
            }
        }

        protected override void OnCommandObjectChanged(Types.DependencyPropertyChangedEventArgs e)
        {
            base.OnCommandObjectChanged(e);

            Command command = e.NewValue as Command;

            if (command != null)
            {
                if (command.Statefull)
                {
                    this.ButtonStyle = ButtonStyle.ActivableIcon;
                }
                else
                {
                    //	TODO: définir clairement quelles valeurs DefaultParameter peut prendre

                    switch (command.CommandParameters["ButtonStyle"])
                    {
                        case "GlyphButton":
                            this.ButtonStyle = ButtonStyle.Icon;
                            break;

                        default:
                            this.ButtonStyle = ButtonStyle.ToolItem;
                            break;
                    }
                }
#if false
				if ((!ToolTip.HasToolTipText (this)) &&
					(!ToolTip.HasToolTipWidget (this)))
				{
					string description = command.GetDescriptionWithShortcut ();

					if (!string.IsNullOrEmpty (description))
					{
						this.DefineToolTipFromCaption (description);
					}
				}
#endif
            }
        }

        protected override string GetCaptionDescription(Types.Caption caption)
        {
            if (caption.HasDescription)
            {
                return caption.Description;
            }
            else
            {
                return caption.DefaultLabel;
            }
        }

        protected override void DefineTextFromCaption(string text)
        {
            //	Do nothing - we don't want the text to be associated with the
            //	IconButton, as it is computed automatically anyway.
        }

        public static IconButton CreateSimple(string command, string icon)
        {
            IconButton button = new IconButton(command, icon);

            button.Name = command;

            return button;
        }

        public static IconButton CreateHidden(string command, string icon)
        {
            IconButton button = new IconButton(command, icon);

            button.Visibility = false;
            button.Name = command;

            return button;
        }

        public static IconButton CreateToggle(string command, string icon)
        {
            IconButton button = new IconButton(command, icon);

            button.AutoToggle = true;
            button.Name = command;

            return button;
        }

        public static IconButton CreateSimple(System.Enum command, string icon)
        {
            return IconButton.CreateSimple(command.ToString(), icon);
        }

        public static IconButton CreateHidden(System.Enum command, string icon)
        {
            return IconButton.CreateHidden(command.ToString(), icon);
        }

        public static IconButton CreateToggle(System.Enum command, string icon)
        {
            return IconButton.CreateToggle(command.ToString(), icon);
        }

        protected Drawing.Size preferredIconSize;
        protected string preferredIconLanguage = System
            .Globalization
            .CultureInfo
            .CurrentCulture
            .TwoLetterISOLanguageName;
        protected string preferredIconStyle;
    }
}
