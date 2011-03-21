//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Print.EntityPrinters;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Print
{
	/// <summary>
	/// Une instance de SectionToPrint représente une section d'une ou plusieurs pages contigües à imprimer
	/// sur une unité d'impression (PrintingUnit).
	/// Initialement, on crée des sections d'une seule page. Elles sont regroupées par la suite.
	/// </summary>
	public class SectionToPrint
	{
		public SectionToPrint(PrintingUnit printingUnit, string job, int firstPage, AbstractPrinter documentPrinter, Size pageSize, PrintingOptionDictionary options)
		{
			//	Crée une section d'une page.
			this.printingUnit    = printingUnit;
			this.job             = job;
			this.firstPage       = firstPage;
			this.PageCount       = 1;
			this.documentPrinter = documentPrinter;
			this.pageSize        = pageSize;
			this.options         = options;

			this.Enable = true;
		}

		public PrintingUnit PrintingUnit
		{
			get
			{
				return this.printingUnit;
			}
		}

		public string InternalJobName
		{
			//	Le nom interne du job a la forme "job.e.d.c":
			//		job		nom interne du job ("All", "Copy", "Spec", etc.)
			//		e		rang de l'entité (1..n)
			//		p		rang du AbstractPrinter (1..n)
			//		d		rang du document (1..n)
			//		c		rang de la copie (1..n)
			//	Par exemple "All.1.1".
			get
			{
				return this.job;
			}
		}

		public int FirstPage
		{
			get
			{
				return this.firstPage;
			}
		}

		public int PageCount
		{
			get;
			set;
		}

		public Size PageSize
		{
			get
			{
				return this.pageSize;
			}
		}

		public PrintingOptionDictionary Options
		{
			get
			{
				return this.options;
			}
		}

		public AbstractPrinter DocumentPrinter
		{
			get
			{
				return this.documentPrinter;
			}
		}

		public bool Enable
		{
			//	Indique si la section doit être imprimée.
			get;
			set;
		}

		public string PagesDescription
		{
			get
			{
				if (this.PageCount == 1)
				{
					return string.Format ("Page {0}", (this.FirstPage+1).ToString ());
				}
				else
				{
					return string.Format ("Pages {0} à {1}", (this.FirstPage+1).ToString (), (this.FirstPage+this.PageCount).ToString ());
				}
			}
		}

		public override string ToString()
		{
			// Pratique pour le debug.
			return string.Format ("PrinterLogicalName={0}, PrinterPhysicalName={1}, Job={2}, FirstPage={3}, PageCount={4}", this.printingUnit.LogicalName, this.printingUnit.PhysicalPrinterName, this.job, this.firstPage, this.PageCount);
		}


		public static int CompareSectionToPrint(SectionToPrint x, SectionToPrint y)
		{
			//	Détermine comment regrouper les pages. On cherche à grouper les pages ansi:
			//	- par jobs
			//	- par impriante physique
			//	- par pages croissantes
			int result;

			result = string.Compare (x.InternalJobName, y.InternalJobName);
			if (result != 0)
			{
				return result;
			}

			result = string.Compare (x.PrintingUnit.PhysicalPrinterName, y.PrintingUnit.PhysicalPrinterName);
			if (result != 0)
			{
				return result;
			}

			if (x.FirstPage != y.FirstPage)
			{
				return (x.FirstPage < y.FirstPage) ? -1 : 1;
			}

			return 0;
		}


		private readonly PrintingUnit				printingUnit;
		private readonly string						job;
		private readonly int						firstPage;
		private readonly AbstractPrinter			documentPrinter;
		private readonly Size						pageSize;
		private readonly PrintingOptionDictionary			options;
	}
}
