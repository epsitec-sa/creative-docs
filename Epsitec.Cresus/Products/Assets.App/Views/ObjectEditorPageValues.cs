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


		public override IEnumerable<EditionObjectPageType> ChildrenPageTypes
		{
			get
			{
				yield return EditionObjectPageType.Amortissements;
				yield return EditionObjectPageType.Compta;
			}
		}


		public override void CreateUI(Widget parent)
		{
			this.CreateComputedAmountController (parent, ObjectField.Valeur1);
			this.CreateComputedAmountController (parent, ObjectField.Valeur2);
			this.CreateComputedAmountController (parent, ObjectField.Valeur3);

#if true
			//	Code bidon pour tester la navigation à 3 niveaux.
			var button = new Button
			{
				Parent      = parent,
				Text        = "Amortissements",
				ButtonStyle = ButtonStyle.Icon,
				Dock        = DockStyle.Top,
				Margins     = new Common.Drawing.Margins (0, 400, 20, 0),
			};

			button.Clicked += delegate
			{
				this.OnPageOpen (EditionObjectPageType.Amortissements, ObjectField.Unknown);
			};
#endif
		}
	}
}
