//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageAmortissements : AbstractEditorPage
	{
		public EditorPageAmortissements(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateStringController  (parent, ObjectField.CategoryName);
			this.CreateDecimalController (parent, ObjectField.AmortizationRate, DecimalFormat.Rate);
			this.CreateEnumController    (parent, ObjectField.AmortizationType, EnumDictionaries.DictAmortizationTypes, editWidth: 90);
			this.CreateEnumController    (parent, ObjectField.Periodicity, EnumDictionaries.DictPeriodicities, editWidth: 90);
			this.CreateEnumController    (parent, ObjectField.Prorata, EnumDictionaries.DictProrataTypes, editWidth: 90);
			this.CreateDecimalController (parent, ObjectField.Round, DecimalFormat.Amount);
			this.CreateDecimalController (parent, ObjectField.ResidualValue, DecimalFormat.Amount);

			new FrameBox
			{
				Parent          = parent,
				Dock            = DockStyle.Top,
				PreferredHeight = 10,
			};

			this.CreateStringController (parent, ObjectField.Compte1);
			this.CreateStringController (parent, ObjectField.Compte2);
			this.CreateStringController (parent, ObjectField.Compte3);
			this.CreateStringController (parent, ObjectField.Compte4);
			this.CreateStringController (parent, ObjectField.Compte5);
			this.CreateStringController (parent, ObjectField.Compte6);
			this.CreateStringController (parent, ObjectField.Compte7);
			this.CreateStringController (parent, ObjectField.Compte8);

			this.CreateImportButton (parent);
		}

		private void CreateImportButton(Widget parent)
		{
			const int h = 22;

			var line = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = h,
				Dock            = DockStyle.Top,
				Margins         = new Common.Drawing.Margins (0, 0, 20, 0),
			};


			var button = new Button
			{
				Parent        = line,
				Text          = "Importer",
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				PreferredSize = new Common.Drawing.Size (70, h),
				Dock          = DockStyle.Left,
				Margins       = new Common.Drawing.Margins (0, 0, 0, 0),
			};

#if false
			var radio1 = new RadioButton
			{
				Parent        = line,
				Text          = "Par copie",
				AutoFocus     = false,
				PreferredSize = new Common.Drawing.Size (80, h),
				Dock          = DockStyle.Left,
				Margins       = new Common.Drawing.Margins (20, 0, 0, 0),
				ActiveState   = ActiveState.Yes,
			};

			var radio2 = new RadioButton
			{
				Parent        = line,
				Text          = "Par référence",
				AutoFocus     = false,
				PreferredSize = new Common.Drawing.Size (100, h),
				Dock          = DockStyle.Left,
				Margins       = new Common.Drawing.Margins (0, 0, 0, 0),
				ActiveState   = ActiveState.No,
			};
#endif

			button.Clicked += delegate
			{
				this.ShowCategoriesPopup (button);
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
			//	Importe par copie.
			var catObj = this.accessor.GetObject (BaseType.Categories, guid);

			//	Copie les champs nécessaires.
			this.ImportField (catObj, ObjectField.Name,             ObjectField.CategoryName);
			this.ImportField (catObj, ObjectField.AmortizationRate, ObjectField.AmortizationRate);
			this.ImportField (catObj, ObjectField.AmortizationType, ObjectField.AmortizationType);
			this.ImportField (catObj, ObjectField.Periodicity,      ObjectField.Periodicity);
			this.ImportField (catObj, ObjectField.Prorata,          ObjectField.Prorata);
			this.ImportField (catObj, ObjectField.Round,            ObjectField.Round);
			this.ImportField (catObj, ObjectField.ResidualValue,    ObjectField.ResidualValue);
			this.ImportField (catObj, ObjectField.Compte1,          ObjectField.Compte1);
			this.ImportField (catObj, ObjectField.Compte2,          ObjectField.Compte2);
			this.ImportField (catObj, ObjectField.Compte3,          ObjectField.Compte3);
			this.ImportField (catObj, ObjectField.Compte4,          ObjectField.Compte4);
			this.ImportField (catObj, ObjectField.Compte5,          ObjectField.Compte5);
			this.ImportField (catObj, ObjectField.Compte6,          ObjectField.Compte6);
			this.ImportField (catObj, ObjectField.Compte7,          ObjectField.Compte7);
			this.ImportField (catObj, ObjectField.Compte8,          ObjectField.Compte8);

			//	Met à jour les contrôleurs.
			this.SetObject (this.objectGuid, this.timestamp);
		}

		private void ImportField(DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var typeSrc = DataAccessor.GetFieldType (fieldSrc);
			var typeDst = DataAccessor.GetFieldType (fieldDst);
			System.Diagnostics.Debug.Assert (typeSrc == typeDst);

			switch (typeSrc)
			{
				case FieldType.String:
					this.ImportFieldString (catObj, fieldSrc, fieldDst);
					break;

				case FieldType.Decimal:
					this.ImportFieldDecimal (catObj, fieldSrc, fieldDst);
					break;

				default:
					System.Diagnostics.Debug.Fail ("Not supported");
					break;
			}
		}

		private void ImportFieldString(DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var s = ObjectCalculator.GetObjectPropertyString (catObj, null, fieldSrc);
			if (!string.IsNullOrEmpty (s))
			{
				this.accessor.EditionAccessor.SetField (fieldDst, s);
			}
		}

		private void ImportFieldDecimal(DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var d = ObjectCalculator.GetObjectPropertyDecimal (catObj, null, fieldSrc);
			if (d.HasValue)
			{
				this.accessor.EditionAccessor.SetField (fieldDst, d);
			}
		}
	}
}
