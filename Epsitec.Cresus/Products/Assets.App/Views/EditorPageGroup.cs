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
		public EditorPageGroup(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateGuidController   (parent, ObjectField.Parent);
			this.CreateStringController (parent, ObjectField.Nom);
			this.CreateStringController (parent, ObjectField.Famille);
			this.CreateStringController (parent, ObjectField.Description, lineCount: 5);
		}
	}
}
