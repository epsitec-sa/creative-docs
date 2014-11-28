//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public class EditorPageUserField : AbstractEditorPage
	{
		public EditorPageUserField(DataAccessor accessor, CommandContext commandContext, BaseType baseType, bool isTimeless)
			: base (accessor, commandContext, baseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent, hasColorsExplanation: false);

			this.CreateStringController (parent, ObjectField.Name);

			Dictionary<int, string> dict;
			if (this.baseType == BaseType.AssetsUserFields)
			{
				dict = EnumDictionaries.GetDictFieldTypes ();
			}
			else
			{
				dict = EnumDictionaries.GetDictFieldTypes (hasComplexTypes: false);
			}
			this.CreateEnumController (parent, ObjectField.UserFieldType, dict, editWidth: 100);

			this.CreateBoolController (parent, ObjectField.UserFieldRequired);

			this.CreateSepartor (parent);

			new StaticText
			{
				Parent  = parent,
				Text    = Res.Strings.EditorPages.UserFields.TreeTable.ToString (),
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 20, 10),
			};

			this.CreateIntController (parent, ObjectField.UserFieldColumnWidth);

			new StaticText
			{
				Parent  = parent,
				Text    = Res.Strings.EditorPages.UserFields.Edition.ToString (),
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 20, 10),
			};

			this.CreateIntController (parent, ObjectField.UserFieldLineWidth);
			this.CreateIntController (parent, ObjectField.UserFieldLineCount);
			this.CreateIntController (parent, ObjectField.UserFieldTopMargin);

			new StaticText
			{
				Parent  = parent,
				Text    = Res.Strings.EditorPages.UserFields.Summary.ToString (),
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 20, 10),
			};

			this.CreateIntController (parent, ObjectField.UserFieldSummaryOrder);
		}
	}
}
