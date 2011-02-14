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

namespace Epsitec.Cresus.Core.Print2.Bands
{
	public class SurfaceBand : AbstractBand
	{
		public SurfaceBand()
			: base ()
		{
			this.BackgroundColor = Color.FromBrightness (0);
		}


		public Color BackgroundColor
		{
			get;
			set;
		}
		public double Height
		{
			get;
			set;
		}


		public override double RequiredHeight(double width)
		{
			this.width = width;
			return this.Height;
		}


		/// <summary>
		/// Effectue la justification verticale pour découper le texte en sections.
		/// </summary>
		/// <param name="width">Largeur pour toutes les sections</param>
		/// <param name="initialHeight">Hauteur de la première section</param>
		/// <param name="middleheight">Hauteur des sections suivantes</param>
		/// <param name="finalHeight">Hauteur de la dernière section</param>
		/// <returns>Retourne false s'il n'a pas été possible de mettre tout le contenu</returns>
		public override bool BuildSections(double width, double initialHeight, double middleheight, double finalHeight)
		{
			this.width = width;
			return true;
		}

		public override int SectionCount
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Retourne la hauteur que l'objet occupe dans une section.
		/// </summary>
		/// <param name="section"></param>
		/// <returns></returns>
		public override double GetSectionHeight(int section)
		{
			if (section == 0)
			{
				return this.Height;
			}

			return 0;
		}

		/// <summary>
		/// Dessine une section de l'objet à une position donnée.
		/// </summary>
		/// <param name="port">Port graphique</param>
		/// <param name="section">Rang de la section à dessiner</param>
		/// <param name="topLeft">Coin supérieur gauche</param>
		/// <returns>Retourne false si le contenu est trop grand et n'a pas pu être dessiné</returns>
		public override bool PaintBackground(IPaintPort port, PreviewMode previewMode, int section, Point topLeft)
		{
			if (section != 0)
			{
				return true;
			}

			port.Color = this.BackgroundColor;
			port.PaintSurface (Path.FromRectangle (topLeft.X, topLeft.Y-this.Height, this.width, this.Height));

			return true;
		}


		private double width;
	}
}
