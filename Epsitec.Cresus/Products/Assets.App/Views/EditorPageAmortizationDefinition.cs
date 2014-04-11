//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageAmortizationDefinition : AbstractEditorPage
	{
		public EditorPageAmortizationDefinition(DataAccessor accessor, BaseType baseType, BaseType subBaseType, bool isTimeless)
			: base (accessor, baseType, subBaseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent);

			this.CreateStringController  (parent, ObjectField.CategoryName);
			this.CreateDecimalController (parent, ObjectField.AmortizationRate, DecimalFormat.Rate);
			this.CreateEnumController    (parent, ObjectField.AmortizationType, EnumDictionaries.DictAmortizationTypes, editWidth: 90);
			this.CreateEnumController    (parent, ObjectField.Periodicity, EnumDictionaries.DictPeriodicities, editWidth: 90);
			this.CreateEnumController    (parent, ObjectField.Prorata, EnumDictionaries.DictProrataTypes, editWidth: 90);
			this.CreateDecimalController (parent, ObjectField.Round, DecimalFormat.Amount);
			this.CreateDecimalController (parent, ObjectField.ResidualValue, DecimalFormat.Amount);

			this.CreateSepartor (parent);

			this.CreateAccountGuidController (parent, ObjectField.Account1);
			this.CreateAccountGuidController (parent, ObjectField.Account2);
			this.CreateAccountGuidController (parent, ObjectField.Account3);
			this.CreateAccountGuidController (parent, ObjectField.Account4);
			this.CreateAccountGuidController (parent, ObjectField.Account5);
			this.CreateAccountGuidController (parent, ObjectField.Account6);
			//this.CreateAccountGuidController (parent, ObjectField.Account7);
			//this.CreateAccountGuidController (parent, ObjectField.Account8);

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
				Margins         = new Margins (0, 0, 20, 0),
			};


			var button = new Button
			{
				Parent        = line,
				Text          = "Importer",
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				PreferredSize = new Size (70, h),
				Dock          = DockStyle.Left,
				Margins       = new Margins (100+10, 0, 0, 0),
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
			this.ImportField (catObj, ObjectField.Account1,         ObjectField.Account1);
			this.ImportField (catObj, ObjectField.Account2,         ObjectField.Account2);
			this.ImportField (catObj, ObjectField.Account3,         ObjectField.Account3);
			this.ImportField (catObj, ObjectField.Account4,         ObjectField.Account4);
			this.ImportField (catObj, ObjectField.Account5,         ObjectField.Account5);
			this.ImportField (catObj, ObjectField.Account6,         ObjectField.Account6);
			this.ImportField (catObj, ObjectField.Account7,         ObjectField.Account7);
			this.ImportField (catObj, ObjectField.Account8,         ObjectField.Account8);

			//	Met à jour les contrôleurs.
			this.SetObject (this.objectGuid, this.timestamp);
		}

		private void ImportField(DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var typeSrc = this.accessor.GetFieldType (fieldSrc);
			var typeDst = this.accessor.GetFieldType (fieldDst);
			System.Diagnostics.Debug.Assert (typeSrc == typeDst);

			switch (typeSrc)
			{
				case FieldType.String:
					this.ImportFieldString (catObj, fieldSrc, fieldDst);
					break;

				case FieldType.Decimal:
					this.ImportFieldDecimal (catObj, fieldSrc, fieldDst);
					break;

				case FieldType.Int:
					this.ImportFieldInt (catObj, fieldSrc, fieldDst);
					break;

				case FieldType.GuidAccount:
					this.ImportFieldGuidAccount (catObj, fieldSrc, fieldDst);
					break;

				default:
					System.Diagnostics.Debug.Fail ("Not supported");
					break;
			}
		}

		private void ImportFieldString(DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var s = ObjectProperties.GetObjectPropertyString (catObj, null, fieldSrc);
			if (!string.IsNullOrEmpty (s))
			{
				this.accessor.EditionAccessor.SetField (fieldDst, s);
			}
		}

		private void ImportFieldDecimal(DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var d = ObjectProperties.GetObjectPropertyDecimal (catObj, null, fieldSrc);
			if (d.HasValue)
			{
				this.accessor.EditionAccessor.SetField (fieldDst, d);
			}
		}

		private void ImportFieldInt(DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var d = ObjectProperties.GetObjectPropertyInt (catObj, null, fieldSrc);
			if (d.HasValue)
			{
				this.accessor.EditionAccessor.SetField (fieldDst, d);
			}
		}

		private void ImportFieldGuidAccount(DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var d = ObjectProperties.GetObjectPropertyGuid (catObj, null, fieldSrc);
			if (!d.IsEmpty)
			{
				this.accessor.EditionAccessor.SetField (fieldDst, d);
			}
		}
	}
}
