//	Copyright � 2004-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Pdf
{
	using CultureInfo = System.Globalization.CultureInfo;

	/// <summary>
	/// La classe Writer g�re la cr�ation du fichier PDF.
	/// Les objets PDF sont nomm�s par des objectName, qui seront remplac�s par
	/// des num�ros dans le fichier PDF. Supposons le fichier suivant:
	/// 
	/// << /Resources 6 0 R >>  % r�f�rence � un objet qui n'existe pas encore
	/// ...
	/// 6 0 obj  % d�finition de l'objet
	/// << /ProcSet [/PDF /Text] >>
	/// endobj
	/// 
	/// Pour g�n�rer ce fichier, il suffit d'�crire:
	/// 
	/// writer.WriteString("<< /Resources ");
	/// writer.WriteObjectRef("Tralala");  // r�f�rence � un objet qui n'existe pas encore
	/// writer.WriteString(">>");
	/// ...
	/// writer.WriteObjectDef("Tralala");  // d�finition de l'objet
	/// writer.WriteString("<< /ProcSet [/PDF /Text] >> endobj");
	/// </summary>
	public sealed class Writer : System.IDisposable
	{
		public Writer(string filename)
		{
			//	Constructeur qui re�oit le nom du fichier.
			//	En fait, le fichier n'est �crit qu'au moment du Flush().
			//	Il n'est pas n�cessaire de se soucier du "%PDF-1.4" en d�but de fichier,
			//	ni des tables "xref..startxref..%%EOF" en fin de fichier.
			this.filename = filename;
			this.parts = new List<Part> ();
			this.dictionary = new Dictionary<string, Object> ();
			this.objectNextId = 1;  // premier identificateur d'objet
			this.streamIO = null;  // fichier pas encore ouvert
		}

		public void WriteObjectDef(string objectName)
		{
			//	Ecrit une d�finition d'objet sous la forme "n 0 obj".
			//	L'objet racine doit �tre nomm� "Root" et �tre le premier d�fini et r�f�renc�.
			this.WriteObject(objectName, " 0 obj ", "D");  // d�finition
		}

		public void WriteObjectRef(string objectName)
		{
			//	Ecrit une r�f�rence � un objet sous la forme "n 0 R".
			this.WriteObject(objectName, " 0 R ", "R");  // r�f�rence
		}

		private void WriteObject(string objectName, string ending, string type)
		{
			//	Ecrit une d�finition ou une r�f�rence d'objet.
			if ( !this.dictionary.ContainsKey(objectName) )
			{
				Object obj = new Object(this.objectNextId++, 0);
				this.dictionary.Add(objectName, obj);
			}

			if ( type == "D" )  // d�finition ?
			{
				//	On v�rifie qu'un objet n'est pas d�fini 2 fois:
				Object obj = this.dictionary[objectName];
				System.Diagnostics.Debug.Assert(obj != null);
				System.Diagnostics.Debug.Assert(!obj.Defined, "PDF.Writer: Attempt to redefine a object");
				System.Diagnostics.Debug.Assert(obj.Id == 1 || objectName != "Root", "PDF.Writer: Root objet must have number 1");
				obj.Defined = true;
			}

			this.parts.Add (new Part (type, objectName));
			this.WriteString (ending);
		}

		private int GetObjectId(string objectName)
		{
			Object obj = this.dictionary[objectName];
			return obj.Id;
		}

		public void WriteString(StringBuffer buffer)
		{
			if (buffer.InMemory)
			{
				this.WriteString (buffer.ToString ());
			}
			else
			{
				var stream = buffer.GetStream ();
				this.WriteStream (stream);
				buffer.CloseStream (stream);
			}

			if (buffer.EndsWithWhitespace == false)
			{
				this.WriteString (" ");
			}
		}

		public void WriteLine(string line)
		{
			//	Ecrit une string suivie d'une fin de ligne.
			line += "\r\n";
			this.WriteString(line);
		}

		public void WriteString(string text)
		{
			//	Ecrit juste une string telle quelle.
			this.parts.Add (new Part ("F", text));  // texte fixe
		}

		public void Flush()
		{
			//	Ecrit tout ce qui est possible dans le fichier sur disque. On peut appeler Flush
			//	autant de fois qu'on veut, pour �crire les donn�es dans le fichier au fur et � mesure,
			//	afin d'utiliser le moins possible de m�moire.
			//	Les objectName sont remplac�s par des num�ros.
			if (this.streamIO == null)  // fichier pas encore ouvert ?
			{
				this.FileOpen(this.filename);
				this.FileWriteLine("%PDF-1.4");
			}

			//	Ecrit toutes les parties fixes ou variables.
			foreach (Part part in this.parts)
			{
				switch (part.Type)
				{
					case "F":	// texte fixe ?
						this.FileWriteString (part.Text);
						break;

					case "D":	// d�finition d'un objet ?
						{
							Object obj = this.dictionary[part.Text];
							obj.Offset = this.streamOffset;
							this.FileWriteString (Writer.ToString (obj.Id));
						}
						break;

					case "R":	// r�f�rence � un objet ?
						{
							Object obj = this.dictionary[part.Text];
							this.FileWriteString (Writer.ToString (obj.Id));
						}
						break;

					default:
						throw new System.NotSupportedException (string.Format ("Part type {0} not supported here", part.Type));
				}
			}

			this.parts.Clear();  // lib�re les donn�es �crites, afin d'utiliser le moins possible de m�moire
		}

		public void Finish()
		{
			//	Ecrit l'objet xref final.
			//	Les tables "xref..startxref..%%EOF" en fin de fichier sont cr��es.
			System.Diagnostics.Debug.Assert(this.streamIO != null);
			int startXref = this.streamOffset;
			this.FileWriteLine(string.Format(CultureInfo.InvariantCulture, "xref 0 {0}", Writer.ToString(this.dictionary.Count+1)));
			this.FileWriteLine("0000000000 65535 f");
			for ( int i=0 ; i<this.dictionary.Count ; i++ )
			{
				Object obj = this.DictionarySearch(i+1);
				System.Diagnostics.Debug.Assert(obj != null);
				System.Diagnostics.Debug.Assert(obj.Defined, "PDF.Writer: Object never defined");
				this.FileWriteLine(string.Format(CultureInfo.InvariantCulture, "{0} 00000 n", Writer.ToStringD10(obj.Offset)));
			}
			this.FileWriteLine(string.Format (CultureInfo.InvariantCulture, "trailer << /Size {0} /Root 1 0 R /Info {1} 0 R >>", Writer.ToString(this.dictionary.Count+1), this.GetObjectId("Info")));
			this.FileWriteLine("startxref");
			this.FileWriteLine(string.Format(CultureInfo.InvariantCulture, "{0}", startXref));
			this.FileWriteLine("%%EOF");

			this.FileClose();

			this.dictionary.Clear();
		}



		#region IDisposable Members

		public void Dispose()
		{
			this.FileClose ();
			
			this.parts.Clear ();
			this.dictionary.Clear ();
		}

		#endregion


		public void WriteHugeString(string text)
		{
			this.Flush ();
			this.FileWriteString (text);
		}
		
		public void WriteStream(System.IO.Stream stream)
		{
			this.Flush ();
			this.FileWriteStream (stream);
		}


		private Object DictionarySearch(int id)
		{
			//	Cherche un objet dans le dictionnaire d'apr�s son identificateur.
			foreach ( Object obj in this.dictionary.Values )
			{
				if ( obj.Id == id )  return obj;
			}
			return null;
		}



		private void FileOpen(string filename)
		{
			//	Ouvre le fichier PDF.
			this.streamIO = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew);
			this.streamOffset = 0;
		}

		private void FileWriteLine(string line)
		{
			//	Ecrit une string suivie d'une fin de ligne.
			line += "\r\n";
			this.FileWriteString(line);
		}

		private void FileWriteString(string text)
		{
			//	Ecrit juste une string telle quelle.
			System.Text.Encoding e = System.Text.Encoding.Default;
			byte[] buffer = e.GetBytes (text);
			this.streamIO.Write (buffer, 0, buffer.Length);
			this.streamOffset += buffer.Length;
		}

		private void FileWriteStream(System.IO.Stream stream)
		{
			byte[] buffer = new byte[64*1024];

			while (true)
			{
				int count = stream.Read (buffer, 0, buffer.Length);

				if (count == 0)
				{
					break;
				}

				this.streamIO.Write (buffer, 0, count);
				this.streamOffset += count;
			}
		}

		private void FileClose()
		{
			//	Ferme le fichier PDF.
			if (this.streamIO != null)
			{
				this.streamIO.Close ();
				this.streamIO = null;
			}
		}


		private static string ToString(int value)
		{
			//	Conversion d'un entier en cha�ne.
			return value.ToString(CultureInfo.InvariantCulture);
		}

		private static string ToStringD10(int value)
		{
			//	Conversion d'un entier en cha�ne.
			return value.ToString("D10", CultureInfo.InvariantCulture);
		}


		//	Objet PDF.
		class Object
		{
			public Object(int id, int offset)
			{
				this.Id     = id;
				this.Offset = offset;
			}

			public int		Id;			// identificateur unique 1..n
			public int		Offset;		// offset de la d�finition dans le fichier
			public bool		Defined;	// true -> objet d�fini
		}


		struct Part
		{
			public Part(string type, string text)
			{
				this.Type = type;
				this.Text = text;
			}

			public readonly string Type;
			public readonly string Text;
		}

		private readonly string					filename;
		private readonly List<Part>				parts;
		private readonly Dictionary<string, Object>	dictionary;
		private int								objectNextId;
		private System.IO.FileStream			streamIO;
		private int								streamOffset;
	}
}
