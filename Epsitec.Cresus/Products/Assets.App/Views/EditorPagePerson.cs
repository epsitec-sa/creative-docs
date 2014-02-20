//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPagePerson : AbstractEditorPage
	{
		public EditorPagePerson(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
#if false
			this.CreateStringController (parent, ObjectField.Title, editWidth: 120);
			this.CreateStringController (parent, ObjectField.FirstName);
			this.CreateStringController (parent, ObjectField.Name);
			this.CreateStringController (parent, ObjectField.Company);
			this.CreateStringController (parent, ObjectField.Address, lineCount: 2);
			this.CreateStringController (parent, ObjectField.Zip, editWidth: 60);
			this.CreateStringController (parent, ObjectField.City);
			this.CreateStringController (parent, ObjectField.Country);

			this.CreateSepartor (parent);

			this.CreateStringController (parent, ObjectField.Phone1, editWidth: 120);
			this.CreateStringController (parent, ObjectField.Phone2, editWidth: 120);
			this.CreateStringController (parent, ObjectField.Phone3, editWidth: 120);
			this.CreateStringController (parent, ObjectField.Mail);

			this.CreateSepartor (parent);

			this.CreateStringController (parent, ObjectField.Description, lineCount: 5);
#else
			foreach (var userField in accessor.Settings.GetUserFields (BaseType.Persons))
			{
				if (userField.TopMargin > 0)
				{
					this.CreateSepartor (parent, userField.TopMargin);
				}

				this.CreateStringController (parent, userField.Field, userField.LineWidth, userField.LineCount);
			}
#endif
		}
	}
}
