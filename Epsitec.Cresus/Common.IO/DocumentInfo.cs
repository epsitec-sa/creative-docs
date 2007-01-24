//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Epsitec.Common.IO
{
	[System.Serializable]
	public class DocumentInfo : ISerializable
	{
		public DocumentInfo()
		{
		}

		public double PageWidth
		{
			//	Dimensions d'une page en unités internes.
			get
			{
				return this.pageWidth;
			}
			set
			{
				this.pageWidth = value;
			}
		}

		public double PageHeight
		{
			//	Dimensions d'une page en unités internes.
			get
			{
				return this.pageHeight;
			}
			set
			{
				this.pageHeight = value;
			}
		}

		public string PageFormat
		{
			//	Format d'une page en clair ("A4" ou "123 x 456").
			get
			{
				return this.pageFormat;
			}
			set
			{
				this.pageFormat = value;
			}
		}

		public int PagesCount
		{
			//	Nombre total de pages.
			get
			{
				return this.pagesCount;
			}
			set
			{
				this.pagesCount = value;
			}
		}

		public int LayersCount
		{
			//	Nombre total de calques.
			get
			{
				return this.layersCount;
			}
			set
			{
				this.layersCount = value;
			}
		}

		public int ObjectsCount
		{
			//	Nombre total d'objets.
			get
			{
				return this.objectsCount;
			}
			set
			{
				this.objectsCount = value;
			}
		}

		public int ComplexesCount
		{
			//	Nombre total d'objets dégradés ou transparents.
			get
			{
				return this.complexesCount;
			}
			set
			{
				this.complexesCount = value;
			}
		}

		public int FontsCount
		{
			//	Nombre total de polices.
			get
			{
				return this.fontsCount;
			}
			set
			{
				this.fontsCount = value;
			}
		}

		public int ImagesCount
		{
			//	Nombre total d'images bitmap.
			get
			{
				return this.imagesCount;
			}
			set
			{
				this.imagesCount = value;
			}
		}


		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			info.AddValue ("Version", 3);
			info.AddValue ("PageWidth", this.pageWidth);
			info.AddValue ("PageHeight", this.pageHeight);
			info.AddValue ("PageFormat", this.pageFormat);
			info.AddValue ("PagesCount", this.pagesCount);
			info.AddValue ("LayersCount", this.layersCount);
			info.AddValue ("ObjectsCount", this.objectsCount);
			info.AddValue ("ComplexesCount", this.complexesCount);
			info.AddValue ("FontsCount", this.fontsCount);
			info.AddValue ("ImagesCount", this.imagesCount);
		}

		protected DocumentInfo(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise l'objet.
			int version = info.GetInt32 ("Version");

			if (version >= 3)
			{
				this.pageWidth = info.GetDouble ("PageWidth");
				this.pageHeight = info.GetDouble ("PageHeight");
			}
			
			this.pageFormat = info.GetString ("PageFormat");
			this.pagesCount = info.GetInt32 ("PagesCount");
			this.layersCount = info.GetInt32 ("LayersCount");
			this.objectsCount = info.GetInt32 ("ObjectsCount");
			this.complexesCount = info.GetInt32 ("ComplexesCount");

			if (version >= 2)
			{
				this.fontsCount = info.GetInt32 ("FontsCount");
				this.imagesCount = info.GetInt32 ("ImagesCount");
			}
		}


		protected double pageWidth;
		protected double pageHeight;
		protected string pageFormat;
		protected int pagesCount;
		protected int layersCount;
		protected int objectsCount;
		protected int complexesCount;
		protected int fontsCount;
		protected int imagesCount;
	}
}
