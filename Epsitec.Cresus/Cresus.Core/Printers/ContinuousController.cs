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
			System.Diagnostics.Debug.Assert (entityPrinter != null);
			this.entityPrinter = entityPrinter;
		}

		public void CreateUI(Widget parent)
		{
			System.Diagnostics.Debug.Assert (parent != null);
			this.parent = parent;

			//	Crée le visualisateur de page ContinuousPagePreviewer.
			this.previewer = new Widgets.ContinuousPagePreviewer ()
			{
				Parent          = this.parent,
				DocumentPrinter = this.entityPrinter.GetDocumentPrinter (0),
				Dock            = DockStyle.Fill,
			};

			this.previewer.SizeChanged += delegate
			{
				this.UpdateScroller ();
			};

			//	Crée l'ascenseur.
			this.scroller = new VScroller ()
			{
				Parent     = this.parent,
				Dock       = DockStyle.Right,
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
			double heightUsed = this.previewer.ContinuousHeight;
			double max = System.Math.Max (heightUsed-this.previewer.Client.Size.Height, 0);

			this.scroller.MaxValue = (decimal) max;
			//?this.scroller.VisibleRangeRatio = 0;
		}

		private void UpdatePagePreview()
		{
			this.previewer.ContinuousVerticalOffset = (double) this.scroller.Value;
		}


		private readonly AbstractEntityPrinter			entityPrinter;

		private Widget									parent;
		private Widgets.ContinuousPagePreviewer			previewer;
		private VScroller								scroller;
	}
}
