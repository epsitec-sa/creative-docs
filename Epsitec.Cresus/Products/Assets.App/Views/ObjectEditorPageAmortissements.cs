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
			var obj = this.accessor.GetObject (BaseType.Categories, guid);

			//	Copie les champs nécessaires.
			var nom = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Nom);
			if (!string.IsNullOrEmpty (nom))
			{
				this.accessor.SetObjectField (ObjectField.NomCatégorie1, nom);
			}

			var taux = ObjectCalculator.GetObjectPropertyDecimal (obj, null, ObjectField.TauxAmortissement);
			if (taux.HasValue)
			{
				this.accessor.SetObjectField (ObjectField.TauxAmortissement, taux);
			}

			var type = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.TypeAmortissement);
			if (!string.IsNullOrEmpty (type))
			{
				this.accessor.SetObjectField (ObjectField.TypeAmortissement, type);
			}

			var period = ObjectCalculator.GetObjectPropertyString (obj, null, ObjectField.Périodicité);
			if (!string.IsNullOrEmpty (period))
			{
				this.accessor.SetObjectField (ObjectField.Périodicité, period);
			}

			var residu = ObjectCalculator.GetObjectPropertyDecimal (obj, null, ObjectField.ValeurRésiduelle);
			if (residu.HasValue)
			{
				this.accessor.SetObjectField (ObjectField.ValeurRésiduelle, residu);
			}

			//	Met à jour les contrôleurs.
			this.SetObject (this.objectGuid, this.timestamp);
		}
	}
}
