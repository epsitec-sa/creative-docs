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
		
		
		public void StartEdition(int row, int column)
		{
			this.EditionIndex = row;
			
			if (row > -1)
			{
				column = System.Math.Max (column, 0);
				column = System.Math.Min (column, this.max_columns-1);
			
				this.ShowEdition (ScrollArrayShowMode.Extremity);
				this.Update ();
				this.DispatchDummyMouseMoveEvent ();
				this.edit_widgets[column].SetFocused (true);
				this.edit_widgets[column].SelectAll ();
			}
			
			this.Invalidate ();
		}
		
		protected override void UpdateColumnCount()
		{
			base.UpdateColumnCount ();
			
			if (this.edit_widgets.Length != this.max_columns)
			{
				AbstractTextField[] widgets = new AbstractTextField[this.max_columns];
				
				int n = System.Math.Min (this.max_columns, this.edit_widgets.Length);
				
				for (int i = 0; i < n; i++)
				{
					widgets[i] = this.edit_widgets[i];
				}
				
				for (int i = 0; i < this.max_columns; i++)
				{
					if (widgets[i] == null)
					{
						widgets[i] = new TextFieldMulti (this.edit_line);
						widgets[i].TabIndex = i;
						widgets[i].TabNavigation = TabNavigationMode.ActivateOnTab;
					}
				}
				
				this.edit_widgets = widgets;
			}
		}
		
		protected override void UpdateScrollView()
		{
			base.UpdateScrollView ();
			
			Drawing.Rectangle bounds = this.GetRowBounds (this.edition_row);
			
			if ((this.edit_bounds != bounds) ||
				(this.edit_offset != this.offset))
			{
				this.edit_bounds = bounds;
				this.edit_offset = offset;
				
				if (this.edit_bounds.IsValid)
				{
					bounds.Inflate (new Drawing.Margins (-1, -1, 0, 1));
					
					this.edit_line.Bounds = bounds;
					this.edit_line.SetVisible (true);
					
					double max_x = bounds.Width;
					
					for (int i = 0; i < this.max_columns; i++)
					{
						Drawing.Rectangle cell = this.GetUnclippedCellBounds (this.edition_row, i);
						
						if (cell.IsValid)
						{
							cell.Offset (- this.edit_bounds.X, - this.edit_bounds.Y);
							cell.Inflate (new Drawing.Margins (1, 0, 1, 0));
							
							this.edit_widgets[i].Bounds = cell;
							this.edit_widgets[i].SetVisible (true);
						}
						else
						{
							this.edit_widgets[i].SetVisible (false);
						}
					}
				}
				else if (this.edit_line.IsVisible)
				{
					this.edit_line.SetVisible (false);
				}
			}
		}
		
		protected override void OnEditionIndexChanged()
		{
			base.OnEditionIndexChanged ();
			
			if (this.edition_row != this.edit_active)
			{
				if (this.edit_active > -1)
				{
					string[] values = new string[this.edit_widgets.Length];
					
					for (int i = 0; i < this.edit_widgets.Length; i++)
					{
						values[i] = this.edit_widgets[i].Text;
					}
					
					this.SetRowTexts (this.edit_active, values);
				}
				
				this.edit_active = this.edition_row;
				
				if (this.edition_row > -1)
				{
					string[] values = this.GetRowTexts (this.edition_row);
					
					for (int i = 0; i < this.max_columns; i++)
					{
						this.edit_widgets[i].Text = values[i];
					}
				}
			}
		}
		
		protected bool MoveEditionToLine(int offset)
		{
			int row = this.edition_row + offset;
			
			row = System.Math.Min (row, this.max_rows-1);
			row = System.Math.Max (row, 0);
			
			if (this.edition_row != row)
			{
				this.EditionIndex = row;
				return true;
			}
			
			return false;
		}
		
		
		protected class EditWidget : Widget
		{
			public EditWidget(EditArray host)
			{
				this.host = host;
				
				this.SetEmbedder (this.host);
				this.TabNavigation = TabNavigationMode.ActivateOnTab | TabNavigationMode.ForwardToChildren | TabNavigationMode.ForwardOnly;
			}
			
			protected override bool ProcessTabChildrenExit(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
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
						focus = this.host.edit_widgets[0];
					}
					else
					{
						focus = this.host.edit_widgets[this.host.edit_widgets.Length-1];
					}
				}
				else
				{
					focus = null;
				}
				
				return true;
			}
			
			protected EditArray					host;
		}
		
		
		protected int							edit_active  = -1;
		protected AbstractTextField[]			edit_widgets = new AbstractTextField[0];
		protected EditWidget					edit_line    = null;
		protected Drawing.Rectangle				edit_bounds  = Drawing.Rectangle.Empty;
		protected double						edit_offset  = 0;
	}
}
