﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// <summary>
	/// Cette classe permet de construire le document complet à imprimer, avec ses différentes pages.
	/// La classe PageContainer contient une page.
	/// La classe BandContainer contient une section d'une bande.
	/// Toutes les dimensions sont données en millimètres.
	/// </summary>
	public class DocumentContainer
	{
		public DocumentContainer()
		{
			this.pages = new List<PageContainer> ();
			this.Clear ();
		}


		public Size PageSize
		{
			get
			{
				return this.pageSize;
			}
			set
			{
				this.pageSize = value;
				this.Clear ();
			}
		}

		public Margins PageMargins
		{
			get
			{
				return this.pageMargins;
			}
			set
			{
				this.pageMargins = value;
				this.Clear ();
			}
		}


		public double CurrentWidth
		{
			get
			{
				return this.pageSize.Width - pageMargins.Left - pageMargins.Right;
			}
		}


		public void Clear()
		{
			this.pages.Clear ();
			this.pages.Add (new PageContainer (0, PageType.First));  // crée la première page

			this.currentPage = 0;

			this.currentVerticalPosition = this.pageSize.Height - this.pageMargins.Top;  // on part en haut
		}

		/// <summary>
		/// Si la dernière page actuelle n'est pas vide, crée une nouvelle page vide.
		/// </summary>
		public int PrepareEmptyPage(PageType pageType)
		{
			if (this.pages.Last ().Count > 0)  // dernière page non vide ?
			{
				this.AddNewPage (pageType);
			}

			return this.currentPage;
		}

		/// <summary>
		/// Termine la génération d'un document. S'il n'y a qu'une page de type PageType.First, son type
		/// est replacé par PageType.Single.
		/// </summary>
		/// <param name="firstPage"></param>
		public void Ending(int firstPage)
		{
			if (this.pages.Count == firstPage+1 && this.pages[firstPage].PageType == PageType.First)
			{
				this.pages[firstPage].PageType = PageType.Single;
			}
		}


		/// <summary>
		/// Ajoute un objet à une position absolue, dans la page courante.
		/// </summary>
		/// <param name="band">Objet à ajouter</param>
		/// <param name="bounds"></param>
		public void AddAbsolute(AbstractBand band, Rectangle bounds)
		{
			band.BuildSections (bounds.Width, bounds.Height, bounds.Height, bounds.Height);

			this.pages[this.currentPage].AddBand (band, 0, bounds.TopLeft);
		}

		/// <summary>
		/// Ajoute un objet (qui occupera toute la largeur) depuis le haut, sur autant de
		/// pages qu'il en faut.
		/// Retourne la liste des rectangles occupés par l'objet sur chaque page.
		/// </summary>
		/// <param name="band">Objet à ajouter</param>
		/// <param name="bottomMargin">Marge inférieure jusqu'à l'objet suivant</param>
		/// <returns>Rectangles occupés par l'objet sur chaque page</returns>
		public List<Rectangle> AddFromTop(AbstractBand band, double bottomMargin)
		{
			var list = new List<Rectangle> ();

			double width  = this.pageSize.Width  - this.pageMargins.Left - this.pageMargins.Right;
			double height = this.pageSize.Height - this.pageMargins.Top  - this.pageMargins.Bottom;
			double rest = this.currentVerticalPosition - this.pageMargins.Bottom;

			band.BuildSections (width, rest, height, height);

			int sectionCount = band.SectionCount;
			for (int section = 0; section < sectionCount; section++)
			{
				double requiredHeight = band.GetSectionHeight (section);

				if (this.currentVerticalPosition - requiredHeight < this.pageMargins.Bottom)  // pas assez de place en bas ?
				{
					this.AddNewPage (PageType.Following);
				}

				this.pages[this.currentPage].AddBand (band, section, new Point (this.pageMargins.Left, this.currentVerticalPosition));

				var bounds = new Rectangle (this.pageMargins.Left, this.currentVerticalPosition-requiredHeight, width, requiredHeight);
				list.Add (bounds);

				this.currentVerticalPosition -= requiredHeight + bottomMargin;
			}

			return list;
		}

		/// <summary>
		/// Ajoute un objet au bas d'une page. S'il n'y a pas assez de place, crée une nouvelle page.
		/// On ne tient pas compte de la marge inférieure, qui peut donc sans problème être dépassée.
		/// </summary>
		/// <param name="band">Objet à ajouter</param>
		/// <param name="bottomPosition">Position depuis le bas</param>
		/// <returns>Rectangle occupé par l'objet</returns>
		public Rectangle AddToBottom(AbstractBand band, double bottomPosition)
		{
			double width  = this.pageSize.Width  - this.pageMargins.Left - this.pageMargins.Right;

			double h = band.RequiredHeight (width);

			if (this.currentVerticalPosition-h < bottomPosition)  // pas assez de place ?
			{
				this.AddNewPage (PageType.Following);
			}

			Rectangle bounds = new Rectangle (this.pageMargins.Left, bottomPosition, width, h);
			this.AddAbsolute (band, bounds);

			return bounds;
		}


		public int CurrentPage
		{
			get
			{
				return this.currentPage;
			}
			set
			{
				value = System.Math.Max (value, 0);
				value = System.Math.Min (value, this.pages.Count-1);

				this.currentPage = value;
			}
		}

		public double CurrentVerticalPosition
		{
			get
			{
				return this.currentVerticalPosition;
			}
			set
			{
				this.currentVerticalPosition = value;
			}
		}

		public PageType GetPageType(int page)
		{
			return this.pages[page].PageType;
		}

		public int[] GetPhysicalPages(PrinterUnitFunction printerFunctionUsed)
		{
			var list = new List<int> ();
			var pages = this.GetPagesForFunction (printerFunctionUsed);

			foreach (var page in pages)
			{
				list.Add (this.pages.IndexOf (page));
			}

			return list.ToArray ();
		}

		/// <summary>
		/// Retourne le nombre de pages pour une unité d'impression donnée que contient le document.
		/// </summary>
		public int PageCount(PrinterUnitFunction printerFunctionUsed = PrinterUnitFunction.ForAllPages)
		{
			return this.GetPagesForFunction (printerFunctionUsed).Count;
		}

		/// <summary>
		/// Retourne true s'il n'y a rien à imprimer pour une unité d'impression donnée.
		/// S'il y a une seule page vide, on considère qu'il n'y a rien à imprimer.
		/// </summary>
		public bool IsEmpty(PrinterUnitFunction printerFunctionUsed)
		{
			var pages = this.GetPagesForFunction (printerFunctionUsed);

			if (pages.Count <= 0)
			{
				return true;
			}

			if (pages.Count > 1)
			{
				return false;
			}

			return pages[0].Count == 0;
		}

		/// <summary>
		/// Dessine une page du document pour une imprimante donnée.
		/// </summary>
		public bool PaintBackground(IPaintPort port, int page, bool isPreview)
		{
			if (page >= 0 && page < this.pages.Count)
			{
				return this.pages[page].PaintBackground (port, isPreview);
			}

			return true;
		}

		/// <summary>
		/// Dessine une page du document pour une imprimante donnée.
		/// </summary>
		public bool PaintForeground(IPaintPort port, int page, bool isPreview)
		{
			if (page >= 0 && page < this.pages.Count)
			{
				return this.pages[page].PaintForeground (port, isPreview);
			}

			return true;
		}

		private List<PageContainer> GetPagesForFunction(PrinterUnitFunction printerFunctionUsed)
		{
			//	Retourne la liste des pages pour une unité d'impression donnée.
			return this.pages.Where (x => Common.IsPrinterAndPageMatching (printerFunctionUsed, x.PageType)).ToList ();
		}


		private void AddNewPage(PageType pageType)
		{
			this.pages.Add (new PageContainer (this.pages.Count, pageType));  // crée une nouvelle page

			this.currentPage = this.pages.Count-1;
			this.currentVerticalPosition = this.pageSize.Height - this.pageMargins.Top;  // revient en haut
		}


		private readonly List<PageContainer>	pages;
		private Size							pageSize;
		private Margins							pageMargins;
		private int								currentPage;
		private double							currentVerticalPosition;
	}
}
