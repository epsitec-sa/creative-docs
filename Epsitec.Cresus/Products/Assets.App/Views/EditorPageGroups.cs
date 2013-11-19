//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageGroups : AbstractEditorPage
	{
		public EditorPageGroups(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
		}


		public override void CreateUI(Widget parent)
		{
			for (int i=0; i<6; i++)
			{
				this.CreateGuidController    (parent, ObjectField.GroupGuid+i);
				this.CreateDecimalController (parent, ObjectField.GroupRate+i, DecimalFormat.Rate);

				new FrameBox
				{
					Parent          = parent,
					Dock            = DockStyle.Top,
					PreferredHeight = 10,
				};
			}
		}
	}
}
