using System.Xml.Serialization;
using System.IO;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe IconObjects contient tous les objets qui forment une icône.
	/// </summary>

	[XmlRootAttribute("EpsitecPictogram")]

	[
	XmlInclude(typeof(ObjectArrow)),
	XmlInclude(typeof(ObjectBezier)),
	XmlInclude(typeof(ObjectCircle)),
	XmlInclude(typeof(ObjectEllipse)),
	XmlInclude(typeof(ObjectLine)),
	XmlInclude(typeof(ObjectPoly)),
	XmlInclude(typeof(ObjectRectangle)),
	XmlInclude(typeof(ObjectRegular)),
	XmlInclude(typeof(ObjectText)),

	XmlInclude(typeof(Handle)),

	XmlInclude(typeof(PropertyBool)),
	XmlInclude(typeof(PropertyColor)),
	XmlInclude(typeof(PropertyDouble)),
	XmlInclude(typeof(PropertyGradient)),
	XmlInclude(typeof(PropertyLine)),
	XmlInclude(typeof(PropertyList)),
	XmlInclude(typeof(PropertyString)),
	]

	public class IconObjects
	{
		public IconObjects()
		{
		}

		// Taille préférentielle de l'icône en pixels.
		public Drawing.Size Size
		{
			get
			{
				return this.size;
			}

			set
			{
				this.size = value;
			}
		}

		// Origine de l'icône en pixels.
		public Drawing.Point Origin
		{
			get
			{
				return this.origin;
			}

			set
			{
				this.origin = value;
			}
		}

		// Liste des objets.
		[XmlArrayItem("Arrow",     Type=typeof(ObjectArrow))]
		[XmlArrayItem("Bezier",    Type=typeof(ObjectBezier))]
		[XmlArrayItem("Circle",    Type=typeof(ObjectCircle))]
		[XmlArrayItem("Ellipse",   Type=typeof(ObjectEllipse))]
		[XmlArrayItem("Line",      Type=typeof(ObjectLine))]
		[XmlArrayItem("Polyline",  Type=typeof(ObjectPoly))]
		[XmlArrayItem("Rectangle", Type=typeof(ObjectRectangle))]
		[XmlArrayItem("Polygon",   Type=typeof(ObjectRegular))]
		[XmlArrayItem("Text",      Type=typeof(ObjectText))]
		public System.Collections.ArrayList Objects
		{
			get
			{
				return this.objects;
			}

			set
			{
				this.objects = value;
			}
		}

		public AbstractObject this[int index]
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.objects[index] != null);
				return this.objects[index] as AbstractObject;
			}
			
			set
			{
				System.Diagnostics.Debug.Assert(this.objects[index] != null);
				this.objects[index] = value;
			}
		}
		
		public int Count
		{
			get { return this.objects.Count; }
		}

		public void Clear()
		{
			this.objects.Clear();
		}

		public void Add(AbstractObject obj)
		{
			this.objects.Add(obj);
		}

		public void Insert(int index, AbstractObject obj)
		{
			this.objects.Insert(index, obj);
		}

		public void RemoveAt(int index)
		{
			this.objects.RemoveAt(index);
		}


		public void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			int total = this.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				this[index].DrawGeometry(graphics, iconContext);
			}
		}

		public void DrawHandle(Drawing.Graphics graphics, IconContext iconContext)
		{
			int total = this.Count;
			for ( int index=0 ; index<total ; index++ )
			{
				this[index].DrawHandle(graphics, iconContext);
			}
		}


		// Copie tous les objets.
		public void CopyTo(IconObjects dst)
		{
			dst.Clear();
			foreach ( AbstractObject obj in this.objects )
			{
				AbstractObject newObject = null;
				if ( !obj.DuplicateObject(ref newObject) )  continue;
				dst.Add(newObject);
			}
		}


		// Sauve tous les objets.
		public bool Write(string filename)
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(IconObjects));
				TextWriter writer = new StreamWriter(filename);
				serializer.Serialize(writer, this);
				writer.Close();
			}
			catch ( System.Exception )
			{
				return false;
			}
			return true;
		}

		// Lit tous les objets.
		public bool Read(string filename)
		{
			try
			{
				using (FileStream fs = new FileStream(filename, FileMode.Open))
				{
					return this.Read(fs);
				}
			}
			catch ( System.Exception )
			{
				return false;
			}
		}

		public bool Read(System.IO.Stream stream)
		{
			try
			{
				XmlSerializer serializer = new XmlSerializer(typeof(IconObjects));
				//serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
				//serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);
				
				IconObjects obj;
				obj = (IconObjects)serializer.Deserialize(stream);

				this.objects.Clear();
				foreach ( AbstractObject ob in obj.Objects )
				{
					this.objects.Add(ob);
				}
			}
			catch ( System.Exception )
			{
				return false;
			}
			return true;
		}


		[XmlAttribute]
		protected Drawing.Size					size = new Drawing.Size(20, 20);
		protected Drawing.Point					origin = new Drawing.Point(0, 0);
		protected System.Collections.ArrayList	objects = new System.Collections.ArrayList();
	}
}
