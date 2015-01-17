//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public class EditorPageCategory : AbstractEditorPageCategory
	{
		public EditorPageCategory(DataAccessor accessor, CommandContext commandContext, BaseType baseType, bool isTimeless)
			: base (accessor, commandContext, baseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent, hasColorsExplanation: false);

			this.CreateStringController  (parent, ObjectField.Number,      editWidth: 90);
			this.CreateStringController  (parent, ObjectField.Name);
			this.CreateStringController  (parent, ObjectField.Description, lineCount: 5);
			this.CreateCommonUI (parent);

			this.CreateSubtitle (parent, Res.Strings.EditorPages.Category.AccountsSubtitle.ToString ());

			//	On permet de choisir un compte dans le dernier plan comptable importé.
			//	A voir à l'usage s'il faut faire mieux ?
			var forcedDate = this.accessor.Mandat.AccountsDateRanges.LastOrDefault ().IncludeFrom;
			this.CreateAccountsUI (parent, forcedDate);
		}
	}
}
