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
			this.InternalState &= ~ InternalState.PossibleContainer;
			
			this.validator = new DataWidgetValidator (this);
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
					this.DetachSource ();
					this.source = value;
					this.AttachSource ();
					this.CreateUI ();
					System.Diagnostics.Debug.WriteLine ("DataSource set to " + value);
				}
			}
		}
		
		
		[Bundle] public Data.Representation		Representation
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
		
		[Bundle] public bool					HasCaption
		{
			get
			{
				return this.has_caption;
			}
			set
			{
				if (this.has_caption != value)
				{
					this.has_caption = value;
					this.CreateUI ();
				}
			}
		}
		
		[Bundle] public double					CaptionWidth
		{
			get
			{
				return this.caption_width;
			}
			set
			{
				if (this.caption_width != value)
				{
					this.caption_width = value;
					this.CreateUI ();
				}
			}
		}
		
		
		public static bool CheckCompatibility(Types.IDataValue data, Data.Representation representation)
		{
			if ((data == null) ||
				(representation == Data.Representation.None))
			{
				return false;
			}
			
			Types.INamedType type      = data.DataType;
			Types.IEnumType  enum_type = type as Types.IEnumType;
			
			if (type == null)
			{
				return false;
			}
			
			representation = DataWidget.GetExactRepresentation (data, representation);
			
			switch (representation)
			{
				case Data.Representation.TextField:
					return true;
				case Data.Representation.NumericUpDown:
					return (type is Types.INumType) || (enum_type != null);
				case Data.Representation.RadioList:
				case Data.Representation.RadioColumns:
				case Data.Representation.RadioRows:
				case Data.Representation.CheckList:
				case Data.Representation.CheckColumns:
				case Data.Representation.CheckRows:
					return (enum_type != null) && (enum_type.IsCustomizable == false);
				case Data.Representation.ComboConstantList:
					return (enum_type != null) && (enum_type.IsCustomizable == false);
				case Data.Representation.ComboEditableList:
					return (enum_type != null) && (enum_type.IsCustomizable == true);
				
				case Data.Representation.StatusLed:
					return (type is Types.INumType) || (enum_type != null);
			}
			
			return false;
		}
		
		
		public override Drawing.Size GetBestFitSize()
		{
			return this.best_fit_size;
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
		
		protected void AttachSource()
		{
			if (this.source != null)
			{
				this.source.Changed += new EventHandler (this.HandleSourceChanged);
			}
		}
		
		protected void DetachSource()
		{
			if (this.source != null)
			{
				this.source.Changed -= new EventHandler (this.HandleSourceChanged);
			}
		}
		
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
			
			this.active_representation = DataWidget.GetExactRepresentation (this.source, this.representation);
			
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
				
				case Data.Representation.CheckList:
					this.CreateUICheck (LayoutMode.None);
					break;
				
				case Data.Representation.CheckColumns:
					this.CreateUICheck (LayoutMode.Columns);
					break;
				
				case Data.Representation.CheckRows:
					this.CreateUICheck (LayoutMode.Rows);
					break;
					
				case Data.Representation.StatusLed:
					this.CreateUIStatusLed ();
					break;
				
				default:
					//	Forme de représentation inconnue : remplace pas "aucune".
					
					this.active_representation = Data.Representation.None;
					break;
			}
		}
		
		
		protected virtual void CreateUITextField()
		{
			this.widget_layout_mode = LayoutMode.None;
			this.widget_container   = this;
			
			TextField  text_field = new TextField (this.widget_container);
			StaticText caption    = null;
			
			if (this.has_caption)
			{
				caption = new StaticText (this.widget_container, this.source.Caption);
				
				caption.ResourceManager = this.ResourceManager;
				caption.Text            = this.source.Caption;
				caption.Anchor          = AnchorStyles.TopLeft;
				caption.Width           = this.caption_width;
				caption.Margins   = new Drawing.Margins (0, 0, 0, 0);
			}
			
			text_field.TabIndex      = 1;
			text_field.Anchor        = AnchorStyles.Top | AnchorStyles.LeftAndRight;
			text_field.Margins = new Drawing.Margins (this.has_caption ? this.caption_width : 0, 0, 0, 0);
			
			Widget.BaseLineAlign (text_field, caption);

			this.DefineBestFitSize (this.caption_width + text_field.GetBestFitSize ().Width, text_field.MinHeight);

			this.MinSize = new Drawing.Size (this.caption_width + text_field.MinWidth, text_field.MinHeight);
			
			Engine.BindWidget (this.source, text_field);
		}
		
		protected virtual void CreateUINumericUpDown()
		{
			this.widget_layout_mode = LayoutMode.None;
			this.widget_container   = this;
			
			TextFieldUpDown text_field = new TextFieldUpDown (this.widget_container);
			StaticText      caption    = null;
			
			if (this.has_caption)
			{
				caption = new StaticText (this.widget_container, this.source.Caption);
				
				caption.ResourceManager = this.ResourceManager;
				caption.Text            = this.source.Caption;
				caption.Anchor          = AnchorStyles.TopLeft;
				caption.Width           = this.caption_width;
				caption.Margins   = new Drawing.Margins (0, 0, 0, 0);
			}
			
			text_field.TabIndex      = 1;
			text_field.Anchor        = AnchorStyles.Top | AnchorStyles.LeftAndRight;
			text_field.Margins = new Drawing.Margins (this.has_caption ? this.caption_width : 0, 0, 0, 0);
			
			Widget.BaseLineAlign (text_field, caption);

			this.DefineBestFitSize (this.caption_width + text_field.GetBestFitSize ().Width, text_field.MinHeight);

			this.MinSize = new Drawing.Size (this.caption_width + text_field.MinWidth, text_field.MinHeight);
			
			Engine.BindWidget (this.source, text_field);
		}
		
		protected virtual void CreateUIRadio(LayoutMode layout_mode)
		{
			this.widget_layout_mode = layout_mode;
			
			Types.IEnumType enum_type = this.source.DataType as Types.IEnumType;
			
			if (enum_type == null)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot setup radio buttons based on type {0}.", this.source.DataType.Name));
			}
			
			Types.IEnumValue[] enum_values = enum_type.Values;
			
			//	C'est une énumération pour laquelle nous devons trouver les légendes des
			//	divers éléments.
			
			if (this.has_caption)
			{
				this.widget_container = new GroupBox (this);
				this.widget_container.ResourceManager = this.ResourceManager;
				this.widget_container.Dock = DockStyle.Fill;
				this.widget_container.Text = this.source.Caption;
				this.widget_container.Padding = new Drawing.Margins (4, 0, 4, 4);
			}
			else
			{
				this.widget_container = this;
			}
			
			RadioButton radio_0 = null;
			
			foreach (Types.IEnumValue enum_value in enum_values)
			{
				if (enum_value.IsHidden)
				{
					continue;
				}
				
				RadioButton button = new RadioButton (this.widget_container);
				
				string caption = enum_value.Caption;
				
				if (caption == null)
				{
					caption = enum_value.Name;
				}
				
				button.ResourceManager = this.ResourceManager;
				
				button.Group    = "Group";
				button.Index    = enum_value.Rank;
				button.Text     = caption;
				button.TabIndex = 1;
				button.Dock     = DockStyle.Top;
				button.MinSize  = button.GetBestFitSize ();
				
				if (radio_0 == null)
				{
					radio_0 = button;
				}
				else
				{
					//	On pourrait ajouter un espace vertical entre les boutons radio; les
					//	informations de distance pourraient venir de IGuideAlignHint, par
					//	exemple...

					//button.Margins = new Drawing.Margins (0, 0, 2, 0);
				}
			}
			
			this.CreateUILayout ();
			
			if (radio_0 != null)
			{
				Engine.BindWidget (this.source, radio_0);
			}
		}
		
		protected virtual void CreateUICheck(LayoutMode layout_mode)
		{
			this.widget_layout_mode = layout_mode;
			
			Types.IEnumType enum_type = this.source.DataType as Types.IEnumType;
			
			if (enum_type == null)
			{
				throw new System.InvalidOperationException (string.Format ("Cannot setup check buttons based on type {0}.", this.source.DataType.Name));
			}
			
			Types.IEnumValue[] enum_values = enum_type.Values;
			
			//	C'est une énumération pour laquelle nous devons trouver les légendes des
			//	divers éléments.
			
			if (this.has_caption)
			{
				this.widget_container = new GroupBox (this);
				this.widget_container.ResourceManager = this.ResourceManager;
				this.widget_container.Dock = DockStyle.Fill;
				this.widget_container.Text = this.source.Caption;
				this.widget_container.Padding = new Drawing.Margins (4, 0, 4, 4);
			}
			else
			{
				this.widget_container = this;
			}
			
			int tab_index = 1;
			
			foreach (Types.IEnumValue enum_value in enum_values)
			{
				if ((enum_value.IsHidden) ||
					(enum_value.Rank <= 0))
				{
					continue;
				}
				
				CheckButton button = new CheckButton (this.widget_container);
				
				string caption = enum_value.Caption;
				
				if (caption == null)
				{
					caption = enum_value.Name;
				}
				
				button.ResourceManager = this.ResourceManager;
				
				button.Index    = enum_value.Rank;
				button.Name     = enum_value.Name;
				button.Text     = caption;
				button.TabIndex = tab_index++;
				button.Dock     = DockStyle.Top;
				button.MinSize  = button.GetBestFitSize ();
				
				Engine.BindWidget (this.source, button);
			}
			
			this.CreateUILayout ();
		}
		
		protected virtual void CreateUIStatusLed()
		{
			this.widget_layout_mode = LayoutMode.None;
			this.widget_container   = this;
			
			CheckButton button  = new CheckButton (this.widget_container);
			StaticText  caption = null;
			
			if (this.has_caption)
			{
				caption = new StaticText (this.widget_container, this.source.Caption);
				
				caption.ResourceManager = this.ResourceManager;
				caption.Text            = this.source.Caption;
				caption.Anchor          = AnchorStyles.TopLeft;
				caption.Width           = this.caption_width;
				caption.Margins   = new Drawing.Margins (0, 0, 0, 0);
			}
			
			button.TabIndex      = 1;
			button.Anchor        = AnchorStyles.Top | AnchorStyles.LeftAndRight;
			button.Margins = new Drawing.Margins (this.has_caption ? this.caption_width : 0, 0, 0, 0);
			
			Widget.BaseLineAlign (button, caption);

			this.DefineBestFitSize (this.caption_width + button.GetBestFitSize ().Width, button.MinHeight);

			this.MinSize = new Drawing.Size (this.caption_width + button.MinWidth, button.MinHeight);
			
			Engine.BindWidget (this.source, button);
		}
		
		
		protected virtual void CreateUILayout()
		{
			Drawing.Size cell_size = this.GetCellSize ();
			
			int n = this.widget_container.Children.Count;
			
			double table_width  = cell_size.Width;
			double table_height = cell_size.Height * n;
			
			switch (this.widget_layout_mode)
			{
				case LayoutMode.None:
					break;
				
				case LayoutMode.Rows:
				case LayoutMode.Columns:
					for (int columns = 1; columns <= n; columns++)
					{
						int lines = (n + columns - 1) / columns;
						
						double dx = cell_size.Width * columns;
						double dy = cell_size.Height * lines;
						
						if (dx > dy)
						{
							break;
						}
						
						table_width  = dx;
						table_height = dy;
					}
					break;
			}
			
			double frame_width  = this.widget_container.InternalPadding.Width  + this.widget_container.Padding.Width;
			double frame_height = this.widget_container.InternalPadding.Height + this.widget_container.Padding.Height;
			
			this.DefineBestFitSize (table_width + frame_width, table_height + frame_height);
			
			this.MinSize = new Drawing.Size (frame_width + cell_size.Width, frame_height + cell_size.Height);
			
			this.UpdateInternalLayout ();
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
				
				this.Children.Clear ();
			}
		}
		
		
		protected static Data.Representation GetExactRepresentation(Types.IDataValue data, Data.Representation representation)
		{
			if (representation == Data.Representation.Automatic)
			{
				return Engine.FindDefaultRepresentation (data);
			}
			
			return representation;
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
				
				double width  = this.widget_container.Client.Bounds.Width  - this.widget_container.InternalPadding.Width  - this.widget_container.Padding.Width;
				double height = this.widget_container.Client.Bounds.Height - this.widget_container.InternalPadding.Height - this.widget_container.Padding.Height;
				
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
							widget.Bounds = new Drawing.Rectangle (this.widget_container.Client.Bounds.Left   + this.widget_container.InternalPadding.Left   + this.widget_container.Padding.Left + ox,
								/**/                               this.widget_container.Client.Bounds.Bottom + this.widget_container.InternalPadding.Bottom + this.widget_container.Padding.Bottom + height - cell_size.Height - oy,
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
							widget.Bounds = new Drawing.Rectangle (this.widget_container.Client.Bounds.Left   + this.widget_container.InternalPadding.Left   + this.widget_container.Padding.Left + ox,
								/**/                               this.widget_container.Client.Bounds.Bottom + this.widget_container.InternalPadding.Bottom + this.widget_container.Padding.Bottom + height - cell_size.Height - oy,
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
		
		protected virtual void DefineBestFitSize(double dx, double dy)
		{
			Drawing.Size size = new Drawing.Size (dx, dy);
			
			if (this.size_change_count < 2)
			{
				this.Size = size;
				this.size_change_count = 2;
			}
			
			this.best_fit_size = size;
		}
		
		
		protected override void ManualArrange()
		{
			this.UpdateInternalLayout ();
			base.ManualArrange ();
		}
		
		protected override void OnBindingInfoChanged()
		{
			base.OnBindingInfoChanged ();
			System.Diagnostics.Debug.WriteLine ("Binding info set to " + this.BindingInfo);
		}
		
		protected override void OnSizeChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			base.OnSizeChanged (e);
			
			this.size_change_count++;
		}

		protected virtual  void OnDataChanged()
		{
			if (this.DataChanged != null)
			{
				this.DataChanged (this);
			}
		}
		
		
		protected override bool ShouldSerializeValidator(IValidator validator)
		{
			if (this.validator == validator)
			{
				//	On ne sérialise pas le validateur interne qui est là juste pour
				//	s'assurer que la valeur est OK.
				
				return false;
			}
			
			return base.ShouldSerializeValidator (validator);
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
				graphics.RenderSolid (Drawing.Color.FromRgb (1, 0, 0));
			}
		}
		
		
		private void HandleSourceChanged(object sender)
		{
			this.OnDataChanged ();
		}
		
		
		public event Support.EventHandler		DataChanged;
		
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
		protected double						caption_width     = 80;
		protected bool							has_caption       = true;
		protected int							size_change_count = 0;
		protected Drawing.Size					best_fit_size = Drawing.Size.Empty;
		protected DataWidgetValidator			validator;
	}
}
