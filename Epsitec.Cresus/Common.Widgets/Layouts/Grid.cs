//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 09/12/2003

namespace Epsitec.Common.Widgets.Layouts
{
	using System.Collections;
	
	/// <summary>
	/// La classe Grid impl�mente un Layout Manager bas� sur une grille.
	/// </summary>
	public class Grid
	{
		public Grid()
		{
			this.columns = new Grid.ColumnCollection (this);
			this.designer = new Design.WidgetWrapper ();
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
				if (this.freeze_count == 0)
				{
					this.Rebuild ();
				}
			}
		}
		
		public void Invalidate()
		{
			this.is_dirty = true;
		}
		
		
		protected void FreezeEnter()
		{
			System.Threading.Interlocked.Increment (ref this.freeze_count);
		}
		
		protected void FreezeRelease()
		{
			System.Threading.Interlocked.Decrement (ref this.freeze_count);
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
			
			//	Pour n colonnes, il doit forc�ment y avoir n+1 s�parations verticales.
			
			System.Diagnostics.Debug.Assert (num_verticals == this.columns.Count+1);
			
			this.UpdateGeometry ();
		}
		
		protected void UpdateGeometry()
		{
			if (this.is_dirty)
			{
				//	On ne va pas mettre � jour la g�om�trie si la structure n'est pas �
				//	jour !
				
				return;
			}
			
			this.UpdateColumnsGeometry ();
			this.UpdateLinesGeometry ();
			this.LayoutWidgets (this.root, 0, 0, this.current_height);
			
			this.root.Size = new Drawing.Size (this.current_width, this.current_height);
			this.root.Invalidate ();
		}
		
		protected void UpdateColumnsGeometry()
		{
			System.Diagnostics.Debug.Assert (this.is_dirty == false);
			
			//	Met � jour les colonnes, en calculant la largeur de chaque colonne, ainsi que
			//	son �lasticit� r�siduelle. Les fronti�res verticales sont positionn�es aux
			//	bonnes coordonn�es.
			
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
					//	S'il n'y a aucun �l�ment �lastique, on pourrait simplifier les calculs
					//	ci-apr�s, mais c'est plus simple d'utiliser le m�me code, sachant que
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
			
			//	TODO: calculer la hauteur r�elle...
		}
		
