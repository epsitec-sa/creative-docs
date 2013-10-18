//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectEditorPageValues : AbstractObjectEditorPage
	{
		public ObjectEditorPageValues(DataAccessor accessor)
			: base (accessor)
		{
		}


		public override string PageTitle
		{
			get
			{
				return "Valeurs";
			}
		}


		protected override void CreateUI(Widget parent)
		{
			if (this.properties != null)
			{
				this.CreateComputedAmountController (parent, ObjectField.Valeur1);
				this.CreateComputedAmountController (parent, ObjectField.Valeur2);
				this.CreateComputedAmountController (parent, ObjectField.Valeur3);
			}
		}
	}
}
