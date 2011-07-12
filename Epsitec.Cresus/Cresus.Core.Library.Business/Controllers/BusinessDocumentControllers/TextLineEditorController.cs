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
	public class TextLineEditorController : AbstractLineEditorController
	{
		public TextLineEditorController(AccessData accessData)
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

			var textField = this.accessData.UIBuilder.CreateTextFieldMulti (box, DockStyle.Left, 40, Marshaler.Create (() => this.Entity.Text, x => this.Entity.Text = x));
			this.PlaceLabelAndField (box, 50, 500, "Texte", textField);
		}

		public override FormattedText TitleTile
		{
			get
			{
				return "Texte";
			}
		}


		private TextDocumentItemEntity Entity
		{
			get
			{
				return this.entity as TextDocumentItemEntity;
			}
		}
	}
}
