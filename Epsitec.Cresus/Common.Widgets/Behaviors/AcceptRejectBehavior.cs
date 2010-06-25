//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Widgets.Behaviors
{
	/// <summary>
	/// The <c>AcceptRejectBehavior</c> class handles buttons to accept or reject
	/// the edition in a text field.
	/// </summary>
	public sealed class AcceptRejectBehavior
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

					if (this.host is TextFieldMultiEx)
					{
						//	Si le widget est un TextFieldMultiEx, les boutons Accept/Reject seront en bas à droite.
						//	Il ne faut donc pas tenir compte de la hauteur totale du widget !
						height = System.Math.Min (height, TextFieldMultiEx.SingleLineDefaultHeight);
					}

					double width = System.Math.Floor ((height - 4) * 15.0 / 17.0);
					
					return width + width - 1;
				}
				
				return 0;
			}
		}
		
		public bool								IsVisible
		{
			get
			{
				return this.isVisible;
			}
		}
		
		public bool								IsAcceptEnabled
		{
			get
			{
				return this.isAcceptEnabled;
			}
		}
		
		public string							InitialText
		{
			get
			{
				return this.initialText;
			}
			set
			{
				this.initialText = value;
			}
		}
		
		
		public void SetVisible(bool visible)
		{
			this.isVisible = visible;
			
			if (this.buttonAccept != null)
			{
				this.buttonAccept.Visibility = (this.isVisible);
				this.buttonReject.Visibility = (this.isVisible);
			}
		}
		
		public void SetAcceptEnabled(bool enableAccept)
		{
			this.isAcceptEnabled = enableAccept;
			
			if (this.buttonAccept != null)
			{
				this.buttonAccept.Enable = this.isAcceptEnabled;
			}
		}
		
		
		public void UpdateButtonGeometry()
		{
			AbstractTextField text = this.host as AbstractTextField;
			
			if ((text != null) &&
				(this.buttonAccept != null))
			{
				Drawing.Rectangle bounds = text.GetButtonBounds ();
				Drawing.Rectangle rect1 = bounds;
				Drawing.Rectangle rect2 = bounds;
				
				rect1.Right = rect1.Left + rect1.Width / 2 + 0.5;
				rect2.Left  = rect1.Right - 1;
				
				this.buttonAccept.SetManualBounds(rect1);
				this.buttonReject.SetManualBounds(rect2);
			}
		}
		
		
		public void CreateButtons()
		{
			System.Diagnostics.Debug.Assert (this.buttonAccept == null);
			System.Diagnostics.Debug.Assert (this.buttonReject == null);
			
			this.buttonAccept = new GlyphButton(this.host);
			this.buttonReject = new GlyphButton(this.host);
			
			IFeel feel = Feel.Factory.Active;
			
			this.buttonAccept.Name        = "Accept";
			this.buttonAccept.GlyphShape  = GlyphShape.Accept;
			this.buttonAccept.Clicked    += this.HandleButtonAcceptClicked;
			this.buttonAccept.ButtonStyle = ButtonStyle.ExListMiddle;
			//?this.buttonAccept.ButtonStyle = ButtonStyle.DefaultAccept;
			
			this.buttonReject.Name        = "Reject";
			this.buttonReject.GlyphShape  = GlyphShape.Reject;
			this.buttonReject.Clicked    += this.HandleButtonRejectClicked;
			this.buttonReject.ButtonStyle = ButtonStyle.ExListRight;
			//?this.buttonReject.ButtonStyle = ButtonStyle.DefaultCancel;
			
			this.SetVisible (this.isVisible);
			this.SetAcceptEnabled (this.isAcceptEnabled);
		}
		
		
		private void HandleButtonAcceptClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.buttonAccept);
			this.OnAcceptClicked ();
		}		
		
		private void HandleButtonRejectClicked(object sender, MessageEventArgs e)
		{
			System.Diagnostics.Debug.Assert (sender == this.buttonReject);
			this.OnRejectClicked ();
		}		
		
		
		private void OnAcceptClicked()
		{
			if (this.AcceptClicked != null)
			{
				this.AcceptClicked (this);
			}
		}
		
		private void OnRejectClicked()
		{
			if (this.RejectClicked != null)
			{
				this.RejectClicked (this);
			}
		}
		
		
		public event Support.EventHandler		AcceptClicked;
		public event Support.EventHandler		RejectClicked;


		private Widget							host;

		private GlyphButton						buttonAccept;
		private GlyphButton						buttonReject;

		private bool							isVisible;
		private bool							isAcceptEnabled = true;
		private string							initialText;
	}
}
