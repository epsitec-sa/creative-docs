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
using Epsitec.Cresus.Core.Print2.Bands;
using Epsitec.Cresus.Core.Print2.Containers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print2.EntityPrinters
{
	public abstract class AbstractPrinter
	{
		public AbstractPrinter(CoreData coreData, IEnumerable<AbstractEntity> entities, OptionsDictionary options, PrintingUnitsDictionary printingUnits)
		{
			this.coreData      = coreData;
			this.entities      = entities;
			this.options       = options;
			this.printingUnits = printingUnits;

			this.documentContainer = new DocumentContainer ();
			this.tableColumns      = new Dictionary<TableColumnKeys, TableColumn> ();
		}


		public virtual string JobName
		{
			get
			{
				return null;
			}
		}


		/// <summary>
		/// Taille minimale possible pour ce document.
		/// </summary>
		public virtual Size MinimalPageSize
		{
			get
			{
				return Size.Zero;
			}
		}

		/// <summary>
		/// Taille maximale possible pour ce document.
		/// </summary>
		public virtual Size MaximalPageSize
		{
			get
			{
				return Size.Zero;
			}
		}

		/// <summary>
		/// Taille préférée pour ce document.
		/// </summary>
		public virtual Size PreferredPageSize
		{
			get
			{
				return Size.Zero;
			}
		}

		/// <summary>
		/// Taille du document à générer. Cette taille est définie par SetPrinterUnit.
		/// </summary>
		public Size RequiredPageSize
		{
			get
			{
				System.Diagnostics.Debug.Assert (!this.requiredPageSize.IsEmpty);

				return this.requiredPageSize;
			}
		}

		protected virtual Margins PageMargins
		{
			get
			{
				return new Margins (10);  // 1 cm
			}
		}


		public bool IsEmpty(PageType printerFunctionUsed = PageType.All)
		{
			return this.documentContainer.IsEmpty (printerFunctionUsed);
		}

		public int[] GetPhysicalPages(PageType printerFunctionUsed = PageType.All)
		{
			return this.documentContainer.GetPhysicalPages (printerFunctionUsed);
		}

		public int PageCount(PageType printerFunctionUsed = PageType.All)
		{
			return this.documentContainer.PageCount (printerFunctionUsed);
		}

		public int GetDocumentRank(int page)
		{
			return this.documentContainer.GetDocumentRank (page);
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

		public double ContinuousVerticalMax
		{
			get
			{
				return this.documentContainer.CurrentVerticalPosition - this.PageMargins.Bottom;
			}
		}

		public bool HasDocumentOption(DocumentOption option)
		{
			return this.HasDocumentOption (option, "true");
		}

		public bool HasDocumentOption(DocumentOption option, string value)
		{
			return this.options.GetValue (option) == value;
		}

		public bool HasPrinterUnitDefined(PageType printerUnitFunction)
		{
			//	Indique si une unité d'impression est définie.
			return this.printingUnits.ContainsPageType (printerUnitFunction);
		}


		public PreviewMode PreviewMode
		{
			//	Permet de savoir si on effectue une impression réelle ou un aperçu avant impression.
			get;
			set;
		}


		/// <summary>
		/// Spécifie l'unité d'impression, afin de déterminer la taille des pages à produire.
		/// </summary>
		public virtual void SetPrintingUnit(PrintingUnit printingUnit)
		{
			Size size;

			if (printingUnit == null)
			{
				size = this.PreferredPageSize;
			}
			else
			{
				size = printingUnit.PhysicalPaperSize;
			}

			if (this.HasDocumentOption (DocumentOption.Orientation, "Landscape"))
			{
				size = new Size (size.Height, size.Width);
			}

			if (!Common.InsidePageSize (size, this.MinimalPageSize, this.MaximalPageSize))
			{
				size = this.PreferredPageSize;
			}

			this.requiredPageSize = size;
		}

		public virtual void BuildSections()
		{
			this.documentContainer.PageSize    = this.RequiredPageSize;
			this.documentContainer.PageMargins = this.PageMargins;
		}

		public virtual void PrintBackgroundCurrentPage(IPaintPort port)
		{
			if (this.HasDocumentOption (DocumentOption.Specimen))
			{
				this.PaintSpecimen (port);
			}
		}

		public virtual void PrintForegroundCurrentPage(IPaintPort port)
		{
		}

		private void PaintSpecimen(IPaintPort port)
		{
			//	Dessine un très gros "SPECIMEN" au travers de la page.
			var bounds = new Rectangle (Point.Zero, this.RequiredPageSize);
			double diagonal = Point.Distance (bounds.BottomLeft, bounds.TopRight);
			double angle = System.Math.Atan2 (bounds.Height, bounds.Width);
			bounds.Inflate (diagonal);

			var initial = port.Transform;
			port.Transform = port.Transform.MultiplyByPostfix (Transform.CreateRotationRadTransform (angle, bounds.Center));

			var layout = new TextLayout
			{
				Text = "SPECIMEN",
				DefaultColor = Color.FromBrightness (this.PreviewMode == PreviewMode.Print ? 0.80 : 0.95),  // plus foncé si impression réelle
				DefaultFont = Font.GetFont ("Arial", "Bold"),
				DefaultFontSize = diagonal / 6.5,
				Alignment = ContentAlignment.MiddleCenter,
				LayoutSize = bounds.Size,
			};

			layout.Paint (bounds.BottomLeft, port);

			port.Transform = initial;
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


		protected static FormattedText GetDefaultLocation()
		{
			//	Retourne la ville de l'entreprise, pour par exemple imprimer "Yverdon-les-Bains, le 30 septembre 2010".
			var m = CoreProgram.Application.BusinessSettings.Company.Person.Contacts.Where (x => x is MailContactEntity).First () as MailContactEntity;

			if (m == null)
			{
				return null;
			}
			else
			{
				return m.Address.Location.Name;
			}
		}


		public static AbstractPrinter CreateDocumentPrinter(CoreData coreData, IEnumerable<AbstractEntity> entities, OptionsDictionary options, PrintingUnitsDictionary printingUnits)
		{
			if (entities == null || entities.Count () == 0)
			{
				return null;
			}

			var first = entities.FirstOrDefault ();

			if (first is DocumentMetadataEntity)
			{
				return new DocumentMetadataPrinter (coreData, entities, options, printingUnits);
			}

			return null;
		}


		private static readonly Font						specimenFont = Font.GetFont ("Arial", "Bold");
		public static readonly double						continuousHeight = 100000;  // 100m devrait suffire

		protected readonly IEnumerable<AbstractEntity>		entities;
		protected readonly OptionsDictionary				options;
		protected readonly PrintingUnitsDictionary			printingUnits;
		protected readonly DocumentContainer				documentContainer;
		protected readonly CoreData							coreData;

		protected Dictionary<TableColumnKeys, TableColumn>	tableColumns;
		protected Size										requiredPageSize;
		private int											currentPage;
		private int											debugParam1;
		private int											debugParam2;
	}
}
