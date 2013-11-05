//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditorPageAmortissements : AbstractObjectEditorPage
	{
		public ObjectEditorPageAmortissements(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateStringController  (parent, ObjectField.NomCatégorie1);
			this.CreateDecimalController (parent, ObjectField.TauxAmortissement, DecimalFormat.Rate);
			this.CreateStringController  (parent, ObjectField.TypeAmortissement, editWidth: 90);
			this.CreateStringController  (parent, ObjectField.Périodicité, editWidth: 90);
			this.CreateDecimalController (parent, ObjectField.ValeurRésiduelle, DecimalFormat.Amount);

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
			this.ImportField (catObj, ObjectField.Nom,               ObjectField.NomCatégorie1);
			this.ImportField (catObj, ObjectField.TauxAmortissement, ObjectField.TauxAmortissement);
			this.ImportField (catObj, ObjectField.TypeAmortissement, ObjectField.TypeAmortissement);
			this.ImportField (catObj, ObjectField.Périodicité,       ObjectField.Périodicité);
			this.ImportField (catObj, ObjectField.ValeurRésiduelle,  ObjectField.ValeurRésiduelle);

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
				this.accessor.SetObjectField (fieldDst, s);
			}
		}

		private void ImportFieldDecimal(DataObject catObj, ObjectField fieldSrc, ObjectField fieldDst)
		{
			var d = ObjectCalculator.GetObjectPropertyDecimal (catObj, null, fieldSrc);
			if (d.HasValue)
			{
				this.accessor.SetObjectField (fieldDst, d);
			}
		}
	}
}
