//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets.Behaviors
{
	public class SlimFieldTextBehavior : System.IDisposable
	{
		public SlimFieldTextBehavior(SlimField host)
		{
			this.host = host;
			
			this.host.Entered   += this.HandleHostEntered;
			this.host.Exited    += this.HandleHostExited;
		}
		
		#region IDisposable Members

		public void Dispose()
		{
			this.host.Entered   -= this.HandleHostEntered;
			this.host.Exited    -= this.HandleHostExited;
		}

		#endregion

		private void HandleHostEntered(object sender, MessageEventArgs e)
		{
			if (this.textFieldInEdition == false)
			{
				this.DisposeTextField ();
				this.CreateTextField ();
			}
			
			this.textFieldHilite = true;
		}

		private void HandleHostExited(object sender, MessageEventArgs e)
		{
			this.textFieldHilite = false;

			if (this.textFieldInEdition == false)
			{
				this.DisposeTextField ();
			}
		}

		private void HandleTextIsFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool)e.NewValue == true)
			{
				this.StartTextFieldEdition ();
			}
			else
			{
				this.HandleTextEditionEnded ();
			}
		}

		private void HandleTextEditionStarted(object sender)
		{
			this.StartTextFieldEdition ();
		}

		private void HandleTextEditionAccepted(object sender)
		{
			this.host.FieldText = this.textField.FormattedText.ToSimpleText ();
			this.HandleTextEditionEnded ();
		}

		private void HandleTextEditionRejected(object sender)
		{
			this.HandleTextEditionEnded ();
		}

		private void HandleTextEditionEnded()
		{
			this.textFieldInEdition = false;

			if (this.textFieldHilite == false)
			{
				this.DisposeTextField ();
			}
		}

		private void CreateTextField()
		{
			this.textField = new TextFieldEx ()
			{
				Parent = this.host,
				Dock = DockStyle.Fill,
				FormattedText = FormattedText.FromSimpleText (this.host.FieldText),
				ButtonShowCondition = ButtonShowCondition.WhenKeyboardFocused
			};

			this.textField.IsFocusedChanged += this.HandleTextIsFocusedChanged;
			this.textField.EditionStarted   += this.HandleTextEditionStarted;
			this.textField.EditionAccepted  += this.HandleTextEditionAccepted;
			this.textField.EditionRejected  += this.HandleTextEditionRejected;
		}
		
		private void DisposeTextField()
		{
			if (this.textField != null)
			{
				this.textField.IsFocusedChanged -= this.HandleTextIsFocusedChanged;
				this.textField.EditionStarted   -= this.HandleTextEditionStarted;
				this.textField.EditionAccepted  -= this.HandleTextEditionAccepted;
				this.textField.EditionRejected  -= this.HandleTextEditionRejected;
				
				this.textField.Dispose ();
				this.textField = null;
				this.textFieldInEdition = false;
			}
		}

		private void StartTextFieldEdition()
		{
			System.Diagnostics.Debug.Assert (this.textField != null);

			this.textFieldInEdition = true;
		}


		private readonly SlimField				host;
		private TextFieldEx						textField;
		private bool							textFieldInEdition;
		private bool							textFieldHilite;
	}
}
