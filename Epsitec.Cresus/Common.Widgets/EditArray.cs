//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 13/02/2004

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe EditArray implémente un ScrollArray éditable.
	/// </summary>
	public class EditArray : ScrollArray
	{
		public EditArray()
		{
			this.edit_line = new EditWidget (this);
			this.edit_line.Hide ();
		}
		
		public EditArray(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public string[]							EditValues
		{
			get
			{
				return this.edit_line.Values;
			}
			set
			{
				this.edit_line.Values = value;
			}
		}
		
		public string							SearchCaption
		{
			get
			{
				return this.search_caption;
			}
			set
			{
				if (this.search_caption != value)
				{
					this.search_caption = value;
					this.edit_line.UpdateCaption ();
					this.OnSearchCaptionChanged ();
				}
			}
		}
		
		
		public EditArray.Header					ArrayHeader
		{
			get
			{
				return this.title_widget as EditArray.Header;
			}
		}
		
		public AbstractToolBar					ArrayToolBar
		{
			get
			{
				Header header = this.ArrayHeader;
				
				if (header != null)
				{
					return header.ToolBar;
				}
				
				return null;
			}
		}
		
		
		public void StartEdition(int row, int column)
		{
			if ((row < 0) || (row >= this.max_rows))
			{
				throw new System.ArgumentOutOfRangeException ("row", row, "Cannot edit specified row.");
			}
			
			if ((column < 0) || (column>= this.max_columns))
			{
				throw new System.ArgumentOutOfRangeException ("column", column, "Cannot edit specified column.");
			}
			
			this.SelectedIndex = row;
			this.SetInteractionMode (ScrollInteractionMode.Edition);
			this.ShowEdition (ScrollShowMode.Extremity);
			this.InvalidateContents ();
			this.Update ();
			
			column = this.FindFirstReadWriteColumn (column, 1);
			
			this.edit_line.Values = this.GetRowTexts (row);
			this.edit_line.FocusColumn (column);
			this.Invalidate ();
		}
		
		public void StartSearch()
		{
			this.SetInteractionMode (ScrollInteractionMode.Search);
			this.InvalidateContents ();
			this.Update ();
			this.edit_line.FocusColumn (0);
			this.Invalidate ();
		}
		
		public void ReloadEdition()
		{
			this.CancelEdition (false);
		}
		
		public void CancelEdition()
		{
			this.CancelEdition (true);
		}
		
		public void ValidateEdition()
		{
			this.ValidateEdition (true);
		}
		
		
		public virtual void CancelEdition(bool finished)
		{
			if (this.InteractionMode == ScrollInteractionMode.Edition)
			{
				if (finished)
				{
					this.edit_line.Defocus ();
				}
				
				if (this.SelectedIndex >= 0)
				{
					this.edit_line.Values = this.GetRowTexts (this.SelectedIndex);
					this.edit_line.SelectFocusedColumn ();
				}
				
				if (finished)
				{
					this.SetInteractionMode (ScrollInteractionMode.ReadOnly);
				}
				
				this.InvalidateContents ();
				this.Update ();
			}
		}
		
		public virtual void ValidateEdition(bool finished)
		{
			if (this.InteractionMode == ScrollInteractionMode.Edition)
			{
				if (finished)
				{
					this.edit_line.Defocus ();
				}
				
				if (this.SelectedIndex >= 0)
				{
					this.SetRowTexts (this.SelectedIndex, this.edit_line.Values);
				}
				
				if (finished)
				{
					this.SetInteractionMode (ScrollInteractionMode.ReadOnly);
				}
				
				this.InvalidateContents ();
				this.Update ();
			}
		}
		
		
		public virtual bool Grow()
		{
			//	Ajoute une ligne en fin de liste.
			
			Support.Data.ITextArrayStore store = this.TextArrayStore;
								
			if (store != null)
			{
				if (store.CheckInsertRows (this.RowCount, 1))
				{
					store.InsertRows (this.RowCount, 1);
					return true;
				}
			}
			
			return false;
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.edit_line.ColumnCount = 0;
				
				this.max_columns  = 0;
			}
			
			base.Dispose (disposing);
		}
		
		
		protected int FindFirstReadWriteColumn(int column, int dir)
		{
			if ((column < 0) ||
				(column >= this.max_columns))
			{
				return -1;
			}
			
			if (dir > 0)
			{
				for (int i = column; i < this.max_columns; i++)
				{
					if (! this.Columns[i].IsReadOnly)
					{
						return i;
					}
				}
			}
			else if (dir < 0)
			{
				for (int i = column; i >= 0; i--)
				{
					if (! this.Columns[i].IsReadOnly)
					{
						return i;
					}
				}
			}
			
			return -1;
		}
		
		protected override void UpdateColumnCount()
		{
			base.UpdateColumnCount ();
			
			if (this.edit_line.ColumnCount != this.max_columns)
			{
				this.edit_line.ColumnCount = this.max_columns;
			}
		}
		
		protected override void UpdateScrollView()
		{
			base.UpdateScrollView ();
			this.UpdateEditGeometry ();
		}
		
		protected virtual  void UpdateEditGeometry()
		{
			//	Si la géométrie a changé, on met à jour la position des divers widgets utilisés
			//	pour l'édition, ainsi que la géométrie du conteneur EditWidget :
			
			Drawing.Rectangle bounds = this.GetEditBounds ();
			
			if ((this.edit_bounds != bounds) ||
				(this.edit_offset != this.offset) ||
				(this.edit_width  != this.total_width))
			{
				this.edit_bounds = bounds;
				this.edit_offset = this.offset;
				this.edit_width  = this.total_width;
				
				if (this.edit_bounds.IsValid)
				{
					bounds.Deflate (1, 1, 0, -1);
					
					this.edit_line.Bounds = bounds;
					this.edit_line.Show ();
					this.edit_line.UpdateGeometry ();
					
					if ((this.focused_column >= 0) &&
						(this.focused_column < this.max_columns))
					{
						this.edit_line.FocusColumn (this.focused_column);
					}
					
					this.focused_column = -1;
				}
				else if (this.edit_line.IsVisible)
				{
					this.focused_column = this.edit_line.FindFocusedColumn ();
					
					if (this.edit_line.ContainsFocus)
					{
						this.SetFocused (true);
					}
					
					this.edit_line.Hide ();
				}
			}
		}
		
		protected virtual  void UpdateInnerTopMargin()
		{
			if (this.InteractionMode == ScrollInteractionMode.Search)
			{
				this.InnerTopMargin = this.edit_line.DesiredHeight;
			}
			else
			{
				this.InnerTopMargin = 0;
			}
		}
		
		protected virtual Drawing.Rectangle GetEditBounds()
		{
			Drawing.Rectangle bounds = Drawing.Rectangle.Empty;
			
			switch (this.InteractionMode)
			{
				case ScrollInteractionMode.ReadOnly:
					break;
				
				case ScrollInteractionMode.Edition:
					bounds = this.GetRowBounds (this.SelectedIndex);
					break;
				
				case ScrollInteractionMode.Search:
					bounds = new Drawing.Rectangle (this.inner_bounds.Left, this.inner_bounds.Top, this.inner_bounds.Width, this.table_bounds.Height - this.inner_bounds.Height);
					break;
			}
			
			if ((this.h_scroller.IsVisible) &&
				(bounds.IsValid) &&
				(bounds.Bottom <= this.h_scroller.Top))
			{
				bounds.Bottom = this.h_scroller.Top + 1;
			}
			
			return bounds;
		}
		
		protected virtual Drawing.Rectangle GetEditCellBounds(int column)
		{
			double x1;
			double x2;
			
			Drawing.Rectangle cell = Drawing.Rectangle.Empty;
			
			switch (this.InteractionMode)
			{
				case ScrollInteractionMode.ReadOnly:
					break;
				case ScrollInteractionMode.Edition:
					cell = this.GetUnclippedCellBounds (this.SelectedIndex, column);
					cell.Inflate (0, 1, 0, 1);
					break;
				case ScrollInteractionMode.Search:
					if (this.GetUnclippedCellX (column, out x1, out x2))
					{
						cell = new Drawing.Rectangle (x1, this.inner_bounds.Top - 1, x2-x1+1, this.table_bounds.Top - this.inner_bounds.Top + 1);
					}
					break;
			}
			
			return cell;
		}
		
		
		protected override void ProcessMessage(Message message, Epsitec.Common.Drawing.Point pos)
		{
			if ((message.IsMouseType) &&
				(message.Type != MessageType.MouseLeave))
			{
				Window window = this.Window;
				
				if (window != null)
				{
					int row, column;
					
					if (this.InteractionMode == ScrollInteractionMode.Edition)
					{
						if (this.HitTestTable (pos, out row, out column))
						{
							window.MouseCursor = this.Columns[column].IsReadOnly ? this.MouseCursor : MouseCursor.AsIBeam;
							
							if ((message.Type == MessageType.MouseDown) &&
								(message.ButtonDownCount == 1) &&
								(message.IsLeftButton) &&
								(row >= 0) && (row < this.max_rows) &&
								(this.CheckChangeSelectedIndexTo (row)))
							{
								//	L'utilisateur a cliqué dans une cellule de la table. On va faire en sorte
								//	de changer la cellule active (repositionner les lignes éditables) :
								
								this.SelectedIndex = row;
								this.Update ();
								
								column = this.FindFirstReadWriteColumn (column, 1);
								
								this.edit_line.FocusColumn (column);
								
								message.Consumer  = this.edit_line[column];
								message.Swallowed = true;
								
								return;
							}
						}
						else
						{
							window.MouseCursor = this.MouseCursor;
						}
					}
				}
			}
			
			base.ProcessMessage (message, pos);
		}

		protected override bool CheckChangeSelectedIndexTo(int index)
		{
			if (base.CheckChangeSelectedIndexTo (index))
			{
				if (this.InteractionMode == ScrollInteractionMode.Edition)
				{
					int column = this.edit_line.FindFocusedColumn ();
					
					if ((column >= 0) &&
						(column < this.max_columns))
					{
						Widget edition_widget = this.Columns[column].EditionWidget;
						
						if ((edition_widget != null) &&
							(edition_widget.AcceptDefocus == false))
						{
							return false;
						}
					}
				}
				
				return true;
			}
			
			return false;
		}
		
		protected override void PaintCellContents(int row_line, int column, Drawing.Graphics graphics, IAdorner adorner, Drawing.Point pos, WidgetState state, TextLayout layout)
		{
			if ((this.Columns[column].IsReadOnly) ||
				((this.TextArrayStore != null) && (this.TextArrayStore.CheckEnabledCell (row_line, column) == false)))
			{
				state &= ~ WidgetState.Enabled;
			}
			
			base.PaintCellContents (row_line, column, graphics, adorner, pos, state, layout);
		}
		
		
		protected override void OnSelectedIndexChanging()
		{
			base.OnSelectedIndexChanging ();
			
			if (this.InteractionMode == ScrollInteractionMode.Edition)
			{
				this.ValidateEdition (false);
			}
		}
		
		protected override void OnSelectedIndexChanged()
		{
			base.OnSelectedIndexChanged ();
			
			if (this.InteractionMode == ScrollInteractionMode.Edition)
			{
				this.ReloadEdition ();
			}
		}
		
		protected override void OnInteractionModeChanging()
		{
			this.edit_line.DeallocateLines ();
			base.OnInteractionModeChanging ();
		}
		
		protected override void OnInteractionModeChanged()
		{
			this.edit_line.ReallocateLines ();
			this.edit_line.UpdateCaption ();
			this.edit_bounds = Drawing.Rectangle.Empty;
			
			base.OnInteractionModeChanged ();
		}
		
		protected virtual  void OnEditTextChanged()
		{
			if (this.EditTextChanged != null)
			{
				this.EditTextChanged (this);
			}
		}
		
		protected virtual  void OnSearchCaptionChanged()
		{
		}
		
		protected override void OnDefocused()
		{
			base.OnDefocused ();
			
			if (this.ContainsFocus)
			{
				//	En fait, on contient toujours le focus...
			}
			else
			{
				this.focused_column = -1;
			}
		}

		protected virtual  void OnEditWidgetsCreated()
		{
			if (this.EditWidgetsCreated != null)
			{
				this.EditWidgetsCreated (this);
			}
		}
		
		protected bool MoveEditionToLine(int offset)
		{
			if (this.InteractionMode == ScrollInteractionMode.Edition)
			{
				int row = this.SelectedIndex + offset;
				
				row = System.Math.Min (row, this.max_rows-1);
				row = System.Math.Max (row, 0);
				
				if ((this.SelectedIndex != row) &&
					(this.CheckChangeSelectedIndexTo (row)))
				{
					this.SelectedIndex = row;
					return true;
				}
			}
			
			return false;
		}
		
		
		#region EditWidget Class
		/// <summary>
		/// La classe EditWidget est utilisée comme conteneur pour les widgets en cours
		/// d'édition. C'est elle qui gère la navigation au moyen de TAB.
		/// </summary>
		[Support.SuppressBundleSupport] protected class EditWidget : Widget
		{
			public EditWidget(EditArray host)
			{
				this.host = host;
				
				this.SetEmbedder (this.host);
				this.TabNavigation = Widget.TabNavigationMode.ForwardTabPassive;
			}
			
			
			public AbstractTextField			this[int i]
			{
				get
				{
					return this.edit_widgets[i];
				}
			}
			
			public int							ColumnCount
			{
				get
				{
					return this.edit_widgets.Length;
				}
				set
				{
					AbstractTextField[] widgets = new AbstractTextField[value];
					
					int n = System.Math.Min (value, this.edit_widgets.Length);
					
					for (int i = 0; i < n; i++)
					{
						widgets[i] = this.edit_widgets[i];
					}
					
					for (int i = n; i < this.edit_widgets.Length; i++)
					{
						this.Detach (this.edit_widgets[i]);
					}
					
					this.edit_widgets = widgets;
					
					this.AttachEditWidgets ();
				}
			}
			
			public string[]						Values
			{
				get
				{
					string[] values = new string[this.edit_widgets.Length];
				    
					for (int i = 0; i < this.edit_widgets.Length; i++)
					{
						values[i] = this.edit_widgets[i].Text;
					}
					
					return values;
				}
				set
				{
					//	Evite de générer des événements EditTextChanged pendant la mise à jour
					//	des divers champs :
					
					this.setting_values = true;
					this.text_change_count = 0;
					
					for (int i = 0; i < this.edit_widgets.Length; i++)
					{
						bool read_only = this.host.Columns[i].IsReadOnly;
						
						this.edit_widgets[i].Text = value[i];
						this.edit_widgets[i].IsReadOnly = read_only;
						this.edit_widgets[i].SetEnabled (!read_only);
					}
					
					this.setting_values = false;
					
					//	S'il y a effectivement eu des changements dans les contenus de champs,
					//	on envoie l'événement maintenant et une seule fois :
					
					if (this.text_change_count > 0)
					{
						this.host.OnEditTextChanged ();
					}
				}
                
			}
			
			
			public double						DesiredHeight
			{
				get
				{
					if ((this.caption == null) ||
						(this.host.InteractionMode != ScrollInteractionMode.Search))
					{
						return this.LineHeight;
					}
					else
					{
						return 4 + this.LineHeight + this.caption.Height;
					}
				}
			}
			
			public double						LineHeight
			{
				get
				{
					return System.Math.Floor (this.host.EditionZoneHeight * this.host.row_height + 2);
				}
			}
			
			
			protected Widget FindEditWidget(int column, int dir)
			{
				if ((column < 0) ||
					(column >= this.edit_widgets.Length))
				{
					return null;
				}
				
				column = this.host.FindFirstReadWriteColumn (column, dir);
				
				if ((column > -1) &&
					(column < this.edit_widgets.Length))
				{
					return this.edit_widgets[column];
				}
				
				return null;
			}
			
			protected override void Dispose(bool disposing)
			{
				if (disposing)
				{
					this.caption = null;
				}
				
				base.Dispose (disposing);
			}

			protected override bool ProcessTabChildrenExit(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
			{
				if (this.host.InteractionMode == ScrollInteractionMode.Search)
				{
					switch (dir)
					{
						case TabNavigationDir.Forwards:
							focus = this.edit_widgets[0];
							break;
						
						case TabNavigationDir.Backwards:
							focus = this.edit_widgets[this.edit_widgets.Length-1];
							break;
						
						default:
							focus = null;
							break;
					}
				}
				else
				{
					int move  =  0;
					int start = -1;
					
					switch (dir)
					{
						case TabNavigationDir.Forwards:  move =  1; start = 0;                          break;
						case TabNavigationDir.Backwards: move = -1; start = this.edit_widgets.Length-1; break;
					}
					
					if (this.host.MoveEditionToLine (move))
					{
						focus = this.FindEditWidget (start, move);
					}
					else
					{
						//	On ne peut plus avancer/reculer, car est arrivé en extrémité de table.
						
						if ((move > 0) &&
							(this.host.InteractionMode == ScrollInteractionMode.Edition) &&
							(this.host.TextArrayStore != null) &&
							(this.host.TextArrayStore.CheckInsertRows (this.host.RowCount, 1)))
						{
							//	Il s'avère que la table est en cours d'édition et que le "store" associé
							//	avec celle-ci permet l'insertion en fin de table. Profitons-en !
							
							this.host.TextArrayStore.InsertRows (this.host.RowCount, 1);
							this.host.MoveEditionToLine (move);
							
							focus = this.FindEditWidget (0, 1);
						}
						else
						{
							focus = null;
						}
					}
				}
				
				return true;
			}
			
			protected override void ProcessMessage(Message message, Drawing.Point pos)
			{
				if ((message.Type == MessageType.KeyPress) &&
					(message.IsCtrlPressed == false) &&
					(message.IsAltPressed == false))
				{
					IFeel feel = Feel.Factory.Active;
					
					if (this.host.InteractionMode == ScrollInteractionMode.Search)
					{
						if (feel.TestAcceptKey (message))
						{
							this.host.OnEditTextChanged ();
							message.Consumer  = this;
							message.Swallowed = true;
							return;
						}
						if (feel.TestCancelKey (message))
						{
							this.host.SetInteractionMode (ScrollInteractionMode.ReadOnly);
							this.host.SetFocused (true);
							message.Consumer  = this;
							message.Swallowed = true;
							return;
						}
						if (this.host.ProcessKeyEvent (message))
						{
							//	L'événement a été traité par la liste; on va donc le consommer.
							
							message.Consumer = this;
							message.Swallowed = true;
							return;
						}
					}
					else if (this.host.InteractionMode == ScrollInteractionMode.Edition)
					{
						if (feel.TestCancelKey (message))
						{
							this.host.ValidateEdition ();
							this.host.SetFocused (true);
							message.Consumer = this;
							message.Swallowed = true;
							return;
						}
						if (feel.TestAcceptKey (message))
						{
							int index = (message.IsShiftPressed) ? this.host.SelectedIndex - 1 : this.host.SelectedIndex + 1;
							
							if (index == this.host.RowCount)
							{
								this.host.Grow ();
							}
							
							if ((index >= 0) &&
								(index < this.host.RowCount))
							{
								this.host.StartEdition (index, 0);
								message.Consumer = this;
								message.Swallowed = true;
								return;
							}
						}
					}
				}
				
				base.ProcessMessage (message, pos);
			}
			
			
			public void Defocus()
			{
				int column = this.FindFocusedColumn ();
				
				if (column >= 0)
				{
					this.edit_widgets[column].SetFocused (false);
				}
			}
			
			public void SelectFocusedColumn()
			{
				this.FocusColumn (this.FindFocusedColumn ());
			}
			
			public void FocusColumn(int column)
			{
				if (column >= 0)
				{
					this.edit_widgets[column].SetFocused (true);
					this.edit_widgets[column].SelectAll ();
				}
			}
			
			public int  FindFocusedColumn()
			{
				for (int i = 0; i < this.edit_widgets.Length; i++)
				{
					if (this.edit_widgets[i].ContainsFocus)
					{
						return i;
					}
				}
				
				return -1;
			}
			
			public void AttachEditWidgets()
			{
				TextFieldStyle style = (this.host.InteractionMode == ScrollInteractionMode.Search ? TextFieldStyle.Normal : TextFieldStyle.Flat);
				int created = 0;
				
				for (int i = 0; i < this.edit_widgets.Length; i++)
				{
					if (this.edit_widgets[i] == null)
					{
						Widget      model = this.host.Columns[i].EditionWidgetModel;
						System.Type type  = this.host.Columns[i].EditionWidgetType;
						
						if (this.host.edition_add_rows > 0)
						{
							type = typeof (TextFieldMulti);
						}
						
						if (this.host.InteractionMode == ScrollInteractionMode.Search)
						{
							type = typeof (TextField);
						}
						
						this.edit_widgets[i] = System.Activator.CreateInstance (type, new object[] { this } ) as AbstractTextField;
						this.edit_widgets[i].TextFieldStyle = style;
						
						if (model != null)
						{
							Support.ObjectBundler.Default.CopyObject (model, this.edit_widgets[i]);
						}
						
						this.Attach (this.edit_widgets[i], i);
						this.host.Columns[i].EditionWidget = this.edit_widgets[i];
						
						created++;
					}
				}
				
				if (created > 0)
				{
					this.host.OnEditWidgetsCreated ();
				}
			}
			
			public void UpdateGeometry()
			{
				double height = this.LineHeight;
				double ox = -this.Bounds.X;
				double oy = -this.Bounds.Y;
				
				if ((this.caption != null) &&
					(this.host.InteractionMode == ScrollInteractionMode.Search))
				{
					double dy = this.caption.Height;
					double yy = 4;
					
					this.caption.Bounds = new Drawing.Rectangle (2, yy + height, this.Client.Width - 4, dy);
					
					oy += yy;
				}
				
				for (int i = 0; i < this.edit_widgets.Length; i++)
				{
					Drawing.Rectangle cell = this.host.GetEditCellBounds (i);
						
					if (cell.IsValid)
					{
						cell.Offset (ox, oy);
						cell.Height = height - 1;
						
						this.edit_widgets[i].Bounds = cell;
						this.edit_widgets[i].Show ();
					}
					else
					{
						this.edit_widgets[i].Hide ();
					}
				}
			}
			
			public void UpdateCaption()
			{
				string caption = null;
				
				if (this.host.InteractionMode == ScrollInteractionMode.Search)
				{
					caption = this.host.search_caption;
				}
				
				if (caption == null)
				{
					if (this.caption != null)
					{
						this.caption.Parent = null;
						this.caption.Dispose ();
						this.caption = null;
					}
				}
				else
				{
					if (this.caption == null)
					{
						this.caption = new StaticText (this);
					}
					
					this.caption.Text   = caption;
					this.caption.Height = System.Math.Floor (this.caption.TextLayout.SingleLineSize.Height * 1.2);
				}
				
				this.host.UpdateInnerTopMargin ();
			}
			
			public void DeallocateLines()
			{
				this.ColumnCount = 0;
			}
			
			public void ReallocateLines()
			{
				this.ColumnCount = 0;
				this.ColumnCount = this.host.max_columns;
			}
			
			
			protected void Attach(AbstractTextField widget, int i)
			{
				widget.Index         = i;
				widget.TabIndex      = i;
				widget.TabNavigation = TabNavigationMode.ActivateOnTab;
				widget.Focused      += new Support.EventHandler (this.HandleEditArrayFocused);
				widget.TextChanged  += new Epsitec.Common.Support.EventHandler (this.HandleTextChanged);
				widget.AutoSelectOnFocus = true;
				widget.Hide ();
			}
			
			protected void Detach(AbstractTextField widget)
			{
				if (widget != null)
				{
					widget.Parent       = null;
					widget.Focused     -= new Support.EventHandler (this.HandleEditArrayFocused);
					widget.TextChanged -= new Epsitec.Common.Support.EventHandler (this.HandleTextChanged);
					widget.Dispose ();
					widget = null;
				}
			}
			
			
			private void HandleEditArrayFocused(object sender)
			{
				AbstractTextField widget = sender as AbstractTextField;
				
				int row    = this.host.SelectedIndex;
				int column = widget.Index;
				
				this.host.ShowCell (ScrollShowMode.Extremity, row, column);
			}
			
			private void HandleTextChanged(object sender)
			{
				if (this.setting_values)
				{
					this.text_change_count++;
				}
				else
				{
					this.host.OnEditTextChanged ();
				}
			}
			
			
			protected bool						setting_values;
			protected int						text_change_count;
			protected EditArray					host;
			protected AbstractTextField[]		edit_widgets = new AbstractTextField[0];
			protected StaticText				caption;

		}
		#endregion
		
		#region Header Class
		[Support.SuppressBundleSupport] public class Header : Widget
		{
			public Header(EditArray host)
			{
				if (host == null)
				{
					throw new System.ArgumentNullException ("host", "Header must be hosted in EditArray.");
				}
				
				this.host = host;
				this.SetEmbedder (this.host);
				
				this.caption = new StaticText (this);
				this.toolbar = new HToolBar (this);
				
				this.caption.Dock = DockStyle.Fill;
				this.caption.Hide ();
				
				this.toolbar.Dock = DockStyle.Bottom;
				this.toolbar.Hide ();
				this.toolbar.ClientGeometryUpdated += new Support.EventHandler (this.HandleToolBarGeometryChanged);
				this.toolbar.ItemsChanged += new Support.EventHandler (this.HandleToolBarItemsChanged);
				
				this.UpdateHeaderHeight ();
			}
			
			
			public string						Caption
			{
				get
				{
					return this.caption.Text;
				}
				set
				{
					if (this.caption.Text != value)
					{
						if ((value == null) ||
							(value.Length == 0))
						{
							this.caption.Text = "";
							this.caption.Hide ();
							this.is_caption_ok = false;
						}
						else
						{
							this.caption.Text = value;
							this.caption_height = System.Math.Floor (this.caption.TextLayout.SingleLineSize.Height * 1.2);
							this.caption.Show ();
							this.is_caption_ok = true;
						}
						
						this.UpdateHeaderHeight ();
					}
				}
			}
			
			public AbstractToolBar				ToolBar
			{
				get
				{
					return this.toolbar;
				}
			}
			
			
			protected void UpdateHeaderHeight()
			{
				double height = 0;
				
				if (this.is_toolbar_ok)
				{
					height += this.toolbar.Height;
				}
				if (this.is_caption_ok)
				{
					height += this.caption_height;
				}
				
				this.host.TitleHeight = height;
			}
			
			
			private void HandleToolBarGeometryChanged(object sender)
			{
				this.UpdateHeaderHeight ();
			}
			
			private void HandleToolBarItemsChanged(object sender)
			{
				if (this.toolbar.Items.Count == 0)
				{
					if (this.is_toolbar_ok == true)
					{
						this.is_toolbar_ok = false;
						this.toolbar.Hide ();
						this.UpdateHeaderHeight ();
					}
				}
				else
				{
					if (this.is_toolbar_ok == false)
					{
						this.is_toolbar_ok = true;
						this.toolbar.Show ();
						this.UpdateHeaderHeight ();
					}
				}
			}
			
			
			protected EditArray					host;
			protected StaticText				caption;
			protected double					caption_height;
			protected HToolBar					toolbar;
			protected bool						is_toolbar_ok;
			protected bool						is_caption_ok;
		}
		#endregion
		
		#region Controller Class
		public class Controller
		{
			public Controller(EditArray host, string name)
			{
				if (host == null)
				{
					throw new System.ArgumentNullException ("host", "Controller must be hosted in EditArray.");
				}
				
				this.host = host;
				this.name = name;
				
				this.host.InteractionModeChanged += new Support.EventHandler (this.HandleHostInteractionModeChanged);
				this.host.SelectedIndexChanged   += new Support.EventHandler (this.HandleHostSelectedIndexChanged);
				this.host.TextArrayStoreChanged  += new Support.EventHandler (this.HandleHostTextArrayStoreChanged);
				this.host.ContentsInvalidated    += new Support.EventHandler (this.HandleHostContentsInvalidated);
				
				this.UpdateStore ();
			}
			
			
			public int							ActiveRow
			{
				get
				{
					return this.host.SelectedIndex;
				}
			}
			
			
			public void CreateCommands()
			{
				Support.CommandDispatcher dispatcher = this.host.CommandDispatcher;
				
				dispatcher.Register (this.GetCommandName ("StartReadOnly"), new Support.CommandEventHandler (this.CommandStartReadOnly));
				dispatcher.Register (this.GetCommandName ("StartEdition"),  new Support.CommandEventHandler (this.CommandStartEdition));
				dispatcher.Register (this.GetCommandName ("StartSearch"),   new Support.CommandEventHandler (this.CommandStartSearch));
				dispatcher.Register (this.GetCommandName ("InsertBefore"),  new Support.CommandEventHandler (this.CommandInsertBefore));
				dispatcher.Register (this.GetCommandName ("InsertAfter"),   new Support.CommandEventHandler (this.CommandInsertAfter));
				dispatcher.Register (this.GetCommandName ("Delete"),        new Support.CommandEventHandler (this.CommandDelete));
				dispatcher.Register (this.GetCommandName ("MoveUp"),        new Support.CommandEventHandler (this.CommandMoveUp));
				dispatcher.Register (this.GetCommandName ("MoveDown"),      new Support.CommandEventHandler (this.CommandMoveDown));
			}
			
			public void CreateToolBarButtons()
			{
				EditArray.Header header = this.host.ArrayHeader;
				
				if (header != null)
				{
					AbstractToolBar toolbar = header.ToolBar;
					
					toolbar.SuspendLayout ();
					toolbar.Items.Add (this.CreateIconButton ("StartReadOnly", "manifest:Epsitec.Common.Widgets.Images.TableReadOnly.icon", "Consultation uniquement"));
					toolbar.Items.Add (this.CreateIconButton ("StartEdition",  "manifest:Epsitec.Common.Widgets.Images.TableEdition.icon",  "Modifie les données", KeyCode.FuncF2));
					toolbar.Items.Add (this.CreateIconButton ("StartSearch",   "manifest:Epsitec.Common.Widgets.Images.TableSearch.icon",   "Démarre une recherche", KeyCode.ModifierControl | KeyCode.AlphaF));
					toolbar.Items.Add (new IconSeparator ());
					toolbar.Items.Add (this.CreateIconButton ("InsertBefore",  "manifest:Epsitec.Common.Widgets.Images.InsertBeforeCell.icon", "Insère une ligne avant"));
					toolbar.Items.Add (this.CreateIconButton ("InsertAfter",   "manifest:Epsitec.Common.Widgets.Images.InsertAfterCell.icon",  "Insère une ligne après"));
					toolbar.Items.Add (this.CreateIconButton ("Delete",        "manifest:Epsitec.Common.Widgets.Images.DeleteCell.icon",       "Supprime une ligne", KeyCode.Delete));
					toolbar.Items.Add (this.CreateIconButton ("MoveUp",        "manifest:Epsitec.Common.Widgets.Images.MoveUpCell.icon",       "Déplace la ligne vers le haut"));
					toolbar.Items.Add (this.CreateIconButton ("MoveDown",      "manifest:Epsitec.Common.Widgets.Images.MoveDownCell.icon",     "Déplace la ligne vers le bas"));
					toolbar.ResumeLayout ();
				}
				
				this.UpdateCommandStates ();
			}
			
			
			public virtual void StartReadOnly()
			{
				if (this.host.InteractionMode == ScrollInteractionMode.Edition)
				{
					this.host.ValidateEdition ();
				}
				
				this.host.SetInteractionMode (ScrollInteractionMode.ReadOnly);
				this.host.SetFocused (true);
				this.host.ShowSelected (ScrollShowMode.Extremity);
			}
			
			public virtual void StartEdition()
			{
				this.StartReadOnly ();
				this.host.StartEdition (this.host.SelectedIndex, 0);
				this.host.ShowEdition (ScrollShowMode.Extremity);
			}
			
			public virtual void StartSearch()
			{
				this.StartReadOnly ();
				this.host.StartSearch ();
				this.host.ShowSelected (ScrollShowMode.Extremity);
			}
			
			public virtual void InsertBefore()
			{
				ScrollInteractionMode mode;
				int row, column;
				this.SaveModeAndReset (out mode, out row, out column);
				
				this.store.InsertRows (row, 1);
				this.RestoreMode (ScrollInteractionMode.Edition, row, 0);
			}
			
			public virtual void InsertAfter()
			{
				ScrollInteractionMode mode;
				int row, column;
				this.SaveModeAndReset (out mode, out row, out column);
				
				this.store.InsertRows (row+1, 1);
				this.RestoreMode (ScrollInteractionMode.Edition, row+1, 0);
			}
			
			public virtual void Delete()
			{
				ScrollInteractionMode mode;
				int row, column;
				this.SaveModeAndReset (out mode, out row, out column);
				
				this.store.RemoveRows (row, 1);
				this.RestoreMode (mode, row, 0);
			}
			
			public virtual void MoveUp()
			{
				ScrollInteractionMode mode;
				int row, column;
				this.SaveModeAndReset (out mode, out row, out column);
				
				this.store.MoveRow (row, -1);
				this.RestoreMode (mode, row-1, column);
			}
			
			public virtual void MoveDown()
			{
				ScrollInteractionMode mode;
				int row, column;
				this.SaveModeAndReset (out mode, out row, out column);
				
				this.store.MoveRow (row, 1);
				this.RestoreMode (mode, row+1, column);
			}
			
			
			protected void SaveModeAndReset(out ScrollInteractionMode mode, out int row, out int column)
			{
				mode   = this.host.InteractionMode;
				column = this.host.edit_line.FindFocusedColumn ();
				
				this.StartReadOnly ();
				
				row = this.host.SelectedIndex;
			}
			
			protected void RestoreMode(ScrollInteractionMode mode, int row, int column)
			{
				if (row >= this.host.RowCount)
				{
					row = this.host.RowCount - 1;
				}
				
				this.host.SelectedIndex = row;
				
				column = this.host.FindFirstReadWriteColumn (column, 1);
				
				switch (mode)
				{
					case ScrollInteractionMode.ReadOnly:
						this.StartReadOnly ();
						break;
					
					case ScrollInteractionMode.Edition:
						this.StartEdition ();
						this.host.edit_line.FocusColumn (column);
						break;
					
					case ScrollInteractionMode.Search:
						this.StartSearch ();
						this.host.edit_line.FocusColumn (column);
						break;
				}
			}
			
			
			protected string       GetCommandName(string command_name)
			{
				return this.name + command_name;
			}
			
			protected CommandState GetCommandState(string command_name)
			{
				return CommandState.Find (this.GetCommandName (command_name), this.host.CommandDispatcher);
			}
			
			
			protected IconButton CreateIconButton(string command_name, string icon_name, string tool_tip)
			{
				IconButton button = new IconButton (this.GetCommandName (command_name), icon_name);
				this.CreateToolTip (button, tool_tip);
				return button;
			}
			
			protected IconButton CreateIconButton(string command_name, string icon_name, string tool_tip, KeyCode shortcut)
			{
				IconButton button = this.CreateIconButton (command_name, icon_name, tool_tip);
				button.Shortcut.KeyCode = shortcut;
				return button;
			}
			
			protected void       CreateToolTip(Widget widget, string text)
			{
				if (this.tips == null)
				{
					this.tips = new ToolTip ();
				}
				
				this.tips.SetToolTip (widget, text);
			}
			
			
			#region Commands...
			private void CommandStartReadOnly(Support.CommandDispatcher sender, Support.CommandEventArgs e)
			{
				this.StartReadOnly ();
			}
			
			private void CommandStartEdition(Support.CommandDispatcher sender, Support.CommandEventArgs e)
			{
				this.StartEdition ();
			}
			
			private void CommandStartSearch(Support.CommandDispatcher sender, Support.CommandEventArgs e)
			{
				this.StartSearch ();
			}
			
			private void CommandInsertBefore(Support.CommandDispatcher sender, Support.CommandEventArgs e)
			{
				this.InsertBefore ();
			}
			
			private void CommandInsertAfter(Support.CommandDispatcher sender, Support.CommandEventArgs e)
			{
				this.InsertAfter ();
			}
			
			private void CommandDelete(Support.CommandDispatcher sender, Support.CommandEventArgs e)
			{
				this.Delete ();
			}
			
			private void CommandMoveUp(Support.CommandDispatcher sender, Support.CommandEventArgs e)
			{
				this.MoveUp ();
			}
			
			private void CommandMoveDown(Support.CommandDispatcher sender, Support.CommandEventArgs e)
			{
				this.MoveDown ();
			}
			#endregion
			
			private void HandleHostInteractionModeChanged(object sender)
			{
				this.UpdateCommandStates ();
			}
			
			private void HandleHostSelectedIndexChanged(object sender)
			{
				this.UpdateCommandStates ();
			}
			
			private void HandleHostTextArrayStoreChanged(object sender)
			{
				this.UpdateStore ();
			}
			
			private void HandleHostContentsInvalidated(object sender)
			{
				this.UpdateStore ();
				this.UpdateCommandStates ();
			}
			
			
			protected virtual void UpdateStore()
			{
				this.store = this.host.TextArrayStore;
				
				if (this.store == null)
				{
					this.store = new EditArray.SelfStore (this.host);
				}
			}
			
			protected virtual void UpdateCommandStates()
			{
				int index  = this.host.SelectedIndex;
				
				WidgetState act_edition  = WidgetState.ActiveNo;
				WidgetState act_search   = WidgetState.ActiveNo;
				WidgetState act_readonly = WidgetState.ActiveNo;
				
				switch (this.host.InteractionMode)
				{
					case ScrollInteractionMode.ReadOnly: act_readonly = WidgetState.ActiveYes;	break;
					case ScrollInteractionMode.Edition:  act_edition = WidgetState.ActiveYes;	break;
					case ScrollInteractionMode.Search:   act_search = WidgetState.ActiveYes;	break;
				}
				
				bool ok_edit       = this.store.CheckSetRow (index);
				bool ok_ins_before = this.store.CheckInsertRows (index, 1);
				bool ok_ins_after  = this.store.CheckInsertRows (index+1, 1);
				bool ok_delete     = this.store.CheckRemoveRows (index, 1);
				bool ok_move_up    = this.store.CheckMoveRow (index, -1);
				bool ok_move_down  = this.store.CheckMoveRow (index, 1);
				
				this.UpdateCommandState ("StartReadOnly", true, act_readonly);
				this.UpdateCommandState ("StartEdition", ok_edit, act_edition);
				this.UpdateCommandState ("StartSearch", true, act_search);
				
				this.UpdateCommandState ("InsertBefore", ok_ins_before, WidgetState.ActiveNo);
				this.UpdateCommandState ("InsertAfter",  ok_ins_after,  WidgetState.ActiveNo);
				this.UpdateCommandState ("Delete",       ok_delete,     WidgetState.ActiveNo);
				this.UpdateCommandState ("MoveUp",       ok_move_up,    WidgetState.ActiveNo);
				this.UpdateCommandState ("MoveDown",     ok_move_down,  WidgetState.ActiveNo);
			}
			
			protected virtual void UpdateCommandState(string name, bool enabled, WidgetState active)
			{
				CommandState state = this.GetCommandState (name);
				
				state.Enabled     = enabled;
				state.ActiveState = active;
				state.Synchronise ();
			}
			
			
			protected EditArray					host;
			protected string					name;
			protected ToolTip					tips;
			Support.Data.ITextArrayStore		store;
		}
		#endregion
		
		#region SelfStore Class
		public class SelfStore : Support.Data.ITextArrayStore
		{
			public SelfStore(ScrollArray host)
			{
				this.host = host;
			}
			
			
			#region ITextArrayStore Members
			public int GetRowCount()
			{
				return this.host.RowCount;
			}

			public int GetColumnCount()
			{
				return this.host.ColumnCount;
			}

			
			public string GetCellText(int row, int column)
			{
				return this.host[row, column];
			}

			public void SetCellText(int row, int column, string value)
			{
				if (this.host[row, column] != value)
				{
					this.host[row, column] = value;
					this.OnStoreChanged ();
				}
			}

			
			public void InsertRows(int row, int num)
			{
				throw new System.NotSupportedException ("Cannot InsertRows.");
			}

			public void RemoveRows(int row, int num)
			{
				throw new System.NotSupportedException ("Cannot RemoveRows.");
			}

			public void MoveRow(int row, int distance)
			{
				int row_a = row;
				int row_b = row + distance;
				
				int n = this.host.ColumnCount;
				
				for (int i = 0; i < n; i++)
				{
					string a = this.host[row_a, i];
					string b = this.host[row_b, i];
					
					this.host[row_a, i] = b;
					this.host[row_b, i] = a;
				}
				
				this.OnStoreChanged ();
			}

			
			public bool CheckSetRow(int row)
			{
				return (row >= 0) && (row < this.host.RowCount);
			}

			public bool CheckInsertRows(int row, int num)
			{
				return false;
			}

			public bool CheckMoveRow(int row, int distance)
			{
				int max = this.host.RowCount;
				
				return (row >= 0) && (row < max) && (row + distance >= 0) && (row + distance < max);
			}

			public bool CheckRemoveRows(int row, int num)
			{
				return false;
			}

			public bool CheckEnabledCell(int row, int column)
			{
				return true;
			}
			
			
			public event Support.EventHandler	StoreChanged;
			#endregion
			
			protected virtual void OnStoreChanged()
			{
				if (this.StoreChanged != null)
				{
					this.StoreChanged (this);
				}
			}
			
			protected ScrollArray				host;
		}
		#endregion
		
		public event Support.EventHandler		EditTextChanged;
		public event Support.EventHandler		EditWidgetsCreated;
		
		protected EditWidget					edit_line    = null;
		protected Drawing.Rectangle				edit_bounds  = Drawing.Rectangle.Empty;
		protected double						edit_offset  = 0;
		protected double						edit_width   = 0;
		
		protected string						search_caption;
		protected int							focused_column = -1;
	}
}
