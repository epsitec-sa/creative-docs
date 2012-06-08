//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

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
			this.DisposeTextField ();

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

			if ((this.textFieldInEdition == false) &&
				(this.host.ContainsKeyboardFocus == false))
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
				this.StopTextFieldEdition ();
			}
		}

		private void HandleTextEditionStarted(object sender)
		{
			this.StartTextFieldEdition ();
		}

		private void HandleTextEditionAccepted(object sender)
		{
			this.StopTextFieldEdition ();
		}

		private void HandleTextEditionRejected(object sender)
		{
			this.StopTextFieldEdition ();
		}

		private void HandleTextTextEdited(object sender)
		{
			this.AdjustHostSize ();
		}

		private void CreateTextField()
		{
			this.textField = new TextFieldEx ()
			{
				Parent = this.host,
				Dock = DockStyle.Fill,
				FormattedText = FormattedText.FromSimpleText (this.host.FieldText),
			};

			this.textField.IsFocusedChanged += this.HandleTextIsFocusedChanged;
			this.textField.EditionStarted   += this.HandleTextEditionStarted;
			this.textField.EditionAccepted  += this.HandleTextEditionAccepted;
			this.textField.EditionRejected  += this.HandleTextEditionRejected;
			this.textField.TextEdited      += this.HandleTextTextEdited;
		}
		
		private void DisposeTextField()
		{
			if (this.textField != null)
			{
				this.textField.IsFocusedChanged -= this.HandleTextIsFocusedChanged;
				this.textField.EditionStarted   -= this.HandleTextEditionStarted;
				this.textField.EditionAccepted  -= this.HandleTextEditionAccepted;
				this.textField.EditionRejected  -= this.HandleTextEditionRejected;
				this.textField.TextEdited      -= this.HandleTextTextEdited;
				
				this.textField.Dispose ();
				this.textField = null;
				this.textFieldInEdition = false;
			}
		}

		private void StartTextFieldEdition()
		{
			System.Diagnostics.Debug.Assert (this.textField != null);

			if (this.textFieldInEdition == false)
			{
				this.textFieldInEdition = true;
			}

			this.AdjustHostSize ();
		}

		private void AdjustHostSize()
		{
			if (this.textFieldInEdition)
			{
				this.host.FieldText = this.textField.FormattedText.ToSimpleText ();
				
				var size = this.host.GetBestFitSize ();
				var padding = this.textField.GetInternalPadding ();
				var width   = System.Math.Max (20, size.Width) + padding.Width - 2;

				this.host.PreferredWidth = width;
			}
			else
			{
				this.host.UpdatePreferredSize ();
			}
		}

		private void StopTextFieldEdition()
		{
			this.host.FieldText = this.textField.FormattedText.ToSimpleText ();
			
			this.textFieldInEdition = false;

			if ((this.textFieldHilite == false) &&
				(this.host.ContainsKeyboardFocus == false))
			{
				this.DisposeTextField ();
			}
			else
			{
//				this.DisposeTextField ();
//				this.CreateTextField ();
				this.textField.SelectAll ();
			}

			this.AdjustHostSize ();
		}

		private readonly SlimField				host;
		private TextFieldEx						textField;
		private bool							textFieldInEdition;
		private bool							textFieldHilite;
	}
}
