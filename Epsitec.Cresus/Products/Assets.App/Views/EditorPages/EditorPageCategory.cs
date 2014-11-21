//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public class EditorPageCategory : AbstractEditorPage
	{
		public EditorPageCategory(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent, hasColorsExplanation: false);

			                             this.CreateStringController  (parent, ObjectField.Number,                editWidth: 90);
			                             this.CreateStringController  (parent, ObjectField.Name);
			                             this.CreateStringController  (parent, ObjectField.Description,           lineCount: 5);
			this.methodController      = this.CreateEnumController    (parent, ObjectField.AmortizationMethod,    EnumDictionaries.DictAmortizationMethods, editWidth: 250);
			this.rateController        = this.CreateDecimalController (parent, ObjectField.AmortizationRate,      DecimalFormat.Rate, editWidth: 90);
			                             this.CreateRateCalculatorButton ();
			this.yearsController       = this.CreateDecimalController (parent, ObjectField.AmortizationYearCount, DecimalFormat.Real, editWidth: 90);
			                             this.CreateYearsCalculatorButton ();
			this.typeController        = this.CreateEnumController    (parent, ObjectField.AmortizationType,      EnumDictionaries.DictAmortizationTypes, editWidth: 90);
			this.periodicityController = this.CreateEnumController    (parent, ObjectField.Periodicity,           EnumDictionaries.DictPeriodicities, editWidth: 90);
			this.prorataController     = this.CreateEnumController    (parent, ObjectField.Prorata,               EnumDictionaries.DictProrataTypes,  editWidth: 90);
			this.roundController       = this.CreateDecimalController (parent, ObjectField.Round,                 DecimalFormat.Amount, editWidth: 90);
			this.residualController    = this.CreateDecimalController (parent, ObjectField.ResidualValue,         DecimalFormat.Amount, editWidth: 90);

			this.CreateSubtitle (parent, Res.Strings.EditorPages.Category.AccountsSubtitle.ToString ());

			//	On permet de choisir un compte dans le dernier plan comptable importé.
			//	A voir à l'usage s'il faut faire mieux ?
			var forcedDate = this.accessor.Mandat.AccountsDateRanges.LastOrDefault ().IncludeFrom;

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

		private void UpdateControllers()
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
		}


		private void CreateRateCalculatorButton()
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

		private void CreateYearsCalculatorButton()
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
			//	Affiche le Popup de calculation du taux.
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


		private EnumFieldController				methodController;
		private DecimalFieldController			rateController;
		private Button							rateCalculatorButton;
		private DecimalFieldController			yearsController;
		private Button							yearsCalculatorButton;
		private EnumFieldController				typeController;
		private EnumFieldController				periodicityController;
		private EnumFieldController				prorataController;
		private DecimalFieldController			roundController;
		private DecimalFieldController			residualController;
	}
}
