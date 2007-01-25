//	Copyright � 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;

using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Epsitec.Common.Support
{
	[System.Serializable]
	public class DocumentInfo : ISerializable
	{
		public DocumentInfo()
		{
		}

		public double PageWidth
		{
			//	Dimensions d'une page en unit�s internes.
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
			//	Dimensions d'une page en unit�s internes.
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

		public int PageCount
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

		public int LayerCount
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

		public int ObjectCount
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

		public int ComplexObjectCount
		{
			//	Nombre total d'objets d�grad�s ou transparents.
			get
			{
				return this.complexesCount;
			}
			set
			{
				this.complexesCount = value;
			}
		}

		public int FontCount
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

		public int ImageCount
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

		protected int Version
		{
			get
			{
				return this.version;
			}
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise l'objet.
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
			//	Constructeur qui d�s�rialise l'objet.
			this.version = info.GetInt32 ("Version");

			if (this.version >= 3)
			{
				this.pageWidth = info.GetDouble ("PageWidth");
				this.pageHeight = info.GetDouble ("PageHeight");
			}
			
			this.pageFormat = info.GetString ("PageFormat");
			this.pagesCount = info.GetInt32 ("PagesCount");
			this.layersCount = info.GetInt32 ("LayersCount");
			this.objectsCount = info.GetInt32 ("ObjectsCount");
			this.complexesCount = info.GetInt32 ("ComplexesCount");

			if (this.version >= 2)
			{
				this.fontsCount = info.GetInt32 ("FontsCount");
				this.imagesCount = info.GetInt32 ("ImagesCount");
			}
		}


		private int version;
		private double pageWidth;
		private double pageHeight;
		private string pageFormat;
		private int pagesCount;
		private int layersCount;
		private int objectsCount;
		private int complexesCount;
		private int fontsCount;
		private int imagesCount;
	}
}
