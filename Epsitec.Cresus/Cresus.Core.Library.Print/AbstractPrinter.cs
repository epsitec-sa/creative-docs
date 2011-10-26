//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Documents.Verbose;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Print.Bands;
using Epsitec.Cresus.Core.Print.Containers;
using Epsitec.Cresus.Core.Resolvers;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print
{
	public abstract class AbstractPrinter : IEntityPrinter
	{
		protected AbstractPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
		{
			this.businessContext   = businessContext;
			this.coreData          = this.businessContext.Data;
			this.entity            = entity;
			this.options           = options;
			this.printingUnits     = printingUnits;

			this.documentContainer = new DocumentContainer ();
			this.tableColumns      = new Dictionary<TableColumnKeys, TableColumn> ();
		}


		public virtual string					JobName
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Taille préférée pour ce document.
		/// </summary>
		public virtual Size						PreferredPageSize
		{
			get
			{
				return Size.Zero;
			}
		}

		/// <summary>
		/// Taille du document à générer. Cette taille est définie par SetPrintingUnit.
		/// </summary>
		public Size								RequiredPageSize
		{
			get
			{
				System.Diagnostics.Debug.Assert (!this.requiredPageSize.IsEmpty);

				return this.requiredPageSize;
			}
		}

		public int								CurrentPage
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

		public double							ContinuousVerticalMax
		{
			get
			{
				return this.documentContainer.CurrentVerticalPosition - this.GetPageMargins ().Bottom;
			}
		}

		public PreviewMode						PreviewMode
		{
			//	Permet de savoir si on effectue une impression réelle ou un aperçu avant impression.
			get
			{
				return this.previewMode;
			}
		}

		
		protected virtual Margins GetPageMargins()
		{
			return new Margins (10);  // 1 cm
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


		public bool HasPrintingUnitDefined(PageType printerUnitFunction)
		{
			//	Indique si une unité d'impression est définie.
			return this.printingUnits.ContainsPageType (printerUnitFunction);
		}



		/// <summary>
		/// Spécifie l'unité d'impression, afin de déterminer la taille des pages à produire.
		/// Cet appel est nécessaire avant d'appeler BuildSections !
		/// </summary>
		public void SetPrintingUnit(PrintingUnit printingUnit, PrintingOptionDictionary options, PreviewMode mode)
		{
			System.Diagnostics.Debug.Assert (options != null);
			this.currentPrintingUnit = printingUnit;
			this.options = options;
			this.previewMode = mode;

			Size size;

			if (this.previewMode == Print.PreviewMode.ContinuousPreview)
			{
				System.Diagnostics.Debug.Assert (printingUnit == null);
				size = this.PreferredPageSize;
			}
			else
			{
				System.Diagnostics.Debug.Assert (printingUnit != null);
				size = printingUnit.PhysicalPaperSize;

				if (this.HasOption (DocumentOption.Orientation, "Portrait"))
				{
					double w = System.Math.Min (size.Width, size.Height);
					double h = System.Math.Max (size.Width, size.Height);
					size = new Size (w, h);
				}

				if (this.HasOption (DocumentOption.Orientation, "Landscape"))
				{
					double w = System.Math.Max (size.Width, size.Height);
					double h = System.Math.Min (size.Width, size.Height);
					size = new Size (w, h);
				}
			}

			this.requiredPageSize = size;
		}

		public virtual FormattedText BuildSections()
		{
			this.documentContainer.PageSize    = this.RequiredPageSize;
			this.documentContainer.PageMargins = this.GetPageMargins ();

			return null;  // ok
		}

		public virtual void PrintBackgroundCurrentPage(IPaintPort port)
		{
			var documentMetadata = this.entity as DocumentMetadataEntity;

			if (documentMetadata != null && documentMetadata.DocumentState == DocumentState.Draft)
			{
				this.PaintDraft (port);
			}
			else if (this.HasOption (DocumentOption.Specimen))
			{
				this.PaintSpecimen (port);
			}
		}

		public virtual void PrintForegroundCurrentPage(IPaintPort port)
		{
		}

		
		private void PaintDraft(IPaintPort port)
		{
			//	Dessine un très gros "BROUILLON" en filigrane en travers de la page.
			this.PaintWatermark (port, this.GetString ("Watermark.Draft"));
		}

		private void PaintSpecimen(IPaintPort port)
		{
			//	Dessine un très gros "SPECIMEN" en filigrane en travers de la page.
			this.PaintWatermark (port, this.GetString ("Watermark.Specimen"));
		}

		private void PaintWatermark(IPaintPort port, string text)
		{
			//	Dessine un très gros texte en filigrane en travers de la page.
			var bounds = new Rectangle (Point.Zero, this.RequiredPageSize);
			double diagonal = Point.Distance (bounds.BottomLeft, bounds.TopRight);
			double angle = System.Math.Atan2 (bounds.Height, bounds.Width);
			bounds.Inflate (diagonal);

			var initial = port.Transform;
			port.Transform = port.Transform.MultiplyByPostfix (Transform.CreateRotationRadTransform (angle, bounds.Center));

			var layout = new TextLayout
			{
				Text = text,
				DefaultColor = Color.FromBrightness (this.PreviewMode == PreviewMode.Print ? 0.80 : 0.95),  // plus foncé si impression réelle
				DefaultFont = Font.GetFont ("Arial", "Bold"),
				Alignment = ContentAlignment.MiddleCenter,
				LayoutSize = bounds.Size,
			};

			double factor = layout.SingleLineSize.Width / 8.56;  // facteur empyrique pour occuper toute la diagonale
			layout.DefaultFontSize = diagonal / factor;

			layout.Paint (bounds.BottomLeft, port);

			port.Transform = initial;
		}


		public static AbstractPrinter CreateDocumentPrinter(IBusinessContext businessContext, AbstractEntity entity, PrintingOptionDictionary options, PrintingUnitDictionary printingUnits)
		{
			//	Crée le XxxPrinter adapté à un type d'entité.
			//	Il ne faut pas perdre de vue qu'il n'y a pas de lien direct entre un type d'entité et
			//	un XxxPrinter. En particulier, il peut y avoir plusieurs XxxPrinter pour une même entité.

			var factory = EntityPrinterFactoryResolver.FindPrinterFactory (entity, options);

			if (factory == null)
			{
				return null;
			}

			return factory.CreatePrinter (businessContext, entity, options, printingUnits) as AbstractPrinter;
		}

		protected string GetString(Druid field)
		{
			var culture = Resources.FindSpecificCultureInfo (this.LanguageId);
			var bundle = Epsitec.Cresus.Core.Library.Documents.Res.Manager.GetBundle ("Strings", ResourceLevel.Merged, culture);

			if (bundle == null)
			{
				return null;
			}
			else
			{
				return bundle[field].AsString;
			}
		}


		#region Options reader
		protected double FontSize
		{
			get
			{
				return this.GetOptionValue (DocumentOption.FontSize);
			}
		}

		protected string LanguageId
		{
			//	Retourne la langue à utiliser pour imprimer le document.
			get
			{
				var languageId = this.GetOptionText (DocumentOption.Language).ToString ();

				if (languageId == "default")
				{
					languageId = null; 
				}

				if (string.IsNullOrEmpty (languageId))
				{
					languageId = TextFormatter.CurrentLanguageId;
				}

				return languageId;
			}
		}

		protected double GetOptionValue(DocumentOption option, double defautlValue = double.NaN)
		{
			//	Retourne la valeur d'une option de type distance.
			if (this.options != null)
			{
				var s = this.options[option];

				if (!string.IsNullOrEmpty (s))
				{
					double value;
					if (double.TryParse (s, out value))
					{
						return value;
					}
				}
			}

			if (double.IsNaN (defautlValue))
			{
				var defaultOption = AbstractPrinter.verboseDocumentOptions.Where (x => x.Option == option).FirstOrDefault ();

				if (defaultOption != null)
				{
					double value;
					if (double.TryParse (defaultOption.DefaultValue, out value))
					{
						return value;
					}
				}
			}

			return defautlValue;
		}

		protected FormattedText GetOptionText(DocumentOption option)
		{
			//	Retourne la valeur d'une option de type texte.
			if (this.options != null)
			{
				var s = this.options[option];

				if (!string.IsNullOrEmpty (s))
				{
					return s;
				}
			}

			var defaultOption = AbstractPrinter.verboseDocumentOptions.Where (x => x.Option == option).FirstOrDefault ();

			if (defaultOption != null)
			{
				return defaultOption.DefaultValue;
			}

			return null;
		}

		protected bool HasOption(DocumentOption option)
		{
			//	Indique si une option de type booléen est choisie.
			return this.HasOption (option, "true");
		}

		protected bool HasOption(DocumentOption option, string value)
		{
			//	Indique si une option de type énumération est choisie.
			if (this.options == null || !this.options.ContainsOption (option))
			{
				var defaultOption = AbstractPrinter.verboseDocumentOptions.Where (x => x.Option == option).FirstOrDefault ();

				if (defaultOption != null)
				{
					return defaultOption.DefaultValue == "true";
				}
				
				return false;
			}
			else
			{
				return this.options[option] == value;
			}
		}
		#endregion


		private static readonly Font						specimenFont = Font.GetFont ("Arial", "Bold");
		private static readonly IEnumerable<VerboseDocumentOption> verboseDocumentOptions = VerboseDocumentOption.GetAll ();
		public static readonly double						ContinuousHeight = 100*1000;  // 100 mètres devraient suffire

		protected readonly IBusinessContext					businessContext;
		protected readonly CoreData							coreData;
		protected readonly AbstractEntity					entity;
		private readonly PrintingUnitDictionary				printingUnits;

		protected PrintingUnit								currentPrintingUnit;
		private PrintingOptionDictionary					options;
		protected readonly DocumentContainer				documentContainer;
		protected readonly Dictionary<TableColumnKeys, TableColumn>	tableColumns;
		protected Size										requiredPageSize;
		private int											currentPage;
		private PreviewMode									previewMode;
	}
}
