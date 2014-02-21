//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageValues : AbstractEditorPage
	{
		public EditorPageValues(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateComputedAmountController (parent, ObjectField.MainValue);

			foreach (var userField in this.accessor.Settings.GetUserFields (BaseType.Assets)
				.Where (x => x.Type == FieldType.ComputedAmount))
			{
				this.CreateComputedAmountController (parent, userField.Field);
			}
		}
	}
}
