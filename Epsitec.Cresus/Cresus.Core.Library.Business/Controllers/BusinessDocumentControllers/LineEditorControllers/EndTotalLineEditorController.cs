//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class EndTotalLineEditorController : AbstractLineEditorController
	{
		public EndTotalLineEditorController(AccessData accessData)
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
				TabIndex = this.NextTabIndex,
				Enable = this.accessData.DocumentLogic.IsDiscountEditionEnabled,
			};

			var line1 = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 20),
				TabIndex = this.NextTabIndex,
			};

			var line2 = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 5),
				TabIndex = this.NextTabIndex,
			};

			var line3 = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 5),
				TabIndex = this.NextTabIndex,
			};

			//	Total arrêté.
			{
				var field = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Entity.FixedPriceAfterTax, x => this.Entity.FixedPriceAfterTax = x));
				this.PlaceLabelAndField (line1, 200, 100, "Grand total arrêté", field);

				this.firstFocusedWidget = field;
			}

			//	Textes.
			{
				var field = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Entity.TextForPrice, x => this.Entity.TextForPrice = x));
				this.PlaceLabelAndField (line2, 200, 400, "Texte pour le grand total", field);
			}
			{
				var field = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Entity.TextForFixedPrice, x => this.Entity.TextForFixedPrice = x));
				this.PlaceLabelAndField (line3, 200, 400, "Texte pour le grand total arrêté", field);
			}
		}

		public override FormattedText TitleTile
		{
			get
			{
				return "Grand total";
			}
		}


		private EndTotalDocumentItemEntity Entity
		{
			get
			{
				return this.entity as EndTotalDocumentItemEntity;
			}
		}
	}
}
