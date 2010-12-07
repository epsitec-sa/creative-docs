﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.TabIds;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionNumericValueArticleParameterDefinitionViewController : EditionViewController<Entities.NumericValueArticleParameterDefinitionEntity>
	{
		public EditionNumericValueArticleParameterDefinitionViewController(string name, Entities.NumericValueArticleParameterDefinitionEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleParameter", "Paramètre");

				this.CreateTabBook (builder);
				this.CreateUIMain1 (builder);
				this.CreateUIUnitOfMeasure (builder);
				this.CreateUIMain2 (builder);

				builder.CreateFooterEditorTile ();
			}
		}


		private void CreateTabBook(UIBuilder builder)
		{
			Common.CreateAbstractArticleParameterTabBook (builder, this, ArticleParameterTabId.Numeric);
		}


		private void CreateUIMain1(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile, 80, "Nom court", Marshaler.Create (() => this.Entity.Name, x => this.Entity.Name = x));
			builder.CreateTextField (tile,  0, "Nom long",  Marshaler.Create (() => this.Entity.Description, x => this.Entity.Description = x));
		}

		private void CreateUIUnitOfMeasure(UIBuilder builder)
		{
			var controller = new SelectionController<UnitOfMeasureEntity> (this.BusinessContext)
			{
				ValueGetter         = () => this.Entity.UnitOfMeasure,
				ValueSetter         = x => this.Entity.UnitOfMeasure = x,
				ReferenceController = new ReferenceController (() => this.Entity.UnitOfMeasure, creator: this.CreateNewUnitOfMeasure),
			};

			builder.CreateAutoCompleteTextField ("Unité", controller);
		}

		private void CreateUIMain2(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateMargin (tile, horizontalSeparator: true);

			var group1 = builder.CreateGroup (tile, "Valeurs minimale et maximale");
			builder.CreateTextField (group1, DockStyle.Left, 80, Marshaler.Create (() => this.Entity.MinValue, x => this.Entity.MinValue = x));
			builder.CreateTextField (group1, DockStyle.Left, 80, Marshaler.Create (() => this.Entity.MaxValue, x => this.Entity.MaxValue = x));

			builder.CreateTextField (tile, 80, "Valeur par défaut", Marshaler.Create (() => this.Entity.DefaultValue, x => this.Entity.DefaultValue = x));

			builder.CreateMargin (tile, horizontalSeparator: true);

			var group2 = builder.CreateGroup (tile, "Valeurs préférentielles");
			this.parameterController = new ArticleParameterControllers.ArticleParameterListPreferredValuesController (this.TileContainer, this.Entity);
			this.parameterController.CreateUI (group2, builder.ReadOnly);

			builder.CreateMargin (tile, horizontalSeparator: true);

			var group3 = builder.CreateGroup (tile, "Modulo et AddBeforeModulo");
			builder.CreateTextField (group3, DockStyle.Left, 80, Marshaler.Create (() => this.Entity.Modulo, x => this.Entity.Modulo = x));
			builder.CreateTextField (group3, DockStyle.Left, 80, Marshaler.Create (() => this.Entity.AddBeforeModulo, x => this.Entity.AddBeforeModulo = x));
		}


		private string PreferredValues
		{
			get
			{
				return Common.EnumInternalToSingleLine (this.Entity.PreferredValues);
			}
			set
			{
				this.Entity.PreferredValues = Common.EnumSingleLineToInternal (value);
			}
		}


		private NewEntityReference CreateNewUnitOfMeasure(DataContext context)
		{
			var title = context.CreateEntityAndRegisterAsEmpty<UnitOfMeasureEntity> ();
			return title;
		}


		private ArticleParameterControllers.ArticleParameterListPreferredValuesController		parameterController;
	}
}