		protected void UpdateLineGap()
		{
			if (this.hot_designer_insert)
			{
				this.line_gap_index  = this.hot_designer_line;
				this.line_gap_height = this.designer.OriginalBounds.Height;
			}
			else
			{
				this.line_gap_index  = -1;
				this.line_gap_height = 0;
			}
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
					
					//	Les index des s�parations sont relatives au parent; il faut donc ajuster en
					//	fonction de l'origine du parent.
					
					arg1 += left_index;
					arg2 += left_index;
					
					if ((flags & LayoutFlags.StartNewLine) != 0)
					{
						//	Si c'est le premier widget, implicitement, c'est une nouvelle ligne, laquelle
						//	est d�j� compt�e. Il ne faut donc changer de ligne que si on a d�j� des widgets
						//	avant le n�tre.
						
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
					
					//	Ins�re le widget dans la ligne en cours.
					
					this.horizontals[lines-1].list.Add (widget);
					
					if (max_dy < dy)
					{
						max_dy = dy;
					}
					
					if ((flags & LayoutFlags.IncludeChildren) != 0)
					{
						//	TODO: faire en sorte que �a fonctionne (horizontals n'est pas pr�vu pour)
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
			
			this.root.LayoutChanged       += new EventHandler (this.HandleRootLayoutChanged);
			this.root.ChildrenChanged     += new EventHandler (this.HandleRootChildrenChanged);
			this.root.PreparePaint        += new EventHandler (this.HandleRootPreparePaint);
			this.root.PaintForeground     += new PaintEventHandler (this.HandleRootPaintForeground);
			this.root.PreProcessing	      += new MessageEventHandler (this.HandleRootPreProcessing);
			this.root.PaintBoundsCallback += new PaintBoundsCallback (this.HandlePaintBoundsCallback);
			
			this.root.SetPropagationModes (Widget.PropagationModes.UpChildrenChanged, true, true);
		}
		
		protected void DetachRootWidget()
		{
			this.root.LayoutChanged       -= new EventHandler (this.HandleRootLayoutChanged);
			this.root.ChildrenChanged     -= new EventHandler (this.HandleRootChildrenChanged);
			this.root.PreparePaint        -= new EventHandler (this.HandleRootPreparePaint);
			this.root.PaintForeground     -= new PaintEventHandler (this.HandleRootPaintForeground);
			this.root.PreProcessing	      -= new MessageEventHandler (this.HandleRootPreProcessing);
			this.root.PaintBoundsCallback -= new PaintBoundsCallback (this.HandlePaintBoundsCallback);
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
					
					//	Les index des s�parations sont relatives au parent; il faut donc ajuster en
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
						//	est d�j� compt�e. Il ne faut donc changer de ligne que si on a d�j� des widgets
						//	avant le n�tre.
						
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
			//	Ins�re les widgets dans les listes respectives des s�parations verticales
			//	qui sont � leur gauche. Cela permettra, dans une deuxi�me passe, de d�terminer
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
			//	temporairement de r�f�rence absolue :
			
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
					
					//	Met � jour la position minimale de la s�paration qui suit.
					
					if (right.min_x < next_x)
					{
						right.min_x = next_x;
					}
				}
			}
			
			//	Transforme les distances absolues et distances relatives d'une
			//	s�paration � l'autre :
			
			for (int i = this.verticals.Length-1; i > 0; i--)
			{
				this.verticals[i].min_x -= this.verticals[i-1].min_x;
			}
		}
		
		
		protected bool FindBestColumns(double x, int num_columns, out int index_left, out int index_right)
		{
			index_left = 0;
			index_right = 0;
			
			bool found = false;
			
			if ((num_columns & 1) == 1)
			{
				//	Nombre de colonnes impair: tant que [x] se trouve dans une colonne,
				//	on prend celle-ci comme r�f�rence.
				
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
				//	Nombre de colonnes pair: tant que [x] se trouve dans la moiti� gauche d'une colonne,
				//	on prend celle-ci comme r�f�rence, si [x] se trouve dans la moiti� droite, on prend
				//	la suivante comme r�f�rence.
				
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
			
			this.PaintDropShape (graphics);
			this.PaintDragShape (graphics);
			this.PaintVerticals (graphics);
		}
		
		private void HandleRootPreProcessing(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.root == sender);
			
			e.Suppress |= this.ProcessMessage (e.Message);
		}
		
		private void HandlePaintBoundsCallback(Widget widget, ref Drawing.Rectangle bounds)
		{
			bounds.Inflate (Grid.GripperRadius + 1, Grid.GripperRadius + 1);
		}

		
		
		protected void PaintVerticals(Drawing.Graphics graphics)
		{
			double dy = this.root.Client.Height;
			
			for (int i = 1; i < this.verticals.Length; i++)
			{
				double x = this.verticals[i].x_live;
				
				graphics.AddLine (x, 0, x, dy);
				graphics.AddFilledRectangle (x-Grid.GripperRadius,  0-Grid.GripperRadius, Grid.GripperRadius*2, Grid.GripperRadius*2);
				graphics.AddFilledRectangle (x-Grid.GripperRadius, dy-Grid.GripperRadius, Grid.GripperRadius*2, Grid.GripperRadius*2);
			}
			
			graphics.RenderSolid (Drawing.Color.FromARGB (0.5, 1.0, 0.0, 0.0));
		}
		
		protected void PaintDropShape(Drawing.Graphics graphics)
		{
		}
		
		protected void PaintDragShape(Drawing.Graphics graphics)
		{
			//	Dessine un rectangle avec une petite marque dans le coin sup�rieur gauche; cette
			//	figure sert de r�f�rence dans les op�rations de drag & drop, pour repr�senter
			//	l'objet en cours de dragging.
			
			if (this.designer_drag_rect.IsValid)
			{
				double x = this.designer_drag_rect.Left;
				double y = this.designer_drag_rect.Top;
				double w = 6;
				
				Drawing.Path path = new Drawing.Path ();
				
				path.MoveTo (x+6, y);
				path.LineTo (x, y);
				path.LineTo (x, y-w);
				path.LineTo (x+6, y);
				path.Close ();
				
				graphics.Rasterizer.FillMode = Drawing.FillMode.NonZero;
				graphics.Rasterizer.AddSurface (path);
				graphics.AddRectangle (this.designer_drag_rect);
				graphics.RenderSolid (Drawing.Color.FromARGB (1.0, 1, 0, 0));
				
				graphics.AddFilledRectangle (this.designer_drag_rect);
				graphics.RenderSolid (Drawing.Color.FromARGB (0.2, 1, 0, 0));
			}
		}
		
		
		internal bool ProcessMessage(Message message)
		{
			this.Update ();
			
			if (message.IsMouseType)
			{
				Drawing.Point abs_mouse = message.Cursor;
				Drawing.Point rel_mouse = this.root.MapRootToClient (abs_mouse);
				
				if (this.is_dragging)
				{
					this.HandleMouseDrag (rel_mouse, abs_mouse, message);
				}
				else
				{
					switch (message.Type)
					{
						case MessageType.MouseEnter:
						case MessageType.MouseLeave:
							break;
						
						case MessageType.MouseDown:
							this.HandleMouseDown (rel_mouse, abs_mouse, message);
							break;
						
						case MessageType.MouseMove:
							this.HandleMouseMove (rel_mouse, abs_mouse);
							break;
					}
				}
				
				this.is_dragging = this.is_dragging_column | this.is_dragging_widget;
			}
			
			return true;
		}
		
