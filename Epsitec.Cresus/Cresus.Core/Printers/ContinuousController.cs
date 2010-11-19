//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public class ContinuousController
	{
		public ContinuousController(AbstractEntityPrinter entityPrinter)
		{
			this.entityPrinter = entityPrinter;
		}


		public void CreateUI(FrameBox parent)
		{
			this.parent = parent;

			this.parent.SizeChanged += delegate
			{
				this.UpdateScroller ();
			};

			var documentPrinter = this.entityPrinter.GetDocumentPrinter (0);

			this.pagePreviewer = new Widgets.PrintedPagePreviewer ()
			{
				Parent = this.parent,
				DocumentPrinter = documentPrinter,
				IsContinuousPreview = true,
				CurrentPage = entityPrinter.GetPageRelative (0),
				Dock = DockStyle.Fill,
			};

			this.scroller = new VScroller ()
			{
				Parent = this.parent,
				Dock = DockStyle.Right,
				IsInverted = true,
			};

			this.scroller.ValueChanged += delegate
			{
				this.UpdatePagePreview ();
			};

			this.UpdateScroller ();
		}


		private void UpdateScroller()
		{
			double heightUsed = this.pagePreviewer.ContinuousHeight;
			double max = System.Math.Max (heightUsed-this.parent.Client.Size.Height, 0);

			this.scroller.MaxValue = (decimal) max;
			//?this.scroller.VisibleRangeRatio = 0;
		}

		private void UpdatePagePreview()
		{
			this.pagePreviewer.ContinuousVerticalOffset = (double) this.scroller.Value;
		}


		private readonly AbstractEntityPrinter			entityPrinter;

		private FrameBox								parent;
		private Widgets.PrintedPagePreviewer			pagePreviewer;
		private VScroller								scroller;
	}
}
