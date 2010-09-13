//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Printers
{
	public abstract class AbstractEntityPrinter
	{
		public AbstractEntityPrinter()
			: base ()
		{
			this.documentPrinters  = new List<AbstractDocumentPrinter> ();
			this.documentTypes     = new List<DocumentTypeDefinition> ();

			this.EntityPrintingSettings = new EntityPrintingSettings ();
		}


		public EntityPrintingSettings EntityPrintingSettings
		{
			get;
			set;
		}


		public List<AbstractDocumentPrinter> DocumentPrinters
		{
			get
			{
				return this.documentPrinters;
			}
		}

		public List<DocumentTypeDefinition> DocumentTypes
		{
			get
			{
				return this.documentTypes;
			}
		}

		public DocumentTypeDefinition DocumentTypeSelected
		{
			get
			{
				return this.DocumentTypes.Where (x => x.Type == this.EntityPrintingSettings.DocumentTypeSelected).FirstOrDefault ();
			}
		}


		public void DefaultPrepare(DocumentType type)
		{
			this.EntityPrintingSettings.DocumentTypeSelected = type;

			var documentType = this.DocumentTypeSelected;
			if (documentType != null)
			{
				foreach (var option in documentType.DocumentOptions)
				{
					if (option.DefautState)
					{
						this.EntityPrintingSettings.DocumentOptionsSelected.Add (option.Option);
					}
				}
			}
		}


		public bool IsPreview
		{
			//	Permet de savoir si on effectue une impression réelle ou un aperçu avant impression.
			get;
			set;
		}


		public int PageCount(PrinterUnitFunction printerFunctionUsed = PrinterUnitFunction.ForAllPages)
		{
			int count = 0;

			foreach (var documentPrinter in this.documentPrinters)
			{
				count += documentPrinter.PageCount (printerFunctionUsed);
			}

			return count;
		}

		public int GetPageRelativ(int page, PrinterUnitFunction printerFunctionUsed = PrinterUnitFunction.ForAllPages)
		{
			int firstDocumentPage = 0;

			foreach (var documentPrinter in this.documentPrinters)
			{
				int pageCount = documentPrinter.PageCount (printerFunctionUsed);

				if (page < firstDocumentPage+pageCount)
				{
					return page-firstDocumentPage;
				}

				firstDocumentPage += pageCount;
			}

			return -1;
		}

		public PageType GetPageType(int page, PrinterUnitFunction printerFunctionUsed = PrinterUnitFunction.ForAllPages)
		{
			int firstDocumentPage = 0;

			foreach (var documentPrinter in this.documentPrinters)
			{
				int pageCount = documentPrinter.PageCount (printerFunctionUsed);

				if (page < firstDocumentPage+pageCount)
				{
					return documentPrinter.GetPageType (page-firstDocumentPage);
				}

				firstDocumentPage += pageCount;
			}

			return PageType.Single;
		}

		public AbstractDocumentPrinter GetDocumentPrinter(int page, PrinterUnitFunction printerFunctionUsed = PrinterUnitFunction.ForAllPages)
		{
			int firstDocumentPage = 0;

			foreach (var documentPrinter in this.documentPrinters)
			{
				firstDocumentPage += documentPrinter.PageCount (printerFunctionUsed);

				if (page < firstDocumentPage)
				{
					return documentPrinter;
				}
			}

			return null;
		}

		public virtual Size MaximalPageSize
		{
			get
			{
				double width  = 0;
				double height = 0;

				foreach (var documentPrinter in this.documentPrinters)
				{
					width  = System.Math.Max (width,  documentPrinter.PageSize.Width);
					height = System.Math.Max (height, documentPrinter.PageSize.Height);
				}

				return new Size (width, height);
			}
		}

		public virtual void BuildSections()
		{
			foreach (var documentPrinter in this.documentPrinters)
			{
				documentPrinter.BuildSections ();
			}
		}


		public int DebugParam1
		{
			get
			{
				return this.documentPrinters[0].DebugParam1;
			}
			set
			{
				if (this.documentPrinters[0].DebugParam1 != value)
				{
					this.documentPrinters[0].DebugParam1 = value;
				}
			}
		}

		public int DebugParam2
		{
			get
			{
				return this.documentPrinters[0].DebugParam2;
			}
			set
			{
				if (this.documentPrinters[0].DebugParam2 != value)
				{
					this.documentPrinters[0].DebugParam2 = value;
				}
			}
		}



		public static AbstractEntityPrinter CreateEntityPrinter(AbstractEntity entity)
		{
			var type = AbstractEntityPrinter.FindEntityPrinterType (entity);

			if (type == null)
			{
				return null;
			}

			return System.Activator.CreateInstance (type, new object[] { entity }) as AbstractEntityPrinter;
		}

		internal static System.Type FindEntityPrinterType(AbstractEntity entity)
		{
			if (entity == null)
			{
				return null;
			}
			else
			{
				return AbstractEntityPrinter.FindEntityPrinterType (entity.GetType ());
			}
		}

		private static System.Type FindEntityPrinterType(System.Type entityType)
		{
			var baseTypeName = "AbstractEntityPrinter`1";

			//	Find all concrete classes which use either the generic AbstractEntityPrinter base classes,
			//	which match the entity type (usually, there should be exactly one such type).

			var types = from type in typeof (AbstractEntityPrinter).Assembly.GetTypes ()
						where type.IsClass && !type.IsAbstract
						let baseType = type.BaseType
						where baseType.IsGenericType && baseType.Name.StartsWith (baseTypeName) && baseType.GetGenericArguments ()[0] == entityType
						select type;

			return types.FirstOrDefault ();
		}


		private static readonly Font specimenFont = Font.GetFont ("Arial", "Bold");

		protected readonly List<AbstractDocumentPrinter>	documentPrinters;
		private readonly List<DocumentTypeDefinition>		documentTypes;
	}


	public class AbstractEntityPrinter<T> : AbstractEntityPrinter
		where T : AbstractEntity
	{
		public AbstractEntityPrinter(T entity)
		{
			this.entity = entity;
		}

		protected readonly T entity;
	}
}
