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
			int num_verticals;
			int num_horizontals;
			
			this.AnalyseVerticals (this.root, 0, ref max_index, out num_horizontals);
			
			num_verticals = max_index + 1;
			
			this.verticals   = new Vertical[num_verticals];
			this.horizontals = new Horizontal[num_horizontals];
			
			for (int i = 0; i < num_verticals; i++)
			{
				this.verticals[i] = new Vertical ();
			}
			
			for (int i = 0; i < num_horizontals; i++)
			{
				this.horizontals[i] = new Horizontal (num_verticals);
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
				this.columns[i].Reset (this.is_dragging);
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
							lines++;
							
							y_offset -= max_dy;
							max_dy = 0;
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
						this.LayoutWidgets (widget, arg1, x1, dy);
					}
					
					count++;
				}
			}
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
			
			if (e.Message.IsMouseType)
			{
				this.Update ();
				
				Drawing.Point pos = e.Point;
				
				if (this.is_dragging)
				{
					System.Diagnostics.Debug.WriteLine ("Dragging...");
					if (e.Message.IsLeftButton)
					{
						double x  = pos.X - this.drag_column_offset;
						double dx = x - this.verticals[this.drag_column_index-1].x_live;
						
						this.columns[this.drag_column_index-1].dx_init = dx;
						this.UpdateGeometry ();
						this.root.Invalidate ();
						this.root.Window.MouseCursor = MouseCursor.AsVSplit;
						
						e.Message.Consumer     = this.root;
						e.Message.ForceCapture = true;
						e.Suppress = true;
						
						return;
					}
					else
					{
						this.is_dragging = false;
					}
				}
				
				double mouse_x = pos.X;
				
				if (e.Message.Type == MessageType.MouseLeave)
				{
					mouse_x = -100;
				}
				
				for (int i = 1; i < this.verticals.Length; i++)
				{
					double x = this.verticals[i].x_live;
					
					if ((mouse_x <= x+1) &&
						(mouse_x >= x-1))
					{
						System.Diagnostics.Debug.WriteLine (i + ": " + x);
						
						this.root.Window.MouseCursor = MouseCursor.AsVSplit;
						this.drag_column_index  = i;
						this.drag_column_offset = mouse_x - x;
						
						if ((e.Message.Type == MessageType.MouseDown) &&
							(e.Message.IsLeftButton) &&
							(i > 0))
						{
							System.Diagnostics.Debug.WriteLine ("Start dragging now.");
							this.is_dragging = true;
							
							for (int j = 0; j < this.columns.Count; j++)
							{
								this.columns[j].dx_init = this.columns[j].dx_live;
							}
							
							e.Message.Consumer     = this.root;
							e.Message.ForceCapture = true;
							e.Suppress = true;
							
							this.UpdateGeometry ();
							this.root.Invalidate ();
						}
						
						return;
					}
				}
				
				if (this.drag_column_index >= 0)
				{
					this.root.Window.MouseCursor = MouseCursor.Default;
					this.drag_column_index = -1;
				}
			}
		}
		
		
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
			public Horizontal(int num_verticals)
			{
			}
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
		
		protected bool					is_dragging;
		protected int					drag_column_index = -1;
		protected double				drag_column_offset;
	}
}
