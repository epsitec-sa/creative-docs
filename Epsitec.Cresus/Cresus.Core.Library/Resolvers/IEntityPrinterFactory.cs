//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Documents;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Resolvers
{
	/// <summary>
	/// The <c>IEntityPrinterFactory</c> interface is used to select and create the proper
	/// <see cref="AbstractPrinter"/> for a given entity.
	/// </summary>
	public interface IEntityPrinterFactory
	{
		/// <summary>
		/// Gets the collection of all supported entity types.
		/// </summary>
		/// <returns>The collection of all supported entity types.</returns>
		IEnumerable<System.Type> GetSupportedEntityTypes();

		/// <summary>
		/// Determines whether the <see cref="AbstractPrinter"/> created by this factory
		/// can print the specified entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <param name="options">The printing options.</param>
		/// <returns>
		///   <c>true</c> if the <see cref="AbstractPrinter"/> created by this factory
		///   can print the specified entity; otherwise, <c>false</c>.
		/// </returns>
		bool CanPrint(AbstractEntity entity, PrintingOptionDictionary options);

		/// <summary>
		/// Creates the <see cref="AbstractPrinter"/> instance needed to print the specified
		/// entity.
		/// </summary>
		/// <param name="businessContext">The business context.</param>
		/// <param name="entity">The entity.</param>
		/// <param name="options">The printing options.</param>
		/// <param name="printingUnits">The <see cref="PageType"/> to printing unit mapping.</param>
		/// <returns>The <see cref="AbstractPrinter"/> instance.</returns>
		IEntityPrinter CreatePrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits);

		/// <summary>
		/// Gets the type of the document, as a <see cref="DocumentType"/> enumeration value,
		/// if the entity is recognized by this factory.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns>The <see cref="DocumentType"/>; otherwise, <c>Unknown</c>.</returns>
		DocumentType GetDocumentType(AbstractEntity entity);

		/// <summary>
		/// Gets the document options required by the specified document type, if it is
		/// supported by this factory.
		/// </summary>
		/// <param name="documentType">The document type.</param>
		/// <returns>The document options; othewise, <c>null</c>.</returns>
		IEnumerable<DocumentOption> GetRequiredDocumentOptions(DocumentType documentType);

		/// <summary>
		/// Gets the page types required by the specified document type, if it is
		/// supported by this factory.
		/// </summary>
		/// <param name="documentType">The document type.</param>
		/// <returns>The page types; otherwise, <c>null</c>.</returns>
		IEnumerable<PageType> GetRequiredPageTypes(DocumentType documentType);
	}
}
