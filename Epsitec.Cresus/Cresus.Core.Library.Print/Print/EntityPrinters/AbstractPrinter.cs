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
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Print.Bands;
using Epsitec.Cresus.Core.Print.Containers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print.EntityPrinters
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


		public void SetOptionsDictionary(OptionsDictionary options)
		{
			//	Impose d'autres options.
			this.options = options;
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
		/// Taille du document à générer. Cette taille est définie par SetPrintingUnit.
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


		public bool IsEmpty(PageType printerFunctionUsed)
		{
			return this.documentContainer.IsEmpty (printerFunctionUsed);
		}

		public int[] GetPhysicalPages(PageType printerFunctionUsed)
		{
			return this.documentContainer.GetPhysicalPages (printerFunctionUsed);
		}

		public int GetDocumentRank(int page)
		{
			return this.documentContainer.GetDocumentRank (page);
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


		public bool HasPrintingUnitDefined(PageType printerUnitFunction)
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

			if (this.HasOption (DocumentOption.Orientation, "Landscape"))
			{
				size = new Size (size.Height, size.Width);
			}

			if (!Common.InsidePageSize (size, this.MinimalPageSize, this.MaximalPageSize))
			{
				size = this.PreferredPageSize;
			}

			this.requiredPageSize = size;
		}

		/// <summary>
		/// A utiliser à la place de SetPrintingUnit.
		/// </summary>
		public void SetContinuousPreviewMode()
		{
			this.PreviewMode = PreviewMode.ContinuousPreview;
			this.requiredPageSize = this.PreferredPageSize;
		}


		public virtual void BuildSections()
		{
			this.documentContainer.PageSize    = this.RequiredPageSize;
			this.documentContainer.PageMargins = this.PageMargins;
		}

		public virtual void PrintBackgroundCurrentPage(IPaintPort port)
		{
			if (this.HasOption (DocumentOption.Specimen))
			{
				this.PaintSpecimen (port);
			}
		}

		public virtual void PrintForegroundCurrentPage(IPaintPort port)
		{
		}

		private void PaintSpecimen(IPaintPort port)
		{
			//	Dessine un très gros "SPECIMEN" en travers de la page.
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


		protected static FormattedText GetDefaultLocation()
		{
			throw new System.NotImplementedException ();
#if false
			//	Retourne la ville de l'entreprise, pour imprimer par exemple "Yverdon-les-Bains, le 30 septembre 2010".
			var m = CoreProgram.Application.BusinessSettings.Company.Person.Contacts.Where (x => x is MailContactEntity).First () as MailContactEntity;

			if (m == null)
			{
				return null;
			}
			else
			{
				return m.Address.Location.Name;
			}
#endif
		}


		public static IEnumerable<AbstractPrinter> CreateDocumentPrinters(CoreData coreData, IEnumerable<AbstractEntity> entities, OptionsDictionary options, PrintingUnitsDictionary printingUnits, bool all = true)
		{
			//	Crée les *Printer adaptés à un type d'entité. Le premier est toujours le principal.
			//	Si all = false, on ne crée que le principal.
			//	Quel bonheur de ne pas utiliser la réflexion, c'est tellement plus simple !
			//	Il ne faut pas perdre de vue qu'il n'y a pas de lien direct entre un type d'entité et
			//	un *Printer. En particulier, il peut y avoir plusieurs *Printer pour une même entité.
			var list = new List<AbstractPrinter> ();

			if (entities != null && entities.Count () != 0)
			{
				var first = entities.FirstOrDefault ();

				throw new System.NotImplementedException ();
#if false
				if (first is DocumentMetadataEntity)
				{
					list.Add (new DocumentMetadataPrinter (coreData, entities, options, printingUnits));

					if (all)
					{
						var metadata = first as DocumentMetadataEntity;

						if (metadata.BusinessDocument != null && metadata.BusinessDocument.BillToMailContact != null)
						{
							list.Add (new DocumentMetadataMailContactPrinter (coreData, entities, options, printingUnits));
						}
					}
				}

				if (first is RelationEntity)
				{
					list.Add (new RelationPrinter (coreData, entities, options, printingUnits));
				}
#endif
			}

			return list;
		}


		protected bool HasOption(DocumentOption option)
		{
			//	Indique si une option de type booléen est choisie.
			return this.HasOption (option, "true");
		}

		protected bool HasOption(DocumentOption option, string value)
		{
			//	Indique si une option de type énumération est choisie.
			if (this.options == null)
			{
				return false;
			}
			else
			{
				return this.options.GetValue (option) == value;
			}
		}


		private static readonly Font						specimenFont = Font.GetFont ("Arial", "Bold");
		public static readonly double						continuousHeight = 100000;  // 100m devrait suffire

		protected readonly IEnumerable<AbstractEntity>		entities;
		private OptionsDictionary							options;
		private readonly PrintingUnitsDictionary			printingUnits;
		protected readonly DocumentContainer				documentContainer;
		protected readonly CoreData							coreData;

		protected Dictionary<TableColumnKeys, TableColumn>	tableColumns;
		protected Size										requiredPageSize;
		private int											currentPage;
	}
}
