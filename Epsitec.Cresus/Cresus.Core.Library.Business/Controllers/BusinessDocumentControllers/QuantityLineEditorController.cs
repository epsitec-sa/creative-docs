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
	public class QuantityLineEditorController : AbstractLineEditorController
	{
		public QuantityLineEditorController(AccessData accessData)
			: base (accessData)
		{
		}

		protected override void CreateUI(UIBuilder builder)
		{
			var box = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Fill,
				Padding = new Margins (5),
			};

			var line1 = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 5),
			};

			var line2 = new FrameBox
			{
				Parent = box,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Margins = new Margins (0, 0, 0, 5),
			};

			//	Quantité.
			var quantityField = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Entity.Quantity, x => this.Entity.Quantity = x));
			this.PlaceLabelAndField (line1, 55, 80, "Quantité", quantityField);

			//	Unité.
			var unitController = new SelectionController<UnitOfMeasureEntity> (this.accessData.BusinessContext)
			{
				ValueGetter         = () => this.Entity.Unit,
				ValueSetter         = x => this.Entity.Unit = x,
				ReferenceController = new ReferenceController (() => this.Entity.Unit),
			};

			var unitField = builder.CreateCompactAutoCompleteTextField (null, "", unitController);
			this.PlaceLabelAndField (line1, 35, 80, "Unité", unitField.Parent);

			//	Type.
			var typeController = new SelectionController<ArticleQuantityColumnEntity> (this.accessData.BusinessContext)
			{
				ValueGetter         = () => this.Entity.QuantityColumn,
				ValueSetter         = x => this.Entity.QuantityColumn = x,
				ReferenceController = new ReferenceController (() => this.Entity.QuantityColumn),
			};

			var typeField = builder.CreateCompactAutoCompleteTextField (null, "", typeController);
			this.PlaceLabelAndField (line1, 65, 100, "Type", typeField.Parent);

			//	Date.
			var dateField = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Entity.BeginDate, x => this.Entity.BeginDate = x));
			this.PlaceLabelAndField (line1, 35, 100, "Date", dateField);
		}

		public override FormattedText TitleTile
		{
			get
			{
				return "Quantité";
			}
		}


		private ArticleQuantityEntity Entity
		{
			get
			{
				return this.entity as ArticleQuantityEntity;
			}
		}
	}
}
