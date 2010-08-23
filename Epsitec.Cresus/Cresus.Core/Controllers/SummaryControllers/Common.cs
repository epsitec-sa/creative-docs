//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	internal static class Common
	{
		public static void CreateUIComments<T1>(DataContext dataContext, SummaryDataItems data, System.Func<T1> source, Expression<System.Func<T1, IList<CommentEntity>>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIComments<T1, CommentEntity, CommentEntity> (dataContext, data, source, collectionResolver);
		}

		private static void InternalCreateUIComments<T1, T2, T3>(DataContext dataContext, SummaryDataItems data, System.Func<T1> source, Expression<System.Func<T1, IList<T2>>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : CommentEntity, new ()
			where T3 : CommentEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("Comment", data.Controller, dataContext)
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


		public static void CreateUIMailContacts<T1>(DataContext dataContext, SummaryDataItems data, System.Func<T1> source, Expression<System.Func<T1, IList<AbstractContactEntity>>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIMailContacts<T1, AbstractContactEntity, MailContactEntity> (dataContext, data, source, collectionResolver);
		}

		private static void InternalCreateUIMailContacts<T1, T2, T3>(DataContext dataContext, SummaryDataItems data, System.Func<T1> source, Expression<System.Func<T1, IList<T2>>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : MailContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("MailContact", data.Controller, dataContext)
				.DefineTitle		(x => TextFormatter.FormatText ("Adresse", "(", FormattedText.Join (", ", x.Roles.Select (role => role.Name).ToArray ()), ")"))
				.DefineText			(x => Common.GetMailContactSummary (x))
				.DefineCompactText	(x => Common.GetCompactMailContactSummary (x));


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

		private static FormattedText GetMailContactSummary(MailContactEntity x)
		{
			return TextFormatter.FormatText (x.LegalPerson.Name, "\n",
											 x.LegalPerson.Complement, "\n",
											 x.Complement, "\n",
											 x.Address.Street.StreetName, "\n",
											 x.Address.Street.Complement, "\n",
											 x.Address.PostBox.Number, "\n",
											 TextFormatter.Mark,
											 x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode, TextFormatter.ClearGroupIfEmpty,
											 TextFormatter.Mark,
											 x.Address.Location.Name);
		}

		private static FormattedText GetCompactMailContactSummary(MailContactEntity x)
		{
			return TextFormatter.FormatText (x.Address.Street.StreetName, "~,", x.Address.Location.PostalCode, x.Address.Location.Name);
		}


		public static void CreateUITelecomContacts<T1>(DataContext dataContext, SummaryDataItems data, System.Func<T1> source, Expression<System.Func<T1, IList<AbstractContactEntity>>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUITelecomContacts<T1, AbstractContactEntity, TelecomContactEntity> (dataContext, data, source, collectionResolver);
		}

		private static void InternalCreateUITelecomContacts<T1, T2, T3>(DataContext dataContext, SummaryDataItems data, System.Func<T1> source, Expression<System.Func<T1, IList<T2>>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : TelecomContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("TelecomContact", data.Controller, dataContext)
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


		public static void CreateUIUriContacts<T1>(DataContext dataContext, SummaryDataItems data, System.Func<T1> source, Expression<System.Func<T1, IList<AbstractContactEntity>>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIUriContacts<T1, AbstractContactEntity, UriContactEntity> (dataContext, data, source, collectionResolver);
		}

		private static void InternalCreateUIUriContacts<T1, T2, T3>(DataContext dataContext, SummaryDataItems data, System.Func<T1> source, Expression<System.Func<T1, IList<T2>>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : UriContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("UriContact", data.Controller, dataContext, filter: x => x.UriScheme.Code == "mailto")
				.DefineText			(x => TextFormatter.FormatText (x.Uri, "(", FormattedText.Join (", ", x.Roles.Select (role => role.Name).ToArray ()), ")"))
				.DefineCompactText	(x => TextFormatter.FormatText (x.Uri))
				.DefineSetupItem	(x => x.UriScheme = CoreProgram.Application.Data.GetUriScheme ("mailto"));

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
