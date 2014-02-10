//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageObject : AbstractEditorPage
	{
		public EditorPageObject(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateStringController (parent, ObjectField.Number, editWidth: 90);
			this.CreateStringController (parent, ObjectField.Name);
			this.CreateStringController (parent, ObjectField.Description, lineCount: 5);
			this.CreateStringController (parent, ObjectField.Maintenance);
			this.CreateStringController (parent, ObjectField.Color, editWidth: 90);
			this.CreateStringController (parent, ObjectField.SerialNumber);
		}
	}
}
