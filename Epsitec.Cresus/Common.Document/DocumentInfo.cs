//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;

using System.Collections.Generic;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe <c>DocumentInfo</c> donne accès aux informations (statistiques)
	/// sur un document.
	/// </summary>
	[System.Serializable]
	public abstract class DocumentInfo : ISerializable, IDocumentInfo
	{
		public DocumentInfo()
		{
		}

		public Size								PageSize
		{
			get
			{
				return new Size (this.PageWidth, this.PageHeight);
			}
			set
			{
				this.PageWidth = value.Width;
				this.PageHeight = value.Height;
			}
		}

		public double							PageWidth
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

		public double							PageHeight
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

		public string							PageFormat
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

		public int								PageCount
		{
			//	Nombre total de pages.
			get
			{
				return this.pageCount;
			}
			set
			{
				this.pageCount = value;
			}
		}

		public int								LayerCount
		{
			//	Nombre total de calques.
			get
			{
				return this.layerCount;
			}
			set
			{
				this.layerCount = value;
			}
		}

		public int								ObjectCount
		{
			//	Nombre total d'objets.
			get
			{
				return this.objectCount;
			}
			set
			{
				this.objectCount = value;
			}
		}

		public int								ComplexObjectCount
		{
			//	Nombre total d'objets dégradés ou transparents.
			get
			{
				return this.complexCount;
			}
			set
			{
				this.complexCount = value;
			}
		}

		public int								FontCount
		{
			//	Nombre total de polices.
			get
			{
				return this.fontCount;
			}
			set
			{
				this.fontCount = value;
			}
		}

		public int								ImageCount
		{
			//	Nombre total d'images bitmap.
			get
			{
				return this.imageCount;
			}
			set
			{
				this.imageCount = value;
			}
		}

		protected int							Version
		{
			get
			{
				return this.version;
			}
		}


		#region IDocumentInfo Members

		public abstract string GetDescription();

		public abstract Image GetThumbnail();

		public virtual void GetAsyncThumbnail(Support.SimpleCallback<Image> callback)
		{
			callback (this.GetThumbnail ());
		}

		#endregion
		
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			info.AddValue ("Version", 3);
			info.AddValue ("PageSize", this.PageSize);
			info.AddValue ("PageWidth", this.pageWidth);
			info.AddValue ("PageHeight", this.pageHeight);
			info.AddValue ("PageFormat", this.pageFormat);
			info.AddValue ("PagesCount", this.pageCount);
			info.AddValue ("LayersCount", this.layerCount);
			info.AddValue ("ObjectsCount", this.objectCount);
			info.AddValue ("ComplexesCount", this.complexCount);
			info.AddValue ("FontsCount", this.fontCount);
			info.AddValue ("ImagesCount", this.imageCount);
		}

		protected DocumentInfo(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise l'objet.
			this.version = info.GetInt32 ("Version");

			if (this.version >= 3)
			{
				this.pageWidth = info.GetDouble ("PageWidth");
				this.pageHeight = info.GetDouble ("PageHeight");
			}
			else
			{
				this.PageSize = (Size) info.GetValue ("PageSize", typeof (Size));
			}
			
			this.pageFormat = info.GetString ("PageFormat");
			this.pageCount = info.GetInt32 ("PagesCount");
			this.layerCount = info.GetInt32 ("LayersCount");
			this.objectCount = info.GetInt32 ("ObjectsCount");
			this.complexCount = info.GetInt32 ("ComplexesCount");

			if (this.version >= 2)
			{
				this.fontCount = info.GetInt32 ("FontsCount");
				this.imageCount = info.GetInt32 ("ImagesCount");
			}
		}


		private int version;
		private double pageWidth;
		private double pageHeight;
		private string pageFormat;
		private int pageCount;
		private int layerCount;
		private int objectCount;
		private int complexCount;
		private int fontCount;
		private int imageCount;
	}
}
