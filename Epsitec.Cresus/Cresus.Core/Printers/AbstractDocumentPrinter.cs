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
	public abstract class AbstractDocumentPrinter
	{
		public AbstractDocumentPrinter(CoreData coreData, AbstractEntityPrinter entityPrinter, AbstractEntity entity)
		{
			this.coreData      = coreData;
			this.entityPrinter = entityPrinter;
			this.entity        = entity;

			this.documentContainer = new DocumentContainer ();
			this.tableColumns      = new Dictionary<TableColumnKeys, TableColumn> ();
		}


		protected Business.DocumentType SelectedDocumentType
		{
			get
			{
				return this.entityPrinter.EntityPrintingSettings.DocumentTypeSelected;
			}
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


		public bool IsEmpty(PrinterUnitFunction printerFunctionUsed = PrinterUnitFunction.ForAllPages)
		{
			return this.documentContainer.IsEmpty (printerFunctionUsed);
		}

		public int[] GetPhysicalPages(PrinterUnitFunction printerFunctionUsed = PrinterUnitFunction.ForAllPages)
		{
			return this.documentContainer.GetPhysicalPages (printerFunctionUsed);
		}

		public int PageCount(PrinterUnitFunction printerFunctionUsed = PrinterUnitFunction.ForAllPages)
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
			//	Indique si une option est utilisée, en tenant compte des options forcées.
			if (this.forcingOptionsToClear != null && this.forcingOptionsToClear.Contains (option))
			{
				return false;
			}

			if (this.forcingOptionsToSet != null && this.forcingOptionsToSet.Contains (option))
			{
				return true;
			}

			return this.entityPrinter.EntityPrintingSettings.HasDocumentOption (option);
		}

		public bool HasPrinterUnitDefined(PrinterUnitFunction printerUnitFunction)
		{
			//	Indique si une unité d'impression est définie.
			DocumentTypeDefinition documentTypeDefinition = this.entityPrinter.SelectedDocumentTypeDefinition;

			if (documentTypeDefinition != null)
			{
				foreach (var documentPrinterFunction in documentTypeDefinition.DocumentPrinterFunctions)
				{
					if (documentPrinterFunction.PrinterFunction == printerUnitFunction)
					{
						return !string.IsNullOrEmpty (documentPrinterFunction.LogicalPrinterName);
					}
				}
			}

			return false;
		}


		public PreviewMode PreviewMode
		{
			//	Permet de savoir si on effectue une impression réelle ou un aperçu avant impression.
			get
			{
				return this.entityPrinter.PreviewMode;
			}
			set
			{
				this.entityPrinter.PreviewMode = value;
			}
		}


		/// <summary>
		/// Spécifie l'unité d'impression, afin de déterminer la taille des pages à produire.
		/// </summary>
		public virtual void SetPrinterUnit(PrinterUnit printerUnit)
		{
			Size size;

			if (printerUnit == null)
			{
				size = this.PreferredPageSize;
			}
			else
			{
				size = printerUnit.PhysicalPaperSize;
			}

			if (this.HasDocumentOption (DocumentOption.OrientationHorizontal))
			{
				size = new Size (size.Height, size.Width);
			}

			if (!Common.InsidePageSize (size, this.MinimalPageSize, this.MaximalPageSize))
			{
				size = this.PreferredPageSize;
			}

			this.requiredPageSize = size;
		}

		public virtual void BuildSections(List<DocumentOption> forcingOptionsToClear = null, List<DocumentOption> forcingOptionsToSet = null)
		{
			this.forcingOptionsToClear = forcingOptionsToClear;
			this.forcingOptionsToSet   = forcingOptionsToSet;

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


		private static readonly Font						specimenFont = Font.GetFont ("Arial", "Bold");
		public static readonly double						continuousHeight = 100000;  // 100m devrait suffire

		protected readonly AbstractEntityPrinter			entityPrinter;
		protected readonly AbstractEntity					entity;
		protected readonly DocumentContainer				documentContainer;
		protected readonly CoreData							coreData;

		protected Dictionary<TableColumnKeys, TableColumn>	tableColumns;
		protected Size										requiredPageSize;
		private List<DocumentOption>						forcingOptionsToClear;
		private List<DocumentOption>						forcingOptionsToSet;
		private int											currentPage;
		private int											debugParam1;
		private int											debugParam2;
	}
}
