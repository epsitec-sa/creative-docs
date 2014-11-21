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
	public class EditorPageAmortizationDefinition : AbstractEditorPageCategory
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
			this.CreateCommonUI (parent);

			this.CreateSubtitle (parent, Res.Strings.EditorPages.Category.AccountsSubtitle.ToString ());
			this.CreateAccountsUI (parent, null);
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
	}
}
