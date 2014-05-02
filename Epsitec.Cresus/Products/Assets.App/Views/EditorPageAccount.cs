//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageAccount : AbstractEditorPage
	{
		public EditorPageAccount(DataAccessor accessor, BaseType baseType, BaseType subBaseType, bool isTimeless)
			: base (accessor, baseType, subBaseType, isTimeless)
		{
		}


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent);

			this.CreateGroupGuidController (parent, ObjectField.GroupParent, BaseType.Accounts);
			this.CreateSepartor            (parent);
			this.CreateStringController    (parent, ObjectField.Number, editWidth: 100);
			this.CreateStringController    (parent, ObjectField.Name);
			this.CreateSepartor            (parent);
			this.CreateEnumController      (parent, ObjectField.AccountCategory, EnumDictionaries.DictAccountCategories, editWidth: 100);
			this.CreateEnumController      (parent, ObjectField.AccountType,     EnumDictionaries.DictAccountTypes,      editWidth: 100);
			this.CreateSepartor            (parent);
			this.CreateStringController    (parent, ObjectField.Description, lineCount: 5);
		}
	}
}
