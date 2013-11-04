//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
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
				PreferredSize = new Common.Drawing.Size (100, h),
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
			};
		}
	}
}
