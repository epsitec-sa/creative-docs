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
			this.edit_line.SetVisible (false);
		}
		
		public EditArray(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public EditArrayMode					EditArrayMode
		{
			get
			{
				return this.mode;
			}
			set
			{
				if (this.mode != value)
				{
					this.mode = value;
					
					if (this.mode == EditArrayMode.Search)
					{
						double height = System.Math.Floor (this.EditionZoneHeight * this.row_height + 2);
						
						this.InnerTopMargin = height;
					}
					else
					{
						this.InnerTopMargin = 0;
					}
					
					this.RefreshContents ();
					this.OnEditArrayModeChanged ();
				}
			}
		}
		
		
		public void StartEdition(int row, int column)
		{
			this.EditionIndex  = row;
			this.SelectedIndex = -1;
			
			if (row > -1)
			{
				column = System.Math.Max (column, 0);
				column = System.Math.Min (column, this.max_columns-1);
			
				this.ShowEdition (ScrollArrayShowMode.Extremity);
				this.Update ();
				this.DispatchDummyMouseMoveEvent ();
				this.edit_line.FocusColumn (column);
			}
			
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
			if (this.edit_active > -1)
			{
				this.edit_line.Values = this.GetRowTexts (this.edit_active);
				
				if (finished)
				{
					this.SelectedIndex = this.edit_active;
					this.EditionIndex  = -1;
				}
			}
		}
		
		public virtual void ValidateEdition(bool finished)
		{
			if (this.edit_active > -1)
			{
				this.SetRowTexts (this.edit_active, this.edit_line.Values);
				
				if (finished)
				{
					this.SelectedIndex = this.edit_active;
					this.EditionIndex = -1;
				}
			}
		}

		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.edit_line.DetachEditWidgets ();
				
				this.max_columns  = 0;
			}
			
			base.Dispose (disposing);
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
					this.edit_line.SetVisible (true);
					
					double max_x = bounds.Width;
					
					for (int i = 0; i < this.max_columns; i++)
					{
						Drawing.Rectangle cell = this.GetEditCellBounds (i);
						
						if (cell.IsValid)
						{
							cell.Offset (- this.edit_bounds.X, - this.edit_bounds.Y);
							cell.Inflate (new Drawing.Margins (1, 0, 1, 0));
							
							this.edit_line[i].Bounds = cell;
							this.edit_line[i].SetVisible (true);
						}
						else
						{
							this.edit_line[i].SetVisible (false);
						}
					}
				}
				else if (this.edit_line.IsVisible)
				{
					this.edit_line.SetVisible (false);
				}
			}
		}
		
		
		protected virtual Drawing.Rectangle GetEditBounds()
		{
			switch (this.mode)
			{
				case EditArrayMode.Standard:
					break;
				case EditArrayMode.Edition:
					return this.GetRowBounds (this.EditionIndex);
				case EditArrayMode.Search:
					return new Drawing.Rectangle (this.inner_bounds.Left, this.inner_bounds.Top, this.inner_bounds.Width, this.table_bounds.Height - this.inner_bounds.Height);
			}
			
			return Drawing.Rectangle.Empty;
		}
		
		protected virtual Drawing.Rectangle GetEditCellBounds(int column)
		{
			double x1;
			double x2;
			
			switch (this.mode)
			{
				case EditArrayMode.Standard:
					break;
				case EditArrayMode.Edition:
					return this.GetUnclippedCellBounds (this.EditionIndex, column);
				case EditArrayMode.Search:
					if (this.GetUnclippedCellX (column, out x1, out x2))
					{
						return new Drawing.Rectangle (x1, this.inner_bounds.Top, x2 - x1, this.table_bounds.Height - this.inner_bounds.Height);
					}
					break;
			}
			
			return Drawing.Rectangle.Empty;
		}
		

		
		protected override void OnEditionIndexChanged()
		{
			base.OnEditionIndexChanged ();
			
			if (this.EditionIndex != this.edit_active)
			{
				this.ValidateEdition (false);
				
				this.edit_active   = this.EditionIndex;
				this.EditArrayMode = (this.edit_active < 0 ? EditArrayMode.Standard : EditArrayMode.Edition);
				
				this.ReloadEdition ();
			}
		}
		
		protected virtual  void OnEditArrayModeChanged()
		{
			if (this.EditArrayModeChanged != null)
			{
				this.EditArrayModeChanged (this);
			}
		}
		
		protected virtual  void OnEditTextChanged()
		{
			if (this.EditTextChanged != null)
			{
				this.EditTextChanged (this);
			}
		}
		
		
		protected bool MoveEditionToLine(int offset)
		{
			int row = this.EditionIndex + offset;
			
			row = System.Math.Min (row, this.max_rows-1);
			row = System.Math.Max (row, 0);
			
			if (this.EditionIndex != row)
			{
				this.EditionIndex = row;
				return true;
			}
			
			return false;
		}
		
		
		#region EditWidget class
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
				this.TabNavigation = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
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
						this.edit_widgets[i].Text = value[i];
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
			
			
			protected override bool ProcessTabChildrenExit(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
			{
				if (this.host.mode == EditArrayMode.Search)
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
					int move = 0;
					
					switch (dir)
					{
						case TabNavigationDir.Forwards:  move =  1; break;
						case TabNavigationDir.Backwards: move = -1; break;
					}
					
					if (this.host.MoveEditionToLine (move))
					{
						if (move > 0)
						{
							focus = this.edit_widgets[0];
						}
						else
						{
							focus = this.edit_widgets[this.edit_widgets.Length-1];
						}
					}
					else
					{
						focus = null;
					}
				}
				
				return true;
			}
			
			protected override void ProcessMessage(Message message, Drawing.Point pos)
			{
				if (message.Type == MessageType.KeyPress)
				{
					if (this.host.mode == EditArrayMode.Search)
					{
						if (message.KeyCode == KeyCode.Escape)
						{
							this.host.EditArrayMode = EditArrayMode.Standard;
							this.host.SetFocused (true);
							message.Consumer = this;
							return;
						}
					}
					else
					{
						if (message.KeyCode == KeyCode.Escape)
						{
							this.host.CancelEdition ();
							this.host.SetFocused (true);
							message.Consumer = this;
							return;
						}
						if (message.KeyCode == KeyCode.Return)
						{
							this.host.ValidateEdition ();
							this.host.SetFocused (true);
							message.Consumer = this;
							return;
						}
					}
				}
				
				base.ProcessMessage (message, pos);
			}
			
			
			public void FocusColumn(int column)
			{
				this.edit_widgets[column].SetFocused (true);
				this.edit_widgets[column].SelectAll ();
			}
			
			public void AttachEditWidgets()
			{
				for (int i = 0; i < this.edit_widgets.Length; i++)
				{
					if (this.edit_widgets[i] == null)
					{
						if (this.host.edition_add_rows > 0)
						{
							this.edit_widgets[i] = new TextFieldMulti (this);
						}
						else
						{
							this.edit_widgets[i] = new TextField (this);
							this.edit_widgets[i].TextFieldStyle = TextFieldStyle.Flat;
						}
						
						this.Attach (this.edit_widgets[i], i);
					}
				}
			}
			
			public void DetachEditWidgets()
			{
				this.ColumnCount = 0;
			}
			
			
			protected void Attach(AbstractTextField widget, int i)
			{
				widget.Index         = i;
				widget.TabIndex      = i;
				widget.TabNavigation = TabNavigationMode.ActivateOnTab;
				widget.Focused      += new Support.EventHandler (this.HandleEditArrayFocused);
				widget.TextChanged  += new Epsitec.Common.Support.EventHandler (this.HandleTextChanged);
			}
			
			protected void Detach(AbstractTextField widget)
			{
				if (widget != null)
				{
					widget.Focused     -= new Support.EventHandler (this.HandleEditArrayFocused);
					widget.TextChanged -= new Epsitec.Common.Support.EventHandler (this.HandleTextChanged);
					widget.Parent       = null;
					widget.Dispose ();
					widget = null;
				}
			}
			
			
			private void HandleEditArrayFocused(object sender)
			{
				Widget widget = sender as Widget;
				
				int row    = this.host.EditionIndex;
				int column = widget.Index;
				
				this.host.ShowCell (ScrollArrayShowMode.Extremity, row, column);
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

		}
		#endregion
		
		
		public event Support.EventHandler		EditArrayModeChanged;
		public event Support.EventHandler		EditTextChanged;
		
		protected int							edit_active  = -1;
		protected EditWidget					edit_line    = null;
		protected Drawing.Rectangle				edit_bounds  = Drawing.Rectangle.Empty;
		protected double						edit_offset  = 0;
		protected double						edit_width   = 0;
		
		protected EditArrayMode					mode = EditArrayMode.Standard;
	}
	
	public enum EditArrayMode
	{
		Standard,
		Edition,
		Search
	}
}
