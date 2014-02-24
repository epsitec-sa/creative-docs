//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageUserField : AbstractEditorPage
	{
		public EditorPageUserField(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateStringController (parent, ObjectField.Name);
			this.CreateEnumController   (parent, ObjectField.UserFieldType, EnumDictionaries.DictFieldTypes, editWidth: 100);

			this.CreateSepartor (parent);

			new StaticText
			{
				Parent  = parent,
				Text    = "Dans le tableau:",
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 20, 10),
			};

			this.CreateIntController (parent, ObjectField.UserFieldColumnWidth);

			new StaticText
			{
				Parent  = parent,
				Text    = "Lors de l'édition:",
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 20, 10),
			};

			this.CreateIntController (parent, ObjectField.UserFieldLineWidth);
			this.CreateIntController (parent, ObjectField.UserFieldLineCount);
			this.CreateIntController (parent, ObjectField.UserFieldTopMargin);

			new StaticText
			{
				Parent  = parent,
				Text    = "Texte résumé:",
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 20, 10),
			};

			this.CreateIntController (parent, ObjectField.UserFieldSummaryOrder);
		}
	}
}
