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

namespace Epsitec.Cresus.Core.Print.Bands
{
	/// <summary>
	/// Classe abstraite pour les 'bandes', qui sont des objets ayant une largeur donnée et une hauteur infinie.
	/// Une 'bande' est découpée en 'sections'. En principe, chaque 'section' occupera une page du document imprimé,
	/// sauf si on cherche à faire du multi-colonnes.
	/// </summary>
	public abstract class AbstractBand
	{
		public AbstractBand()
		{
			this.Font = Font.GetFont ("Arial", "Regular");
			this.FontSize = 3.0;
		}


		public Font Font
		{
			get;
			set;
		}

		public double FontSize
		{
			get;
			set;
		}


		public virtual double RequiredHeight(double width)
		{
			return 0;
		}


		/// <summary>
		/// Effectue la justification verticale pour découper le contenu en sections.
		/// </summary>
		/// <param name="width">Largeur pour toutes les sections</param>
		/// <param name="initialHeight">Hauteur de la première section</param>
		/// <param name="middleheight">Hauteur des sections suivantes</param>
		/// <param name="finalHeight">Hauteur de la dernière section</param>
		public virtual void BuildSections(double width, double initialHeight, double middleheight, double finalHeight)
		{
		}

		/// <summary>
		/// Retourne le nombre total de sections de la bande.
		/// </summary>
		public virtual int SectionCount
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
		public virtual double GetSectionHeight(int section)
		{
			return 0;
		}

		/// <summary>
		/// Dessine une section de l'objet à une position donnée.
		/// </summary>
		/// <param name="port">Port graphique</param>
		/// <param name="section">Rang de la section à dessiner</param>
		/// <param name="topLeft">Coin supérieur gauche</param>
		/// <returns>Retourne false si le contenu est trop grand et n'a pas pu être dessiné</returns>
		public virtual bool PaintBackground(IPaintPort port, PreviewMode previewMode, int section, Point topLeft)
		{
			return true;
		}

		/// <summary>
		/// Dessine une section de l'objet à une position donnée.
		/// </summary>
		/// <param name="port">Port graphique</param>
		/// <param name="section">Rang de la section à dessiner</param>
		/// <param name="topLeft">Coin supérieur gauche</param>
		/// <returns>Retourne false si le contenu est trop grand et n'a pas pu être dessiné</returns>
		public virtual bool PaintForeground(IPaintPort port, PreviewMode previewMode, int section, Point topLeft)
		{
			return true;
		}


		public static readonly double defaultUnderlineWidth = 0.2;
		public static readonly double defaultWaveWidth = 0.75;
	}
}
