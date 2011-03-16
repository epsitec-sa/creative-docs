//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	internal static class Common
	{
		public static void CreateUIComments<T1>(BusinessContext businessContext, TileDataItems data, System.Func<T1> source, System.Func<T1, IList<CommentEntity>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIComments<T1, CommentEntity, CommentEntity> (businessContext, data, source, collectionResolver);
		}

		private static void InternalCreateUIComments<T1, T2, T3>(BusinessContext businessContext, TileDataItems data, System.Func<T1> source, System.Func<T1, IList<T2>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : CommentEntity, new ()
			where T3 : CommentEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("Comment", businessContext)
				.DefineText        (x => x.GetCompactSummary ())
				.DefineCompactText (x => x.GetCompactSummary ());

			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "Comment",
					IconUri		 = "Data.Comment",
					Title		 = TextFormatter.FormatText ("Commentaires"),
					CompactTitle = TextFormatter.FormatText ("Commentaires"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
		}


		public static void CreateUIMailContacts<T1>(BusinessContext businessContext, TileDataItems data, System.Func<T1> source, System.Func<T1, IList<AbstractContactEntity>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIMailContacts<T1, AbstractContactEntity, MailContactEntity> (businessContext, data, source, collectionResolver);
		}

		private static void InternalCreateUIMailContacts<T1, T2, T3>(BusinessContext businessContext, TileDataItems data, System.Func<T1> source, System.Func<T1, IList<T2>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : MailContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("MailContact", businessContext)
				.DefineTitle		(x => x.GetTitle ())
				.DefineText			(x => x.GetSummary ())
				.DefineCompactText	(x => x.GetCompactSummary ());


			data.Add (
				new TileDataItem
				{
					Name		 = "MailContact",
					IconUri		 = "Data.MailContact",
					Title		 = TextFormatter.FormatText ("Adresses"),
					CompactTitle = TextFormatter.FormatText ("Adresses"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
		}


		public static void CreateUITelecomContacts<T1>(BusinessContext businessContext, TileDataItems data, System.Func<T1> source, System.Func<T1, IList<AbstractContactEntity>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUITelecomContacts<T1, AbstractContactEntity, TelecomContactEntity> (businessContext, data, source, collectionResolver);
		}

		private static void InternalCreateUITelecomContacts<T1, T2, T3>(BusinessContext businessContext, TileDataItems data, System.Func<T1> source, System.Func<T1, IList<T2>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : TelecomContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("TelecomContact", businessContext)
				.DefineText			(x => x.GetSummary ())
				.DefineCompactText  (x => x.GetCompactSummary ());

			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "TelecomContact",
					IconUri		 = "Data.TelecomContact",
					Title		 = TextFormatter.FormatText ("Téléphones"),
					CompactTitle = TextFormatter.FormatText ("Téléphones"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
		}


		public static void CreateUIUriContacts<T1>(BusinessContext businessContext, TileDataItems data, System.Func<T1> source, System.Func<T1, IList<AbstractContactEntity>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIUriContacts<T1, AbstractContactEntity, UriContactEntity> (businessContext, data, source, collectionResolver);
		}

		private static void InternalCreateUIUriContacts<T1, T2, T3>(BusinessContext businessContext, TileDataItems data, System.Func<T1> source, System.Func<T1, IList<T2>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : UriContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("UriContact", businessContext, filter: x => x.UriType.Protocol == "mailto")
				.DefineText			(x => x.GetSummary ())
				.DefineCompactText	(x => x.GetCompactSummary ())
				.DefineSetupItem    (x => x.UriType = businessContext.GetLocalEntity (businessContext.Data.GetAllEntities<UriTypeEntity> ().Where (y => y.Protocol == "mailto").FirstOrDefault ()));

			data.Add (
				new TileDataItem
				{
					AutoGroup    = true,
					Name		 = "UriContact",
					IconUri		 = "Data.UriContact",
					Title		 = TextFormatter.FormatText ("Emails"),
					CompactTitle = TextFormatter.FormatText ("Emails"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
		}
	}
}
