//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 09/12/2003

namespace Epsitec.Common.Widgets.Layouts
{
	using System.Collections;
	
	/// <summary>
	/// La classe Grid implémente un Layout Manager basé sur une grille.
	/// </summary>
	public class Grid
	{
		public Grid()
		{
			this.columns = new Grid.ColumnCollection (this);
			this.widget_wrapper = new Design.WidgetWrapper ();
		}
		
		
		public Widget					Root
		{
			get { return this.root; }
			set
			{
				if (this.root != value)
				{
					if (this.root == null)
					{
						this.AttachRootWidget (value);
					}
					else
					{
						throw new System.InvalidOperationException ("Root cannot be changed once set.");
					}
				}
			}
		}
		
		public double[]					MinColumnWidths
		{
			get
			{
				this.Update ();
				double[] dx = new double[this.verticals.Length-1];
				for (int i = 1; i < this.verticals.Length; i++)
				{
					dx[i-1] = this.verticals[i].min_x;
				}
				return dx;
			}
		}
		
		public double[]					CurrentColumnWidths
		{
			get
			{
				this.Update ();
				double[] dx = new double[this.verticals.Length-1];
				for (int i = 1; i < this.verticals.Length; i++)
				{
					dx[i-1] = this.verticals[i].x_live - this.verticals[i-1].x_live;
				}
				return dx;
			}
		}
		
		public Grid.ColumnCollection	Columns
		{
			get { return this.columns; }
		}
		
		
		public double					DesiredWidth
		{
			get { return this.desired_width; }
			set
			{
				if (this.desired_width != value)
				{
					this.desired_width = value;
					this.UpdateGeometry ();
				}
			}
		}
		
		public double					CurrentWidth
		{
			get
			{
				this.Update ();
				return this.current_width;
			}
		}
		
		public double					DesiredHeight
		{
			get { return this.desired_height; }
			set
			{
				if (this.desired_height != value)
				{
					this.desired_height = value;
					this.UpdateGeometry ();
				}
			}
		}
		
		public double					CurrentHeight
		{
			get
			{
				this.Update ();
				return this.current_height;
			}
		}
		
		
		public void Update()
		{
			if (this.is_dirty)
			{
				this.Rebuild ();
			}
		}
		
		public void Invalidate()
		{
			this.is_dirty = true;
		}
		
		
		protected void Rebuild()
		{
			//	Reconstruit toute l'information sur les widgets.
			
			int max_index = 0;
			int num_lines;
			int num_verticals;
			int num_horizontals;
			
			this.AnalyseVerticals (this.root, 0, ref max_index, out num_lines);
			
			num_verticals   = max_index + 1;
			num_horizontals = num_lines + 1;
			
			this.verticals   = new Vertical[num_verticals];
			this.horizontals = new Horizontal[num_horizontals];
			
			for (int i = 0; i < num_verticals; i++)
			{
				this.verticals[i] = new Vertical ();
			}
			
			this.FillListVerticals (this.Root, 0);
			this.UpdateMinVerticals ();
			
			this.is_dirty = false;
			
			//	Pour n colonnes, il doit forcément y avoir n+1 séparations verticales.
			
			System.Diagnostics.Debug.Assert (num_verticals == this.columns.Count+1);
			
			this.UpdateGeometry ();
		}
		
		protected void UpdateGeometry()
		{
			if (this.is_dirty)
			{
				//	On ne va pas mettre à jour la géométrie si la structure n'est pas à
				//	jour !
				
				return;
			}
			
			this.UpdateColumnsGeometry ();
			this.UpdateLinesGeometry ();
			this.LayoutWidgets (this.root, 0, 0, this.current_height);
			
			this.root.Size = new Drawing.Size (this.current_width, this.current_height);
		}
		
