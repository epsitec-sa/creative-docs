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
			this.DocumentTypeEnumSelected = DocumentTypeEnum.None;
			this.PrinterFunctionUsed = PrinterFunction.All;

			this.documentTypes = new List<DocumentType> ();
			this.documentOptionsNameSelected = new List<string> ();
			this.documentContainer = new DocumentContainer ();
			this.tableColumns = new Dictionary<TableColumnKeys, TableColumn> ();
		}


		public List<DocumentType> DocumentTypes
		{
			get
			{
				return this.documentTypes;
			}
		}

		public DocumentTypeEnum DocumentTypeEnumSelected
		{
			get;
			set;
		}

		public DocumentType DocumentTypeSelected
		{
			get
			{
				return this.DocumentTypes.Where (x => x.Type == this.DocumentTypeEnumSelected).FirstOrDefault ();
			}
		}

		public List<string> DocumentOptionsSelected
		{
			get
			{
				return this.documentOptionsNameSelected;
			}
		}

		public bool HasDocumentOption(string name)
		{
			return this.documentOptionsNameSelected.Contains (name);
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

		public PrinterFunction PrinterFunctionUsed
		{
			//	Détermine le type de l'imprimante utilisée pour imprimer le document.
			//	IsEmpty, PageCount, etc. dépendent de PrinterFunctionUsed.
			get;
			set;
		}

		public bool IsEmpty
		{
			get
			{
				return this.documentContainer.IsEmpty (this.PrinterFunctionUsed);
			}
		}

		public PageType GetPageType(int page)
		{
			return this.documentContainer.GetPageType (page);
		}

		public int PageCount
		{
			get
			{
				return this.documentContainer.PageCount (this.PrinterFunctionUsed);
			}
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
			if (this.HasDocumentOption ("Generic.Specimen"))
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
		private readonly List<string>						documentOptionsNameSelected;
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
