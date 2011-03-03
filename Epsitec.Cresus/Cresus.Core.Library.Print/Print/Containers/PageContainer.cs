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

using Epsitec.Cresus.Core.Documents;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Print.Containers
{
	/// <summary>
	/// Cette classe contient tous les objets BandContainer d'une page du document DocumentContainer.
	/// </summary>
	public class PageContainer
	{
		public PageContainer(int page, int documentRank, PageType pageType)
		{
			this.page         = page;
			this.documentRank = documentRank;
			this.pageType     = pageType;

			this.bands = new List<BandContainer> ();
		}

		public int DocumentRank
		{
			get
			{
				return this.documentRank;
			}
		}

		public PageType PageType
		{
			get
			{
				return this.pageType;
			}
			set
			{
				this.pageType = value;
			}
		}

		public int Count
		{
			get
			{
				return this.bands.Count;
			}
		}

		/// <summary>
		/// Ajoute un objet AbstractBand dans la page.
		/// </summary>
		/// <param name="band"></param>
		/// <param name="section"></param>
		/// <param name="topLeft"></param>
		public void AddBand(Bands.AbstractBand band, int section, Point topLeft)
		{
			var bandContainer = new BandContainer (band, section, topLeft);

			this.bands.Add (bandContainer);
		}

		/// <summary>
		/// Dessine tous les objets AbstractBand contenus dans la page.
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public bool PaintBackground(IPaintPort port, PreviewMode previewMode)
		{
			bool ok = true;

			foreach (var band in this.bands)
			{
				if (!band.PaintBackground (port, previewMode))
				{
					ok = false;
				}
			}

			return ok;
		}

		/// <summary>
		/// Dessine tous les objets AbstractBand contenus dans la page.
		/// </summary>
		/// <param name="port"></param>
		/// <returns></returns>
		public bool PaintForeground(IPaintPort port, PreviewMode previewMode)
		{
			bool ok = true;

			foreach (var band in this.bands)
			{
				if (!band.PaintForeground (port, previewMode))
				{
					ok = false;
				}
			}

			return ok;
		}


		private readonly int					page;
		private readonly int					documentRank;
		private readonly List<BandContainer>	bands;

		private PageType						pageType;
	}
}
