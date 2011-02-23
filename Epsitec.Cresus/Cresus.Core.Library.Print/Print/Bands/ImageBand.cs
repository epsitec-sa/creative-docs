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
	public class ImageBand : AbstractBand
	{
		public ImageBand()
			: base ()
		{
		}


		public void Load(CoreData coreData, string name)
		{
			//	Associe à l'objet une image cherchée dans la base de données.
			var example = new ImageEntity ();
			example.Name = TextFormatter.FormatText (name);
			var imageEntity = coreData.DataContext.GetByExample<ImageEntity> (example).FirstOrDefault ();

			this.Load (coreData, imageEntity);
		}

		public void Load(CoreData coreData, ImageEntity imageEntity)
		{
			//	Associe à l'objet une image de la base de données.
			if (imageEntity.IsNotNull ())
			{
				throw new System.NotImplementedException ();
#if false
				var store = coreData.ImageDataStore;
				var data = store.GetImageData (imageEntity.ImageBlob.Code);
				this.image = data.GetImage ();
				this.image.Id = imageEntity.ImageBlob.Code;  // (*)
#endif
			}
		}

		// (*)	Il est très important de donner l'identificateur à l'image, pour permettre la
		//		désérialisation du port graphique XmlPort !


		public override double RequiredHeight(double width)
		{
			this.width = width;
			return this.ProportionnalHeight;
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
			if (this.image == null)
			{
				this.width = width;
			}
			else
			{
				double sx = width         / this.image.Width;
				double sy = initialHeight / this.image.Height;
				double scale = System.Math.Min (sx, sy);

				this.width = this.image.Width * scale;
			}

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
				return this.ProportionnalHeight;
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
			if (section != 0 || this.image == null)
			{
				return true;
			}

			double h = this.ProportionnalHeight;
			port.PaintImage (this.image, new Rectangle (topLeft.X, topLeft.Y-h, this.width, h));

			return true;
		}


		private double ProportionnalHeight
		{
			get
			{
				if (this.image != null)
				{
					return this.width * this.image.Height / this.image.Width;
				}

				return 0;
			}
		}


		private Image image;
		private double width;
	}
}
