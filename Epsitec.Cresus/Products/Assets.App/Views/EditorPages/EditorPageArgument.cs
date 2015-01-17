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
	public class EditorPageArgument : AbstractEditorPage
	{
		public EditorPageArgument(DataAccessor accessor, CommandContext commandContext, BaseType baseType, bool isTimeless)
			: base (accessor, commandContext, baseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent, hasColorsExplanation: false);

			this.CreateStringController (parent, ObjectField.Name);
			this.CreateStringController (parent, ObjectField.Description);
			this.CreateSepartor         (parent, 30);
			this.CreateEnumController   (parent, ObjectField.ArgumentType, EnumDictionaries.DictArgumentTypes);
			this.CreateBoolController   (parent, ObjectField.ArgumentNullable);
			this.CreateStringController (parent, ObjectField.ArgumentVariable);
			this.CreateStringController (parent, ObjectField.ArgumentDefault);
		}
	}
}
