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
	public class DocumentFiller
	{
		public DocumentFiller(Size pageSize, Margins pageMargins)
		{
			this.pageSize = pageSize;
			this.pageMargins = pageMargins;

			this.pages = new List<PageContent> ();
			this.pages.Add (new PageContent (0));  // crée la première page

			this.top = this.pageSize.Height - this.pageMargins.Top;
		}

		public void AddFromTop(AbstractObject obj, double bottomMargin)
		{
			//	Ajoute un objet (qui occupera toute la largeur) depuis le haut, sur autant de
			//	pages qu'il en faut.
			double width  = this.pageSize.Width  - this.pageMargins.Left - this.pageMargins.Right;
			double height = this.pageSize.Height - this.pageMargins.Top  - this.pageMargins.Bottom;
			double rest = this.top - this.pageMargins.Bottom;

			obj.InitializePages (width, rest, height, height);

			int pageCount = obj.PageCount;
			for (int objectPageIndex = 0; objectPageIndex < pageCount; objectPageIndex++)
			{
				double requiredHeight = obj.GetPageHeight (objectPageIndex);

				if (this.top - requiredHeight < this.pageMargins.Bottom)
				{
					this.top = this.pageSize.Height - this.pageMargins.Top;
					this.pages.Add (new PageContent (this.pages.Count));  // crée la page
				}

				this.pages[this.pages.Count-1].AddObject (obj, objectPageIndex, new Point (this.pageMargins.Left, this.top));

				this.top -= requiredHeight + bottomMargin;
			}
		}

		public int PageCount
		{
			get
			{
				return this.pages.Count;
			}
		}

		public bool Paint(IPaintPort port, int page)
		{
			if (page >= 0 && page < this.pages.Count)
			{
				return this.pages[page].Paint (port);
			}

			return true;
		}


		private readonly Size pageSize;
		private readonly Margins pageMargins;
		private readonly List<PageContent> pages;
		private double top;
	}
}