		protected void UpdateColumnsGeometry()
		{
			System.Diagnostics.Debug.Assert (this.is_dirty == false);
			
			//	Met à jour les colonnes, en calculant la largeur de chaque colonne, ainsi que
			//	son élasticité résiduelle. Les frontières verticales sont positionnées aux
			//	bonnes coordonnées.
			
			for (int i = 0; i < this.columns.Count; i++)
			{
				this.columns[i].Reset (this.is_dragging_column);
			}
			
			again:
			{
				double total_width = 0;
				double total_k     = 0;
				
				for (int i = 0; i < this.columns.Count; i++)
				{
					total_width += this.columns[i].dx_live;
					total_k     += this.columns[i].k_live;
				}
				
				double space = this.desired_width - total_width;
				double x     = 0;
				
				if (total_k == 0)
				{
					//	S'il n'y a aucun élément élastique, on pourrait simplifier les calculs
					//	ci-après, mais c'est plus simple d'utiliser le même code, sachant que
					//	"width = column.dx_live" sera toujours vrai.
					
					total_k = 1;
				}
				
				for (int i = 0; i < this.columns.Count; i++)
				{
					Column   column   = this.columns[i];
					Vertical vertical = this.verticals[i+1];
					
					double width = column.dx_live + column.k_live * space / total_k;
					
					if (width < vertical.min_x)
					{
						column.dx_live = vertical.min_x;
						column.k_live  = 0;
						goto again;
					}
					
					if (width > vertical.max_x)
					{
						column.dx_live = vertical.max_x;
						column.k_live  = 0;
						goto again;
					}
					
					column.x_live  = x;
					x += width;
					vertical.x_live = x;
				}
				
				this.current_width = x;
				
				for (int i = 0; i < this.columns.Count; i++)
				{
					this.columns[i].dx_live = this.verticals[i+1].x_live - this.verticals[i].x_live;
				}
			}
		}
		
		protected void UpdateLinesGeometry()
		{
			this.current_height = this.desired_height;
			
			//	TODO: calculer la hauteur réelle...
		}
		
		protected void LayoutWidgets(Widget root, int left_index, double x_offset, double y_offset)
		{
			double max_dy = 0;
			
			int lines = 1;
			int count = 0;
			
			Widget.WidgetCollection widgets = root.Children;
			
			this.horizontals[0] = new Horizontal (y_offset);
			
			if (this.line_gap_index == 0)
			{
				y_offset -= this.line_gap_height;
				this.horizontals[0].gap = this.line_gap_height;
			}
			
			for (int i = 0; i < widgets.Count; i++)
			{
				Widget widget = widgets[i] as Widget;
				
				if (widget.Dock == DockStyle.Layout)
				{
					int arg1, arg2;
					LayoutFlags flags = widget.LayoutFlags;
					
					widget.GetLayoutArgs (out arg1, out arg2);
					
					//	Les index des séparations sont relatives au parent; il faut donc ajuster en
					//	fonction de l'origine du parent.
					
					arg1 += left_index;
					arg2 += left_index;
					
					if ((flags & LayoutFlags.StartNewLine) != 0)
					{
						//	Si c'est le premier widget, implicitement, c'est une nouvelle ligne, laquelle
						//	est déjà comptée. Il ne faut donc changer de ligne que si on a déjà des widgets
						//	avant le nôtre.
						
						if (count > 0)
						{
							this.horizontals[lines] = new Horizontal (y_offset - max_dy);
							
							y_offset -= max_dy;
							max_dy = 0;
							
							if (this.line_gap_index == lines)
							{
								y_offset -= this.line_gap_height;
								this.horizontals[lines].gap = this.line_gap_height;
							}
							
							lines++;
						}
					}
					
					Drawing.Margins margins = widget.LayoutMargins;
					
					double x1 = this.verticals[arg1].x_live + margins.Left;
					double x2 = this.verticals[arg2].x_live - margins.Right;
					double dx = x2 - x1;
					double dy = widget.Height - margins.Top - margins.Bottom;
					
					widget.Bounds = new Drawing.Rectangle (x1 - x_offset, y_offset - margins.Top - dy, dx, dy);
					
					if (max_dy < dy)
					{
						max_dy = dy;
					}
					
					if ((flags & LayoutFlags.IncludeChildren) != 0)
					{
						//	TODO: faire en sorte que ça fonctionne (horizontals n'est pas prévu pour)
						//this.LayoutWidgets (widget, arg1, x1, dy);
					}
					
					count++;
				}
			}
			
			this.horizontals[lines] = new Horizontal (y_offset - max_dy);
		}
		
		
		protected void AttachRootWidget(Widget root)
		{
			this.root = root;
			this.Invalidate ();
			
			this.root.LayoutChanged   += new EventHandler (this.HandleRootLayoutChanged);
			this.root.ChildrenChanged += new EventHandler (this.HandleRootChildrenChanged);
			this.root.PreparePaint    += new EventHandler (this.HandleRootPreparePaint);
			this.root.PaintForeground += new PaintEventHandler (this.HandleRootPaintForeground);
			this.root.PreProcessing   += new MessageEventHandler (this.HandleRootPreProcessing);
			
			this.root.SetPropagationModes (Widget.PropagationModes.UpChildrenChanged, true, true);
		}
		
		
		protected void AnalyseVerticals(Widget root, int left_index, ref int max_index, out int lines)
		{
			int count;
			
			lines = 1;
			count = 0;
			
			Widget.WidgetCollection widgets = root.Children;
			
			for (int i = 0; i < widgets.Count; i++)
			{
				Widget widget = widgets[i] as Widget;
				
				if (widget.Dock == DockStyle.Layout)
				{
					int arg1, arg2;
					LayoutFlags flags = widget.LayoutFlags;
				
					widget.GetLayoutArgs (out arg1, out arg2);
					
					//	Les index des séparations sont relatives au parent; il faut donc ajuster en
					//	fonction de l'origine du parent.
					
					arg1 += left_index;
					arg2 += left_index;
					
					if (arg1 > max_index) max_index = arg1;
					if (arg2 > max_index) max_index = arg2;
					
					if ((flags & LayoutFlags.IncludeChildren) != 0)
					{
						int sub_lines;
						this.AnalyseVerticals (widget, arg1, ref max_index, out sub_lines);
					}
					
					if ((flags & LayoutFlags.StartNewLine) != 0)
					{
						//	Si c'est le premier widget, implicitement, c'est une nouvelle ligne, laquelle
						//	est déjà comptée. Il ne faut donc changer de ligne que si on a déjà des widgets
						//	avant le nôtre.
						
						if (count > 0)
						{
							lines++;
						}
					}
					
					count++;
				}
			}
		}
		
