//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers.DataAccessors;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers.EditionControllers
{
	internal static class Common
	{
		public static void CreateUIComments<T1>(SummaryDataItems data, System.Func<T1> source)
			where T1 : CommentEntity, new ()
		{
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

			var template = new CollectionTemplate<CommentEntity> ("Comment", data.Controller)
				.DefineText (x => UIBuilder.FormatText (x.Text))
				.DefineCompactText (x => UIBuilder.FormatText (x.Text));

			//?data.Add (CollectionAccessor.Create (source, x => x.Comments, template));
		}


#if false
		public static void CreateUIComments<T1>(SummaryDataItems data, System.Func<T1> source, Expression<System.Func<T1, IList<CommentEntity>>> collectionResolver)
			where T1 : AbstractEntity, new ()
		{
			Common.InternalCreateUIComments<T1, AbstractEntity, CommentEntity> (data, source, collectionResolver);
		}

		private static void InternalCreateUIComments<T1, T2, T3>(SummaryDataItems data, System.Func<T1> source, Expression<System.Func<T1, IList<T2>>> collectionResolver)
			where T1 : AbstractEntity, new ()
			where T2 : AbstractEntity, new ()
			where T3 : CommentEntity, T2, new ()
		{
			var template = new CollectionTemplate<T3> ("Comment", data.Controller)
				.DefineText (x => UIBuilder.FormatText (x.Text))
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
#endif
	}
}
