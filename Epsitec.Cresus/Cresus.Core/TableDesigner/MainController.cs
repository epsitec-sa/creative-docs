//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.TableDesigner
{
	public class MainController
	{
		public MainController(Core.Business.BusinessContext businessContext, PriceCalculatorEntity priceCalculatorEntity, ArticleDefinitionEntity articleDefinitionEntity)
		{
			this.businessContext         = businessContext;
			this.priceCalculatorEntity   = priceCalculatorEntity;
			this.articleDefinitionEntity = articleDefinitionEntity;

			this.table = this.priceCalculatorEntity.GetPriceTable ();
		}

		public void CreateUI(Widget parent)
		{
			var tabBook = new TabBook
			{
				Parent = parent,
				Arrows = TabBookArrows.Right,
				Dock = DockStyle.Fill,
			};

			var pageDim = new TabPage
			{
				TabTitle = "Axes",
			};

			var pageVal = new TabPage
			{
				TabTitle = "Prix",
			};

			tabBook.Items.Add (pageDim);
			tabBook.Items.Add (pageVal);
			tabBook.ActivePage = pageDim;

			var dc = new DimensionsController (this.table, this.articleDefinitionEntity);
			dc.CreateUI (pageDim);

			var tc = new TableController (this.table);
			tc.CreateUI (pageVal);
		}


		public void SaveDesign()
		{
		}


		private readonly Core.Business.BusinessContext	businessContext;
		private readonly PriceCalculatorEntity			priceCalculatorEntity;
		private readonly ArticleDefinitionEntity		articleDefinitionEntity;
		private readonly DimensionTable					table;
	}
}
