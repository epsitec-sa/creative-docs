﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

		public override void CreateUI(FrameBox parent, AbstractEntity entity)
		{
			base.CreateUI (parent, entity);

			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
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

			var quantityField = this.accessData.UIBuilder.CreateTextField (box, 0, null, Marshaler.Create (() => this.Entity.Quantity, x => this.Entity.Quantity = x));
			this.PlaceLabelAndField (line1, 50, 80, "Quantité", quantityField);

			// TODO: Ne fonctionne pas, pfff...
			var unitController = new ReferenceController (() => this.Entity.Unit);
			var unitField = this.accessData.UIBuilder.CreateAutoCompleteTextField (parent, null, x => this.Entity.Unit = x as UnitOfMeasureEntity, unitController);
			this.PlaceLabelAndField (line1, 25, 80, "Unité", unitField.Parent);

			// TODO: Ne fonctionne pas, pfff...
			var typeController = new ReferenceController (() => this.Entity.QuantityColumn);
			var typeField = this.accessData.UIBuilder.CreateAutoCompleteTextField (parent, null, x => this.Entity.QuantityColumn = x as ArticleQuantityColumnEntity, typeController);
			this.PlaceLabelAndField (line1, 60, 100, "Type", typeField.Parent);

			var dateField = this.accessData.UIBuilder.CreateTextField (box, 0, null, Marshaler.Create (() => this.Entity.BeginDate, x => this.Entity.BeginDate = x));
			this.PlaceLabelAndField (line1, 25, 100, "Date", dateField);
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
