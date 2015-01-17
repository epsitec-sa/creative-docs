//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.EditorPages
{
	public class EditorPagePerson : AbstractEditorPage
	{
		public EditorPagePerson(DataAccessor accessor, CommandContext commandContext, BaseType baseType, bool isTimeless)
			: base (accessor, commandContext, baseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent, hasColorsExplanation: false);

			foreach (var userField in accessor.UserFieldsAccessor.GetUserFields (BaseType.PersonsUserFields))
			{
				this.CreateController (parent, userField);
			}
		}
	}
}
