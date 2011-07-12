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
	public abstract class AbstractLineEditorController
	{
		public AbstractLineEditorController(AccessData accessData)
		{
			this.accessData = accessData;
		}

		public virtual void CreateUI(FrameBox parent, AbstractEntity entity)
		{
			this.entity = entity;
		}

		public virtual FormattedText TitleTile
		{
			get
			{
				return null;
			}
		}


		protected FrameBox PlaceLabelAndField(Widget parent, int labelWidth, int fieldWidth, FormattedText labelText, Widget field)
		{
			var box = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Left,
				PreferredWidth = labelWidth + 5 + fieldWidth,
			};

			var label = new StaticText
			{
				FormattedText = labelText,
				ContentAlignment = Common.Drawing.ContentAlignment.TopRight,
				Parent = box,
				Dock = DockStyle.Left,
				PreferredWidth = labelWidth + 5,
				Margins = new Margins (0, 5, 2, 0),
			};

			field.Parent = box;
			field.Dock = DockStyle.Fill;

			return box;
		}


		protected readonly AccessData					accessData;

		protected AbstractEntity						entity;
	}
}
