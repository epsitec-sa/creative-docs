//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.Designer.Widgets
{
	using IStringCollectionHost = Epsitec.Common.Widgets.Helpers.IStringCollectionHost;
	using StringCollection      = Epsitec.Common.Widgets.Helpers.StringCollection;
	
	public enum SwitcherMode
	{
		None,
		Select,
		AcceptReject,
	}
	
	/// <summary>
	/// Summary description for Switcher.
	/// </summary>
	public class Switcher : Widget, IStringCollectionHost, Support.Data.INamedStringSelection
	{
		public Switcher()
		{
			this.items = new StringCollection (this);
			
			this.CreateCaption ();
			this.CreateButtons ();
			this.UpdateButtons ();
		}
		
		public Switcher(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		
		public override double					DefaultHeight
		{
			get
			{
				return 32;
			}
		}
		
		public override Drawing.Margins			ExtraPadding
		{
			get
			{
				Drawing.Margins padding = base.ExtraPadding;
				return padding + new Drawing.Margins (2, 2, 2, 2);
			}
		}
		
		public SwitcherMode						Mode
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
					this.UpdateButtons ();
				}
			}
		}
		
		public Support.EventHandler				AcceptHandler
		{
			get
			{
				return this.accept_handler;
			}
			set
			{
				this.accept_handler = value;
			}
		}
		
		public bool								IsAcceptEnabled
		{
			get
			{
				return this.button_accept.IsEnabled;
			}
			set
			{
				this.button_accept.SetEnabled (value);
			}
		}
		
		public bool								IsRejectEnabled
		{
			get
			{
				return this.button_reject.IsEnabled;
			}
			set
			{
				this.button_reject.SetEnabled (value);
			}
		}
		
		
		#region IStringCollectionHost Members
		public void StringCollectionChanged()
		{
		}
		
		
		public StringCollection					Items
		{
			get
			{
				return this.items;
			}
		}
		#endregion
		
		#region INamedStringSelection Members
		public int									SelectedIndex
		{
			get
			{
				return this.items.FindExactMatch (this.Text);
			}
			set
			{
				if ((value >= 0) &&
					(value < this.items.Count))
				{
					if (this.Text != this.items[value])
					{
						this.Text = this.items[value];
						this.OnSelectedIndexChanged();
					}
				}
				else
				{
					throw new System.ArgumentOutOfRangeException ("value", value, "Specified index is out of range.");
				}
			}
		}
		
		public string								SelectedItem
		{
			get
			{
				int index = this.SelectedIndex;
				return (index < 0) ? "" : this.items[index];
			}
			set
			{
				this.SelectedIndex = this.items.IndexOf (value);
			}
		}
		
		public string								SelectedName
		{
			get
			{
				int index = this.SelectedIndex;
				return (index < 0) ? "" : this.items.GetName (index);
			}
			set
			{
				this.SelectedIndex = this.items.FindNameIndex(value);
			}
		}
		
		
		public event Support.EventHandler			SelectedIndexChanged;
		#endregion
		
		protected void CreateCaption()
		{
			this.caption = new StaticText (this);
			this.caption.Dock = DockStyle.Fill;
			this.caption.SetClientZoom (1.2);
		}
		
		protected void CreateButtons()
		{
			this.button_accept = this.CreateButton ("Accept", GlyphShape.Accept);
			this.button_reject = this.CreateButton ("Reject", GlyphShape.Reject);
			this.button_select = this.CreateButton ("Select", GlyphShape.ArrowDown);
			
			this.button_accept.Clicked += new MessageEventHandler (this.HandleAcceptClicked);
			this.button_reject.Clicked += new MessageEventHandler (this.HandleRejectClicked);
			this.button_select.Pressed += new MessageEventHandler (this.HandleSelectPressed);
		}
		
		protected void UpdateButtons()
		{
			switch (this.Mode)
			{
				case SwitcherMode.None:
					this.button_accept.SetVisible (false);
					this.button_reject.SetVisible (false);
					this.button_select.SetVisible (false);
					break;
				
				case SwitcherMode.AcceptReject:
					this.button_accept.SetVisible (true);
					this.button_reject.SetVisible (true);
					this.button_select.SetVisible (false);
					break;
				
				case SwitcherMode.Select:
					this.button_accept.SetVisible (false);
					this.button_reject.SetVisible (false);
					this.button_select.SetVisible (true);
					break;
					
				default:
					throw new System.NotImplementedException (string.Format ("Mode {0} not implemented.", this.Mode));
			}
		}
		
		
		protected Button CreateButton(string name, GlyphShape shape)
		{
			GlyphButton button = new GlyphButton (this);
			
			double dim = this.DefaultHeight - 8;
			
			button.Dock        = DockStyle.Right;
			button.DockMargins = new Drawing.Margins (2, 2, 2, 2);
			button.Name        = name;
			button.GlyphShape  = shape;
			button.Size        = new Drawing.Size (dim, dim);
			button.ButtonStyle = ButtonStyle.ToolItem;
			
			return button;
		}
		
		
		protected override void OnTextChanged()
		{
			base.OnTextChanged ();
			
			if (this.caption != null)
			{
				this.caption.Text = this.Text;
			}
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			Drawing.Rectangle rect = this.Client.Bounds;
			
			rect.Deflate (1);
			
			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (Drawing.Color.FromRGB (0.60, 0.80, 1.00));
			
			rect.Inflate (0.5);
			
			graphics.LineWidth = 1;
			graphics.RenderSolid (Drawing.Color.FromRGB (0.40, 0.53, 0.65));
		}
		
		
		private void HandleAcceptClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.button_accept == sender);
			
			if (e.Message.Button == MouseButtons.Left)
			{
				this.OnAcceptClicked ();
			}
		}
		
		private void HandleRejectClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.button_reject == sender);
			
			if (e.Message.Button == MouseButtons.Left)
			{
				this.OnRejectClicked ();
			}
		}
		
		private void HandleSelectPressed(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (this.button_select == sender);
			
			VMenu      menu  = new VMenu ();
			MenuItem[] items = new MenuItem[this.items.Count];
			
			int selected = this.SelectedIndex;
			
			for (int i = 0; i < this.items.Count; i++)
			{
				items[i] = MenuItem.CreateYesNo ("", this.items[i], "", this.items.GetName (i));
				items[i].ActiveState = (i == selected) ? WidgetState.ActiveYes : WidgetState.ActiveNo;
				menu.Items.Add (items[i]);
			}
			
			menu.Host = this;
			menu.AdjustSize ();
			
			Drawing.Point pos = this.button_select.MapClientToScreen (new Drawing.Point (this.button_select.Client.Width, 0));
			
			pos.X -= menu.Width;
			
			menu.Accepted += new MenuEventHandler(this.HandleMenuAccepted);
			menu.ShowAsContextMenu (this.Window, pos);
		}
		
		private void HandleMenuAccepted(object sender, MenuEventArgs e)
		{
			VMenu menu = sender as VMenu;
			this.SelectedName = e.MenuItem.Name;
		}
		
		
		
		protected virtual void OnSelectedIndexChanged()
		{
			if (this.SelectedIndexChanged != null)
			{
				this.SelectedIndexChanged (this);
			}
		}
		
		protected virtual void OnAcceptClicked()
		{
			if (this.AcceptClicked != null)
			{
				this.AcceptClicked (this);
			}
		}
		
		protected virtual void OnRejectClicked()
		{
			if (this.RejectClicked != null)
			{
				this.RejectClicked (this);
			}
		}
		
		
		public Support.EventHandler				AcceptClicked;
		public Support.EventHandler				RejectClicked;
		
		protected StaticText					caption;
		
		protected Button						button_accept;
		protected Button						button_reject;
		protected Button						button_select;
		
		private SwitcherMode					mode;
		private StringCollection				items;
		private Support.EventHandler			accept_handler;
	}
}
