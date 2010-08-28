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
			this.documentTypes = new List<DocumentType> ();
			this.documentContainer = new DocumentContainer ();
			this.tableColumns = new Dictionary<TableColumnKeys, TableColumn> ();

			this.EntityPrintingSettings = new EntityPrintingSettings ();
		}


		public EntityPrintingSettings EntityPrintingSettings
		{
			get;
			set;
		}

		public List<DocumentType> DocumentTypes
		{
			get
			{
				return this.documentTypes;
			}
		}

		public DocumentType DocumentTypeSelected
		{
			get
			{
				return this.DocumentTypes.Where (x => x.Type == this.EntityPrintingSettings.DocumentTypeEnumSelected).FirstOrDefault ();
			}
		}


		public virtual string JobName
		{
			get
			{
				return null;
			}
		}

		public virtual Size PageSize
		{
			get
			{
				return Size.Zero;
			}
		}

		public virtual Margins PageMargins
		{
			get
			{
				return new Margins (10);
			}
		}

		public bool IsEmpty(PrinterFunction printerFunctionUsed = PrinterFunction.ForAllPages)
		{
			return this.documentContainer.IsEmpty (printerFunctionUsed);
		}

		public int[] GetPhysicalPages(PrinterFunction printerFunctionUsed = PrinterFunction.ForAllPages)
		{
			return this.documentContainer.GetPhysicalPages (printerFunctionUsed);
		}

		public int PageCount(PrinterFunction printerFunctionUsed = PrinterFunction.ForAllPages)
		{
			return this.documentContainer.PageCount (printerFunctionUsed);
		}

		public PageType GetPageType(int page)
		{
			return this.documentContainer.GetPageType (page);
		}

		public int CurrentPage
		{
			get
			{
				return this.currentPage;
			}
			set
			{
				if (this.currentPage != value)
				{
					this.currentPage = value;
				}
			}
		}

		public int DebugParam1
		{
			get
			{
				return this.debugParam1;
			}
			set
			{
				if (this.debugParam1 != value)
				{
					this.debugParam1 = value;
				}
			}
		}

		public int DebugParam2
		{
			get
			{
				return this.debugParam2;
			}
			set
			{
				if (this.debugParam2 != value)
				{
					this.debugParam2 = value;
				}
			}
		}

		public bool IsPreview
		{
			//	Permet de savoir si on effectue une impression réelle ou un aperçu avant impression.
			get;
			set;
		}

		public void Clear()
		{
			this.documentContainer.Clear ();
		}

		public virtual void BuildSections()
		{
			this.documentContainer.PageSize    = this.PageSize;
			this.documentContainer.PageMargins = this.PageMargins;
		}

		public virtual void PrintCurrentPage(IPaintPort port)
		{
			if (this.EntityPrintingSettings.HasDocumentOption ("Generic.Specimen"))
			{
				this.PaintSpecimen(port);
			}
		}

		private void PaintSpecimen(IPaintPort port)
		{
			//	Dessine un très gros "SPECIMEN" au travers de la page.
			var font = Font.GetFont ("Arial", "Bold");

			var initial = port.Transform;

			if (this.PageSize.Height > this.PageSize.Width)  // portrait ?
			{
				port.Transform = port.Transform.MultiplyByPostfix (Transform.CreateRotationDegTransform (60));

				port.Color = Color.FromBrightness (0.95);
				port.PaintText (34, -36, "SPECIMEN", AbstractEntityPrinter.specimenFont, 56);
			}
			else  // paysage ?
			{
				port.Transform = port.Transform.MultiplyByPostfix (Transform.CreateRotationDegTransform (30));

				port.Color = Color.FromBrightness (0.95);
				port.PaintText (30, -4, "SPECIMEN", AbstractEntityPrinter.specimenFont, 56);
			}

			port.Transform = initial;
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

		private readonly List<DocumentType>					documentTypes;
		protected readonly DocumentContainer				documentContainer;
		protected Dictionary<TableColumnKeys, TableColumn>	tableColumns;
		private int											currentPage;
		private int											debugParam1;
		private int											debugParam2;
	}


	public class AbstractEntityPrinter<T> : AbstractEntityPrinter
		where T : AbstractEntity
	{
		public AbstractEntityPrinter(T entity)
		{
			this.entity = entity;
		}

		protected T entity;
	}
}
