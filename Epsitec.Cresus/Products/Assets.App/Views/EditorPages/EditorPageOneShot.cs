//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public class EditorPageOneShot : AbstractEditorPage
	{
		public EditorPageOneShot(DataAccessor accessor, CommandContext commandContext, BaseType baseType, bool isTimeless)
			: base (accessor, commandContext, baseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent, hasColorsExplanation: true);

			new StaticText
			{
				Parent  = parent,
				Text    = Res.Strings.EditorPages.OneShot.Info.ToString (),
				Dock    = DockStyle.Top,
				Margins = new Epsitec.Common.Drawing.Margins (0, 0, 0, 20),
			};

			this.CreateStringController (parent, ObjectField.OneShotNumber,    editWidth: 90);
			this.CreateStringController (parent, ObjectField.OneShotUser);
			this.CreateDateController   (parent, ObjectField.OneShotDateEvent, DateRangeCategory.Mandat);
			this.CreateDateController   (parent, ObjectField.OneShotDateOperation);
			this.CreateStringController (parent, ObjectField.OneShotComment,   lineCount: 5);
		}
	}
}
