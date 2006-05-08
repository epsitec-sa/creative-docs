//	Copyright © 2003-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Behaviors
{
	/// <summary>
	/// La classe AcceptRejectBehavior gère les boutons pour accepter/rejeter une
	/// édition.
	/// </summary>
	public class AcceptRejectBehavior
	{
		public AcceptRejectBehavior(Widget host)
		{
			this.host = host;
		}
		
		
		public Widget							Host
		{
			get
			{
				return this.host;
			}
		}
		
		public double							DefaultWidth
		{
			get
			{
				if (this.IsVisible)
				{
					double height = this.host.ActualHeight;
					double width  = System.Math.Floor ((height - 4) * 15.0 / 17.0);
					
					return width + width - 1;
				}
				
				return 0;
			}
		}
		
		public bool								IsVisible
		{
			get
			{
				return this.is_visible;
			}
		}
		
		public bool								IsAcceptEnabled
		{
			get
			{
				return this.is_accept_enabled;
			}
		}
		
		public string							InitialText
		{
			get
			{
				return this.initial_text;
			}
			set
			{
				this.initial_text = value;
			}
		}
		
		
		public void SetVisible(bool visible)
		{
			this.is_visible = visible;
			
			if (this.button_accept != null)
			{
				this.button_accept.Visibility = (this.is_visible);
				this.button_reject.Visibility = (this.is_visible);
			}
		}
		
		public void SetAcceptEnabled(bool enable_accept)
		{
			this.is_accept_enabled = enable_accept;
			
			if (this.button_accept != null)
			{
				this.button_accept.Enable = this.is_accept_enabled;
			}
		}
		
		
		public void UpdateButtonGeometry()
		{
			AbstractTextField text = this.host as AbstractTextField;
			
			if ((text != null) &&
				(this.button_accept != null))
			{
				Drawing.Rectangle bounds = text.GetButtonBounds ();
				Drawing.Rectangle rect_1 = bounds;
				Drawing.Rectangle rect_2 = bounds;
				
				rect_1.Right = rect_1.Left + rect_1.Width / 2 + 0.5;
				rect_2.Left  = rect_1.Right - 1;
				
				this.button_accept.SetManualBounds(rect_1);
				this.button_reject.SetManualBounds(rect_2);
			}
		}
		
		
		public void CreateButtons()
		{
			System.Diagnostics.Debug.Assert (this.button_accept == null);
			System.Diagnostics.Debug.Assert (this.button_reject == null);
			
			this.button_accept = new GlyphButton(this.host);
			this.button_reject = new GlyphButton(this.host);
			
			IFeel feel = Feel.Factory.Active;
			
			this.button_accept.Name        = "Accept";
			this.button_accept.GlyphShape  = GlyphShape.Accept;
			this.button_accept.ButtonStyle = ButtonStyle.ExListMiddle;
			this.button_accept.Clicked    += new MessageEventHandler(this.HandleButtonAcceptClicked);
			this.button_accept.Shortcuts.Define (feel.AcceptShortcut);
			
			this.button_reject.Name        = "Reject";
			this.button_reject.GlyphShape  = GlyphShape.Reject;
			this.button_reject.ButtonStyle = ButtonStyle.ExListRight;
			this.button_reject.Clicked    += new MessageEventHandler(this.HandleButtonRejectClicked);
			this.button_reject.Shortcuts.Define (feel.CancelShortcut);
			
			this.SetVisible (this.is_visible);
			this.SetAcceptEnabled (this.is_accept_enabled);
		}
		
		
		private void HandleButtonAcceptClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.button_accept);
			this.OnAcceptClicked ();
		}		
		
		private void HandleButtonRejectClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.button_reject);
			this.OnRejectClicked ();
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
		
		
		public event Support.EventHandler		AcceptClicked;
		public event Support.EventHandler		RejectClicked;
		
		
		protected Widget						host;
		
		protected GlyphButton					button_accept;
		protected GlyphButton					button_reject;
		
		protected bool							is_visible;
		protected bool							is_accept_enabled = true;
		protected string						initial_text;
	}
}
