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
			Helpers.VisualPropertyMetadata metadataDy = new Helpers.VisualPropertyMetadata(50.0, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);
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


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le widget.
			base.PaintBackgroundImplementation(graphics, clipRect);

			IAdorner adorner = Widgets.Adorners.Factory.Active;

			double h = 20;  // hauteur utilisée pour le nom de fichier

			Rectangle filenameRect = this.Client.Bounds;
			filenameRect.Top = filenameRect.Bottom+h;

			Rectangle iconRect = this.Client.Bounds;
			iconRect.Bottom += h;
			iconRect.Offset(0, -5);

			if (this.filenameLayout == null)
			{
				this.filenameLayout = new TextLayout();
			}

			this.filenameLayout.Alignment = ContentAlignment.MiddleCenter;
			this.filenameLayout.LayoutSize = filenameRect.Size;
			this.filenameLayout.Text = this.filename;
			this.filenameLayout.Paint(filenameRect.BottomLeft, graphics);

			if (this.iconLayout == null)
			{
				this.iconLayout = new TextLayout();
			}

			this.iconLayout.Alignment = ContentAlignment.MiddleCenter;
			this.iconLayout.LayoutSize = iconRect.Size;
			this.iconLayout.Text = string.Format(@"<img src=""{0}""/>", this.icon);
			this.iconLayout.Paint(iconRect.BottomLeft, graphics);
		}


		protected string					filename;
		protected string					icon;
		protected TextLayout				filenameLayout;
		protected TextLayout				iconLayout;
	}
}
