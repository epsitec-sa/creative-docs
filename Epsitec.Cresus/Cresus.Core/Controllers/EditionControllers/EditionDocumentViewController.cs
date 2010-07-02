//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	public class EditionDocumentViewController : EditionViewController<Entities.DocumentEntity>
	{
		public EditionDocumentViewController(string name, Entities.DocumentEntity entity)
			: base (name, entity)
		{
			this.InitializeDefaultValues ();
		}


		private void InitializeDefaultValues()
		{
			if (this.Entity.CreationDate.Ticks == 0)
			{
				this.Entity.CreationDate = System.DateTime.Now;
			}

			if (this.Entity.LastModificationDate.Ticks == 0)
			{
				this.Entity.LastModificationDate = System.DateTime.Now;
			}
		}


		protected override EditionStatus GetEditionStatus()
		{
			if (string.IsNullOrEmpty (this.Entity.Description))
			{
				return EditionStatus.Empty;
			}

			// TODO: Comment implémenter un vraie validation ? Est-ce que le Marshaler sait faire cela ?

			return EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			context.UpdateEmptyEntityStatus (entity, isEmpty);
		}


		protected override void CreateUI(TileContainer container)
		{
			using (var builder = new UIBuilder (container, this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.Document", "Document");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}

			//	Summary:
			var containerController = new TileContainerController (this, container);
			var data = containerController.DataItems;

			this.CreateUIComments (data);

			containerController.GenerateTiles ();
		}

		private void CreateUIComments(SummaryDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (data, this.EntityGetter, x => x.Comments);
		}


		private void CreateUIMain(Epsitec.Cresus.Core.UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			builder.CreateTextField (tile,   0, "Description",                            Marshaler.Create (() => this.Entity.Description,          x => this.Entity.Description = x));
			builder.CreateTextField (tile, 150, "Date et heure de création",              Marshaler.Create (() => this.Entity.CreationDate,         x => this.Entity.CreationDate = x));
			builder.CreateTextField (tile, 150, "Date et heure de dernière modification", Marshaler.Create (() => this.Entity.LastModificationDate, x => this.Entity.LastModificationDate = x));
		}
	}
}
