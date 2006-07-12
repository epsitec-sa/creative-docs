using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// La classe PropertyPanel est la classe de base pour tous les panels des propri�t�s.
	/// </summary>
	public class PropertyPanel : Widget
	{
		public PropertyPanel()
		{
			this.Margins = new Margins(0, 0, 0, 4);
			this.Padding = new Margins(1, 1, 1, 1);

			Widget header = new Widget(this);
			header.PreferredHeight = this.extendedZoneWidth;
			header.Dock = DockStyle.Top;

			this.fixIcon = new StaticText(header);
			this.fixIcon.ContentAlignment = ContentAlignment.MiddleCenter;
			this.fixIcon.PreferredSize = new Size(this.extendedZoneWidth, this.extendedZoneWidth);
			this.fixIcon.Dock = DockStyle.Left;

			this.label = new StaticText(header);
			this.label.Dock = DockStyle.Fill;
			this.label.Margins = new Margins(4, 4, 0, 0);

			this.extendedButton = new GlyphButton(header);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.GlyphShape = GlyphShape.ArrowDown;
			this.extendedButton.AutoFocus = false;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);
			this.extendedButton.TabIndex = 0;
			this.extendedButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			this.extendedButton.PreferredSize = new Size(this.extendedZoneWidth-4, this.extendedZoneWidth-4);
			this.extendedButton.Margins = new Margins(2, 1, 2, 2);
			this.extendedButton.Dock = DockStyle.Right;
			//?ToolTip.Default.SetToolTip(this.extendedButton, Res.Strings.Panel.Abstract.Extend);

			this.container = new Widget(this);
			this.container.Dock = DockStyle.Fill;
			this.container.Padding = new Margins(this.extendedZoneWidth+4, this.extendedZoneWidth+4, 4, 4);
			this.container.Visibility = this.isExtendedSize;

			this.grid = new GridLayoutEngine();
			this.grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Proportional)));
			this.grid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(50, GridUnitType.Absolute)));
			LayoutEngine.SetLayoutEngine(this.container, this.grid);

			this.Entered += new MessageEventHandler(this.HandleMouseEntered);
			this.Exited += new MessageEventHandler(this.HandleMouseExited);
		}
		
		public PropertyPanel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.extendedButton.Clicked -= new MessageEventHandler(this.ExtendedButtonClicked);
				this.Entered -= new MessageEventHandler(this.HandleMouseEntered);
				this.Exited -= new MessageEventHandler(this.HandleMouseExited);
			}
			
			base.Dispose(disposing);
		}


		public string Icon
		{
			set
			{
				this.fixIcon.Text = Misc.Image(value);
			}
		}

		public string Title
		{
			get
			{
				return this.label.Text;
			}
			set
			{
				this.label.Text = value;
			}
		}

		public double DataColumnWidth
		{
			get
			{
				GridLength length = this.grid.ColumnDefinitions[1].Width;
				return length.Value;
			}
			set
			{
				this.grid.ColumnDefinitions[1].Width = new GridLength(value, GridUnitType.Absolute);
			}
		}

		public double RowsSpacing
		{
			get
			{
				return this.rowsSpacing;
			}
			set
			{
				this.rowsSpacing = value;
			}
		}

		public bool IsExtendedSize
		{
			get
			{
				return this.isExtendedSize;
			}
			set
			{
				if (this.isExtendedSize != value)
				{
					this.isExtendedSize = value;

					this.extendedButton.GlyphShape = this.isExtendedSize ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
					this.container.Visibility = this.isExtendedSize;
					this.OnExtendedSize();
				}
			}
		}

		public int Rank
		{
			get
			{
				return this.rank;
			}
			set
			{
				this.rank = value;
			}
		}

		public void AddPlaceHolder(Placeholder placeholder)
		{
			this.grid.RowDefinitions.Add(new RowDefinition());
			int row = this.grid.RowDefinitions.Count-1;
			
			if (row > 0)
			{
				this.grid.RowDefinitions[row].TopBorder = this.rowsSpacing;
			}

			GridLayoutEngine.SetColumn(placeholder, 0);
			GridLayoutEngine.SetRow(placeholder, row);
			GridLayoutEngine.SetColumnSpan(placeholder, 2);
			this.container.Children.Add(placeholder);
		}

		
		private void HandleMouseEntered(object sender, MessageEventArgs e)
		{
			//	La souris est entr�e dans le panneau.
		}

		private void HandleMouseExited(object sender, MessageEventArgs e)
		{
			//	La souris est sortie du panneau.
		}

		//	Le bouton pour �tendre/r�duire le panneau a �t� cliqu�.
		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			this.IsExtendedSize = !this.isExtendedSize;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			if (this.backgroundIntensity < 1.0)
			{
				graphics.AddFilledRectangle(rect);
				Color cap = adorner.ColorCaption;
				Color color = Color.FromAlphaRgb(1.0-this.backgroundIntensity, 0.5+cap.R*0.5, 0.5+cap.G*0.5, 0.5+cap.B*0.5);
				graphics.RenderSolid(color);
			}

			rect.Deflate(0.5, 0.5);
			graphics.AddLine(rect.Left, rect.Bottom-0.5, rect.Left, rect.Top-0.5);
			graphics.AddLine(rect.Left+this.extendedZoneWidth+1, rect.Bottom-0.5, rect.Left+this.extendedZoneWidth+1, rect.Top-0.5);
			graphics.AddLine(rect.Right-this.extendedZoneWidth+1, rect.Bottom-0.5, rect.Right-this.extendedZoneWidth+1, rect.Top-0.5);
			graphics.AddLine(rect.Right, rect.Bottom-0.5, rect.Right, rect.Top-0.5);
			graphics.AddLine(rect.Left-0.5, rect.Top, rect.Right+0.5, rect.Top);
			graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		protected virtual void OnExtendedSize()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler("ExtendedSize");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler ExtendedSize
		{
			add
			{
				this.AddUserEventHandler("ExtendedSize", value);
			}
			remove
			{
				this.RemoveUserEventHandler("ExtendedSize", value);
			}
		}

		
		protected double					backgroundIntensity = 1.0;
		protected double					extendedZoneWidth = 16;
		protected StaticText				label;
		protected StaticText				fixIcon;
		protected GlyphButton				extendedButton;
		protected Widget					container;
		protected GridLayoutEngine			grid;
		protected double					rowsSpacing = -1;
		protected bool						isExtendedSize = false;
		protected int						rank;
	}
}