		protected void HandleMouseDrag(Drawing.Point rel_mouse, Drawing.Point abs_mouse, Message message)
		{
			bool end_dragging = (message.Button == MouseButtons.None) | (message.Type == MessageType.MouseUp);
			
			if (this.is_dragging_column)
			{
				if (end_dragging)
				{
					this.ColumnDraggingEnd (rel_mouse);
				}
				else
				{
					this.ColumnDraggingMove (rel_mouse);
				}
			}
			if (this.is_dragging_widget)
			{
				if (end_dragging)
				{
					this.WidgetDraggingEnd (rel_mouse);
				}
				else
				{
					this.WidgetDraggingMove (rel_mouse);
				}
			}
			
			//	Met � jour le flag g�n�ral indiquant si oui ou non une op�ration de
			//	dragging est en cours.
			
			if (end_dragging)
			{
				this.HandleMouseMove (rel_mouse, abs_mouse);
			}
		}
		
		protected void HandleMouseDown(Drawing.Point rel_mouse, Drawing.Point abs_mouse, Message message)
		{
			if (this.hot_column_index > 0)
			{
				this.ColumnDraggingBegin (rel_mouse);
			}
			else if (this.is_widget_hot)
			{
				this.designer.Attach (this.hot_widget);
				this.WidgetDraggingBegin (rel_mouse);
			}
			else if (this.is_designer_hot)
			{
				this.WidgetDraggingBegin (rel_mouse);
			}
			
			this.HandleMouseMove (rel_mouse, abs_mouse);
		}
		
		protected void HandleMouseMove(Drawing.Point rel_mouse, Drawing.Point abs_mouse)
		{
			this.hot_column_index = 0;
			this.is_widget_hot    = false;
			this.is_designer_hot  = false;
			
			if (this.DetectSelectedWidget (rel_mouse, abs_mouse))	return;
			if (this.DetectColumn (rel_mouse))						return;
			if (this.DetectWidget (rel_mouse))						return;
		}
		
		
		protected bool DetectSelectedWidget(Drawing.Point mouse, Drawing.Point root_mouse)
		{
			if (this.designer.Widget != null)
			{
				Widget        widget = this.designer.Widget;
				Drawing.Point pos    = widget.MapRootToClient (root_mouse);
				
				if (widget.Client.Bounds.Contains (pos))
				{
					this.designer.GripsHilited = true;
					this.is_designer_hot = true;
					return true;
				}
				
				this.designer.GripsHilited = false;
			}
			
			return false;
		}
		
		protected bool DetectWidget(Drawing.Point mouse)
		{
			Widget widget = this.root.FindChild (mouse);
			
			if (widget != null)
			{
				while (widget != this.root)
				{
					if (widget.Dock == DockStyle.Layout)
					{
						this.is_widget_hot = true;
						this.hot_widget    = widget;
						return true;
					}
					
					widget = widget.Parent;
				}
			}
			
			this.is_widget_hot = false;
			return false;
		}
		
		protected bool DetectColumn(Drawing.Point mouse)
		{
			for (int i = 1; i < this.verticals.Length; i++)
			{
				double x = this.verticals[i].x_live;
				
				if ((mouse.X <= x+1) &&
					(mouse.X >= x-1))
				{
					this.hot_column_index  = i;
					this.hot_column_offset = mouse.X - x;
					
					return true;
				}
			}
			
			return false;
		}
		

