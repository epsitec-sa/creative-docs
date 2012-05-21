//	Copyright © 2007-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

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
			//	Format d'une page en clair ("A4" ou "123 × 456").
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

		public long DocumentVersion
		{
			get
			{
				return this.documentVersion;
			}
			set
			{
				this.documentVersion = value;
			}
		}

		public string DocumentVersionString
		{
			get
			{
				if (this.documentVersion == 0)
				{
					return "?";
				}
				else
				{
					int revision   = (int) (this.documentVersion >> 32) & 0xffff;
					int version    = (int) (this.documentVersion >> 16) & 0xffff;
					int subversion = (int) (this.documentVersion >>  0) & 0xffff;

					return string.Format ("{0}.{1}.{2}", revision, version, subversion);
				}
			}
		}

		public void DefineDocumentVersion(System.Reflection.Assembly assembly)
		{
			string[] args = assembly.GetVersionString ().Split ('.');

			int revision   = int.Parse (args[0], System.Globalization.CultureInfo.InvariantCulture);
			int version    = int.Parse (args[1], System.Globalization.CultureInfo.InvariantCulture);
			int subversion = int.Parse (args[2], System.Globalization.CultureInfo.InvariantCulture);

			this.documentVersion = ((long)(revision) << 32) + ((long)(version) << 16) + (long)(subversion);
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
			info.AddValue ("Version", 4);
			info.AddValue ("PageSize", this.PageSize);
			info.AddValue ("PageWidth", this.pageWidth);
			info.AddValue ("PageHeight", this.pageHeight);
			info.AddValue ("PageFormat", this.pageFormat);
			info.AddValue ("PageCount", this.pageCount);
			info.AddValue ("LayerCount", this.layerCount);
			info.AddValue ("ObjectCount", this.objectCount);
			info.AddValue ("ComplexCount", this.complexCount);
			info.AddValue ("FontCount", this.fontCount);
			info.AddValue ("ImageCount", this.imageCount);
			info.AddValue ("DocumentVersion", this.documentVersion);
		}

		protected DocumentInfo(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui désérialise l'objet.
			this.version = info.GetInt32 ("Version");

			try
			{
				if (this.version >= 3)
				{
					this.pageWidth = info.GetDouble ("PageWidth");
					this.pageHeight = info.GetDouble ("PageHeight");
					this.pageFormat = info.GetString ("PageFormat");
				}
				else
				{
					this.PageSize = (Size) info.GetValue ("PageSize", typeof (Size));
					this.pageFormat = info.GetString ("PageFormat");
				}

				if (this.version >= 4)
				{
					this.pageCount = info.GetInt32 ("PageCount");
					this.layerCount = info.GetInt32 ("LayerCount");
					this.objectCount = info.GetInt32 ("ObjectCount");
					this.complexCount = info.GetInt32 ("ComplexCount");
					this.fontCount = info.GetInt32 ("FontCount");
					this.imageCount = info.GetInt32 ("ImageCount");
					this.documentVersion = info.GetInt64 ("DocumentVersion");
				}
				else
				{
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
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("Statistics: " + ex.Message);
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
		private long documentVersion;
	}
}
