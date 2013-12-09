//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EditorPagePersons : AbstractEditorPage
	{
		public EditorPagePersons(DataAccessor accessor, BaseType baseType, bool isTimeless)
			: base (accessor, baseType, isTimeless)
		{
		}


		public override void CreateUI(Widget parent)
		{
			this.CreatePersonGuidController (parent, ObjectField.Personne1);
			this.CreatePersonGuidController (parent, ObjectField.Personne2);
			this.CreatePersonGuidController (parent, ObjectField.Personne3);
			this.CreatePersonGuidController (parent, ObjectField.Personne4);
			this.CreatePersonGuidController (parent, ObjectField.Personne5);
		}
	}
}
