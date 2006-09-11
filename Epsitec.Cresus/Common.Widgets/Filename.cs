//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Filename permet d'afficher un nom de fichier surmonté d'une icône.
	/// </summary>
	public class Filename : Button
	{
		public Filename()
		{
			this.ButtonStyle = ButtonStyle.ToolItem;
		}

		public Filename(Widget embedder) : this()
		{
			this.SetEmbedder (embedder);
		}


		public static double ExtendedHeight
		{
			get
			{
				return Filename.textHeight + Filename.iconHeight + Filename.topMargin;
			}
		}

		public static double CompactedHeight
		{
			get
			{
				return 20;
			}
		}

		static Filename()
		{
			double h = Filename.ExtendedHeight;
			Helpers.VisualPropertyMetadata metadataDy = new Helpers.VisualPropertyMetadata(h, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
			Visual.PreferredHeightProperty.OverrideMetadata(typeof(Filename), metadataDy);
		}

		
		public string FilenameValue
		{
			get
			{
				return this.filename;
			}
			set
			{
				if (this.filename != value)
				{
					this.filename = value;
					this.Invalidate();
				}
			}
		}

		public string IconValue
		{
			get
			{
				return this.icon;
			}
			set
			{
				if (this.icon != value)
				{
					this.icon = value;
					this.Invalidate();
				}
			}
		}

		public Image ImageValue
		{
			get
			{
				return this.image;
			}
			set
			{
				if (this.image != value)
				{
					this.image = value;
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
			if (textRect.Height <= Filename.CompactedHeight)
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

				iconRect.Top -= Filename.topMargin;
				iconRect.Bottom += Filename.textHeight;
				textRect.Top = textRect.Bottom+Filename.textHeight;
			}

			//	Affiche le texte.
			if (this.textLayout == null)
			{
				this.textLayout = new TextLayout();
			}

			this.textLayout.Alignment = compact ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
			this.textLayout.LayoutSize = textRect.Size;
			this.textLayout.BreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
			this.textLayout.Text = TextLayout.ConvertToTaggedText(this.filename);
			this.textLayout.Paint(textRect.BottomLeft, graphics);

			//	Affiche l'icône.
			if (this.icon != null)
			{
				if (this.iconLayout == null)
				{
					this.iconLayout = new TextLayout();
				}

				this.iconLayout.Alignment = compact ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleCenter;
				this.iconLayout.LayoutSize = iconRect.Size;
				this.iconLayout.Text = string.Format(@"<img src=""{0}"" {1}/>", this.icon, dim);
				this.iconLayout.Paint(iconRect.BottomLeft, graphics);
			}

			if (this.image != null)
			{
				double m = (iconRect.Width-iconRect.Height)/2;
				iconRect.Left += m;
				iconRect.Right -= m;
				graphics.Align(ref iconRect);
				graphics.PaintImage(this.image, iconRect);
			}
		}


		protected static readonly double	topMargin = 5;
		protected static readonly double	iconHeight = 32;
		protected static readonly double	textHeight = 16;

		protected string					filename;
		protected string					icon;
		protected Image						image;
		protected TextLayout				textLayout;
		protected TextLayout				iconLayout;
	}
}
