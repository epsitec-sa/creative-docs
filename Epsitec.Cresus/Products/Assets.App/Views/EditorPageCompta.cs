//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPageCompta : AbstractEditorPage
	{
		public EditorPageCompta(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateStringController (parent, ObjectField.Compte1);
			this.CreateStringController (parent, ObjectField.Compte2);
			this.CreateStringController (parent, ObjectField.Compte3);
			this.CreateStringController (parent, ObjectField.Compte4);
			this.CreateStringController (parent, ObjectField.Compte5);
			this.CreateStringController (parent, ObjectField.Compte6);
			this.CreateStringController (parent, ObjectField.Compte7);
			this.CreateStringController (parent, ObjectField.Compte8);
		}
	}
}