		protected void FillListVerticals(Widget root, int left_index)
		{
			//	Insère les widgets dans les listes respectives des séparations verticales
			//	qui sont à leur gauche. Cela permettra, dans une deuxième passe, de déterminer
			//	les largeurs des colonnes.
			
			Widget.WidgetCollection widgets = root.Children;
			
			for (int i = 0; i < widgets.Count; i++)
			{
				Widget widget = widgets[i] as Widget;
				
				if (widget.Dock == DockStyle.Layout)
				{
					LayoutFlags flags = widget.LayoutFlags;
					int   start_index = widget.LayoutArg1 + left_index;
					
					System.Diagnostics.Debug.Assert (start_index > -1);
					System.Diagnostics.Debug.Assert (start_index < this.verticals.Length);
					
					this.verticals[start_index].list.Add (widget);
					
					if ((flags & LayoutFlags.IncludeChildren) != 0)
					{
						this.FillListVerticals (widget, start_index);
					}
				}
			}
		}
		
		protected void UpdateMinVerticals()
		{
			for (int i = 0; i < this.verticals.Length; i++)
			{
				this.verticals[i].min_x = 0;
			}
			
			//	Calcule les distances minimales depuis le bord gauche, lequel sert
			//	temporairement de référence absolue :
			
			for (int i = 0; i < this.verticals.Length; i++)
			{
				Vertical vertical = this.verticals[i];
				
				for (int j = 0; j < vertical.list.Count; j++)
				{
					Widget          widget  = vertical.list[j] as Widget;
					Drawing.Margins margins = widget.LayoutMargins;
					
					double next_x = widget.MinSize.Width + vertical.min_x + margins.Left + margins.Right;
					int    next_i = widget.LayoutArg2 - widget.LayoutArg1 + i;
					
					System.Diagnostics.Debug.Assert (next_i > 0);
					System.Diagnostics.Debug.Assert (next_i < this.verticals.Length);
					System.Diagnostics.Debug.Assert (next_i > i);
					
					Vertical right = this.verticals[next_i];
					
					//	Met à jour la position minimale de la séparation qui suit.
					
					if (right.min_x < next_x)
					{
						right.min_x = next_x;
					}
				}
			}
			
			//	Transforme les distances absolues et distances relatives d'une
			//	séparation à l'autre :
			
			for (int i = this.verticals.Length-1; i > 0; i--)
			{
				this.verticals[i].min_x -= this.verticals[i-1].min_x;
			}
		}
		
		
		protected Widget DetectWidget(Drawing.Point pos)
		{
			Widget widget = this.root.FindChild (pos);
			
			if (widget != null)
			{
				while (widget != this.root)
				{
					if (widget.Dock == DockStyle.Layout)
					{
						return widget;
					}
					
					widget = widget.Parent;
				}
			}
			
			return null;
		}
		
