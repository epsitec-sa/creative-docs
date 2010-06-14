//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.SummaryControllers
{
	internal static class Common
	{
		public static void CreateUIMailContacts<T1>(SummaryDataItems data, System.Func<T1> source, System.Func<T1, System.Collections.Generic.IList<Entities.AbstractContactEntity>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIMailContacts<T1, Entities.AbstractContactEntity, Entities.MailContactEntity> (data, source, collectionResolver);
		}

		private static void InternalCreateUIMailContacts<T1, T2, T3>(SummaryDataItems data, System.Func<T1> source, System.Func<T1, System.Collections.Generic.IList<T2>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : Entities.MailContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("MailContact")
				.DefineTitle		(x => UIBuilder.FormatText ("Adresse", "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineText			(x => UIBuilder.FormatText (x.LegalPerson.Name, "\n", x.LegalPerson.Complement, "\n", x.Complement, "\n", x.Address.Street.StreetName, "\n", x.Address.Street.Complement, "\n", x.Address.PostBox.Number, "\n", x.Address.Location.Country.Code, "~-", x.Address.Location.PostalCode, x.Address.Location.Name))
				.DefineCompactText	(x => UIBuilder.FormatText (x.Address.Street.StreetName, "~,", x.Address.Location.PostalCode, x.Address.Location.Name));


			data.Add (
				new SummaryData
				{
					Name		 = "MailContact",
					IconUri		 = "Data.Mail",
					Title		 = UIBuilder.FormatText ("Adresse"),
					CompactTitle = UIBuilder.FormatText ("Adresse"),
					Text		 = UIBuilder.FormatText ("<i>vide</i>")
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
		}


		public static void CreateUITelecomContacts<T1>(SummaryDataItems data, System.Func<T1> source, System.Func<T1, System.Collections.Generic.IList<Entities.AbstractContactEntity>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUITelecomContacts<T1, Entities.AbstractContactEntity, Entities.TelecomContactEntity> (data, source, collectionResolver);
		}

		private static void InternalCreateUITelecomContacts<T1, T2, T3>(SummaryDataItems data, System.Func<T1> source, System.Func<T1, System.Collections.Generic.IList<T2>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : Entities.TelecomContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("TelecomContact")
				.DefineTitle		(x => UIBuilder.FormatText (x.TelecomType.Name))
				.DefineText			(x => UIBuilder.FormatText (x.Number, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText  (x => UIBuilder.FormatText (x.Number, "(", x.TelecomType.Name, ")"));

			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "TelecomContact",
					IconUri		 = "Data.Telecom",
					Title		 = UIBuilder.FormatText ("Téléphone"),
					CompactTitle = UIBuilder.FormatText ("Téléphone"),
					Text		 = UIBuilder.FormatText ("<i>vide</i>")
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
		}


		public static void CreateUIUriContacts<T1>(SummaryDataItems data, System.Func<T1> source, System.Func<T1, System.Collections.Generic.IList<Entities.AbstractContactEntity>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIUriContacts<T1, Entities.AbstractContactEntity, Entities.UriContactEntity> (data, source, collectionResolver);
		}

		private static void InternalCreateUIUriContacts<T1, T2, T3>(SummaryDataItems data, System.Func<T1> source, System.Func<T1, System.Collections.Generic.IList<T2>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : Entities.UriContactEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("UriContact", filter: x => x.UriScheme.Code == "mailto")
				.DefineText			(x => UIBuilder.FormatText (x.Uri, "(", string.Join (", ", x.Roles.Select (role => role.Name)), ")"))
				.DefineCompactText	(x => UIBuilder.FormatText (x.Uri))
				.DefineSetupItem	(x => x.UriScheme = CoreProgram.Application.Data.GetUriScheme ("mailto"));

			data.Add (
				new SummaryData
				{
					AutoGroup    = true,
					Name		 = "UriContact",
					IconUri		 = "Data.Uri",
					Title		 = UIBuilder.FormatText ("E-Mail"),
					CompactTitle = UIBuilder.FormatText ("E-Mail"),
					Text		 = UIBuilder.FormatText ("<i>vide</i>")
				});

			data.Add (CollectionAccessor.Create (source, collectionResolver, template));
		}
	}
}
