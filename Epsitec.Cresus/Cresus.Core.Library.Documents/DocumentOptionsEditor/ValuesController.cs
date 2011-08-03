//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.Verbose;
using Epsitec.Cresus.Core.Entities;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentOptionsEditor
{
	public class ValuesController
	{
		public ValuesController(IBusinessContext businessContext, DocumentOptionsEntity documentOptionsEntity, PrintingOptionDictionary options)
		{
			this.businessContext       = businessContext;
			this.documentOptionsEntity = documentOptionsEntity;
			this.options               = options;

			this.allOptions   = VerboseDocumentOption.GetAll ().Where (x => !x.IsTitle).ToList ();
			this.titleOptions = VerboseDocumentOption.GetAll ().Where (x => x.IsTitle).ToList ();

			this.editableWidgets = new List<Widget> ();
		}


		public void CreateUI(Widget parent)
		{
			this.optionsFrame = new Scrollable
			{
				Parent = parent,
				PreferredWidth = 300,
				Dock = DockStyle.Left,
				HorizontalScrollerMode = ScrollableScrollerMode.HideAlways,
				VerticalScrollerMode = ScrollableScrollerMode.Auto,
				PaintViewportFrame = false,
			};

			this.optionsFrame.Viewport.IsAutoFitting = true;

			this.UpdateOptionButtons ();
		}


		public void Update()
		{
			this.UpdateOptionButtons ();
		}


		private void UpdateOptionButtons()
		{
			var controller = new OptionsController (null, this.options);
			controller.CreateUI (this.optionsFrame.Viewport, this.SetDirty);
		}

		private void SetDirty()
		{
			this.businessContext.NotifyExternalChanges ();
		}


		private readonly IBusinessContext					businessContext;
		private readonly DocumentOptionsEntity				documentOptionsEntity;
		private readonly PrintingOptionDictionary			options;
		private readonly List<VerboseDocumentOption>		allOptions;
		private readonly List<VerboseDocumentOption>		titleOptions;
		private readonly List<Widget>						editableWidgets;

		private Scrollable									optionsFrame;
	}
}
