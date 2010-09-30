//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	public class SummaryRelationViewController : SummaryViewController<Entities.RelationEntity>
	{
		public SummaryRelationViewController(string name, Entities.RelationEntity entity)
			: base (name, entity)
		{
		}


		protected override void CreateUI()
		{
			using (var data = TileContainerController.Setup (this))
			{
				this.CreateUIRelation (data);
				this.CreateUIMailContacts (data);
				this.CreateUITelecomContacts (data);
				this.CreateUIUriContacts (data);
				this.CreateUIAffairs (data);
			}
		}

		protected override EditionStatus GetEditionStatus()
		{
			var entity = this.Entity;
			return entity.IsEmpty () ? EditionStatus.Empty : EditionStatus.Valid;
		}

		protected override void UpdateEmptyEntityStatus(DataLayer.Context.DataContext context, bool isEmpty)
		{
			var entity = this.Entity;
			
			context.UpdateEmptyEntityStatus (entity, isEmpty);
			context.UpdateEmptyEntityStatus (entity.Person, isEmpty);
		}

		private void CreateUIRelation(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					Name				= "Customer",
					IconUri				= "Data.Customer",
					Title				= TextFormatter.FormatText ("Client"),
					CompactTitle		= TextFormatter.FormatText ("Client"),
					TextAccessor		= this.CreateAccessor (x => x.GetSummary ()),
					CompactTextAccessor = this.CreateAccessor (x => x.GetCompactSummary ()),
					EntityMarshaler		= this.CreateEntityMarshaler (),
				});
		}

		private void CreateUIMailContacts(SummaryDataItems data)
		{
			Common.CreateUIMailContacts (this.BusinessContext, data, this.EntityGetter, x => x.Person.Contacts);
		}

		private void CreateUITelecomContacts(SummaryDataItems data)
		{
			Common.CreateUITelecomContacts (this.BusinessContext, data, this.EntityGetter, x => x.Person.Contacts);
		}

		private void CreateUIUriContacts(SummaryDataItems data)
		{
			Common.CreateUIUriContacts (this.BusinessContext, data, this.EntityGetter, x => x.Person.Contacts);
		}


		private void CreateUIAffairs(SummaryDataItems data)
		{
			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "Affair",
					IconUri		 = "Data.Affair",
					Title		 = TextFormatter.FormatText ("Affaires"),
					CompactTitle = TextFormatter.FormatText ("Affaires"),
					Text		 = CollectionTemplate.DefaultEmptyText,
					DefaultMode  = ViewControllerMode.Summary,
				});

			var template = new CollectionTemplate<AffairEntity> ("Affair", this.BusinessContext);

			template.DefineText (x => x.GetSummary ());
			template.DefineCompactText (x => TextFormatter.FormatText (x.IdA));

			data.Add (this.CreateCollectionAccessor (template, x => x.Affairs));
		}
	}
}
