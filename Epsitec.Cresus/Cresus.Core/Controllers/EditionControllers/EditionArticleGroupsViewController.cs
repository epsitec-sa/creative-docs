//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	[ControllerSubType (3)]
	public class EditionArticleGroupsViewController : EditionViewController<Entities.ArticleDefinitionEntity>
	{
		public EditionArticleGroupsViewController(string name, Entities.ArticleDefinitionEntity entity)
			: base (name, entity)
		{
		}

		protected override void CreateUI()
		{
			using (var builder = new UIBuilder (this))
			{
				builder.CreateHeaderEditorTile ();
				builder.CreateEditionTitleTile ("Data.ArticleGroup", "Groupes d'article");

				this.CreateUIMain (builder);

				builder.CreateFooterEditorTile ();
			}
		}

		protected override EditionStatus GetEditionStatus()
		{
			return EditionStatus.Valid;
		}

		private void CreateUIMain(UIBuilder builder)
		{
			var tile = builder.CreateEditionTile ();

			// TODO: code provisoire...
			System.Text.StringBuilder sb = new System.Text.StringBuilder ();

			for (int i=0; i<this.Entity.ArticleGroups.Count; i++)
			{
				var group = this.Entity.ArticleGroups[i];

				sb.Append ((i+1).ToString ());
				sb.Append (": ");
				sb.Append (group.Name);
				sb.Append (" (");
				sb.Append (group.Code);
				sb.Append (")<br/>");
			}

			builder.CreateStaticText (tile, 200, sb.ToString ());
		}
	}
}
