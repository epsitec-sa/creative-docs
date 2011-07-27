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
using Epsitec.Cresus.Core.Library;

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
			var leftFrame = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Fill,
				Padding = new Margins (10),
				TabIndex = this.NextTabIndex,
			};

			var rightFrame = new FrameBox
			{
				Parent = this.tileContainer,
				Dock = DockStyle.Right,
				PreferredWidth = 360,
				TabIndex = this.NextTabIndex,
			};

			var separator = new Separator
			{
				IsVerticalLine = true,
				PreferredWidth = 1,
				Parent = this.tileContainer,
				Dock = DockStyle.Right,
			};

			this.CreateUILeftFrame (builder, leftFrame);
			this.CreateUIRightFrame (builder, rightFrame);

			this.UpdateDateLine ();
		}

		private void CreateUILeftFrame(UIBuilder builder, FrameBox parent)
		{
			//	Pour conserver la même disposition que ArticleLineEditorController, cette partie reste vide.
		}
		
		private void CreateUIRightFrame(UIBuilder builder, FrameBox parent)
		{
			bool enable = this.accessData.BusinessLogic.ArticleQuantityTypeEditionEnabled.Select (x => x.Key).Contains (this.Entity.QuantityColumn.QuantityType);

			var topFrame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Top,
				PreferredHeight = 20,
				Padding = new Margins (10),
				TabIndex = this.NextTabIndex,
				Enable = enable,
			};

			var separator = new Separator
			{
				IsHorizontalLine = true,
				PreferredHeight = 1,
				Parent = parent,
				Dock = DockStyle.Top,
			};

			var bottomFrame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				PreferredWidth = 360,
				Padding = new Margins (10),
				TabIndex = this.NextTabIndex,
				Enable = enable,
			};

			this.CreateUIRightTopFrame (builder, topFrame);
			this.CreateUIRightBottomFrame (builder, bottomFrame);
		}

		private void CreateUIRightTopFrame(UIBuilder builder, FrameBox parent)
		{
			//	Quantité.
			var quantityField = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Entity.Quantity, x => this.Entity.Quantity = x));
			this.PlaceLabelAndField (parent, 50, 80, "Quantité", quantityField);

			this.firstFocusedWidget = quantityField;

			//	Unité.
			var unitController = new SelectionController<UnitOfMeasureEntity> (this.accessData.BusinessContext)
			{
				ValueGetter         = () => this.Entity.Unit,
				ValueSetter         = x => this.Entity.Unit = x,
				ReferenceController = new ReferenceController (() => this.Entity.Unit),
			};

			var unitField = builder.CreateCompactAutoCompleteTextField (null, "", unitController);
			this.PlaceLabelAndField (parent, 35, 80, "Unité", unitField.Parent);
		}

		private void CreateUIRightBottomFrame(UIBuilder builder, FrameBox parent)
		{
			//	Type.
			this.CreateTypeUI (parent);

			var rightBox = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
				TabIndex = this.NextTabIndex,
			};

			//	Date.
			this.dateLine = new FrameBox
			{
				Parent = rightBox,
				Dock = DockStyle.Top,
				TabIndex = this.NextTabIndex,
			};

			var dateField = builder.CreateTextField (null, DockStyle.None, 0, Marshaler.Create (() => this.Entity.BeginDate, x => this.Entity.BeginDate = x));
			this.PlaceLabelAndField (this.dateLine, 35, 100, "Date", dateField);
		}

		private Widget CreateTypeUI(Widget parent)
		{
			//	Crée le widget pour choisir le type de la quantité. Les boutons ne montrent que les types
			//	définis dans les réglages globaux (ArticleQuantityColumnEntity) et compatibles avec
			//	la logique d'entreprise.
			ItemPicker widget = new ItemPicker
			{
				Parent = parent,
				Dock = DockStyle.Left,
				Margins = new Margins (10, 0, 0, 0),
				PreferredWidth = 150,
				TabIndex = this.NextTabIndex,
			};

			foreach (var e in this.accessData.BusinessLogic.ArticleQuantityTypeEditionEnabled.Where (x => x.Key != ArticleQuantityType.Ordered))
			{
				if (e.Key != ArticleQuantityType.Ordered)
				{
					widget.Items.Add (e.Key.ToString (), e);
				}
			}

			widget.ValueToDescriptionConverter = delegate (object o)
			{
				var e = o as EnumKeyValues<ArticleQuantityType>;
				return e.Values[0];
			};

			widget.Cardinality = EnumValueCardinality.ExactlyOne;
			widget.RefreshContents ();

			widget.SelectedKey = this.Entity.QuantityColumn.QuantityType.ToString ();

			widget.SelectedItemChanged += delegate
			{
				var key = widget.Items.GetValue (widget.SelectedItemIndex) as EnumKeyValues<ArticleQuantityType>;
				var entity = this.accessData.BusinessLogic.GetArticleQuantityColumnEntity (key.Key);
				if (entity != null)
				{
					this.Entity.QuantityColumn = entity;
					this.UpdateDateLine ();
				}
			};

			return widget;
		}

		private void UpdateDateLine()
		{
			bool visible = (this.Entity.QuantityColumn.QuantityType != ArticleQuantityType.Ordered &&
							this.Entity.QuantityColumn.QuantityType != ArticleQuantityType.Information);

			this.dateLine.Visibility = visible;

			if (!visible)
			{
				this.Entity.BeginDate = null;
				this.Entity.EndDate   = null;
			}
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



		private FrameBox			dateLine;
	}
}
