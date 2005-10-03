using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	using GroupController = Epsitec.Common.Widgets.Helpers.GroupController;
	
	/// <summary>
	/// La classe RadioIconGrid est un widget affichant une tableau de IconButton,
	/// dont un seul � la fois est s�lectionn�.
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
			
			this.controller = GroupController.GetGroupController(this, "GridGroup");
			this.controller.Changed += new Support.EventHandler(this.HandleRadioChanged);
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
					}
					items[i].Dispose();
				}
				
				this.controller.Changed -= new Support.EventHandler(this.HandleRadioChanged);
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
			icon.SetParent (this);
			icon.TabIndex = this.list.Count;
			icon.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			icon.Group = this.controller.Group;
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

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			if ( this.list == null )  return;

			Rectangle box = this.Client.Bounds;
			Point corner = this.Client.Bounds.TopLeft;
			int column = 0;
			int row = 0;

			foreach ( Widgets.RadioIcon icon in this.list )
			{
				System.Diagnostics.Debug.Assert(icon != null);

				Rectangle rect = new Rectangle(corner.X, corner.Y-this.defaultButtonHeight, this.defaultButtonWidth, this.defaultButtonHeight);
				icon.Bounds = rect;
				icon.Column = column;
				icon.Row = row;
				icon.Index = row * 1000 + column;
				icon.SetVisible(box.Contains(rect));

				corner.X += this.defaultButtonWidth;
				column ++;

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


		private void HandleRadioChanged(object sender)
		{
			System.Diagnostics.Debug.Assert(this.controller == sender);
			
			RadioIcon icon = this.controller.FindActiveWidget() as RadioIcon;
			
			if ( icon != null &&
				 this.selectedValue != icon.EnumValue )
			{
				this.selectedValue = icon.EnumValue;
				this.OnSelectionChanged();
			}
		}
		
		// G�n�re un �v�nement pour dire que la s�lection a chang�.
		protected virtual void OnSelectionChanged()
		{
			if ( this.SelectionChanged != null )  // qq'un �coute ?
			{
				this.SelectionChanged(this);
			}
		}

		
		public event Support.EventHandler SelectionChanged;

		protected GroupController				controller;
		protected System.Collections.ArrayList	list;
		protected int							selectedValue;
		protected double						defaultButtonWidth;
		protected double						defaultButtonHeight;
		protected bool							enableEndOfLine;
	}
}
