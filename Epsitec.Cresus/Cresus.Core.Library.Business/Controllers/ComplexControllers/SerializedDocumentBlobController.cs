//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Accounting;
using Epsitec.Cresus.Core.Print;
using Epsitec.Cresus.Core.Print.Controllers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Controllers.ComplexControllers
{
	/// <summary>
	/// Permet de voir un document déjà imprimé.
	/// </summary>
	public class SerializedDocumentBlobController
	{
		public SerializedDocumentBlobController(BusinessContext businessContext, SerializedDocumentBlobEntity serializedDocumentBlobEntity)
		{
			this.businessContext = businessContext;
			this.serializedDocumentBlobEntity = serializedDocumentBlobEntity;
		}


		public void CreateUI(Widget parent)
		{
			var previewFrame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 0, 0),
			};

			var footer = new FrameBox
			{
				Parent = parent,
				PreferredHeight = 24,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 10, 10),
			};

			var toolbarFrame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, Library.UI.Constants.RightMargin, 2, 0),
			};

			var jobs = PrintEngine.SearchXmlSource (this.businessContext, this.serializedDocumentBlobEntity);

			var controller = new XmlPreviewerController (this.businessContext, jobs, showCheckButtons: true);
			controller.CreateUI (previewFrame, toolbarFrame);

			var button = new Button
			{
				Parent = footer,
				Text = "Réimprimer le document",
				Dock = DockStyle.Fill,
			};

			button.Clicked += delegate
			{
				PrintEngine.PrintJobs (this.businessContext, jobs);
			};
		}


		private readonly BusinessContext				businessContext;
		private readonly SerializedDocumentBlobEntity	serializedDocumentBlobEntity;
	}
}
