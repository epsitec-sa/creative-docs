//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class TaxLineEditorController : AbstractLineEditorController
	{
		public TaxLineEditorController(AccessData accessData)
			: base (accessData)
		{
		}

		protected override void CreateUI(UIBuilder builder)
		{
			var box = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Fill,
				Padding = new Margins (10),
				TabIndex = this.GetNextTabIndex (),
				Enable = this.accessData.DocumentLogic.IsPriceEditionEnabled,
			};

			var line1 = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 5),
				TabIndex = this.GetNextTabIndex (),
			};

			//	Textes.
			{
				var field = builder.CreateTextField (null, DockStyle.None, 0, false, Marshaler.Create (() => this.Entity.Text, x => this.Entity.Text = x));
				this.PlaceLabelAndField (line1, 200, 400, "Texte pour la TVA", field);

				this.firstFocusedWidget = field;
			}
		}

		public override FormattedText TileTitle
		{
			get
			{
				return "TVA";
			}
		}


		private TaxDocumentItemEntity Entity
		{
			get
			{
				return this.entity as TaxDocumentItemEntity;
			}
		}
	}
}
