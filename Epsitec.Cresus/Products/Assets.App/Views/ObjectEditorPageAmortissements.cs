//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditorPageAmortissements : AbstractObjectEditorPage
	{
		public ObjectEditorPageAmortissements(DataAccessor accessor)
			: base (accessor)
		{
			this.baseType = BaseType.Objects;
		}


		public override IEnumerable<EditionObjectPageType> ChildrenPageTypes
		{
			get
			{
				yield return EditionObjectPageType.Compta;
			}
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateStringController (parent, ObjectField.NomCatégorie1);
			this.CreateStringController (parent, ObjectField.NomCatégorie2);
			this.CreateStringController (parent, ObjectField.NomCatégorie3);
		}
	}
}
