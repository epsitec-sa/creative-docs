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
			this.pages.Add (new PageContainer (0, PageTypeEnum.First));  // crée la première page

			this.currentPage = 0;

			this.currentVerticalPosition = this.pageSize.Height - this.pageMargins.Top;  // on part en haut
		}

		/// <summary>
		/// Si la dernière page actuelle n'est pas vide, crée une nouvelle page vide.
		/// </summary>
		public int PrepareEmptyPage(PageTypeEnum pageType)
		{
			if (this.pages.Last ().Count > 0)  // dernière page non vide ?
			{
				this.AddNewPage (pageType);
			}

			return this.currentPage;
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
					this.AddNewPage (PageTypeEnum.Following);
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
				this.AddNewPage (PageTypeEnum.Following);
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

		public PageTypeEnum GetPageType(int page)
		{
			return this.pages[page].PageType;
		}

		/// <summary>
		/// Retourne le nombre de pages d'un type donné que contient le document.
		/// </summary>
		public int PageCount(PageTypeEnum pageType)
		{
			return this.GetPagesFromType (pageType).Count;
		}

		/// <summary>
		/// Retourne true s'il n'y a rien à imprimer.
		/// </summary>
		public bool IsEmpty(PageTypeEnum pageType)
		{
			var pages = this.GetPagesFromType (pageType);

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
		/// Dessine une page d'un type donné du document.
		/// </summary>
		/// <param name="port">Port graphique</param>
		/// <param name="page">Rang de la page (0..n)</param>
		/// <returns></returns>
		public bool Paint(IPaintPort port, PageTypeEnum pageType, int page, bool isPreview)
		{
			var pages = this.GetPagesFromType (pageType);

			if (page >= 0 && page < pages.Count)
			{
				return pages[page].Paint (port, isPreview);
			}

			return true;
		}

		private List<PageContainer> GetPagesFromType(PageTypeEnum pageType)
		{
			if (pageType == PageTypeEnum.All ||
				pageType == PageTypeEnum.Copy)
			{
				return this.pages;
			}
			else
			{
				return this.pages.Where (x => x.PageType == pageType).ToList ();
			}
		}


		private void AddNewPage(PageTypeEnum pageType)
		{
			this.pages.Add (new PageContainer (this.pages.Count, pageType));  // crée une nouvelle page

			this.currentPage++;
			this.currentVerticalPosition = this.pageSize.Height - this.pageMargins.Top;  // revient en haut
		}


		private readonly List<PageContainer>	pages;
		private Size							pageSize;
		private Margins							pageMargins;
		private int								currentPage;
		private double							currentVerticalPosition;
	}
}
