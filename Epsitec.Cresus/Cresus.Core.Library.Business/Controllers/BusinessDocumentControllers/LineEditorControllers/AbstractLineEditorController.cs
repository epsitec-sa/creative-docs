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
using Epsitec.Cresus.Core.Business.Finance;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public abstract class AbstractLineEditorController
	{
		public AbstractLineEditorController(AccessData accessData)
		{
			this.accessData = accessData;
			this.tabIndex = 1;

			if ((this.accessData.BusinessDocument.IsNotNull ()) &&
				(this.accessData.BusinessDocument.PriceGroup.IsNotNull ()))
			{
				this.billingMode = this.accessData.BusinessDocument.PriceGroup.BillingMode;
			}
		}

		public void CreateUI(Widget parent, AbstractEntity entity)
		{
			this.entity = entity;

			var controller = this.accessData.ViewController;
			
			this.tileContainer = new TileContainer (controller)
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			using (var builder = new UIBuilder (controller))
			{
				this.CreateUI (builder);
			}
		}

		protected abstract void CreateUI(UIBuilder builder);

		public virtual FormattedText TitleTile
		{
			get
			{
				return null;
			}
		}

		public void SetInitialFocus()
		{
			if (this.firstFocusedWidget != null)
			{
				this.firstFocusedWidget.Focus ();

				if (this.firstFocusedWidget is AbstractTextField)
				{
					var textField = this.firstFocusedWidget as AbstractTextField;
					textField.SelectAll ();
				}
			}
		}


		protected FrameBox PlaceLabelAndField(Widget parent, int labelWidth, int fieldWidth, FormattedText labelText, Widget field)
		{
			var box = new FrameBox
			{
				Parent = parent,
				TabIndex = this.NextTabIndex,
			};

			if (fieldWidth == 0)
			{
				box.Dock = DockStyle.Fill;
			}
			else
			{
				box.Dock = DockStyle.Left;
				box.PreferredWidth = labelWidth + 5 + fieldWidth;
			}

			var label = new StaticText
			{
				FormattedText = labelText,
				ContentAlignment = ContentAlignment.TopRight,
				Parent = box,
				Dock = DockStyle.Left,
				PreferredWidth = labelWidth,
				Margins = new Margins (0, 5, 2, 0),
			};

			field.Parent = box;
			field.Dock = DockStyle.Fill;
			field.Margins = new Margins (0);
			field.TabIndex = this.NextTabIndex;

			return box;
		}

		protected StaticText CreateStaticText(Widget parent, int labelWidth, FormattedText labelText)
		{
			var label = new StaticText
			{
				FormattedText = labelText,
				ContentAlignment = ContentAlignment.TopLeft,
				Parent = parent,
				Dock = DockStyle.Left,
				PreferredWidth = labelWidth,
				Margins = new Margins (0, 5, 2, 0),
			};

			return label;
		}

		protected int NextTabIndex
		{
			get
			{
				return this.tabIndex++;
			}
		}


		protected readonly AccessData					accessData;
		protected readonly BillingMode					billingMode;

		protected AbstractEntity						entity;
		protected TileContainer							tileContainer;
		protected Widget								firstFocusedWidget;
		private int										tabIndex;
	}
}
