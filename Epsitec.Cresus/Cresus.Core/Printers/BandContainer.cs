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
	/// Cette classe correspond à un objet graphique AbstractBand dans une page.
	/// Toute l'information permettant de dessiner l'objet y est incluse (rang de la section
	/// et coin supérieur/gauche).
	/// Le même objet AbstractBand peut apparaître dans plusieurs pages, s'il contient
	/// plusieurs sections.
	/// </summary>
	public class BandContainer
	{
		public BandContainer(AbstractBand band, int section, Point topLeft)
		{
			this.band = band;
			this.section = section;
			this.topLeft = topLeft;
		}

		public bool Paint(IPaintPort port, bool isPreview)
		{
			return this.band.Paint (port, isPreview, this.section, this.topLeft);
		}


		private readonly AbstractBand		band;
		private readonly int				section;
		private readonly Point				topLeft;
	}
}
