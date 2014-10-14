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

			this.CreateStringController  (parent, ObjectField.Number, editWidth: 90);
			this.CreateStringController  (parent, ObjectField.Name);
			this.CreateStringController  (parent, ObjectField.Description, lineCount: 5);

			this.rateController = this.CreateDecimalController (parent, ObjectField.AmortizationRate, DecimalFormat.Rate);
			this.CreateCalculatorButton ();

			this.typeController = this.CreateEnumController (parent, ObjectField.AmortizationType, EnumDictionaries.DictAmortizationTypes, editWidth: 90);

			this.CreateEnumController    (parent, ObjectField.Periodicity,   EnumDictionaries.DictPeriodicities, editWidth: 90);
			this.CreateEnumController    (parent, ObjectField.Prorata,       EnumDictionaries.DictProrataTypes,  editWidth: 90);
			this.CreateDecimalController (parent, ObjectField.Round,         DecimalFormat.Amount);
			this.CreateDecimalController (parent, ObjectField.ResidualValue, DecimalFormat.Amount);

			this.CreateSubtitle (parent, Res.Strings.EditorPages.Category.AccountsSubtitle.ToString ());

			//	On permet de choisir un compte dans le dernier plan comptable importé.
			//	A voir à l'usage s'il faut faire mieux ?
			var forcedDate = this.accessor.Mandat.AccountsDateRanges.LastOrDefault ().IncludeFrom;

			this.CreateAccountController (parent, ObjectField.Account1, forcedDate);
			this.CreateAccountController (parent, ObjectField.Account2, forcedDate);
			this.CreateAccountController (parent, ObjectField.Account3, forcedDate);
			this.CreateAccountController (parent, ObjectField.Account4, forcedDate);
			this.CreateAccountController (parent, ObjectField.Account5, forcedDate);
			this.CreateAccountController (parent, ObjectField.Account6, forcedDate);
			this.CreateAccountController (parent, ObjectField.Account7, forcedDate);

			this.entrySamples = new EntrySamples (this.accessor, forcedDate);
			this.entrySamples.CreateUI (parent);
		}

		private void CreateCalculatorButton()
		{
			//	Crée le bouton qui ouvre le Popup de calculation du taux.
			var text = Res.Strings.EditorPages.Category.CalculatorButton.ToString ();
			var width = text.GetTextWidth ();

			var button = new Button
			{
				Parent         = this.rateController.FrameBox,
				Text           = text,
				PreferredWidth = width + 20,
				ButtonStyle    = ButtonStyle.Icon,
				AutoFocus      = false,
				Dock           = DockStyle.Left,
			};

			button.Clicked += delegate
			{
				this.ShowCalculatorPopup (button);
			};
		}

		private void ShowCalculatorPopup(Widget target)
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

		private void SetRate(decimal? value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AmortizationRate, value);
			this.rateController.Value = value;

			this.OnValueEdited (ObjectField.AmortizationRate);
		}

		private void SetType(AmortizationType value)
		{
			this.accessor.EditionAccessor.SetField (ObjectField.AmortizationType, (int) value);
			this.typeController.Value = (int) value;

			this.OnValueEdited (ObjectField.AmortizationType);
		}


		private DecimalFieldController			rateController;
		private EnumFieldController				typeController;
	}
}