		protected bool FindBestColumns(double x, int num_columns, out int index_left, out int index_right)
		{
			index_left = 0;
			index_right = 0;
			
			bool found = false;
			
			if ((num_columns & 1) == 1)
			{
				//	Nombre de colonnes impair: tant que [x] se trouve dans une colonne,
				//	on prend celle-ci comme référence.
				
				for (int i = 0; i < this.columns.Count; i++)
				{
					double x_left  = this.columns[i].x_live;
					double x_right = x_left + this.columns[i].dx_live;
					
					if ((x >= x_left) &&
						(x <= x_right))
					{
						found = true;
						index_left = i - num_columns / 2;
						break;
					}
				}
			}
			else
			{
				//	Nombre de colonnes pair: tant que [x] se trouve dans la moitié gauche d'une colonne,
				//	on prend celle-ci comme référence, si [x] se trouve dans la moitié droite, on prend
				//	la suivante comme référence.
				
				for (int i = 0; i < this.columns.Count; i++)
				{
					double x_left  = this.columns[i].x_live;
					double x_mid   = x_left + this.columns[i].dx_live / 2;
					double x_right = x_left + this.columns[i].dx_live;
					
					if ((x >= x_left) &&
						(x <= x_mid))
					{
						found = true;
						index_left = i - num_columns / 2;
						break;
					}
					if ((x >= x_mid) &&
						(x <= x_right))
					{
						found = true;
						index_left = i - num_columns / 2 + 1;
						break;
					}
				}
			}
			
			index_left  = System.Math.Max (0, index_left);
			index_right = System.Math.Min (this.columns.Count, index_left + num_columns);
			
			return found;
		}
		
		
		private void HandleRootLayoutChanged(object sender)
		{
			System.Diagnostics.Debug.Assert (this.root == sender);
			this.UpdateGeometry ();
		}
		
		private void HandleRootChildrenChanged(object sender)
		{
			this.Invalidate ();
		}
		
		private void HandleRootPreparePaint(object sender)
		{
			System.Diagnostics.Debug.Assert (this.root == sender);
			this.Update ();
		}
		
		private void HandleRootPaintForeground(object sender, PaintEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.root == sender);
			System.Diagnostics.Debug.Assert (this.is_dirty == false);
			
			Drawing.Graphics graphics = e.Graphics;
			
			if (this.hilite_box.IsEmpty == false)
			{
				graphics.AddFilledRectangle (this.hilite_box);
				graphics.RenderSolid (Drawing.Color.FromARGB (0.5, 1, 0, 0));
			}
			
			double dy = this.root.Client.Height;
			
			for (int i = 1; i < this.verticals.Length; i++)
			{
				double x = this.verticals[i].x_live - 1;
				
				graphics.AddLine (x+0.5, 0, x+0.5, dy);
				graphics.AddFilledRectangle (x-1,    0, 3, 3);
				graphics.AddFilledRectangle (x-1, dy-3, 3, 3);
			}
			
			graphics.RenderSolid (Drawing.Color.FromARGB (0.5, 1.0, 0.0, 0.0));
		}
		
		private void HandleRootPreProcessing(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.root == sender);
			
