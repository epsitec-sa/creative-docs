//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	internal static class Common
	{
		public static void CreateUIComments<T1>(CoreData coreData, SummaryDataItems data, System.Func<T1> source, System.Func<T1, IList<CommentEntity>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIComments<T1, CommentEntity, CommentEntity> (coreData, data, source, collectionResolver);
		}

		private static void InternalCreateUIComments<T1, T2, T3>(CoreData coreData, SummaryDataItems data, System.Func<T1> source, System.Func<T1, IList<T2>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : CommentEntity, new ()
			where T3 : CommentEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("Comment", data.Controller, coreData.DataContext)
				.DefineText        (x => TextFormatter.FormatText (Misc.FirstLine (x.Text)))
				.DefineCompactText (x => TextFormatter.FormatText (Misc.FirstLine (x.Text)));

			data.Add (
				new SummaryData
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


		public static void CreateUIMailContacts<T1>(CoreData coreData, SummaryDataItems data, System.Func<T1> source, System.Func<T1, IList<AbstractContactEntity>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIMailContacts<T1, AbstractContactEntity, MailContactEntity> (coreData, data, source, collectionResolver);
		}

		private static void InternalCreateUIMailContacts<T1, T2, T3>(CoreData coreData, SummaryDataItems data, System.Func<T1> source, System.Func<T1, IList<T2>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : MailContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("MailContact", data.Controller, coreData.DataContext)
				.DefineTitle		(x => TextFormatter.FormatText ("Adresse", "(", FormattedText.Join (", ", x.Roles.Select (role => role.Name).ToArray ()), ")"))
				.DefineText			(x => x.GetSummary ())
				.DefineCompactText	(x => x.GetCompactSummary ());


			data.Add (
				new SummaryData
				{
					Name		 = "MailContact",
					IconUri		 = "Data.Mail",
					Title		 = TextFormatter.FormatText ("Adresses"),
					CompactTitle = TextFormatter.FormatText ("Adresses"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
		}


		public static void CreateUITelecomContacts<T1>(CoreData coreData, SummaryDataItems data, System.Func<T1> source, System.Func<T1, IList<AbstractContactEntity>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUITelecomContacts<T1, AbstractContactEntity, TelecomContactEntity> (coreData, data, source, collectionResolver);
		}

		private static void InternalCreateUITelecomContacts<T1, T2, T3>(CoreData coreData, SummaryDataItems data, System.Func<T1> source, System.Func<T1, IList<T2>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : TelecomContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("TelecomContact", data.Controller, coreData.DataContext)
				.DefineTitle		(x => TextFormatter.FormatText (x.TelecomType.Name))
				.DefineText			(x => TextFormatter.FormatText (x.Number, "(", FormattedText.Join (", ", x.Roles.Select (role => role.Name).ToArray ()), ")"))
				.DefineCompactText  (x => TextFormatter.FormatText (x.Number, "(", x.TelecomType.Name, ")"));

			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "TelecomContact",
					IconUri		 = "Data.Telecom",
					Title		 = TextFormatter.FormatText ("Téléphones"),
					CompactTitle = TextFormatter.FormatText ("Téléphones"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
		}


		public static void CreateUIUriContacts<T1>(CoreData coreData, SummaryDataItems data, System.Func<T1> source, System.Func<T1, IList<AbstractContactEntity>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIUriContacts<T1, AbstractContactEntity, UriContactEntity> (coreData, data, source, collectionResolver);
		}

		private static void InternalCreateUIUriContacts<T1, T2, T3>(CoreData coreData, SummaryDataItems data, System.Func<T1> source, System.Func<T1, IList<T2>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : UriContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("UriContact", data.Controller, coreData.DataContext, filter: x => x.UriScheme.Code == "mailto")
				.DefineText			(x => TextFormatter.FormatText (x.Uri, "(", FormattedText.Join (", ", x.Roles.Select (role => role.Name).ToArray ()), ")"))
				.DefineCompactText	(x => TextFormatter.FormatText (x.Uri))
				.DefineSetupItem    (x => x.UriScheme = coreData.GetAllEntities<UriSchemeEntity> ().Where (y => y.Code == "mailto").FirstOrDefault ());

			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "UriContact",
					IconUri		 = "Data.Uri",
					Title		 = TextFormatter.FormatText ("Emails"),
					CompactTitle = TextFormatter.FormatText ("Emails"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
		}
	}
}
