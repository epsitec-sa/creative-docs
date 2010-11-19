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
				//IsInverted = true,
			};

			this.scroller.ValueChanged += delegate
			{
				this.UpdatePagePreview ();
			};

			this.UpdateScroller ();
		}


		private void UpdateScroller()
		{
			double totalHeight   = this.previewer.ContinuousHeight;
			double visibleHeight = this.previewer.Client.Size.Height;
			double max = System.Math.Max (totalHeight-visibleHeight, 0);

			if (max == 0)
			{
				this.scroller.MaxValue = 0;
				this.scroller.Value    = 0;
			}
			else
			{
				this.scroller.MaxValue          = (decimal) max;
				this.scroller.Value             = (decimal) max;
				this.scroller.VisibleRangeRatio = (decimal) (visibleHeight/totalHeight);
				this.scroller.SmallChange       = (decimal) 10;
				this.scroller.LargeChange       = (decimal) (visibleHeight*0.5);
			}
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