			e.Suppress |= this.ProcessMessage (e.Message);
		}
		
		
		internal bool ProcessMessage(Message message)
		{
			if (message.IsMouseType)
			{
				this.Update ();
				
				Drawing.Point mouse = this.root.MapRootToClient (message.Cursor);
				
				if (this.is_dragging_column)
				{
					if (message.Type == MessageType.MouseUp)
					{
						this.ColumnDraggingEnd (mouse);
					}
					else
					{
						this.ColumnDraggingMove (mouse);
						return true;
					}
				}
				
				if (this.is_dragging_widget)
				{
					if (message.Type == MessageType.MouseUp)
					{
						this.WidgetDraggingEnd (mouse);
					}
					else
					{
						this.WidgetDraggingMove (mouse);
						return true;
					}
				}
				
				switch (message.Type)
				{
					case MessageType.MouseLeave:
						this.mouse_cursor = MouseCursor.Default;
						break;
					
					case MessageType.MouseDown:
						if (this.hot_column_index > 0)
						{
							this.ColumnDraggingBegin (mouse);
						}
						else if (this.hot_widget != null)
						{
							this.widget_wrapper.Attach (this.hot_widget);
							this.mouse_cursor = MouseCursor.AsSizeAll;
							this.WidgetDraggingBegin (mouse);
						}
						else if (this.hot_designer)
						{
							this.WidgetDraggingBegin (mouse);
						}
						break;
					
					case MessageType.MouseMove:
						this.mouse_cursor     = MouseCursor.Default;
						this.hot_column_index = 0;
						this.hot_widget       = null;
						this.hot_designer     = false;
						
						if (this.HandleDesignerDetection (mouse, message.Cursor) ||
							this.HandleColumnDetection (mouse) ||
							this.HandleWidgetDetection (mouse))
						{
						}
						break;
				}
			}
			
			this.root.Window.MouseCursor = this.mouse_cursor;
			
			return true;
		}
		
		
		protected bool HandleDesignerDetection(Drawing.Point mouse, Drawing.Point root_mouse)
		{
			if (this.widget_wrapper.Widget != null)
			{
				Widget        widget = this.widget_wrapper.Widget;
				Drawing.Point pos    = widget.MapRootToClient (root_mouse);
				
				if (widget.Client.Bounds.Contains (pos))
				{
					this.mouse_cursor = MouseCursor.AsSizeAll;
					this.hot_designer = true;
					return true;
				}
			}
			
			return false;
		}
		
		protected bool HandleWidgetDetection(Drawing.Point mouse)
		{
			this.hot_widget = this.DetectWidget (mouse);
			return this.hot_widget != null;
		}
		
		protected bool HandleColumnDetection(Drawing.Point mouse)
		{
			for (int i = 1; i < this.verticals.Length; i++)
			{
				double x = this.verticals[i].x_live;
				
				if ((mouse.X <= x+1) &&
					(mouse.X >= x-1))
				{
					this.mouse_cursor = MouseCursor.AsVSplit;
					
					this.hot_column_index  = i;
					this.hot_column_offset = mouse.X - x;
					
					return true;
				}
			}
			
			return false;
		}
		

		protected void WidgetDraggingBegin(Drawing.Point mouse)
		{
			Widget select = this.widget_wrapper.Widget;
			Drawing.Point center = select.Bounds.Center;
			
			this.is_dragging_widget = true;
			
			this.hot_designer_origin  = mouse;
			this.hot_designer_offset  = mouse - center;
			this.hot_designer_columns = select.LayoutArg2 - select.LayoutArg1;
			
			this.widget_dummy = new StaticText ();
			
			this.widget_dummy.Size        = select.Size;
			this.widget_dummy.BackColor   = Drawing.Color.FromRGB (1.0, 0.8, 0.8);
			this.widget_dummy.Dock        = select.Dock;
			this.widget_dummy.LayoutFlags = select.LayoutFlags;
			this.widget_dummy.MinSize     = select.MinSize;
			this.widget_dummy.MaxSize     = select.MaxSize;
			this.widget_dummy.Name        = "DummyWidgetPlaceholder";
			
			this.widget_dummy.SetLayoutArgs (select.LayoutArg1, select.LayoutArg2);
			
			select.Parent.Children.Replace (select, this.widget_dummy);
			
			this.Invalidate ();
			this.Update ();
			
			this.root.Invalidate ();
		}
		
		protected void WidgetDraggingMove(Drawing.Point mouse)
		{
			Drawing.Point center = mouse - this.hot_designer_offset;
			
			if (this.FindBestColumns (center.X, this.hot_designer_columns, out this.hot_designer_col1, out this.hot_designer_col2))
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("[{0} -> {1}/{2}]", center.X, this.hot_designer_col1, this.hot_designer_col2));
			}
		}
		
		protected void WidgetDraggingEnd(Drawing.Point mouse)
		{
			this.is_dragging_widget = false;
			
			this.widget_dummy.Parent.Children.Replace (this.widget_dummy, this.widget_wrapper.Widget);
			
			this.Invalidate ();
			this.Update ();
			
			this.root.Invalidate ();
		}
		
		
		protected void ColumnDraggingBegin(Drawing.Point mouse)
		{
			this.is_dragging_column = true;
			
			for (int j = 0; j < this.columns.Count; j++)
			{
				this.columns[j].dx_init = this.columns[j].dx_live;
			}
			
			this.UpdateGeometry ();
			this.root.Invalidate ();
		}
		
		protected void ColumnDraggingMove(Drawing.Point mouse)
		{
			double x  = mouse.X - this.hot_column_offset;
			double dx = x - this.verticals[this.hot_column_index-1].x_live;
			
			this.columns[this.hot_column_index-1].dx_init = dx;
			
			this.UpdateGeometry ();
			this.root.Invalidate ();
			this.mouse_cursor = MouseCursor.AsVSplit;
		}
		
		protected void ColumnDraggingEnd(Drawing.Point mouse)
		{
			this.is_dragging_column = false;
			this.desired_width = this.verticals[this.columns.Count].x_live;
			
			this.UpdateGeometry ();
			this.root.Invalidate ();
		}
		
		
