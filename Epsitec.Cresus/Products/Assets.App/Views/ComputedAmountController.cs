//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;

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

		public ComputedAmount?					ComputedAmountNoEditing
		{
			set
			{
				if (this.computedAmount != value)
				{
					this.computedAmount = value;
					this.UpdateNoEditingUI ();
				}
			}
		}

		public bool								IsReadOnly
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

		public PropertyState					PropertyState
		{
			get
			{
				return this.propertyState;
			}
			set
			{
				if (this.propertyState != value)
				{
					this.propertyState = value;
					this.UpdateUI ();
				}
			}
		}

		public bool HasError
		{
			get
			{
				return this.hasError;
			}
			set
			{
				if (this.hasError != value)
				{
					this.hasError = value;
					this.UpdateUI ();
				}
			}
		}


		public void UpdateValue()
		{
			this.UpdateUI ();

			using (this.ignoreChanges.Enter ())
			{
				if (this.computedAmount.HasValue)
				{
					this.SetArgumentValue (this.computedAmount.Value.ArgumentAmount);
					this.SetFinalValue (this.computedAmount.Value.FinalAmount);
				}
				else
				{
					this.SetArgumentValue (null);
					this.SetFinalValue (null);
				}
			}
		}

		public void CreateUI(Widget parent)
		{
			this.addSubButton = new GlyphButton
			{
				Parent        = parent,
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (ComputedAmountController.LineHeight, ComputedAmountController.LineHeight),
			};

			this.argumentTextField = new TextField
			{
				Parent        = parent,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (ComputedAmountController.EditWidth, ComputedAmountController.LineHeight),
				TabIndex      = 1,
			};

			this.rateButton = new Button
			{
				Parent        = parent,
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (30, ComputedAmountController.LineHeight),
			};

			this.equalText = new StaticText
			{
				Parent           = parent,
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Left,
				PreferredSize    = new Size (20, ComputedAmountController.LineHeight),
			};

			this.finalTextField = new TextField
			{
				Parent        = parent,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (ComputedAmountController.EditWidth, ComputedAmountController.LineHeight),
				TabIndex      = 2,
			};

			ToolTip.Default.SetToolTip (this.addSubButton,      "Détermine si la valeur est augmentée ou diminuée");
			ToolTip.Default.SetToolTip (this.argumentTextField, "Valeur de la modification");
			ToolTip.Default.SetToolTip (this.rateButton,        "Détermine si la modification est en francs ou en pourcents");
			ToolTip.Default.SetToolTip (this.finalTextField,    "Nouvelle valeur");

			//	Connexion des événements.
			this.addSubButton.Clicked += delegate
			{
				this.SwapAddSub ();
				this.UpdateUI ();
				this.SetFocus (this.argumentTextField);
				this.OnValueEdited ();
			};

			this.rateButton.Clicked += delegate
			{
				this.SwapRate ();
				this.UpdateUI ();
				this.SetFocus (this.argumentTextField);
				this.OnValueEdited ();
			};

			this.argumentTextField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.ArgumentChanged ();
					this.UpdateUI ();
					this.OnValueEdited ();
				}
			};

			this.finalTextField.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.FinalChanged ();
					this.UpdateUI ();
					this.OnValueEdited ();
				}
			};

			this.argumentTextField.KeyboardFocusChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.SetFocus (this.argumentTextField);
					this.OnFocusEngage ();
				}
				else  // perdu le focus ?
				{
					this.OnFocusLost ();
				}
			};

			this.finalTextField.KeyboardFocusChanged += delegate (object sender, DependencyPropertyChangedEventArgs e)
			{
				bool focused = (bool) e.NewValue;

				if (focused)  // pris le focus ?
				{
					this.SetFocus (this.finalTextField);
					this.OnFocusEngage ();
				}
				else  // perdu le focus ?
				{
					this.OnFocusLost ();
				}
			};

			this.UpdateUI ();
		}

		public void SetFocus()
		{
			if (this.computedAmount.HasValue && this.computedAmount.Value.ArgumentDefined)
			{
				this.SetFocus (this.argumentTextField);
			}
			else
			{
				this.SetFocus (this.finalTextField);
			}
		}

		private void SetFocus(TextField textField)
		{
			textField.SelectAll ();
			textField.Focus ();
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
					!ca.Subtract,
					ca.Rate,
					ca.ArgumentDefined
				);

				ca = this.computedAmount.Value;
				var final = ca.ComputeFinal (ca.ArgumentAmount);

				this.computedAmount = new ComputedAmount
				(
					ca.InitialAmount,
					ca.ArgumentAmount,
					final,
					ca.Subtract,
					ca.Rate,
					ca.ArgumentDefined
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
					ca.Subtract,
					!ca.Rate,
					ca.ArgumentDefined
				);

				ca = this.computedAmount.Value;
				var argument = ca.ComputeArgument (ca.FinalAmount);

				this.computedAmount = new ComputedAmount
				(
					ca.InitialAmount,
					argument,
					ca.FinalAmount,
					ca.Subtract,
					ca.Rate,
					ca.ArgumentDefined
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
					ca.Subtract,
					ca.Rate,
					true
				);
			}
		}

		private void FinalChanged()
		{
			var ca = this.computedAmount.GetValueOrDefault (new ComputedAmount (0.0m));

			if (ca.Computed)
			{
				var final = this.EditedFinalAmount;
				var argument = ca.ComputeArgument (this.EditedFinalAmount);

				this.computedAmount = new ComputedAmount
				(
					ca.InitialAmount,
					argument,
					final,
					ca.Subtract,
					ca.Rate,
					false
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


		public void UpdateNoEditingUI()
		{
			if (this.computedAmount.HasValue)
			{
				var ca = this.computedAmount.Value;

				if (ca.ArgumentDefined)
				{
					this.UpdateUI (updateArgument: false);
				}
				else
				{
					this.UpdateUI (updateFinal: false);
				}
			}
		}

		private void UpdateUI(bool updateArgument = true, bool updateFinal = true)
		{
			if (this.addSubButton == null)
			{
				return;
			}

			using (this.ignoreChanges.Enter ())
			{
				if (this.computedAmount.HasValue)
				{
					var ca = this.computedAmount.Value;

					this.addSubButton     .Visibility = ca.InitialAmount.HasValue;
					this.rateButton       .Visibility = ca.InitialAmount.HasValue;
					this.equalText        .Visibility = ca.InitialAmount.HasValue;
					this.argumentTextField.Visibility = ca.InitialAmount.HasValue;

					this.addSubButton.GlyphShape = ca.Subtract ? GlyphShape.Minus : GlyphShape.Plus;
					this.rateButton.Text = ca.Rate ? "%" : "CHF";
					this.equalText.Text = "=";

					if (updateArgument)
					{
						this.EditedArgumentAmount = ca.ArgumentAmount;
					}

					if (updateFinal)
					{
						this.EditedFinalAmount = ca.FinalAmount;
					}

					var argumentType = AbstractFieldController.GetFieldColorType (ca.ArgumentDefined ? this.propertyState : PropertyState.Undefined, isLocked: this.isReadOnly, isError: this.hasError);
					AbstractFieldController.UpdateTextField (this.argumentTextField, argumentType, this.isReadOnly);

					var finalType = AbstractFieldController.GetFieldColorType (!ca.ArgumentDefined ? this.propertyState : PropertyState.Undefined, isLocked: this.isReadOnly, isError: this.hasError);
					AbstractFieldController.UpdateTextField (this.finalTextField, finalType, this.isReadOnly);
				}
				else
				{
					this.addSubButton     .Visibility = false;
					this.rateButton       .Visibility = false;
					this.equalText        .Visibility = false;
					this.argumentTextField.Visibility = false;

					this.addSubButton.GlyphShape = GlyphShape.None;
					this.rateButton.Text = null;
					this.equalText.Text = null;

					this.argumentTextField.Text = null;

					this.EditedArgumentAmount = null;
					this.EditedFinalAmount = null;

					var type = this.hasError ? FieldColorType.Error : FieldColorType.Editable;
					AbstractFieldController.UpdateTextField (this.argumentTextField, type, true);
					AbstractFieldController.UpdateTextField (this.finalTextField,    type, this.isReadOnly);
				}

				this.addSubButton.Enable = !this.isReadOnly;
				this.rateButton  .Enable = !this.isReadOnly;
				this.equalText   .Enable = !this.isReadOnly;
			}
		}


		private decimal? EditedArgumentAmount
		{
			get
			{
				if (this.computedAmount.GetValueOrDefault ().Rate)
				{
					return TypeConverters.ParseRate (this.argumentTextField.Text);
				}
				else
				{
					return TypeConverters.ParseAmount (this.argumentTextField.Text);
				}
			}
			set
			{
				var current = this.EditedArgumentAmount;

				//	Si le champ contenait "12%" et qu'on cherche à y mettre null en mode
				//	Rate = false, on va se trouver avec current == null et value == null !
				if (current != value || (current == null && value == null))
				{
					this.SetArgumentValue (value);
				}
			}
		}

		private void SetArgumentValue(decimal? value)
		{
			if (this.computedAmount.GetValueOrDefault ().Rate)
			{
				this.argumentTextField.Text = TypeConverters.RateToString (value);
			}
			else
			{
				this.argumentTextField.Text = TypeConverters.AmountToString (value);
			}

			this.argumentTextField.SelectAll ();
		}

		private decimal? EditedFinalAmount
		{
			get
			{
				return TypeConverters.ParseAmount (this.finalTextField.Text);
			}
			set
			{
				var current = this.EditedFinalAmount;

				if (current != value)
				{
					this.SetFinalValue (value);
				}
			}
		}

		private void SetFinalValue(decimal? value)
		{
			this.finalTextField.Text = TypeConverters.AmountToString (value);
			this.finalTextField.SelectAll ();
		}


		#region Events handler
		private void OnValueEdited()
		{
			this.ValueEdited.Raise (this);
		}

		public event EventHandler ValueEdited;


		protected void OnFocusEngage()
		{
			this.FocusEngage.Raise (this);
		}

		public event EventHandler FocusEngage;


		protected void OnFocusLost()
		{
			this.FocusLost.Raise (this);
		}

		public event EventHandler FocusLost;
		#endregion


		protected const int LineHeight = AbstractFieldController.lineHeight;
		protected const int EditWidth  = 90;


		private readonly SafeCounter			ignoreChanges;

		private ComputedAmount?					computedAmount;
		private PropertyState					propertyState;
		private bool							isReadOnly;
		private bool							hasError;

		private GlyphButton						addSubButton;
		private TextField						argumentTextField;
		private Button							rateButton;
		private StaticText						equalText;
		private TextField						finalTextField;
	}
}
