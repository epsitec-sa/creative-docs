//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Widgets.Behaviors
{
	public class SlimFieldTextBehavior : SlimFieldBehavior, System.IDisposable
	{
		public SlimFieldTextBehavior(SlimField host)
			: base (host)
		{
			this.host.Entered   += this.HandleHostEntered;
			this.host.Exited    += this.HandleHostExited;
			this.host.IsFocusedChanged += this.HandleHostIsFocusedChanged;
		}


		public bool								HasFocus
		{
			get
			{
				if (this.textField == null)
				{
					return false;
				}
				else
				{
					return this.textField.IsFocused;
				}
			}
		}

		public bool								HasButtons
		{
			get
			{
				if (this.textField == null)
				{
					return false;
				}
				else
				{
					return this.textField.ComputeButtonVisibility ();
				}
			}
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
			if (this.HasFocus || this.HasButtons)
			{
				return;
			}
			
			this.DisposeTextField ();
			this.CreateTextField ();
			
			this.textFieldHilite = true;
		}

		private void HandleHostExited(object sender, MessageEventArgs e)
		{
			this.textFieldHilite = false;

			if (this.HasFocus || this.HasButtons)
			{
				return;
			}
			
			this.DisposeTextField ();
		}

		private void HandleHostIsFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if ((bool) e.NewValue == true)
			{
				this.DisposeTextField ();
				this.CreateTextField ();
				this.textField.Focus ();
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
			this.AdjustGeometry ();
		}

		
		private void CreateTextField()
		{
			this.textField = new TextFieldEx (TextFieldStyle.Flat)
			{
				Parent = this.host,
				Dock = DockStyle.Fill,
				FormattedText = FormattedText.FromSimpleText (this.host.FieldText),
				DefocusAction = DefocusAction.Modal,
			};

			this.textField.SelectAll ();
			
			this.textField.IsFocusedChanged += this.HandleTextIsFocusedChanged;
			this.textField.EditionStarted   += this.HandleTextEditionStarted;
			this.textField.EditionAccepted  += this.HandleTextEditionAccepted;
			this.textField.EditionRejected  += this.HandleTextEditionRejected;
			this.textField.TextEdited       += this.HandleTextTextEdited;

			this.AdjustGeometry ();
		}
		
		private void DisposeTextField()
		{
			if (this.textField != null)
			{
				this.textField.IsFocusedChanged -= this.HandleTextIsFocusedChanged;
				this.textField.EditionStarted   -= this.HandleTextEditionStarted;
				this.textField.EditionAccepted  -= this.HandleTextEditionAccepted;
				this.textField.EditionRejected  -= this.HandleTextEditionRejected;
				this.textField.TextEdited       -= this.HandleTextTextEdited;
				
				this.textField.Dispose ();
				this.textField = null;
			}
		}

		
		private void StartTextFieldEdition()
		{
			System.Diagnostics.Debug.Assert (this.textField != null);

			this.AdjustGeometry ();
		}

		private void StopTextFieldEdition()
		{
			if (this.textField == null)
			{
				return;
			}

			this.host.FieldText = this.textField.FormattedText.ToSimpleText ();

			if (this.HasFocus || this.HasButtons)
			{
				this.textField.SelectAll ();
			}
			else
			{
				this.DisposeTextField ();
			}

			this.AdjustGeometry ();
		}

		
		private void AdjustGeometry()
		{
			if (this.textField != null)
			{
				this.host.FieldText = this.textField.FormattedText.ToSimpleText ();
			}
				
			var width = this.host.MeasureWidth (SlimFieldDisplayMode.MeasureTextOnly);
			var total = this.host.MeasureWidth (SlimFieldDisplayMode.Text);

			if (this.HasFocus)
			{
				total += System.Math.Max (20, width) - width;
			}

			this.AdjustHostPreferredWidth (total);
			this.AdjustTextFieldMargins ();
			
			this.host.Invalidate ();
		}

		private void AdjustHostPreferredWidth(double total)
		{
			if (this.HasButtons)
			{
				var padding = this.textField.GetInternalPadding ();
				var width   = total + padding.Width - 1;

				this.host.PreferredWidth = System.Math.Ceiling (width);
			}
			else
			{
				this.host.PreferredWidth = System.Math.Ceiling (total);
			}
		}
		
		private void AdjustTextFieldMargins()
		{
			if (this.textField == null)
			{
				return;
			}

			var prefix = this.host.MeasureWidth (SlimFieldDisplayMode.MeasureTextPrefix) + 1;
			var suffix = this.host.MeasureWidth (SlimFieldDisplayMode.MeasureTextSuffix);
			
			//	We do not want to round the position, or else we would see the text move
			//	a little bit horizontally whenever the slim field is being hovered:

			double left  = prefix;
			double right = suffix;

			this.textField.Margins = new Margins (left, right, 0, 0);
		}
		
		
		private TextFieldEx						textField;
		private bool							textFieldHilite;
	}
}