#if false
		{
	
				
				int i1, i2;
				
				if (this.FindBestColumns (mouse_x, 1, out i1, out i2))
				{
					double x1 = this.verticals[i1].x_live;
					double x2 = this.verticals[i2].x_live;
					
					this.hilite_box = new Drawing.Rectangle (x1, 0, x2-x1, this.root.Client.Height);
				}
				else
				{
					this.hilite_box = Drawing.Rectangle.Empty;
				}
				
				this.line_gap_index  = -1;
				this.line_gap_height = 0;
				
				for (int i = 1; i < this.horizontals.Length; i++)
				{
					double y1_top = this.horizontals[i-1].y_live + 3;
					double y1_bot = this.horizontals[i-1].y_live - 3;
					
					if ((mouse_y <= y1_top) &&
						(mouse_y >= y1_bot))
					{
						this.line_gap_index = i-1;
						this.line_gap_height = 20;
						this.hilite_box = Drawing.Rectangle.Intersection (this.hilite_box, new Drawing.Rectangle (0, this.horizontals[i-1].y_live - 20, this.root.Client.Width, 20));
						
						break;
					}
				}
				
				this.UpdateGeometry ();
				this.root.Invalidate ();
				
				if (this.hot_column_index >= 0)
				{
					this.root.Window.MouseCursor = MouseCursor.Default;
					this.hot_column_index = -1;
				}
			}
			
			e.Message.Consumer = this.root;
			e.Suppress = true;
		}
