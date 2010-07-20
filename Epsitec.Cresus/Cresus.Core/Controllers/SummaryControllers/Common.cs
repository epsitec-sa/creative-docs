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
				.DefineText        (x => UIBuilder.FormatText (x.Text))
				.DefineCompactText (x => UIBuilder.FormatText (x.Text));

			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "Comment",
					IconUri		 = "Data.Comment",
					Title		 = UIBuilder.FormatText ("Commentaires"),
					CompactTitle = UIBuilder.FormatText ("Commentaires"),
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
				.DefineTitle		(x => UIBuilder.FormatText ("Adresse", "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineText			(x => UIBuilder.FormatText (x.LegalPerson.Name, "\n", x.LegalPerson.Complement, "\n", x.Complement, "\n", x.Address.Street.StreetName, "\n", x.Address.Street.Complement, "\n", x.Address.PostBox.Number, "\n", x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode, x.Address.Location.Name))
				.DefineCompactText	(x => UIBuilder.FormatText (x.Address.Street.StreetName, "~,", x.Address.Location.PostalCode, x.Address.Location.Name));


			data.Add (
				new SummaryData
				{
					Name		 = "MailContact",
					IconUri		 = "Data.Mail",
					Title		 = UIBuilder.FormatText ("Adresses"),
					CompactTitle = UIBuilder.FormatText ("Adresses"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
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
				.DefineTitle		(x => UIBuilder.FormatText (x.TelecomType.Name))
				.DefineText			(x => UIBuilder.FormatText (x.Number, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText  (x => UIBuilder.FormatText (x.Number, "(", x.TelecomType.Name, ")"));

			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "TelecomContact",
					IconUri		 = "Data.Telecom",
					Title		 = UIBuilder.FormatText ("Téléphones"),
					CompactTitle = UIBuilder.FormatText ("Téléphones"),
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
				.DefineText			(x => UIBuilder.FormatText (x.Uri, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText	(x => UIBuilder.FormatText (x.Uri))
				.DefineSetupItem	(x => x.UriScheme = CoreProgram.Application.Data.GetUriScheme ("mailto"));

			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "UriContact",
					IconUri		 = "Data.Uri",
					Title		 = UIBuilder.FormatText ("Emails"),
					CompactTitle = UIBuilder.FormatText ("Emails"),
					Text		 = CollectionTemplate.DefaultEmptyText
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
		}
	}
}
