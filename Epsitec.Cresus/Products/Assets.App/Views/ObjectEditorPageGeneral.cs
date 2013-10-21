//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditorPageGeneral : AbstractObjectEditorPage
	{
		public ObjectEditorPageGeneral(DataAccessor accessor)
			: base (accessor)
		{
		}


		protected override void CreateUI(Widget parent)
		{
			if (this.properties != null)
			{
				this.CreateStringController (parent, ObjectField.Numéro, editWidth: 90);
				this.CreateStringController (parent, ObjectField.Nom);
				this.CreateStringController (parent, ObjectField.Description, lineCount: 5);
				this.CreateStringController (parent, ObjectField.Responsable);
				this.CreateStringController (parent, ObjectField.Couleur, editWidth: 90);
				this.CreateStringController (parent, ObjectField.NuméroSérie);
			}
		}
	}
}