#endif
		
		
		#region Class: Vertical & Horizontal
		protected class Vertical
		{
			public Vertical()
			{
				this.list   = new ArrayList ();
				this.min_x  = 0;
				this.max_x  = 1000000;
				this.x_live = 0;
			}
			
			
			internal ArrayList			list;
			
			internal double				min_x;
			internal double				max_x;
			
			internal double				x_live;
		}
		
		protected class Horizontal
		{
			public Horizontal(double y)
			{
				this.y_live = y;
				this.gap    = 0;
			}
			
			internal double				y_live;
			internal double				gap;
		}
		#endregion
		
		#region Class: Column & ColumnCollection
		public class Column
		{
			public Column(double width, double elasticity)
			{
				this.dx_init = width;
				this.dx_live = width;
				this.k_init  = elasticity;
				this.k_live  = elasticity;
			}
			
			
			public double				Elasticity
			{
				get { return this.k_init; }
			}
			
			public double				InitialWidth
			{
				get { return this.dx_init; }
			}
			
			public double				CurrentWidth
			{
				get { return this.dx_live; }
			}
			
			
			internal void Reset(bool suppress_elasticity)
			{
				this.dx_live = this.dx_init;
				this.k_live  = suppress_elasticity ? 0 : this.k_init;
				this.x_live  = 0;
			}
			
			
			internal double				dx_init;
			internal double				dx_live;
			internal double				k_init;			//	élasticité initiale
			internal double				k_live;			//	élasticité active
			internal double				x_live;
		}
		
		public class ColumnCollection : IList, System.IDisposable
		{
			public ColumnCollection(Grid host)
			{
				this.host = host;
				this.list = new ArrayList ();
			}
			
			
			public Grid.Column			this[int index]
			{
				get
				{
					if (index == -1) return null;
					return this.list[index] as Grid.Column;
				}
			}
			
			
			#region IDisposable Members
			public void Dispose()
			{
				this.Dispose (true);
				System.GC.SuppressFinalize (this);
			}
			#endregion
			
			#region IList Members
			public bool					IsReadOnly
			{
				get { return false; }
			}

			public bool					IsFixedSize
			{
				get { return this.list.IsFixedSize; }
			}
			
			object						IList.this[int index]
			{
				get { return this.list[index]; }
				set
				{
					if (this.list[index] != value)
					{
						this.list[index] = value;
						this.InvalidateHost ();
					}
				}
			}

			
			public void RemoveAt(int index)
			{
				this.list.RemoveAt (index);
				this.InvalidateHost ();
			}

			public void Insert(int index, object value)
			{
				this.list.Insert (index, value);
				this.InvalidateHost ();
			}

			public void Remove(object value)
			{
				this.list.Remove (value);
				this.InvalidateHost ();
			}

			public bool Contains(object value)
			{
				return this.list.Contains (value);
			}

			public void Clear()
			{
				this.list.Clear ();
				this.InvalidateHost ();
			}

			public int IndexOf(object value)
			{
				return this.list.IndexOf (value);
			}

			public int Add(object value)
			{
				int index = this.list.Add (value);
				this.InvalidateHost ();
				return index;
			}
			#endregion
			
			#region ICollection Members
			public bool					IsSynchronized
			{
				get { return this.list.IsSynchronized; }
			}
			
			public int					Count
			{
				get { return this.list.Count; }
			}
			
			public void CopyTo(System.Array array, int index)
			{
				this.list.CopyTo (array, index);
			}
			
			public object SyncRoot
			{
				get { return this.list.SyncRoot; }
			}
			#endregion
			
			#region IEnumerable Members
			public IEnumerator GetEnumerator()
			{
				return this.list.GetEnumerator ();
			}
			#endregion
			
			protected virtual void Dispose(bool disposing)
			{
				if (disposing)
				{
					this.list.Clear ();
					this.list = null;
				}
			}
			
			protected virtual void InvalidateHost()
			{
				if (this.host != null)
				{
					this.host.Invalidate ();
				}
			}
			
			
			private Grid				host;
			private ArrayList			list;
		}
		#endregion
		
		
		protected Widget				root;
		
		protected Grid.ColumnCollection	columns;
		protected Grid.Vertical[]		verticals;
		protected Grid.Horizontal[]		horizontals;
		
		protected double				desired_width;
		protected double				desired_height;
		protected double				current_width;
		protected double				current_height;
		
		protected bool					is_dirty;
		
		protected bool					is_dragging_column;
		protected bool					is_dragging_widget;
		
		protected int					hot_column_index = -1;
		protected double				hot_column_offset;
		protected Widget				hot_widget;
		protected bool					hot_designer;
		protected Drawing.Point			hot_designer_origin;
		protected Drawing.Point			hot_designer_offset;
		protected int					hot_designer_columns;
		protected int					hot_designer_col1;
		protected int					hot_designer_col2;
		
		protected Design.WidgetWrapper	widget_wrapper;
		protected Widget				widget_dummy;
		protected MouseCursor			mouse_cursor;
		
		protected Drawing.Rectangle		hilite_box;
		
		protected int					line_gap_index;
		protected double				line_gap_height;
	}
}
