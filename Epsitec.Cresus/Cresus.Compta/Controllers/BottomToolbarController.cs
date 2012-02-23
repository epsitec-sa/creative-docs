//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la barre d'outil inférieure pour la comptabilité.
	/// </summary>
	public class BottomToolbarController
	{
		public BottomToolbarController(BusinessContext businessContext)
		{
			this.businessContext = businessContext;
		}


		public bool ShowPanel
		{
			get
			{
				return this.showPanel;
			}
			set
			{
				this.showPanel = value;
				this.toolbar.Visibility = this.showPanel;
			}
		}


		public FrameBox CreateUI(FrameBox parent)
		{
			this.toolbar = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = BottomToolbarController.toolbarHeight,
				Dock            = DockStyle.Bottom,
				Margins         = new Margins (0, 0, 1, 0),
				Padding         = new Margins (0, 20, 0, 0),
			};

			this.operationLabel = new StaticText
			{
				Parent           = this.toolbar,
				ContentAlignment = ContentAlignment.MiddleLeft,
				PreferredWidth   = 300,
				PreferredHeight  = BottomToolbarController.toolbarHeight,
				Dock             = DockStyle.Left,
			};

			this.editionLabel = new StaticText
			{
				Parent           = this.toolbar,
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredHeight  = BottomToolbarController.toolbarHeight,
				Dock             = DockStyle.Fill,
			};

			return this.toolbar;
		}


		public void SetOperationDescription(FormattedText text, bool hilited)
		{
#if false
			text = text.ApplyFontSize (12.5);

			if (hilited)
			{
				text = text.ApplyBold ().ApplyFontColor (Color.FromHexa ("b00000"));  // gras + rouge
			}
#else
			if (hilited)
			{
				text = text.ApplyBold ();
			}
#endif

			this.operationLabel.FormattedText = text;
		}

		public void SetEditionDescription(FormattedText text)
		{
			this.editionLabel.FormattedText = text;
		}


		private static readonly double			toolbarHeight = 20;

		private readonly BusinessContext		businessContext;

		private FrameBox						toolbar;
		private StaticText						operationLabel;
		private StaticText						editionLabel;
		private bool							showPanel;
	}
}
