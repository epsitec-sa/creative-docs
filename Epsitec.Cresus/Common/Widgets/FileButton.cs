//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Filename permet d'afficher un nom de fichier surmonté d'une icône.
	/// </summary>
	public class FileButton : Button
	{
		public FileButton()
		{
			this.ButtonStyle = ButtonStyle.ToolItem;
		}

		public FileButton(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}


		public static double ExtendedHeight
		{
			get
			{
				return FileButton.textHeight + FileButton.iconHeight + FileButton.topMargin;
			}
		}

		public static double CompactHeight
		{
			get
			{
				return 20;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				if (this.displayName != value)
				{
					this.displayName = value;
					this.Invalidate();
				}
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le widget.
			base.PaintBackgroundImplementation(graphics, clipRect);

			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Rectangle iconRect = this.Client.Bounds;
			Rectangle textRect = this.Client.Bounds;

			bool compact;
			string dim;
			if (textRect.Height <= FileButton.CompactHeight)
			{
				compact = true;
				dim = "dx=\"16\" dy=\"16\"";

				iconRect.Width = iconRect.Height;
				textRect.Left = iconRect.Right+2;
				iconRect.Deflate(2);
			}
			else
			{
				compact = false;
				dim = "dx=\"32\" dy=\"32\"";

				iconRect.Top -= FileButton.topMargin;
				iconRect.Bottom += FileButton.textHeight;
				textRect.Top = textRect.Bottom+FileButton.textHeight;
			}

			//	Affiche le texte.
			if (this.textLayout == null)
			{
				this.textLayout = new TextLayout();
			}

			this.textLayout.Alignment = compact ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
			this.textLayout.LayoutSize = textRect.Size;
			this.textLayout.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			this.textLayout.Text = TextLayout.ConvertToTaggedText(this.displayName);
			this.textLayout.Paint(textRect.BottomLeft, graphics);

			//	Affiche l'icône.
			if (!string.IsNullOrEmpty (this.IconUri))
			{
				if (this.iconLayout == null)
				{
					this.iconLayout = new TextLayout();
				}

				this.iconLayout.Alignment = compact ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
				this.iconLayout.LayoutSize = iconRect.Size;
				this.iconLayout.Text = string.Format (@"<img src=""{0}"" {1}/>", this.IconUri, dim);
				this.iconLayout.Paint(iconRect.BottomLeft, graphics);
			}
		}

		static FileButton()
		{
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDy.DefineDefaultValue (FileButton.ExtendedHeight);

			Visual.PreferredHeightProperty.OverrideMetadata (typeof (FileButton), metadataDy);
		}

		
		protected static readonly double	topMargin = 5;
		protected static readonly double	iconHeight = 32;
		protected static readonly double	textHeight = 16;

		private string						displayName;
		private TextLayout					textLayout;
		private TextLayout					iconLayout;
	}
}
