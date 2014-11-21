//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public abstract class AbstractEditorPageCategory : AbstractEditorPage
	{
		public AbstractEditorPageCategory(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		protected void CreateCommonUI(Widget parent)
		{
			this.methodController      = this.CreateEnumController    (parent, ObjectField.AmortizationMethod,    EnumDictionaries.DictAmortizationMethods, editWidth: 250);
			this.rateController        = this.CreateDecimalController (parent, ObjectField.AmortizationRate,      DecimalFormat.Rate, editWidth: 90);
			                             this.CreateRateCalculatorButton ();
			this.yearsController       = this.CreateDecimalController (parent, ObjectField.AmortizationYearCount, DecimalFormat.Real, editWidth: 90);
			                             this.CreateYearsCalculatorButton ();
			this.typeController        = this.CreateEnumController    (parent, ObjectField.AmortizationType,      EnumDictionaries.DictAmortizationTypes, editWidth: 90);
			this.periodicityController = this.CreateEnumController    (parent, ObjectField.Periodicity,           EnumDictionaries.DictPeriodicities, editWidth: 90);
			this.prorataController     = this.CreateEnumController    (parent, ObjectField.Prorata,               EnumDictionaries.DictProrataTypes, editWidth: 90);
			this.roundController       = this.CreateDecimalController (parent, ObjectField.Round,                 DecimalFormat.Amount, editWidth: 90);
			this.residualController    = this.CreateDecimalController (parent, ObjectField.ResidualValue,         DecimalFormat.Amount, editWidth: 90);
			this.expressionController  = this.CreateStringController  (parent, ObjectField.Expression,            editWidth: AbstractFieldController.maxWidth, lineCount: 3);
		}

		protected void CreateAccountsUI(Widget parent, System.DateTime? forcedDate)
		{
			foreach (var field in DataAccessor.AccountFields)
			{
				this.CreateAccountController (parent, field, forcedDate);
			}

			this.entrySamples = new EntrySamples (this.accessor, forcedDate);
			this.entrySamples.CreateUI (parent);

			this.methodController.ValueEdited += delegate
			{
				this.UpdateControllers ();
			};
		}


		public override void SetObject(Guid objectGuid, Timestamp timestamp)
		{
			base.SetObject (objectGuid, timestamp);
			this.UpdateControllers ();
		}

		protected void UpdateControllers()
		{
			AmortizationMethod method;

			if (this.methodController.Value.HasValue)
			{
				method = (AmortizationMethod) this.methodController.Value;
			}
			else
			{
				method = AmortizationMethod.Unknown;
			}

			this.rateController       .IsReadOnly =  Amortizations.IsHidden (method, ObjectField.AmortizationRate);
			this.rateCalculatorButton     .Enable = !Amortizations.IsHidden (method, ObjectField.AmortizationRate);
			this.yearsController      .IsReadOnly =  Amortizations.IsHidden (method, ObjectField.AmortizationYearCount);
			this.yearsCalculatorButton    .Enable = !Amortizations.IsHidden (method, ObjectField.AmortizationYearCount);
			this.typeController       .IsReadOnly =  Amortizations.IsHidden (method, ObjectField.AmortizationType);
			this.periodicityController.IsReadOnly =  Amortizations.IsHidden (method, ObjectField.Periodicity);
			this.prorataController    .IsReadOnly =  Amortizations.IsHidden (method, ObjectField.Prorata);
			this.roundController      .IsReadOnly =  Amortizations.IsHidden (method, ObjectField.Round);
			this.residualController   .IsReadOnly =  Amortizations.IsHidden (method, ObjectField.ResidualValue);
			this.expressionController .IsReadOnly =  Amortizations.IsHidden (method, ObjectField.Expression);
		}


		protected void CreateRateCalculatorButton()
		{
			//	Crée le bouton qui ouvre le Popup de calculation du taux.
			this.rateCalculatorButton = new Button
			{
				Parent          = this.rateController.FrameBox,
				Text            = Res.Strings.EditorPages.Category.RateCalculatorButton.ToString (),
				PreferredWidth  = AbstractFieldController.maxWidth - 90,
				PreferredHeight = AbstractFieldController.lineHeight,
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				Dock            = DockStyle.Left,
			};

			this.rateCalculatorButton.Clicked += delegate
			{
				this.ShowRateCalculatorPopup (this.rateCalculatorButton);
			};
		}

		protected void CreateYearsCalculatorButton()
		{
			//	Crée le bouton qui ouvre le Popup de calculation du nombre d'années.
			this.yearsCalculatorButton = new Button
			{
				Parent          = this.yearsController.FrameBox,
				Text            = Res.Strings.EditorPages.Category.YearsCalculatorButton.ToString (),
				PreferredWidth  = AbstractFieldController.maxWidth - 90,
				PreferredHeight = AbstractFieldController.lineHeight,
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				Dock            = DockStyle.Left,
			};

			this.yearsCalculatorButton.Clicked += delegate
			{
				this.ShowYearsCalculatorPopup (this.yearsCalculatorButton);
			};
		}


		private void ShowRateCalculatorPopup(Widget target)
		{
			//	Affiche le Popup de calculation du taux.
			var popup = new RateCalculatorPopup (accessor)
			{
				Rate = this.accessor.EditionAccessor.GetFieldDecimal (ObjectField.AmortizationRate),
			};

			popup.Create (target, leftOrRight: false);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.SetRate (popup.Rate);
					this.SetType (AmortizationType.Linear);
				}
			};
		}

		private void ShowYearsCalculatorPopup(Widget target)
		{
			//	Affiche le Popup de calculation du nombre d'années.
			var popup = new YearsCalculatorPopup (accessor)
			{
				Years = this.accessor.EditionAccessor.GetFieldDecimal (ObjectField.AmortizationYearCount),
			};

			popup.Create (target, leftOrRight: false);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.SetYears (popup.Years);
				}
			};
		}


		private void SetRate(decimal? value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AmortizationRate, value);
			this.rateController.Value = value;

			this.OnValueEdited (ObjectField.AmortizationRate);
		}

		private void SetYears(decimal? value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AmortizationYearCount, value);
			this.yearsController.Value = value;

			this.OnValueEdited (ObjectField.AmortizationYearCount);
		}

		private void SetType(AmortizationType value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AmortizationType, (int) value);
			this.typeController.Value = (int) value;

			this.OnValueEdited (ObjectField.AmortizationType);
		}


		protected EnumFieldController			methodController;
		protected DecimalFieldController		rateController;
		protected Button						rateCalculatorButton;
		protected DecimalFieldController		yearsController;
		protected Button						yearsCalculatorButton;
		protected EnumFieldController			typeController;
		protected EnumFieldController			periodicityController;
		protected EnumFieldController			prorataController;
		protected DecimalFieldController		roundController;
		protected DecimalFieldController		residualController;
		protected StringFieldController			expressionController;
	}
}
