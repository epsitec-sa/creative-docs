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


		static Filename()
		{
			double h = Filename.textHeight + Filename.iconHeight + Filename.topMargin;
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

			Rectangle textRect = this.Client.Bounds;
			textRect.Top = textRect.Bottom+Filename.textHeight;

			Rectangle iconRect = this.Client.Bounds;
			iconRect.Top -= Filename.topMargin;
			iconRect.Bottom += Filename.textHeight;

			//	Affiche le texte.
			if (this.textLayout == null)
			{
				this.textLayout = new TextLayout();
			}

			this.textLayout.Alignment = ContentAlignment.MiddleCenter;
			this.textLayout.LayoutSize = textRect.Size;
			this.textLayout.Text = TextLayout.ConvertToTaggedText(this.filename);
			this.textLayout.Paint(textRect.BottomLeft, graphics);

			//	Affiche l'icône.
			if (this.icon != null)
			{
				if (this.iconLayout == null)
				{
					this.iconLayout = new TextLayout();
				}

				this.iconLayout.Alignment = ContentAlignment.MiddleCenter;
				this.iconLayout.LayoutSize = iconRect.Size;
				this.iconLayout.Text = string.Format(@"<img src=""{0}""/>", this.icon);
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
