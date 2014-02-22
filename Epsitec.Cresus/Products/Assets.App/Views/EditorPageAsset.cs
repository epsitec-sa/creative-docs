//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageAsset : AbstractEditorPage
	{
		public EditorPageAsset(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateStringController (parent, ObjectField.Number, editWidth: 90);
			this.CreateStringController (parent, ObjectField.Name);
			this.CreateStringController (parent, ObjectField.Description, lineCount: 5);

			foreach (var userField in accessor.Settings.GetUserFields (BaseType.Assets)
				.Where (x => x.Type != FieldType.ComputedAmount && x.Type != FieldType.GuidPerson))
			{
				this.CreateController (parent, userField);
			}
		}
	}
}
