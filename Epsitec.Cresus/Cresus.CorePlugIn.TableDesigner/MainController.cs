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

namespace Epsitec.Cresus.CorePlugIn.TableDesigner
{
	public class MainController
	{
		public MainController(Core.Business.BusinessContext businessContext, PriceCalculatorEntity priceCalculatorEntity)
		{
			this.businessContext = businessContext;
			this.priceCalculatorEntity = priceCalculatorEntity;

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
				TabTitle = "Dimensions",
			};

			var pageVal = new TabPage
			{
				TabTitle = "Prix",
			};

			tabBook.Items.Add (pageDim);
			tabBook.Items.Add (pageVal);
			tabBook.ActivePage = pageDim;

			var dc = new DimensionsController (this.table);
			dc.CreateUI (pageDim);

			var tc = new TableController (this.table);
			tc.CreateUI (pageVal);
		}


		private static string GetDesciption(AbstractDimension dimension)
		{
			var builder = new System.Text.StringBuilder ();

			builder.Append (dimension.Name);
			builder.Append (": ");

			foreach (var v in dimension.Values)
			{
				if (v is decimal)
				{
					decimal s = (decimal) v;
					builder.Append (s.ToString ());
					builder.Append (", ");
				}
			}

			builder.Append ("<br/>");

			return builder.ToString ();
		}


		public void SaveDesign()
		{
		}


		private readonly Core.Business.BusinessContext	businessContext;
		private readonly PriceCalculatorEntity			priceCalculatorEntity;
		private readonly DimensionTable					table;
	}
}
