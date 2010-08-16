﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	public class SummaryArticleDefinitionViewController : SummaryViewController<Entities.ArticleDefinitionEntity>
	{
		public SummaryArticleDefinitionViewController(string name, Entities.ArticleDefinitionEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIArticleDefinition (data);
				this.CreateUIComments (data);
			}
		}
		

		private void CreateUIArticleDefinition(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "ArticleDefinition",
					IconUri				= "Data.ArticleDefinition",
					Title				= TextFormatter.FormatText ("Article"),
					CompactTitle		= TextFormatter.FormatText ("Article"),
					TextAccessor		= Accessor.Create (this.EntityGetter, x => TextFormatter.FormatText ("N°", x.IdA, "\n", x.ShortDescription)),
					CompactTextAccessor = Accessor.Create (this.EntityGetter, x => TextFormatter.FormatText ("N°", x.IdA, "\n", x.LongDescription)),
					EntityMarshaler		= this.EntityMarshaler,
				});
		}

		private void CreateUIComments(SummaryDataItems data)
		{
			SummaryControllers.Common.CreateUIComments (this.DataContext, data, this.EntityGetter, x => x.Comments);
		}



		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return entity.IsEmpty () ? EditionStatus.Empty : EditionStatus.Valid;
		}
	}
}
