namespace Epsitec.Common.Widgets.Helpers
{
	/// <summary>
	/// La classe AcceptCancelBehavior gère les boutons pour accepter/annuler un
	/// choix.
	/// </summary>
	public class AcceptCancelBehavior
	{
		public AcceptCancelBehavior(Widget host)
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
				return this.button_ok.DefaultWidth + this.button_cancel.DefaultWidth;
			}
		}
		
		public void SetVisible(bool visible)
		{
			this.button_ok.SetVisible (visible);
			this.button_cancel.SetVisible (visible);
		}
		
		public void SetEnabledOk(bool enable_ok)
		{
			this.button_ok.SetEnabled (enable_ok);
		}
		
		
		public void UpdateButtonGeometry()
		{
			AbstractTextField text = this.host as AbstractTextField;
			
			if (text != null)
			{
				Drawing.Rectangle bounds = text.GetButtonBounds ();
				Drawing.Rectangle rect_1 = bounds;
				Drawing.Rectangle rect_2 = bounds;
				
				rect_1.Right = rect_1.Left + rect_1.Width / 2;
				rect_2.Left  = rect_1.Right;
				
				this.button_ok.Bounds     = rect_1;
				this.button_cancel.Bounds = rect_2;
			}
		}
		
		
		protected void CreateButtons()
		{
			this.button_ok     = new GlyphButton(this.host);
			this.button_cancel = new GlyphButton(this.host);
			
			IFeel feel = Feel.Factory.Active;
			
			this.button_ok.Name        = "OK";
			this.button_ok.GlyphShape  = GlyphShape.Validate;
			this.button_ok.ButtonStyle = ButtonStyle.ExListMiddle;
			this.button_ok.Clicked    += new MessageEventHandler(this.HandleButtonOkClicked);
			this.button_ok.Shortcut    = feel.AcceptShortcut;
			
			this.button_cancel.Name        = "Cancel";
			this.button_cancel.GlyphShape  = GlyphShape.Cancel;
			this.button_cancel.ButtonStyle = ButtonStyle.ExListRight;
			this.button_cancel.Clicked    += new MessageEventHandler(this.HandleButtonCancelClicked);
			this.button_cancel.Shortcut    = feel.CancelShortcut;
		}
		
		
		private void HandleButtonOkClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.button_ok);
			this.OnAcceptClicked ();
		}		
		
		private void HandleButtonCancelClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.button_cancel);
			this.OnCancelClicked ();
		}		
		
		
		protected virtual void OnAcceptClicked()
		{
			if (this.AcceptClicked != null)
			{
				this.AcceptClicked (this);
			}
		}
		
		protected virtual void OnCancelClicked()
		{
			if (this.CancelClicked != null)
			{
				this.CancelClicked (this);
			}
		}
		
		
		public event Support.EventHandler		AcceptClicked;
		public event Support.EventHandler		CancelClicked;
		
		
		protected Widget						host;
		
		protected GlyphButton					button_ok;
		protected GlyphButton					button_cancel;
	}
}
