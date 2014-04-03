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


		protected internal override void CreateUI(Widget parent)
		{
			parent = this.CreateScrollable (parent);

			this.CreateGroupGuidController (parent, ObjectField.GroupParent, BaseType.Groups);
			this.CreateSepartor            (parent);
			this.CreateStringController    (parent, ObjectField.Number, editWidth: 90);
			this.CreateStringController    (parent, ObjectField.Name);
			this.CreateStringController    (parent, ObjectField.Description, lineCount: 5);
		}
	}
}
