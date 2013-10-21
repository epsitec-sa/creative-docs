//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditorPageAmortissements : AbstractObjectEditorPage
	{
		public ObjectEditorPageAmortissements(DataAccessor accessor)
			: base (accessor)
		{
		}


		public override IEnumerable<EditionObjectPageType> ChildrenPageTypes
		{
			get
			{
				yield return EditionObjectPageType.Compta;
			}
		}


		protected override void CreateUI(Widget parent)
		{
			if (this.properties != null)
			{
				this.CreateStringController  (parent, ObjectField.NomCatégorie);
				this.CreateDecimalController (parent, ObjectField.TauxAmortissement, isRate: true);
				this.CreateStringController  (parent, ObjectField.TypeAmortissement, editWidth: 90);
				this.CreateDecimalController (parent, ObjectField.FréquenceAmortissement);
				this.CreateDecimalController (parent, ObjectField.ValeurRésiduelle);
			}
		}
	}
}
