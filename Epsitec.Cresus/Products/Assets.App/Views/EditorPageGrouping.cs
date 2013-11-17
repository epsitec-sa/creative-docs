//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageGrouping : AbstractEditorPage
	{
		public EditorPageGrouping(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateGuidController   (parent, ObjectField.Parent);
			this.CreateStringController (parent, ObjectField.Numéro, editWidth: 90);
			this.CreateStringController (parent, ObjectField.Nom);
			this.CreateStringController (parent, ObjectField.Description, lineCount: 5);
		}
	}
}
