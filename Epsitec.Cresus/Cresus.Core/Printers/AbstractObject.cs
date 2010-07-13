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
	public abstract class AbstractObject
	{
		public AbstractObject()
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
		/// Effectue la justification verticale pour découper le contenu en pages.
		/// </summary>
		/// <param name="width">Largeur sur toutes les pages</param>
		/// <param name="initialHeight">Hauteur de la première page</param>
		/// <param name="middleheight">Hauteur des pages suivantes</param>
		/// <param name="finalHeight">Hauteur de la dernière page</param>
		/// <returns>Retourne false s'il n'a pas été possible de mettre tout le contenu</returns>
		public virtual bool InitializePages(double width, double initialHeight, double middleheight, double finalHeight)
		{
			return true;
		}

		public virtual int PageCount
		{
			get
			{
				return 1;
			}
		}

		/// <summary>
		/// Dessine une page de l'objet à une position donnée.
		/// </summary>
		/// <param name="port">Port graphique</param>
		/// <param name="page">Rang de la page à dessiner</param>
		/// <param name="topLeft">Coin supérieur gauche</param>
		/// <returns>Retourne false si le contenu est trop grand et n'a pas pu être dessiné</returns>
		public virtual bool Paint(IPaintPort port, int page, Point topLeft)
		{
			return true;
		}
	}
}
