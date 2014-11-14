//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	/// <summary>
	/// Page utilisée pour l'onglet "Amortissement" d'un objet d'immobilisation.
	/// </summary>
	public class EditorPageAmortizationDefinition : AbstractEditorPage
	{
		public EditorPageAmortizationDefinition(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent, hasColorsExplanation: true);

			this.CreateImportButton (parent);

			                         this.CreateStringController  (parent, ObjectField.CategoryName);
			this.methodController  = this.CreateEnumController    (parent, ObjectField.AmortizationMethod, EnumDictionaries.DictAmortizationMethods, editWidth: 250);
			this.rateController    = this.CreateDecimalController (parent, ObjectField.AmortizationRate,   DecimalFormat.Rate);
			                         this.CreateEnumController    (parent, ObjectField.AmortizationType,   EnumDictionaries.DictAmortizationTypes, editWidth: 90);
			this.yearController    = this.CreateIntController     (parent, ObjectField.AmortizationYearCount);
			                         this.CreateEnumController    (parent, ObjectField.Periodicity,        EnumDictionaries.DictPeriodicities, editWidth: 90);
			this.prorataController = this.CreateEnumController    (parent, ObjectField.Prorata,            EnumDictionaries.DictProrataTypes, editWidth: 90);
			                         this.CreateDecimalController (parent, ObjectField.Round,              DecimalFormat.Amount);
			                         this.CreateDecimalController (parent, ObjectField.ResidualValue,      DecimalFormat.Amount);

			this.CreateSubtitle (parent, Res.Strings.EditorPages.Category.AccountsSubtitle.ToString ());

			foreach (var field in DataAccessor.AccountFields)
			{
				this.CreateAccountController (parent, field);
			}

			this.entrySamples = new EntrySamples (this.accessor, null);
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
			bool rate = (this.methodController.Value == (int) AmortizationMethod.Rate);

			this.rateController   .IsReadOnly = !rate;
			this.yearController   .IsReadOnly =  rate;
			this.prorataController.IsReadOnly = !rate;
		}

		private void CreateImportButton(Widget parent)
		{
			const int h = 30;

			var line = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = h,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, 20),
			};


			var button = new Button
			{
				Parent        = line,
				Text          = Res.Strings.EditorPages.AmortizationDefinition.Import.ToString (),
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				PreferredSize = new Size (70, h),
				Dock          = DockStyle.Left,
				Margins       = new Margins (AbstractFieldController.labelWidth+10, 0, 0, 0),
			};

			new StaticText
			{
				Parent        = line,
				Text          = Res.Strings.EditorPages.AmortizationDefinition.ImportHelp.ToString (),
				Dock          = DockStyle.Fill,
				Margins       = new Margins (10, 0, 0, 0),
			};

			button.Clicked += delegate
			{
				if (!this.isLocked)
				{
					this.ShowCategoriesPopup (button);
				}
			};
		}

		private void ShowCategoriesPopup(Widget target)
		{
			var popup = new CategoriesPopup (this.accessor);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.Import (guid);
			};
		}

		private void Import(Guid guid)
		{
			//	On importe la catégorie d'immobilisation directement dans l'objet en édition.
			CategoriesLogic.ImportCategoryToAsset (this.accessor, null, null, guid);

			//	Met à jour les contrôleurs.
			this.SetObject (this.objectGuid, this.timestamp);
		}


		private EnumFieldController				methodController;
		private DecimalFieldController			rateController;
		private IntFieldController				yearController;
		private EnumFieldController				prorataController;
	}
}
