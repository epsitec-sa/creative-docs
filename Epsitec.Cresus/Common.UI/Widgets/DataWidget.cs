//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Widgets
{
	/// <summary>
	/// La classe DataWidget génère dynamiquement des widgets internes, de manière
	/// à offrir l'accès interactif à une donnée IDataValue.
	/// </summary>
	public class DataWidget : AbstractGroup, ISelfBindingWidget
	{
		public DataWidget()
		{
		}
		
		public DataWidget(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public Types.IDataValue					DataSource
		{
			get
			{
				return this.source;
			}
			set
			{
				if (this.source != value)
				{
					this.source = value;
					this.CreateUI ();
				}
			}
		}
		
		public Data.Representation				Representation
		{
			get
			{
				return this.representation;
			}
			set
			{
				if (this.representation != value)
				{
					this.representation = value;
					this.CreateUI ();
				}
			}
		}
		
		
		#region ISelfBindingWidget Members
		public bool BindWidget(Epsitec.Common.Types.IDataValue source)
		{
			System.Diagnostics.Debug.Assert (this.source == null);
			System.Diagnostics.Debug.Assert (source != null);
			
			this.DataSource = source;
			
			return true;
		}
		#endregion
		
		protected virtual bool CreateUI()
		{
			if (this.HasChildren)
			{
				this.DisposeUI ();
			}
			
			if (this.source == null)
			{
				return false;
			}
			
			this.active_representation = this.GetExactRepresentation ();
			
			this.CreateUIFromRepresentation ();
			
			return true;
		}
		
		protected virtual void CreateUIFromRepresentation()
		{
			switch (this.active_representation)
			{
				case Data.Representation.None:
					//	Aucune représentation n'a été sélectionnée. On va vivre avec et nous allons
					//	peindre nous-même le widget de manière à représenter ce fait.
					break;

				case Data.Representation.TextField:
					this.CreateUITextField ();
					break;
				
				case Data.Representation.NumericUpDown:
					this.CreateUINumericUpDown ();
					break;
				
				case Data.Representation.RadioList:
					this.CreateUIRadio (LayoutMode.None);
					break;
				
				case Data.Representation.RadioColumns:
					this.CreateUIRadio (LayoutMode.Columns);
					break;
				
				case Data.Representation.RadioRows:
					this.CreateUIRadio (LayoutMode.Rows);
					break;
				
				default:
					//	Forme de représentation inconnue : remplace pas "aucune".
					
					this.active_representation = Data.Representation.None;
					break;
			}
		}
		
		protected virtual void CreateUITextField()
		{
			this.widget_container = this;
			
			StaticText caption    = new StaticText (this.widget_container, this.source.Caption);
			TextField  text_field = new TextField (this.widget_container);
			
			caption.Text          = this.source.Caption;
			caption.Anchor        = AnchorStyles.TopAndBottom | AnchorStyles.Left;
			caption.Width         = this.caption_width;
			caption.AnchorMargins = new Drawing.Margins (0, 0, 0, 0);
			
			text_field.TabIndex      = 1;
			text_field.Anchor        = AnchorStyles.Top | AnchorStyles.LeftAndRight;
			text_field.AnchorMargins = new Drawing.Margins (this.caption_width, 0, 0, 0);
			
			this.MinSize = new Drawing.Size (this.caption_width + text_field.MinSize.Width, text_field.MinSize.Height);
			this.Size    = this.MinSize;
			
			Engine.BindWidget (this.source, text_field);
		}
		
		protected virtual void CreateUINumericUpDown()
		{
			this.widget_container = this;
			
			StaticText       caption    = new StaticText (this.widget_container, this.source.Caption);
			TextFieldUpDown  text_field = new TextFieldUpDown (this.widget_container);
			
			caption.Text          = this.source.Caption;
			caption.Anchor        = AnchorStyles.TopAndBottom | AnchorStyles.Left;
			caption.Width         = this.caption_width;
			caption.AnchorMargins = new Drawing.Margins (0, 0, 0, 0);
			
			text_field.TabIndex      = 1;
			text_field.Anchor        = AnchorStyles.Top | AnchorStyles.LeftAndRight;
			text_field.AnchorMargins = new Drawing.Margins (this.caption_width, 0, 0, 0);
			
			this.MinSize = new Drawing.Size (this.caption_width + text_field.MinSize.Width, text_field.MinSize.Height);
			this.Size    = this.MinSize;
			
			Engine.BindWidget (this.source, text_field);
		}
		
		protected virtual void CreateUIRadio(LayoutMode layout_mode)
		{
			this.widget_layout_mode = layout_mode;
			
			Types.IEnum enum_type = this.source.DataType as Types.IEnum;
			
			if (enum_type == null)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot setup radio buttons based on type {0}.", this.source.DataType.Name));
			}
			
			Types.IEnumValue[] enum_values = enum_type.Values;
			
			//	C'est une énumération pour laquelle nous devons trouver les légendes des
			//	divers éléments.
			
			this.widget_container = new GroupBox (this);
			this.widget_container.Dock = DockStyle.Fill;
			this.widget_container.Text = this.source.Caption;
			this.widget_container.DockPadding = new Drawing.Margins (4, 0, 4, 4);
			
			RadioButton radio_0 = null;
			
			foreach (Types.IEnumValue enum_value in enum_values)
			{
				RadioButton button = new RadioButton (this.widget_container, "Group", enum_value.Rank);
				
				string caption = enum_value.Caption;
				
				if (caption == null)
				{
					caption = enum_value.Name;
				}
				
				button.Text     = caption;
				button.TabIndex = 1;
				button.Dock     = DockStyle.Top;
				button.MinSize  = button.GetBestFitSize ();
				
				if (radio_0 == null)
				{
					radio_0 = button;
				}
			}
			
			this.UpdateInternalLayout ();
			
			if (radio_0 != null)
			{
				Engine.BindWidget (this.source, radio_0);
			}
		}
		
		protected virtual void DisposeUI()
		{
			if (this.widget_container != null)
			{
				if (this.widget_container != this)
				{
					this.widget_container.Dispose ();
				}
				
				this.widget_container = null;
			}
		}
		
		
		protected virtual Data.Representation GetExactRepresentation()
		{
			if (this.representation == Data.Representation.Automatic)
			{
				return Engine.FindDefaultRepresentation (this.source);
			}
			
			return this.representation;
		}
		
		protected virtual Drawing.Size GetCellSize()
		{
			System.Diagnostics.Debug.Assert (this.widget_container != null);
			
			double max_dx = 0;
			double max_dy = 0;
			
			foreach (Widget cell in this.widget_container.Children)
			{
				Drawing.Size min_size = cell.MinSize;
				
				if (min_size.Width  > max_dx) max_dx = min_size.Width;
				if (min_size.Height > max_dy) max_dy = min_size.Height;
			}
			
			return new Drawing.Size (max_dx, max_dy);
		}
		
		protected virtual void UpdateInternalLayout()
		{
			if ((this.widget_container != null) &&
				(this.widget_container.HasChildren) &&
				(this.widget_layout_mode != LayoutMode.None))
			{
				Drawing.Size cell_size = this.GetCellSize ();
				
				cell_size.Width += 4;
				
				int num_columns = 0;
				int num_rows    = 0;
				int num_total   = this.widget_container.Children.Count;
				
				int row    = 0;
				int column = 0;
				
				double width  = this.widget_container.InnerBounds.Width  - this.widget_container.DockPadding.Width;
				double height = this.widget_container.InnerBounds.Height - this.widget_container.DockPadding.Height;
				
				double ox = 0;
				double oy = 0;
				
				switch (this.widget_layout_mode)
				{
					case LayoutMode.Rows:
						num_columns = System.Math.Max (1, (int) (width / cell_size.Width));
						num_rows    = (num_total + num_columns - 1) / num_columns;
						
						foreach (Widget widget in this.widget_container.Children)
						{
							widget.Dock   = DockStyle.None;
							widget.Anchor = AnchorStyles.None;
							widget.Bounds = new Drawing.Rectangle (this.widget_container.InnerBounds.Left + this.widget_container.DockPadding.Left + ox,
								/**/                               this.widget_container.InnerBounds.Bottom + this.widget_container.DockPadding.Bottom + height - cell_size.Height - oy,
								/**/                               cell_size.Width, cell_size.Height);
							
							ox += cell_size.Width;
							column++;
							
							if (column >= num_columns)
							{
								column = 0;
								ox     = 0;
								oy    += cell_size.Height;
								row   += 1;
							}
						}
						
						break;
					
					case LayoutMode.Columns:
						num_rows    = System.Math.Max (1, (int) (height / cell_size.Height));
						num_columns = (num_total + num_rows - 1) / num_rows;
						
						foreach (Widget widget in this.widget_container.Children)
						{
							widget.Dock   = DockStyle.None;
							widget.Anchor = AnchorStyles.None;
							widget.Bounds = new Drawing.Rectangle (this.widget_container.InnerBounds.Left + this.widget_container.DockPadding.Left + ox,
								/**/                               this.widget_container.InnerBounds.Bottom + this.widget_container.DockPadding.Bottom + height - cell_size.Height - oy,
								/**/                               cell_size.Width, cell_size.Height);
							
							oy += cell_size.Height;
							row++;
							
							if (row >= num_rows)
							{
								row     = 0;
								oy      = 0;
								ox     += cell_size.Width;
								column += 1;
							}
						}
						
						break;
				}
			}
		}
		
		
		protected override void OnLayoutChanged()
		{
			this.UpdateInternalLayout ();
			base.OnLayoutChanged ();
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			base.PaintBackgroundImplementation (graphics, clip_rect);
			
			if (this.active_representation == Data.Representation.None)
			{
				Drawing.Rectangle bounds = this.Client.Bounds;
				bounds.Deflate (0.5);
				
				graphics.LineWidth = 1.0;
				graphics.AddRectangle (bounds);
				graphics.AddLine (bounds.BottomLeft, bounds.TopRight);
				graphics.AddLine (bounds.TopLeft, bounds.BottomRight);
				graphics.RenderSolid (Drawing.Color.FromRGB (1, 0, 0));
			}
		}
		
		protected enum LayoutMode
		{
			None,
			Rows,
			Columns
		}
		

		
		protected Types.IDataValue				source;
		protected Data.Representation			representation;
		protected Data.Representation			active_representation;
		
		protected LayoutMode					widget_layout_mode;
		protected AbstractGroup					widget_container;
		protected double						caption_width = 80;
	}
}
