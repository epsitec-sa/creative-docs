//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>EntityPrinterFactoryResolver</c> class is used to find implementations of the
	/// <see cref="IEntityPrinterFactory"/> interface and functionality related to the <see cref="IEntityPrinter"/>
	/// interface.
	/// </summary>
	public sealed class EntityPrinterFactoryResolver
	{
		public static IEnumerable<IEntityPrinterFactory> Resolve()
		{
			if (EntityPrinterFactoryResolver.factories == null)
			{
				EntityPrinterFactoryResolver.factories = InterfaceImplementationResolver<IEntityPrinterFactory>.CreateInstances ().ToList ();
			}

			return EntityPrinterFactoryResolver.factories;
		}


		public static IEnumerable<System.Type> GetPrintableEntityTypes()
		{
			return EntityPrinterFactoryResolver.Resolve ().SelectMany (x => x.GetSupportedEntityTypes ()).Distinct ();
		}

		public static IEnumerable<Druid> GetPrintableEntityIds()
		{
			return EntityPrinterFactoryResolver.GetPrintableEntityTypes ().Select (x => EntityInfo.GetTypeId (x));
		}

		
		public static IEnumerable<PageType> FindRequiredPageTypes(DocumentType documentType)
		{
			return EntityPrinterFactoryResolver.Resolve ().Select (x => x.GetRequiredPageTypes (documentType)).FirstOrDefault (x => x != null);
		}

		public static IEnumerable<DocumentOption> FindRequiredDocumentOptions(AbstractEntity entity)
		{
			return EntityPrinterFactoryResolver.FindRequiredDocumentOptions (EntityPrinterFactoryResolver.FindDocumentType (entity));
		}

		public static IEnumerable<DocumentOption> FindRequiredDocumentOptions(DocumentType documentType)
		{
			return EntityPrinterFactoryResolver.Resolve ().Select (x => x.GetRequiredDocumentOptions (documentType)).FirstOrDefault (x => x != null);
		}

		public static IEntityPrinterFactory FindPrinterFactory(AbstractEntity entity, PrintingOptionDictionary options)
		{
			return EntityPrinterFactoryResolver.Resolve ().FirstOrDefault (x => x.CanPrint (entity, options));
		}

		private static DocumentType FindDocumentType(AbstractEntity entity)
		{
			return EntityPrinterFactoryResolver.Resolve ().Select (x => x.GetDocumentType (entity)).Where (x => x != DocumentType.Unknown).FirstOrDefault ();
		}


		[System.ThreadStatic]
		private static List<IEntityPrinterFactory> factories;
	}
}
