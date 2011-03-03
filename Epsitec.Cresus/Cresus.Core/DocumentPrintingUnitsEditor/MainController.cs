//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Print;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentPrintingUnitsEditor
{
	public class MainController
	{
		public MainController(Core.Business.BusinessContext businessContext, DocumentPrintingUnitsEntity documentPrintingUnitsEntity)
		{
			this.businessContext             = businessContext;
			this.documentPrintingUnitsEntity = documentPrintingUnitsEntity;

			throw new System.NotImplementedException ();
#if false
			this.printingUnits = this.documentPrintingUnitsEntity.GetPrintingUnits ();
#endif
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
				TabTitle = "Choix des types de pages à utiliser",
				Padding = new Margins (10),
			};

			var valuesPage = new TabPage
			{
				Name = "values",
				TabTitle = "Attribution des unités d'impression",
				Padding = new Margins (10),
			};

			tabBook.Items.Add (selectPage);
			tabBook.Items.Add (valuesPage);

			if (this.printingUnits.Count == 0)
			{
				tabBook.ActivePage = selectPage;
			}
			else
			{
				tabBook.ActivePage = valuesPage;
			}

			//	Peuple les onglets.
			var sc = new SelectController (this.businessContext, this.documentPrintingUnitsEntity, this.printingUnits);
			sc.CreateUI (selectPage);

			var vc = new ValuesController (this.businessContext, this.documentPrintingUnitsEntity, this.printingUnits);
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
			throw new System.NotImplementedException ();
#if false
			this.documentPrintingUnitsEntity.SetPrintingUnits (this.printingUnits);
#endif
		}


		private readonly Core.Business.BusinessContext		businessContext;
		private readonly DocumentPrintingUnitsEntity		documentPrintingUnitsEntity;
		private readonly PrintingUnitsDictionary			printingUnits;
	}
}
