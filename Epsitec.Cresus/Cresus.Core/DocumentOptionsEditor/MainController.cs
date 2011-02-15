//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core.Entities;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentOptionsEditor
{
	public class MainController
	{
		public MainController(Core.Business.BusinessContext businessContext, DocumentOptionsEntity documentOptionsEntity)
		{
			this.businessContext       = businessContext;
			this.documentOptionsEntity = documentOptionsEntity;

			this.options = this.documentOptionsEntity.GetOptions ();
		}


		public void CreateUI(Widget parent)
		{
			var tabBook = new TabBook
			{
				Parent = parent,
				Arrows = TabBookArrows.Right,
				Dock = DockStyle.Fill,
			};

			//	Crée les onglets.
			var selectPage = new TabPage
			{
				Name = "select",
				TabTitle = "Choix des options à utiliser",
				Padding = new Margins (10),
			};

			var valuesPage = new TabPage
			{
				Name = "values",
				TabTitle = "Valeurs des options",
				Padding = new Margins (10),
			};

			tabBook.Items.Add (selectPage);
			tabBook.Items.Add (valuesPage);

			if (this.options.Count == 0)
			{
				tabBook.ActivePage = selectPage;
			}
			else
			{
				tabBook.ActivePage = valuesPage;
			}

			//	Peuple les onglets.
			var sc = new SelectController (this.businessContext, this.documentOptionsEntity, this.options);
			sc.CreateUI (selectPage);

			var vc = new ValuesController (this.businessContext, this.documentOptionsEntity, this.options);
			vc.CreateUI (valuesPage);

			//	Connection des événements.
			tabBook.ActivePageChanged += delegate
			{
				if (tabBook.ActivePage.Name == "select")
				{
					sc.Update ();
				}

				if (tabBook.ActivePage.Name == "values")
				{
					vc.Update ();
				}
			};
		}


		public void SaveDesign()
		{
			this.documentOptionsEntity.SetOptions (this.options);
		}


		private readonly Core.Business.BusinessContext		businessContext;
		private readonly DocumentOptionsEntity				documentOptionsEntity;
		private readonly Print.OptionsDictionary			options;
	}
}
