using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe RadioIconGrid est un widget affichant une tableau de IconButton,
	/// dont un seul à la fois est sélectionné.
	/// </summary>
	public class RadioIconGrid : AbstractGroup
	{
		public RadioIconGrid()
		{
			this.list = new System.Collections.ArrayList();
			this.selectedValue = -1;
			this.enableEndOfLine = true;

			using ( IconButton button = new IconButton() )
			{
				this.defaultButtonWidth  = button.DefaultWidth;
				this.defaultButtonHeight = button.DefaultHeight;
			}
		}

		public RadioIconGrid(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				Widget[] items = new Widget[this.list.Count];
				
				this.list.CopyTo(items, 0);
				this.list.Clear();
				
				for ( int i=0 ; i<items.Length ; i++ )
				{
					if ( items[i] is RadioIcon )
					{
						RadioIcon icon = items[i] as RadioIcon;
						icon.Clicked -= new MessageEventHandler(this.HandleIconClicked);
					}
					items[i].Dispose();
				}
				
				this.list = null;
			}
			
			base.Dispose(disposing);
		}

		public void AddRadioIcon(string iconText, string tooltip, int enumValue, bool endOfLine)
		{
			Widgets.RadioIcon icon = new Widgets.RadioIcon();
			icon.IconName = Misc.Icon(iconText);
			icon.EnumValue = enumValue;
			icon.EndOfLine = endOfLine;
			icon.Parent = this;
			icon.Clicked += new MessageEventHandler(this.HandleIconClicked);
			icon.TabIndex = this.list.Count;
			icon.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(icon, tooltip);
			this.list.Add(icon);
		}

		public RadioIcon SelectedRadioIcon
		{
			get
			{
				foreach ( RadioIcon icon in this.list )
				{
					if ( icon == null )  continue;

					if ( icon.EnumValue == this.selectedValue )
					{
						return icon;
					}
				}
				
				return null;
			}
		}

		public int SelectedValue
		{
			get
			{
				return this.selectedValue;
			}

			set
			{
				if ( this.selectedValue != value )
				{
					this.selectedValue = value;

					foreach ( RadioIcon icon in this.list )
					{
						if ( icon == null )  continue;

						if ( icon.EnumValue == this.selectedValue )
						{
							icon.ActiveState = WidgetState.ActiveYes;
						}
						else
						{
							icon.ActiveState = WidgetState.ActiveNo;
						}
					}

					this.OnSelectionChanged();
				}
			}
		}

		public bool EnableEndOfLine
		{
			get
			{
				return this.enableEndOfLine;
			}

			set
			{
				this.enableEndOfLine = value;
			}
		}

		// Détermine quel widget il faut activer, en fonction de la
		// ligne et de la colonne où l'on se trouve.
		public bool Navigate(RadioIcon icon, KeyCode key)
		{
			Widgets.RadioIcon dest;

			switch ( key )
			{
				case KeyCode.ArrowUp:
					dest = this.Search(icon.Column, icon.Row-1);
					if ( dest != null )
					{
						this.SelectedValue = dest.EnumValue;
						dest.Focus();
						return true;
					}
					return false;

				case KeyCode.ArrowDown:
					dest = this.Search(icon.Column, icon.Row+1);
					if ( dest != null )
					{
						this.SelectedValue = dest.EnumValue;
						dest.Focus();
						return true;
					}
					return false;

				case KeyCode.ArrowLeft:
					dest = this.Search(icon.Column-1, icon.Row);
					if ( dest != null )
					{
						this.SelectedValue = dest.EnumValue;
						dest.Focus();
						return true;
					}
					dest = this.Search(icon.Rank-1);
					if ( dest != null )
					{
						this.SelectedValue = dest.EnumValue;
						dest.Focus();
						return true;
					}
					return false;

				case KeyCode.ArrowRight:
					dest = this.Search(icon.Column+1, icon.Row);
					if ( dest != null )
					{
						this.SelectedValue = dest.EnumValue;
						dest.Focus();
						return true;
					}
					dest = this.Search(icon.Rank+1);
					if ( dest != null )
					{
						this.SelectedValue = dest.EnumValue;
						dest.Focus();
						return true;
					}
					return false;
				
				default:
					return false;
			}
		}

		protected Widgets.RadioIcon Search(int column, int row)
		{
			foreach ( Widgets.RadioIcon icon in this.list )
			{
				if ( !icon.IsVisible )  continue;
				if ( icon.Column == column && icon.Row == row )  return icon;
			}

			return null;
		}

		protected Widgets.RadioIcon Search(int rank)
		{
			foreach ( Widgets.RadioIcon icon in this.list )
			{
				if ( !icon.IsVisible )  continue;
				if ( icon.Rank == rank )  return icon;
			}

			return null;
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			if ( this.list == null )  return;

			Rectangle box = this.Client.Bounds;
			Point corner = this.Client.Bounds.TopLeft;
			int column = 0;
			int row = 0;
			int rank = 0;

			foreach ( Widgets.RadioIcon icon in this.list )
			{
				System.Diagnostics.Debug.Assert(icon != null);

				Rectangle rect = new Rectangle(corner.X, corner.Y-this.defaultButtonHeight, this.defaultButtonWidth, this.defaultButtonHeight);
				icon.Bounds = rect;
				icon.Column = column;
				icon.Row = row;
				icon.Rank = rank;
				icon.SetVisible(box.Contains(rect));

				corner.X += this.defaultButtonWidth;
				column ++;
				rank ++;

				if ( corner.X > this.Client.Bounds.Right-this.defaultButtonWidth ||
					 (icon.EndOfLine && this.enableEndOfLine) )
				{
					corner.X = this.Client.Bounds.Left;
					corner.Y -= this.defaultButtonHeight;
					column = 0;
					row ++;
				}
			}
		}

		private void HandleIconClicked(object sender, MessageEventArgs e)
		{
			RadioIcon icon = sender as RadioIcon;
			if ( icon != null )
			{
				this.SelectedValue = icon.EnumValue;
			}
		}

		// Génère un événement pour dire que la sélection a changé.
		protected virtual void OnSelectionChanged()
		{
			if ( this.SelectionChanged != null )  // qq'un écoute ?
			{
				this.SelectionChanged(this);
			}
		}

		
		public event Support.EventHandler SelectionChanged;

		protected System.Collections.ArrayList	list;
		protected int							selectedValue;
		protected double						defaultButtonWidth;
		protected double						defaultButtonHeight;
		protected bool							enableEndOfLine;
	}
}
