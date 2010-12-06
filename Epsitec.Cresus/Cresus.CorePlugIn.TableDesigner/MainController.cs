//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;

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
		}

		public void CreateUI(Widget parent)
		{
			var editorGroup = new FrameBox(parent);
			editorGroup.Dock = DockStyle.Fill;

			//	Crée les grands blocs de widgets.
			var band = new FrameBox(editorGroup);
			band.Dock = DockStyle.Fill;
		}


		public void SaveDesign()
		{
		}


		private readonly Core.Business.BusinessContext	businessContext;
		private readonly PriceCalculatorEntity			priceCalculatorEntity;
	}
}
