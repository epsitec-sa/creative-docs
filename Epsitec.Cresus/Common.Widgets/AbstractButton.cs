//	Copyright © 2003-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe AbstractButton implémente les fonctions de base communes de tous
	/// les boutons (notamment au niveau de la gestion des événements).
	/// </summary>
	public abstract class AbstractButton : Widget
	{
		public AbstractButton()
		{
			this.AutoFocus  = true;
			this.AutoEngage = true;
			
			this.InternalState |= WidgetInternalState.Focusable;
			this.InternalState |= WidgetInternalState.Engageable;
		}
		
		public AbstractButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}

		static AbstractButton()
		{
			Types.DependencyPropertyMetadata metadata = Visual.ContentAlignmentProperty.DefaultMetadata.Clone ();
			
			metadata.DefineDefaultValue (Drawing.ContentAlignment.MiddleCenter);
			
			Visual.ContentAlignmentProperty.OverrideMetadata (typeof (AbstractButton), metadata);
		}

		public override Drawing.Margins GetShapeMargins()
		{
			return Widgets.Adorners.Factory.Active.GeometryButtonShapeMargins;
		}


		public override Drawing.Size GetBestFitSize()
		{
			Drawing.Size size = StaticText.GetTextBestFitSize (this);

			size.Width  += 6;
			size.Height += 6;

			return size;
		}


		protected override void OnCommandObjectChanged(Types.DependencyPropertyChangedEventArgs e)
		{
			//	If the command requires a special user level (administrator privilege)
			//	to execute, take notice of it, so that we can decorate the button with
			//	a special icon (i.e. under Windows, this is a small shield).

			Command command = e.NewValue as Command;

			if ((command != null) &&
				(command.IsAdminLevelRequired))
			{
				this.isAdminLevelRequired = true;
			}
			else
			{
				this.isAdminLevelRequired = false;
			}
			
			base.OnCommandObjectChanged (e);
		}

		protected override void DefineTextFromCaption(string text)
		{
			if ((this.isAdminLevelRequired) &&
				(!Support.PrivilegeManager.Current.IsUserAnAdministrator) &&
				(Support.PrivilegeManager.Current.GetShieldIcon (Epsitec.Common.Drawing.IconSize.Small) != null))
			{
				//	The button starts an action which requires administrative privileges
				//	to be launched : add a special shield "stock icon" (this is related
				//	to Windows UAC).
				//	See http://msdn.microsoft.com/en-us/library/bb756973.aspx for UAC.

				text = string.Concat (@"<img src=""stockicon:shield.small"" /> ", text);
			}

			base.DefineTextFromCaption (text);
		}

		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.MessageType )
			{
				case MessageType.MouseEnter:
				case MessageType.MouseDown:
				case MessageType.MouseUp:
				case MessageType.MouseLeave:
				case MessageType.MouseMove:
					break;
				
				case MessageType.KeyPress:
					if ( message.IsAltPressed || message.IsControlPressed )
					{
						base.ProcessMessage (message, pos);
						return;
					}
					
					if ( Feel.Factory.Active.TestPressButtonKey(message) &&
						 this.isKeyboardPressed == false  )
					{
						this.isKeyboardPressed = true;
						this.OnShortcutPressed();
						break;
					}
					
					base.ProcessMessage (message, pos);
					return;
				
				case MessageType.KeyUp:
					if ( this.isKeyboardPressed )
					{
						this.isKeyboardPressed = false;
						break;
					}
					base.ProcessMessage (message, pos);
					return;
				
				default:
					base.ProcessMessage (message, pos);
					return;
			}
			
			message.Consumer = this;
		}

		protected override void OnShortcutPressed()
		{
			if ( this.AutoToggle )
			{
				this.Toggle();
			}
			else
			{
				this.SimulatePressed();
				this.SimulateReleased();
				this.SimulateClicked();
			}
			
			base.OnShortcutPressed();
		}

		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered (e);
			this.Invalidate ();
		}
		
		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);
			this.Invalidate ();
		}

		
		
		private bool								isKeyboardPressed;
		private bool								isAdminLevelRequired;
	}
}
