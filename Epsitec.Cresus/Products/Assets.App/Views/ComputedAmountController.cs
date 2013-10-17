//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ComputedAmountController
	{
		public ComputedAmountController()
		{
			this.ignoreChanges = new SafeCounter ();
		}


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

		public bool IsReadOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				if (this.isReadOnly != value)
				{
					this.isReadOnly = value;
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
			};

			this.computedButton = new GlyphButton
			{
				Parent        = parent,
				ButtonStyle   = ButtonStyle.ToolItem,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (ComputedAmountController.lineHeight, ComputedAmountController.lineHeight),
			};

			//	Connexion des événements.
			this.computedButton.Clicked += delegate
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
				if (this.ignoreChanges.IsZero)
				{
					this.ArgumentChanged ();
					this.UpdateUI ();
					this.OnValueChanged ();
				}
			};

			this.finalTextField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
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

			using (this.ignoreChanges.Enter ())
			{
				bool computedButtonEnable = !this.isReadOnly;

				if (this.computedAmount.HasValue)
				{
					var ca = this.computedAmount.Value;

					this.initialTextField .Visibility = ca.Computed;
					this.addSubButton     .Visibility = ca.Computed;
					this.argumentTextField.Visibility = ca.Computed;
					this.rateButton       .Visibility = ca.Computed;
					this.equalText        .Visibility = ca.Computed;

					this.computedButton.GlyphShape = ca.Computed ? GlyphShape.TriangleLeft : GlyphShape.TriangleRight;
					this.addSubButton.GlyphShape = ca.Substract ? GlyphShape.Minus : GlyphShape.Plus;
					this.rateButton.Text = ca.Rate ? "%" : "CHF";

					this.initialTextField.Text = Helpers.Converters.AmountToString (ca.InitialAmount);

					this.EditedArgumentAmount = ca.ArgumentAmount;
					this.EditedFinalAmount    = ca.FinalAmount;

					if (!ca.Computed && !ca.FinalAmount.HasValue)
					{
						computedButtonEnable = false;
					}
				}
				else
				{
					this.initialTextField .Visibility = false;
					this.addSubButton     .Visibility = false;
					this.argumentTextField.Visibility = false;
					this.rateButton       .Visibility = false;
					this.equalText        .Visibility = false;

					this.EditedFinalAmount = null;
					computedButtonEnable = false;
				}


				this.argumentTextField.IsReadOnly =  this.isReadOnly;
				this.finalTextField   .IsReadOnly =  this.isReadOnly;
				this.addSubButton     .Enable     = !this.isReadOnly;
				this.rateButton       .Enable     = !this.isReadOnly;
				this.equalText        .Enable     = !this.isReadOnly;
				this.computedButton   .Enable     = computedButtonEnable;
			}
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


		private readonly SafeCounter			ignoreChanges;

		private ComputedAmount?					computedAmount;
		private bool							isReadOnly;

		private TextField						initialTextField;
		private GlyphButton						addSubButton;
		private TextField						argumentTextField;
		private Button							rateButton;
		private StaticText						equalText;
		private TextField						finalTextField;
		private GlyphButton						computedButton;
	}
}
