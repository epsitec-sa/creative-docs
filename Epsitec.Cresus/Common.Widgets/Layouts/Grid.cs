
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
					dx[i-1] = this.verticals[i].x - this.verticals[i-1].x;
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
			
			for (int i = 0; i < this.columns.Count; i++)
			{
				this.columns[i].Reset ();
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
					
					x += width;
					
					vertical.x = x;
				}
				
				this.current_width = x;
			}
		}
		
		
		protected void AttachRootWidget(Widget root)
		{
			this.root = root;
			this.Invalidate ();
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
					Widget widget = vertical.list[j] as Widget;
					
					double next_x = widget.MinSize.Width + vertical.min_x;
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
		
		
		protected class Vertical
		{
			public Vertical()
			{
				this.list  = new ArrayList ();
				this.min_x = 0;
				this.max_x = 1000000;
				this.x     = 0;
			}
			
			
			internal ArrayList			list;
			
			internal double				min_x;
			internal double				max_x;
			
			internal double				x;
		}
		
		
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
			
			
			internal void Reset()
			{
				this.dx_live = this.dx_init;
				this.k_live  = this.k_init;
			}
			
			internal double				dx_init;
			internal double				dx_live;
			internal double				k_init;			//	élasticité initiale
			internal double				k_live;			//	élasticité active
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
		
		protected class Horizontal
		{
			public Horizontal(int num_verticals)
			{
			}
		}
		
		
		
		
		protected Widget				root;
		
		protected Grid.ColumnCollection	columns;
		protected Grid.Vertical[]		verticals;
		protected Grid.Horizontal[]		horizontals;
		
		protected double				desired_width;
		protected double				current_width;
		
		protected bool					is_dirty;
	}
}
