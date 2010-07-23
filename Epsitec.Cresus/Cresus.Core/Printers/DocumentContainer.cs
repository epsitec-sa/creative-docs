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


		public void Clear()
		{
			this.pages.Clear ();
			this.pages.Add (new PageContainer (0));  // crée la première page

			this.currentPage = 0;

			this.currentVerticalPosition = this.pageSize.Height - this.pageMargins.Top;  // on part en haut
		}


		/// <summary>
		/// Ajoute un objet à une position absolue, dans la page courante.
		/// </summary>
		/// <param name="band"></param>
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
		/// <param name="band"></param>
		/// <param name="bottomMargin"></param>
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
					this.pages.Add (new PageContainer (this.pages.Count));  // crée la page

					this.currentPage++;
					this.currentVerticalPosition = this.pageSize.Height - this.pageMargins.Top;  // revient en haut
				}

				this.pages[this.currentPage].AddBand (band, section, new Point (this.pageMargins.Left, this.currentVerticalPosition));

				var bounds = new Rectangle (this.pageMargins.Left, this.currentVerticalPosition-requiredHeight, width, requiredHeight);
				list.Add (bounds);

				this.currentVerticalPosition -= requiredHeight + bottomMargin;
			}

			return list;
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

		/// <summary>
		/// Retourne le nombre de pages que contient le document.
		/// </summary>
		public int PageCount
		{
			get
			{
				return this.pages.Count;
			}
		}

		/// <summary>
		/// Dessine une page du document.
		/// </summary>
		/// <param name="port">Port graphique</param>
		/// <param name="page">Rang de la page (0..n)</param>
		/// <returns></returns>
		public bool Paint(IPaintPort port, int page, bool isPreview)
		{
			if (page >= 0 && page < this.pages.Count)
			{
				return this.pages[page].Paint (port, isPreview);
			}

			return true;
		}


		private readonly List<PageContainer>	pages;
		private Size							pageSize;
		private Margins							pageMargins;
		private int								currentPage;
		private double							currentVerticalPosition;
	}
}