		protected void WidgetDraggingBegin(Drawing.Point mouse)
		{
			this.is_dragging_widget = true;
			
			//	Conserve une copie des positions verticales des diverses s�parations
			//	horizontales :
			
			this.frozen_lines_y = new double[this.horizontals.Length];
			
			for (int i = 0; i < this.horizontals.Length; i++)
			{
				this.frozen_lines_y[i] = this.horizontals[i].y_live;
			}
			
			this.designer.DraggingBegin (mouse);
			
			this.Invalidate ();
			this.Update ();
			this.WidgetDraggingMove (mouse);
		}
		
		protected void WidgetDraggingMove(Drawing.Point mouse)
		{
			Drawing.Rectangle rect_drop = this.designer.OriginalBounds;
			
			this.designer_drag_rect = this.designer.GetDraggingRectangle (mouse);
			
			int    col_count = this.designer.Widget.LayoutArg2 - this.designer.Widget.LayoutArg1;
			double center_x  = this.designer_drag_rect.Center.X;
			double corner_y  = this.designer_drag_rect.Top;
			
			if (this.FindBestColumns (center_x, col_count, out this.hot_designer_col1, out this.hot_designer_col2))
			{
				if (this.FindBestHotLine (corner_y, out this.hot_designer_line, out this.hot_designer_insert))
				{
					if ((this.hot_designer_insert) ||
						(this.IsHorizontalRangeEmpty (this.hot_designer_line, this.hot_designer_col1, this.hot_designer_col2, this.designer.WidgetOriginalSurface)))
					{
						//	Evite d'ins�rer une ligne vide apr�s (ou avant) si la ligne courante est adjacente
						//	et qu'elle ne contient que notre �l�ment; ce d�placement n'aurait aucun sens !
						
						if ((this.hot_designer_insert) &&
							(this.hot_designer_line > 0) &&
							(this.horizontals[this.hot_designer_line-1].list.Count == 1) &&
							(this.horizontals[this.hot_designer_line-1].list[0] == this.designer.WidgetOriginalSurface))
						{
							this.hot_designer_insert = false;
							this.hot_designer_line--;
						}
						
						if ((this.hot_designer_insert) &&
							(this.horizontals[this.hot_designer_line].list.Count == 1) &&
							(this.horizontals[this.hot_designer_line].list[0] == this.designer.WidgetOriginalSurface))
						{
							this.hot_designer_insert = false;
						}
						
						double x1 = this.verticals[this.hot_designer_col1].x_live;
						double x2 = this.verticals[this.hot_designer_col2].x_live;
						double y2 = this.frozen_lines_y[this.hot_designer_line];
						double dy = this.designer.OriginalBounds.Height;
						
						if (this.hot_designer_insert == false)
						{
							//	Utilise la hauteur de la ligne en cours, sauf si c'est une ligne qui va �tre
							//	cr��e suite � une insertion, ou si c'est la m�me ligne que celle o� se
							//	trouve l'original...
							
							int index = this.hot_designer_line+1;
							
							System.Diagnostics.Debug.Assert (index < this.frozen_lines_y.Length);
							
							double y1 = this.frozen_lines_y[index];
							
							dy = System.Math.Min (dy, y2-y1);
						}
						
						rect_drop = new Drawing.Rectangle (x1, y2-dy, x2-x1, dy);
					}
				}
			}
			
			this.designer.DraggingSetDropHint (rect_drop);
			
			this.UpdateLineGap ();
			this.UpdateGeometry ();
		}
		
		
		
