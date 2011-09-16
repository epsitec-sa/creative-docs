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
using Epsitec.Cresus.Core.Business.Finance;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public abstract class AbstractLineEditorController
	{
		public AbstractLineEditorController(AccessData accessData)
		{
			this.accessData    = accessData;
			this.billingMode   = accessData.BillingMode;
			this.tabIndex      = 1;
			this.updateActions = new List<System.Action> ();
		}

		
		public virtual FormattedText			TileTitle
		{
			get
			{
				return null;
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

		public void UpdateUI()
		{
			this.updateActions.ForEach (action => action ());
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

		protected abstract void CreateUI(UIBuilder builder);

		protected FrameBox PlaceLabelAndField(Widget parent, int labelWidth, int fieldWidth, FormattedText labelText, Widget field)
		{
			var frame = new FrameBox
			{
				Parent   = parent,
				TabIndex = this.GetNextTabIndex (),
			};

			if (fieldWidth == 0)
			{
				frame.Dock = DockStyle.Fill;
			}
			else
			{
				frame.Dock = DockStyle.Left;
				frame.PreferredWidth = labelWidth + AbstractLineEditorController.HorizontalSpace + fieldWidth;
			}

			this.CreateStaticText (frame, labelWidth, labelText, ContentAlignment.TopRight);

			field.Parent   = frame;
			field.Dock     = DockStyle.Fill;
			field.Margins  = Margins.Zero;
			field.TabIndex = this.GetNextTabIndex ();

			return frame;
		}

		protected StaticText CreateStaticText(Widget parent, int labelWidth, FormattedText labelText, ContentAlignment contentAlignment = ContentAlignment.TopLeft)
		{
			var label = new StaticText
			{
				FormattedText = labelText,
				ContentAlignment = contentAlignment,
				Parent = parent,
				Dock = DockStyle.Left,
				PreferredWidth = labelWidth,
				Margins = new Margins (0, AbstractLineEditorController.HorizontalSpace, 2, 0),
			};

			return label;
		}

		protected int GetNextTabIndex()
		{
			return this.tabIndex++;
		}

		protected static readonly int			HorizontalSpace = 5;


		protected readonly AccessData			accessData;
		protected readonly BillingMode			billingMode;
		protected readonly List<System.Action>	updateActions;

		protected AbstractEntity				entity;
		protected TileContainer					tileContainer;
		protected Widget						firstFocusedWidget;
		
		private int								tabIndex;
	}
}
