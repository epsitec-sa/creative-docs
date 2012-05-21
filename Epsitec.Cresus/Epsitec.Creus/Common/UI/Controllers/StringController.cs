//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

[assembly: Controller (typeof (Epsitec.Common.UI.Controllers.StringController))]

namespace Epsitec.Common.UI.Controllers
{
	public class StringController : AbstractController
	{
		public StringController(ControllerParameters parameters)
			: base (parameters)
		{
		}

		public override object GetUserInterfaceValue()
		{
			return this.field.Text;
		}

		protected override void CreateUserInterface(INamedType namedType, Caption caption)
		{
			this.Placeholder.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			if ((this.Placeholder.IsReadOnlyValueBinding) ||
				(this.ControllerParameters.GetParameterValue (AbstractController.ReadOnlyParameter) != null))
			{
				this.CreateReadOnlyUserInterface (caption);
			}
			else
			{
				this.CreateReadWriteUserInterface (caption);
			}
		}

		private void CreateReadOnlyUserInterface(Caption caption)
		{
			this.label = new StaticText ();
			this.field = null;

			this.AddWidget (this.label, WidgetType.Content);

			this.label.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.label.VerticalAlignment = VerticalAlignment.BaseLine;
			this.label.ContentAlignment = Drawing.ContentAlignment.MiddleLeft;
			this.label.Dock = DockStyle.Stacked;
			this.label.Name = caption.Name;

			ToolTip.Default.SetToolTip (this.label, caption.Description);
		}

		private void CreateReadWriteUserInterface(Caption caption)
		{
			this.label = new StaticText ();
			this.field = this.CreateTextField ();

			this.AddWidget (this.label, WidgetType.Label);
			this.AddWidget (this.field, WidgetType.Input);

			this.label.HorizontalAlignment = HorizontalAlignment.Right;
			this.label.VerticalAlignment = VerticalAlignment.BaseLine;
			this.label.ContentAlignment = Drawing.ContentAlignment.MiddleRight;
			this.label.Dock = DockStyle.Stacked;

			this.SetupLabelWidget (this.label, caption);

			this.field.HorizontalAlignment = HorizontalAlignment.Stretch;
			this.field.VerticalAlignment = VerticalAlignment.BaseLine;
			this.field.TextChanged += this.HandleFieldTextChanged;
			this.field.PreferredWidth = 40;
			
			this.field.TabIndex = 1;
			this.field.TabNavigationMode = TabNavigationMode.ActivateOnTab;
			this.field.Dock = DockStyle.Stacked;

			this.SetupToolTip (this.field, caption);

			this.label.Name = null;
			this.field.Name = caption.Name;
			
			this.validator = new Validators.ControllerBasedValidator (this.field, this);
		}

		private AbstractTextField CreateTextField()
		{
			AbstractTextField text;

			switch (this.ControllerParameters.GetParameterValue ("Mode"))
			{
				case StringController.MultilineMode:
					text = new TextFieldMulti ();
					text.PreferredHeight = this.Placeholder.PreferredHeight;
					break;

				case StringController.PasswordMode:
					text = new TextField ();
					text.IsPassword = true;
					break;
				
				default:
					text = new TextField ();
					break;
			}

			return text;
		}

		protected override void PrepareUserInterfaceDisposal()
		{
			base.PrepareUserInterfaceDisposal ();

			if (this.field != null)
			{
				this.field.TextChanged -= this.HandleFieldTextChanged;
			}
		}

		protected override void RefreshUserInterface(object oldValue, object newValue)
		{
			string text = null;
			
			if ((newValue != UndefinedValue.Value) &&
				(newValue != InvalidValue.Value) &&
				(newValue != null))
			{
				text = this.ConvertFromValue (newValue);
			}

			if (this.field != null)
			{
				switch (this.Placeholder.SuggestionMode)
				{
					case PlaceholderSuggestionMode.DisplayActiveHint:
						this.field.TextDisplayMode = TextFieldDisplayMode.ActiveHint;
						this.field.HintText = text;
						break;

					case PlaceholderSuggestionMode.DisplayPassiveHint:
						this.field.TextDisplayMode = TextFieldDisplayMode.PassiveHint;
						this.field.HintText = text;
						break;

					case PlaceholderSuggestionMode.DisplayHintResetText:
						this.field.TextDisplayMode = TextFieldDisplayMode.ActiveHint;
						this.field.Text = text;
						break;

					default:
						this.field.TextDisplayMode = TextFieldDisplayMode.Default;
						this.field.Text = text;
						break;
				}
			}
			else
			{
				this.label.Text = text;
				this.label.PreferredWidth = this.label.GetBestFitSize ().Width;
			}
		}
		
		private void HandleFieldTextChanged(object sender)
		{
			if (this.field.IsValid)
			{
				this.OnActualValueChanged ();
			}
		}


		private string ConvertFromValue(object newValue)
		{
			return newValue.ToString ();
		}

		public const string						ModeParameter	= "Mode";
		public const string						PasswordMode	= "Password";
		public const string						MultilineMode	= "Multiline";
		
		private AbstractTextField				field;
		private StaticText						label;
		private IValidator						validator;
	}
}
