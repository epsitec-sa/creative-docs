//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageGroup : AbstractEditorPage
	{
		public EditorPageGroup(DataAccessor accessor, BaseType baseType, BaseType subBaseType, bool isTimeless)
			: base (accessor, baseType, subBaseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent);

			this.CreateGroupGuidController (parent, ObjectField.GroupParent, BaseType.Groups);
			this.CreateStringController    (parent, ObjectField.Name);
			this.CreateStringController    (parent, ObjectField.Description, lineCount: 5);
		}
	}
}