		protected void WidgetDraggingEnd(Drawing.Point mouse)
		{
			this.designer.DraggingEnd ();
			
			//	Reconstruit la liste des lignes avec l'�tat d'origine. Cela va nous simplifier le
			//	travail pour trouver les anciennes et nouvelles positions dans le Z-order :
			
			this.Update ();
			
			if (this.designer.IsDropTargetValid)
			{
				this.RemoveWidgetFromHorizontals (this.designer.Widget);
				
				int pos_old = this.root.Children.IndexOf (this.designer.Widget);
				int pos_new = this.FindWidgetIndexBeforeHotDrop ();
				
				if (pos_old != pos_new)
				{
					//	La position a chang� dans le Z-order. La nouvelle position correspond � l'endroit
					//	o� nous devons proc�der � l'insertion.
					
					if (pos_old < pos_new)
					{
						//	La suppression du widget de son ancienne position va d�caler les widgets
						//	qui suivent et il faut par cons�quent adapter la position d'insertion :
						
						pos_new--;
					}
					
					this.designer.Widget.Parent = null;
					this.root.Children.InsertAt (pos_new, this.designer.Widget);
				}
				
				//	Le widget est positionn� correctement parmi ses fr�res; il faut encore d�finir
				//	les s�parations verticales gauche et droite, puis mettre � jour les passages �
				//	la ligne forc�s :
				
				this.designer.Widget.SetLayoutArgs (this.hot_designer_col1, this.hot_designer_col2);
				
				if (this.hot_designer_insert)
				{
					//	Le widget se trouve dans une nouvelle ligne, il faut donc en ins�rer une :
					
					this.InsertHorizontal (this.hot_designer_line);
				}
				
				this.InsertWidgetIntoHorizontal (this.designer.Widget, this.hot_designer_line);
				this.UpdateNewLinesInHorizontal ();
			}
			
			this.hot_designer_insert = false;
			this.is_dragging_widget  = false;
			this.designer_drag_rect  = Drawing.Rectangle.Empty;
			
			this.Invalidate ();
			this.UpdateLineGap ();
			this.Update ();
		}
		
		
		protected int FindWidgetIndexBeforeHotDrop()
		{
			Widget find = null;
			
			for (int i = 0; i < this.horizontals.Length-1; i++)
			{
				Grid.Horizontal horizontal = this.horizontals[i];
				
				for (int j = 0; j < horizontal.list.Count; j++)
				{
					Widget widget = horizontal.list[j] as Widget;
					
					if (i < this.hot_designer_line)
					{
						find = widget;
					}
					else if ( (i == this.hot_designer_line)
						   && (this.hot_designer_insert == false)
						   && (this.hot_designer_col1 > widget.LayoutArg1) )
					{
						find = widget;
					}
				}
			}
			
			int pos_new = (find == null) ? 0 : (this.root.Children.IndexOf (find) + 1);
			int pos_old = this.root.Children.IndexOf (this.designer.Widget);
			
			return (find == this.designer.WidgetOriginalSurface) ? pos_old : pos_new;
		}
		
		protected int FindWidgetLineIndex(Widget widget)
		{
			//	Trouve l'index de la ligne o� se trouve le widget sp�cifi�.
			//	Retourne -1 si le widget n'est pas trouv�.
			
			for (int i = 0; i < this.horizontals.Length-1; i++)
			{
				Grid.Horizontal horizontal = this.horizontals[i];
				
				for (int j = 0; j < horizontal.list.Count; j++)
				{
					if (horizontal.list[j] == widget)
					{
						return i;
					}
				}
			}
			
			return -1;
		}
		
		
		protected bool IsHorizontalRangeEmpty(int index, int col1, int col2, Widget exclude)
		{
			Grid.Horizontal horizontal = this.horizontals[index];
			
			for (int i = 0; i < horizontal.list.Count; i++)
			{
				Widget iter = horizontal.list[i] as Widget;
				
				if ((iter.LayoutArg2 > col1) &&
					(iter.LayoutArg1 < col2) &&
					(iter != exclude))
				{
					return false;
				}
			}
			
			return true;
		}
		
		protected void RemoveWidgetFromHorizontals(Widget widget)
		{
			for (int i = 0; i < this.horizontals.Length-1; i++)
			{
				this.horizontals[i].list.Remove (widget);
			}
		}
		
		protected void InsertWidgetIntoHorizontal(Widget widget, int index)
		{
			Grid.Horizontal horizontal = this.horizontals[index];
			
			int column = widget.LayoutArg1;
			
			for (int i = 0; i < horizontal.list.Count; i++)
			{
				Widget iter = horizontal.list[i] as Widget;
				
				if (column < iter.LayoutArg1)
				{
					horizontal.list.Insert (i, widget);
					return;
				}
			}
			
			//	Ajoute � la fin...
			
			horizontal.list.Add (widget);
		}
		
		protected void UpdateNewLinesInHorizontal()
		{
			//	Met � jour LayoutFlags des widgets contenus dans toutes les lignes. Pour cela, on
			//	active StartNewLine pour le premier widget de chaque ligne, et on le d�sactive pour
			//	tous les autres widgets.
			
			for (int i = 0; i < this.horizontals.Length; i++)
			{
				ArrayList list = this.horizontals[i].list;
				
				for (int j = 0; j < list.Count; j++)
				{
					Widget widget = list[j] as Widget;
					
					if (j == 0)
					{
						widget.LayoutFlags |= LayoutFlags.StartNewLine;
					}
					else
					{
						widget.LayoutFlags &= ~LayoutFlags.StartNewLine;
					}
				}
			}
		}

