//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ComputedAmountController
	{
		public ComputedAmount?					ComputedAmount
		{
			get
			{
				return this.computedAmount;
			}
			set
			{
				if (this.computedAmount != value)
				{
					this.computedAmount = value;
					this.UpdateUI ();
				}
			}
		}


		public void CreateUI(Widget parent)
		{
			this.initialTextField = new TextField
			{
				Parent        = parent,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (ComputedAmountController.editWidth, ComputedAmountController.lineHeight),
				IsReadOnly    = true,
			};

			this.addSubButton = new GlyphButton
			{
				Parent        = parent,
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (ComputedAmountController.lineHeight, ComputedAmountController.lineHeight),
			};

			this.argumentTextField = new TextField
			{
				Parent        = parent,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (ComputedAmountController.editWidth, ComputedAmountController.lineHeight),
				TabIndex      = 1,
			};

			this.rateButton = new Button
			{
				Parent        = parent,
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (30, ComputedAmountController.lineHeight),
			};

			this.equalText = new StaticText
			{
				Parent           = parent,
				Text             = "=",
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Left,
				PreferredSize    = new Size (20, ComputedAmountController.lineHeight),
			};

			this.finalTextField = new TextField
			{
				Parent        = parent,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (ComputedAmountController.editWidth, ComputedAmountController.lineHeight),
				TabIndex      = 2,
				Margins       = new Margins (0, 10, 0, 0),
			};

			this.calcButton = new IconButton
			{
				Parent        = parent,
				IconUri       = AbstractCommandToolbar.GetResourceIconUri ("Field.Calc"),
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (ComputedAmountController.lineHeight, ComputedAmountController.lineHeight),
			};

			//	Connexion des événements.
			this.calcButton.Clicked += delegate
			{
				this.SwapComputed ();
				this.UpdateUI ();
				this.SetFocus (this.finalTextField);
				this.OnValueChanged ();
			};

			this.addSubButton.Clicked += delegate
			{
				this.SwapAddSub ();
				this.UpdateUI ();
				this.SetFocus (this.argumentTextField);
				this.OnValueChanged ();
			};

			this.rateButton.Clicked += delegate
			{
				this.SwapRate ();
				this.UpdateUI ();
				this.SetFocus (this.argumentTextField);
				this.OnValueChanged ();
			};

			this.argumentTextField.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.ArgumentChanged ();
					this.UpdateUI ();
					this.OnValueChanged ();
				}
			};

			this.finalTextField.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.FinalChanged ();
					this.UpdateUI ();
					this.OnValueChanged ();
				}
			};

			this.UpdateUI ();
		}

		public void SetFocus()
		{
			this.SetFocus (this.finalTextField);
		}

		private void SetFocus(TextField textField)
		{
			textField.SelectAll ();
			textField.Focus ();
		}


		private void SwapComputed()
		{
			if (this.computedAmount.HasValue)
			{
				var ca = this.computedAmount.Value;
				var final = this.EditedFinalAmount;

				if (ca.Computed)
				{
					this.computedAmount = new ComputedAmount
						(
							final
						);
				}
				else
				{
					this.computedAmount = new ComputedAmount
						(
							final,
							0.0m,
							final,
							false,
							false
						);
				}
			}
		}

		private void SwapAddSub()
		{
			if (this.computedAmount.HasValue)
			{
				var ca = this.computedAmount.Value;

				this.computedAmount = new ComputedAmount
					(
						ca.InitialAmount,
						ca.ArgumentAmount,
						ca.FinalAmount,
						!ca.Substract,
						ca.Rate
					);

				ca = this.computedAmount.Value;
				var final = ca.ComputeFinal (ca.ArgumentAmount);

				this.computedAmount = new ComputedAmount
					(
						ca.InitialAmount,
						ca.ArgumentAmount,
						final,
						ca.Substract,
						ca.Rate
					);
			}
		}

		private void SwapRate()
		{
			if (this.computedAmount.HasValue)
			{
				var ca = this.computedAmount.Value;

				this.computedAmount = new ComputedAmount
					(
						ca.InitialAmount,
						0.0m,
						ca.FinalAmount,
						ca.Substract,
						!ca.Rate
					);

				ca = this.computedAmount.Value;
				var final = ca.ComputeFinal (ca.ArgumentAmount);

				this.computedAmount = new ComputedAmount
					(
						ca.InitialAmount,
						ca.ArgumentAmount,
						final,
						ca.Substract,
						ca.Rate
					);
			}
		}


		private void ArgumentChanged()
		{
			if (this.computedAmount.HasValue)
			{
				var ca = this.computedAmount.Value;
				var argument = this.EditedArgumentAmount;
				var final = ca.ComputeFinal (argument);

				this.computedAmount = new ComputedAmount
					(
						ca.InitialAmount,
						argument,
						final,
						ca.Substract,
						ca.Rate
					);
			}
		}

		private void FinalChanged()
		{
			if (this.computedAmount.HasValue)
			{
				var ca = this.computedAmount.Value;

				if (ca.Computed)
				{
					var final = this.EditedFinalAmount;
					var argument = ca.ComputeArgument (this.EditedFinalAmount);

					this.computedAmount = new ComputedAmount
						(
							ca.InitialAmount,
							argument,
							final,
							ca.Substract,
							ca.Rate
						);
				}
				else
				{
					var final = this.EditedFinalAmount;

					this.computedAmount = new ComputedAmount
						(
							final
						);
				}
			}
		}


		private void UpdateUI()
		{
			if (this.initialTextField == null)
			{
				return;
			}

			this.ignoreChange = true;

			if (this.computedAmount.HasValue)
			{
				var ca = this.computedAmount.Value;

				this.initialTextField.Visibility = ca.Computed;
				this.addSubButton.Visibility = ca.Computed;
				this.argumentTextField.Visibility = ca.Computed;
				this.rateButton.Visibility = ca.Computed;
				this.equalText.Visibility = ca.Computed;

				this.calcButton.ActiveState = ca.Computed ? ActiveState.Yes : ActiveState.No;
				this.addSubButton.GlyphShape = ca.Substract ? GlyphShape.Minus : GlyphShape.Plus;
				this.rateButton.Text = ca.Rate ? "%" : "CHF";

				this.initialTextField.Text  = Helpers.Converters.AmountToString (ca.InitialAmount);

				this.EditedArgumentAmount = ca.ArgumentAmount;
				this.EditedFinalAmount    = ca.FinalAmount;
			}
			else
			{
				this.initialTextField.Visibility = false;
				this.addSubButton.Visibility = false;
				this.argumentTextField.Visibility = false;
				this.rateButton.Visibility = false;
				this.equalText.Visibility = false;

				this.EditedFinalAmount = null;
			}

			this.ignoreChange = false;
		}


		private decimal? EditedArgumentAmount
		{
			get
			{
				if (this.computedAmount.GetValueOrDefault ().Rate)
				{
					return Helpers.Converters.ParseRate (this.argumentTextField.Text);
				}
				else
				{
					return Helpers.Converters.ParseAmount (this.argumentTextField.Text);
				}
			}
			set
			{
				var current = this.EditedArgumentAmount;

				if (current != value)
				{
					if (this.computedAmount.GetValueOrDefault ().Rate)
					{
						this.argumentTextField.Text = Helpers.Converters.RateToString (value);
					}
					else
					{
						this.argumentTextField.Text = Helpers.Converters.AmountToString (value);
					}
				}
			}
		}

		private decimal? EditedFinalAmount
		{
			get
			{
				return Helpers.Converters.ParseAmount (this.finalTextField.Text);
			}
			set
			{
				var current = this.EditedFinalAmount;

				if (current != value)
				{
					this.finalTextField.Text = Helpers.Converters.AmountToString (value);
				}
			}
		}


		#region Events handler
		protected void OnValueChanged()
		{
			if (this.ValueChanged != null)
			{
				this.ValueChanged (this);
			}
		}

		public delegate void ValueChangedEventHandler(object sender);
		public event ValueChangedEventHandler ValueChanged;
		#endregion


		protected static readonly int lineHeight = 17;
		protected static readonly int editWidth  = 90;

	
		private ComputedAmount?					computedAmount;
		private bool							ignoreChange;

		private IconButton						calcButton;
		private TextField						initialTextField;
		private GlyphButton						addSubButton;
		private TextField						argumentTextField;
		private Button							rateButton;
		private StaticText						equalText;
		private TextField						finalTextField;
	}
}
