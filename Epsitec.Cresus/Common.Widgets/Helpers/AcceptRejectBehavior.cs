namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// La classe AcceptRejectBehavior g�re les boutons pour accepter/rejeter une
	/// �dition.
	/// </summary>
	public class AcceptRejectBehavior
	{
		public AcceptRejectBehavior(Widget host)
		{
			this.host = host;
			this.CreateButtons ();
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
					double height = this.host.Height;
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
		
		public void SetVisible(bool visible)
		{
			this.is_visible = visible;
			
			this.button_accept.SetVisible (visible);
			this.button_reject.SetVisible (visible);
		}
		
		public void SetEnabledOk(bool enable_accept)
		{
			this.button_accept.SetEnabled (enable_accept);
		}
		
		
		public void UpdateButtonGeometry()
		{
			AbstractTextField text = this.host as AbstractTextField;
			
			if (text != null)
			{
				Drawing.Rectangle bounds = text.GetButtonBounds ();
				Drawing.Rectangle rect_1 = bounds;
				Drawing.Rectangle rect_2 = bounds;
				
				rect_1.Right = rect_1.Left + rect_1.Width / 2 + 0.5;
				rect_2.Left  = rect_1.Right - 1;
				
				this.button_accept.Bounds = rect_1;
				this.button_reject.Bounds = rect_2;
			}
		}
		
		
		protected void CreateButtons()
		{
			this.button_accept = new GlyphButton(this.host);
			this.button_reject = new GlyphButton(this.host);
			
			IFeel feel = Feel.Factory.Active;
			
			this.button_accept.Name        = "Accept";
			this.button_accept.GlyphShape  = GlyphShape.Validate;
			this.button_accept.ButtonStyle = ButtonStyle.ExListMiddle;
			this.button_accept.Clicked    += new MessageEventHandler(this.HandleButtonOkClicked);
			this.button_accept.Shortcut    = feel.AcceptShortcut;
			
			this.button_reject.Name        = "Reject";
			this.button_reject.GlyphShape  = GlyphShape.Cancel;
			this.button_reject.ButtonStyle = ButtonStyle.ExListRight;
			this.button_reject.Clicked    += new MessageEventHandler(this.HandleButtonRejectClicked);
			this.button_reject.Shortcut    = feel.CancelShortcut;
		}
		
		
		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
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
	}
}
