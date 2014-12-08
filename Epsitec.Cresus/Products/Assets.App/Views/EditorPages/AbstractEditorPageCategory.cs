//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public abstract class AbstractEditorPageCategory : AbstractEditorPage
	{
		public AbstractEditorPageCategory(DataAccessor accessor, CommandContext commandContext, BaseType baseType, bool isTimeless)
			: base (accessor, commandContext, baseType, isTimeless)
		{
		}


		protected void CreateCommonUI(Widget parent)
		{
			this.methodController      = this.CreateMethodGuidController (parent, ObjectField.MethodGuid);
			this.periodicityController = this.CreateEnumController       (parent, ObjectField.Periodicity, EnumDictionaries.DictPeriodicities, editWidth: 90);

			//??this.argumentsController = new ArgumentValueFieldsController (this.accessor);
			//??this.argumentsController.CreateUI (parent);

			foreach (var argument in ArgumentsLogic.GetSortedArguments (this.accessor))
			{
				var type = (ArgumentType) ObjectProperties.GetObjectPropertyInt (argument, null, ObjectField.ArgumentType);
				var field = (ObjectField) ObjectProperties.GetObjectPropertyInt (argument, null, ObjectField.ArgumentField);

				switch (type)
				{
					case ArgumentType.Decimal:
						this.CreateDecimalController (parent, field, DecimalFormat.Real);
						break;

					case ArgumentType.Amount:
						this.CreateDecimalController (parent, field, DecimalFormat.Amount);
						break;

					case ArgumentType.Rate:
						this.CreateDecimalController (parent, field, DecimalFormat.Rate);
						break;

					case ArgumentType.Int:
						this.CreateIntController (parent, field);
						break;

					case ArgumentType.Date:
						this.CreateDateController (parent, field);
						break;

					case ArgumentType.String:
						this.CreateStringController (parent, field);
						break;
				}
			}

			this.methodController.ValueEdited += delegate
			{
				this.UpdateControllers ();
			};

			this.UpdateControllers ();
		}

		protected void CreateAccountsUI(Widget parent, System.DateTime? forcedDate)
		{
			foreach (var field in DataAccessor.AccountFields)
			{
				this.CreateAccountController (parent, field, forcedDate);
			}

			this.entrySamples = new EntrySamples (this.accessor, forcedDate);
			this.entrySamples.CreateUI (parent);
		}


		public override void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			base.SetObject (objectGuid, timestamp);
			this.UpdateControllers ();
		}

		protected void UpdateControllers()
		{
			this.methodController     .IsReadOnly =  Amortizations.IsHidden (ObjectField.MethodGuid);
			this.periodicityController.IsReadOnly =  Amortizations.IsHidden (ObjectField.Periodicity);

			//??this.argumentsController.SetMethod (this.methodController.Value);
		}


		//??protected void CreateRateCalculatorButton()
		//??{
		//??	//	Crée le bouton qui ouvre le Popup de calculation du taux.
		//??	this.rateCalculatorButton = new Button
		//??	{
		//??		Parent          = this.rateController.FrameBox,
		//??		Text            = Res.Strings.EditorPages.Category.RateCalculatorButton.ToString (),
		//??		PreferredWidth  = AbstractFieldController.maxWidth - 90,
		//??		PreferredHeight = AbstractFieldController.lineHeight,
		//??		ButtonStyle     = ButtonStyle.Icon,
		//??		AutoFocus       = false,
		//??		Dock            = DockStyle.Left,
		//??	};
		//??
		//??	this.rateCalculatorButton.Clicked += delegate
		//??	{
		//??		this.ShowRateCalculatorPopup (this.rateCalculatorButton);
		//??	};
		//??}
		//??
		//??protected void CreateYearsCalculatorButton()
		//??{
		//??	//	Crée le bouton qui ouvre le Popup de calculation du nombre d'années.
		//??	this.yearsCalculatorButton = new Button
		//??	{
		//??		Parent          = this.yearsController.FrameBox,
		//??		Text            = Res.Strings.EditorPages.Category.YearsCalculatorButton.ToString (),
		//??		PreferredWidth  = AbstractFieldController.maxWidth - 90,
		//??		PreferredHeight = AbstractFieldController.lineHeight,
		//??		ButtonStyle     = ButtonStyle.Icon,
		//??		AutoFocus       = false,
		//??		Dock            = DockStyle.Left,
		//??	};
		//??
		//??	this.yearsCalculatorButton.Clicked += delegate
		//??	{
		//??		this.ShowYearsCalculatorPopup (this.yearsCalculatorButton);
		//??	};
		//??}


		//??private void ShowRateCalculatorPopup(Widget target)
		//??{
		//??	//	Affiche le Popup de calculation du taux.
		//??	var popup = new RateCalculatorPopup (accessor)
		//??	{
		//??		Rate = this.accessor.EditionAccessor.GetFieldDecimal (ObjectField.AmortizationRate),
		//??	};
		//??
		//??	popup.Create (target, leftOrRight: false);
		//??
		//??	popup.ButtonClicked += delegate (object sender, string name)
		//??	{
		//??		if (name == "ok")
		//??		{
		//??			this.SetRate (popup.Rate);
		//??		}
		//??	};
		//??}
		//??
		//??private void ShowYearsCalculatorPopup(Widget target)
		//??{
		//??	//	Affiche le Popup de calculation du nombre d'années.
		//??	var popup = new YearsCalculatorPopup (accessor)
		//??	{
		//??		Years = this.accessor.EditionAccessor.GetFieldDecimal (ObjectField.AmortizationYearCount),
		//??	};
		//??
		//??	popup.Create (target, leftOrRight: false);
		//??
		//??	popup.ButtonClicked += delegate (object sender, string name)
		//??	{
		//??		if (name == "ok")
		//??		{
		//??			this.SetYears (popup.Years);
		//??		}
		//??	};
		//??}


		//??private void SetRate(decimal? value)
		//??{
		//??	this.accessor.EditionAccessor.SetField (ObjectField.AmortizationRate, value);
		//??	this.rateController.Value = value;
		//??
		//??	this.OnValueEdited (ObjectField.AmortizationRate);
		//??}
		//??
		//??private void SetYears(decimal? value)
		//??{
		//??	this.accessor.EditionAccessor.SetField (ObjectField.AmortizationYearCount, value);
		//??	this.yearsController.Value = value;
		//??
		//??	this.OnValueEdited (ObjectField.AmortizationYearCount);
		//??}


		private MethodGuidFieldController		methodController;
		private EnumFieldController				periodicityController;
		private ArgumentValueFieldsController	argumentsController;
	}
}
