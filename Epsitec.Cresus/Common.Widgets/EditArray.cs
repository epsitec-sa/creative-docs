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
						this.InnerTopMargin = this.edit_line.DesiredHeight;
						this.edit_line.DetachEditWidgets ();
						this.edit_line.ColumnCount = this.max_columns;
					}
					else
					{
						this.InnerTopMargin = 0;
						this.edit_line.DetachEditWidgets ();
						this.edit_line.ColumnCount = this.max_columns;
					}
					
					this.RefreshContents ();
					this.OnEditArrayModeChanged ();
				}
			}
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
		
		public void StartEdition(int row, int column)
		{
			this.EditionIndex  = row;
			this.SelectedIndex = -1;
			
			if (row > -1)
			{
				column = System.Math.Max (column, 0);
				column = System.Math.Min (column, this.max_columns-1);
			
				this.ShowEdition (ScrollShowMode.Extremity);
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
					this.edit_line.UpdateGeometry ();
				}
				else if (this.edit_line.IsVisible)
				{
					if (this.edit_line.ContainsFocus)
					{
						this.SetFocused (true);
					}
					
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
			
			Drawing.Rectangle cell = Drawing.Rectangle.Empty;
			
			switch (this.mode)
			{
				case EditArrayMode.Standard:
					break;
				case EditArrayMode.Edition:
					cell = this.GetUnclippedCellBounds (this.EditionIndex, column);
					break;
				case EditArrayMode.Search:
					if (this.GetUnclippedCellX (column, out x1, out x2))
					{
						cell = new Drawing.Rectangle (x1, this.inner_bounds.Top, x2-x1, this.table_bounds.Top - this.inner_bounds.Top);
					}
					break;
			}
			
			return cell;
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
		
		protected virtual  void OnSearchCaptionChanged()
		{
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
			
			
			public double						DesiredHeight
			{
				get
				{
					if (this.caption == null)
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
				if ((message.Type == MessageType.KeyPress) &&
					(message.IsCtrlPressed == false) &&
					(message.IsShiftPressed == false) &&
					(message.IsAltPressed == false))
				{
					if (this.host.mode == EditArrayMode.Search)
					{
						switch (message.KeyCode)
						{
							case KeyCode.Escape:
								this.host.EditArrayMode = EditArrayMode.Standard;
								this.host.SetFocused (true);
								break;
							
							case KeyCode.Return:
								this.host.OnEditTextChanged ();
								break;
							
							default:
								if (this.host.ProcessKeyEvent (message))
								{
									//	L'événement a été traité par la liste; on va donc le consommer.
								}
								else
								{
									base.ProcessMessage (message, pos);
									return;
								}
								break;
						}
						
						message.Consumer = this;
						return;
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
				TextFieldStyle style = (this.host.mode == EditArrayMode.Search ? TextFieldStyle.Normal : TextFieldStyle.Flat);
				
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
							this.edit_widgets[i].TextFieldStyle = style;
						}
						
						this.Attach (this.edit_widgets[i], i);
					}
				}
			}
			
			public void DetachEditWidgets()
			{
				this.ColumnCount = 0;
			}
			
			public void UpdateGeometry()
			{
				double height = this.LineHeight;
				double ox = -this.Bounds.X;
				double oy = -this.Bounds.Y;
				
				if (this.caption != null)
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
						cell.Inflate (0, 1, 0, 1);
						cell.Height = height;
						
						this.edit_widgets[i].Bounds = cell;
						this.edit_widgets[i].SetVisible (true);
					}
					else
					{
						this.edit_widgets[i].SetVisible (false);
					}
				}
			}
			
			public void UpdateCaption()
			{
				string caption = null;
				
				if (this.host.mode == EditArrayMode.Search)
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
						this.host.InnerTopMargin = this.LineHeight;
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
					
					this.host.InnerTopMargin = this.DesiredHeight;
				}
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
		
		
		public event Support.EventHandler		EditArrayModeChanged;
		public event Support.EventHandler		EditTextChanged;
		
		protected int							edit_active  = -1;
		protected EditWidget					edit_line    = null;
		protected Drawing.Rectangle				edit_bounds  = Drawing.Rectangle.Empty;
		protected double						edit_offset  = 0;
		protected double						edit_width   = 0;
		
		protected EditArrayMode					mode = EditArrayMode.Standard;
		protected string						search_caption;
	}
	
	public enum EditArrayMode
	{
		Standard,
		Edition,
		Search
	}
}
