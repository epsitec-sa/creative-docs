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

namespace Epsitec.Cresus.Core.DocumentPrintingUnitsEditor
{
	public class ValuesController
	{
		public ValuesController(Core.Business.BusinessContext businessContext, DocumentPrintingUnitsEntity documentPrintingUnitsEntity, Dictionary<string, string> printingUnits)
		{
			this.businessContext             = businessContext;
			this.documentPrintingUnitsEntity = documentPrintingUnitsEntity;
			this.printingUnits               = printingUnits;

			this.editableWidgets = new List<Widget> ();
		}


		public void CreateUI(Widget parent)
		{
			this.printingUnitsFrame = new Scrollable
			{
				Parent = parent,
				PreferredWidth = 300,
				Dock = DockStyle.Left,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.Auto,
				PaintViewportFrame = false,
			};

			new Separator
			{
				Parent = parent,
				PreferredWidth = 1,
				Dock = DockStyle.Left,
				Margins = new Margins (5, 0, 0, 0),
			};

			this.printingUnitsFrame.Viewport.IsAutoFitting = true;

			this.UpdateOptionButtons ();
		}


		public void Update()
		{
			this.UpdateOptionButtons ();
		}


		private void UpdateOptionButtons()
		{
		}



		private void SetDirty()
		{
			this.businessContext.NotifyExternalChanges ();
		}


		private readonly Core.Business.BusinessContext		businessContext;
		private readonly DocumentPrintingUnitsEntity		documentPrintingUnitsEntity;
		private readonly Dictionary<string, string>			printingUnits;
		private readonly List<Widget>						editableWidgets;

		private Scrollable									printingUnitsFrame;
	}
}