		protected void InsertHorizontal(int index)
		{
			Grid.Horizontal[] copy = new Grid.Horizontal[this.horizontals.Length+1];
			
			for (int i = 0; i < this.horizontals.Length; i++)
			{
				if (i < index)
				{
					copy[i] = this.horizontals[i];
				}
				else if (i >= index)
				{
					copy[i+1] = this.horizontals[i];
				}
			}
			
			copy[index] = new Grid.Horizontal (0);
			
			this.horizontals = copy;
		}
		
		
		protected void ColumnDraggingBegin(Drawing.Point mouse)
		{
			this.is_dragging_column = true;
			
			for (int j = 0; j < this.columns.Count; j++)
			{
				this.columns[j].dx_init = this.columns[j].dx_live;
			}
			
			this.UpdateGeometry ();
		}
		
		protected void ColumnDraggingMove(Drawing.Point mouse)
		{
			double x  = mouse.X - this.hot_column_offset;
			double dx = x - this.verticals[this.hot_column_index-1].x_live;
			
			this.columns[this.hot_column_index-1].dx_init = dx;
			
			this.UpdateGeometry ();
		}
		
		protected void ColumnDraggingEnd(Drawing.Point mouse)
		{
			this.is_dragging_column = false;
			this.desired_width = this.verticals[this.columns.Count].x_live;
			
			this.UpdateGeometry ();
		}
		
		
		protected bool FindBestHotLine(double y, out int line_index, out bool line_insert)
		{
			double m = 2;
			y -= m+1;
			
			for (int i = 1; i < this.frozen_lines_y.Length; i++)
			{
				double y1 = this.frozen_lines_y[i-1];
				double y2 = this.frozen_lines_y[i-0];
				
				double y1_top = y1 + m;
				double y1_bot = y1 - m;
				double y2_top = y2 + m;
				double y2_bot = y2 - m;
					
				if ((y <= y1_top) &&
					(y >= y1_bot))
				{
					line_index  = i-1;
					line_insert = true;
					return true;
				}
				
				if ((y <= y2_top) &&
					(y >= y2_bot))
				{
					line_index  = i;
					line_insert = true;
					return true;
				}
				
				if ((y <= y1) &&
					(y >= y2))
				{
					line_index  = i-1;
					line_insert = false;
					return true;
				}
			}
			
			int last_line = this.frozen_lines_y.Length - 1;
			
			if (last_line > 0)
			{
				if (y > this.frozen_lines_y[0])
				{
					line_index = 0;
					line_insert = true;
					return true;
				}
				if (y < this.frozen_lines_y[last_line])
				{
					line_index  = last_line;
					line_insert = true;
					return true;
				}
			}
			
			line_index  = -1;
			line_insert = false;
			
			return false;
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
				this.list   = new ArrayList ();
				this.y_live = y;
				this.gap    = 0;
			}
			
			internal ArrayList			list;
			
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
			internal double				k_init;			//	�lasticit� initiale
			internal double				k_live;			//	�lasticit� active
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
		
		private int						freeze_count;
		
		protected Grid.ColumnCollection	columns;
		protected Grid.Vertical[]		verticals;
		protected Grid.Horizontal[]		horizontals;
		
		protected double				desired_width;
		protected double				desired_height;
		protected double				current_width;
		protected double				current_height;
		
		protected bool					is_dirty;
		protected bool					is_dragging;				//	op�ration de dragging en cours
		protected bool					is_dragging_column;			//	dragging d'une colonne
		protected bool					is_dragging_widget;			//	dragging d'un widget complet
		
		protected bool					is_designer_hot;			//	souris sur le widget s�lectionn�
		protected bool					is_widget_hot;				//	souris sur un widget non s�lectionn�
		
		protected Widget				hot_widget;
		
		protected int					hot_column_index = -1;
		protected double				hot_column_offset;
		protected int					hot_designer_col1;
		protected int					hot_designer_col2;
		protected int					hot_designer_line;
		protected bool					hot_designer_insert;
		
		protected Design.WidgetWrapper	designer;
		protected Drawing.Rectangle		designer_drag_rect;
		
		protected double[]				frozen_lines_y;
		
		protected int					line_gap_index;
		protected double				line_gap_height;
		
		protected const	double			GripperRadius = 2;
	}
}
