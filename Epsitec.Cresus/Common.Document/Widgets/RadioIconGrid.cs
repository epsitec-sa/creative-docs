using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe RadioIconGrid est un widget affichant une tableau de IconButton,
	/// dont un seul à la fois est sélectionné.
	/// </summary>
	public class RadioIconGrid : Widget
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
			ToolTip.Default.SetToolTip(icon, tooltip);
			this.list.Add(icon);
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

			Point corner = this.Client.Bounds.TopLeft;

			foreach ( Widgets.RadioIcon icon in this.list )
			{
				System.Diagnostics.Debug.Assert(icon != null);

				Rectangle rect = new Rectangle(corner.X, corner.Y-this.defaultButtonHeight, this.defaultButtonWidth, this.defaultButtonHeight);
				icon.Bounds = rect;

				corner.X += this.defaultButtonWidth;

				if ( corner.X > this.Client.Bounds.Right-this.defaultButtonWidth ||
					 (icon.EndOfLine && this.enableEndOfLine) )
				{
					corner.X = this.Client.Bounds.Left;
					corner.Y -= this.defaultButtonHeight;
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
