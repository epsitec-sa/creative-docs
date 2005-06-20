namespace Epsitec.Common.Document.PDF
{
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
	public class Writer
	{
		// Constructeur qui re�oit le nom du fichier.
		// En fait, le fichier n'est �crit qu'au moment du Flush().
		// Il n'est pas n�cessaire de se soucier du "%PDF-1.4" en d�but de fichier,
		// ni des tables "xref..startxref..%%EOF" en fin de fichier.
		public Writer(string filename)
		{
			this.filename = filename;
			this.parts = new System.Collections.ArrayList();
			this.dictionary = new System.Collections.Hashtable();
			this.objectNextId = 1;  // premier identificateur d'objet
			this.flushed = false;
		}

		// Ecrit une d�finition d'objet sous la forme "n 0 obj".
		// L'objet racine doit �tre nomm� "Root" et �tre le premier d�fini et r�f�renc�.
		public void WriteObjectDef(string objectName)
		{
			this.WriteObject(objectName, " 0 obj ", "D");  // d�finition
		}

		// Ecrit une r�f�rence � un objet sous la forme "n 0 R".
		public void WriteObjectRef(string objectName)
		{
			this.WriteObject(objectName, " 0 R ", "R");  // r�f�rence
		}

		// Ecrit une d�finition ou une r�f�rence d'objet.
		protected void WriteObject(string objectName, string ending, string type)
		{
			System.Diagnostics.Debug.Assert(!this.flushed);

			if ( !this.dictionary.ContainsKey(objectName) )
			{
				Object obj = new Object(this.objectNextId++, 0);
				this.dictionary.Add(objectName, obj);
			}

			if ( type == "D" )  // d�finition ?
			{
				// On v�rifie qu'un objet n'est pas d�fini 2 fois:
				Object obj = this.dictionary[objectName] as Object;
				System.Diagnostics.Debug.Assert(obj != null);
				System.Diagnostics.Debug.Assert(!obj.Defined, "PDF.Writer: Attempt to redefine a object");
				System.Diagnostics.Debug.Assert(obj.Id == 1 || objectName != "Root", "PDF.Writer: Root objet must have number 1");
				obj.Defined = true;
			}

			this.parts.Add(string.Format("{0}{1}", type, objectName));
			this.WriteString(ending);
		}

		// Ecrit une string suivie d'une fin de ligne.
		public void WriteLine(string line)
		{
			line += "\r\n";
			this.WriteString(line);
		}

		// Ecrit juste une string telle quelle.
		public void WriteString(string text)
		{
			System.Diagnostics.Debug.Assert(!this.flushed);
			this.parts.Add(string.Format("F{0}", text));  // texte fixe
		}

		// Ecrit effectivement le fichier.
		// Les objectName sont remplac�s par des num�ros.
		// Les tables "xref..startxref..%%EOF" en fin de fichier sont cr��es.
		public void Flush()
		{
			System.Diagnostics.Debug.Assert(!this.flushed);

			this.FileOpen(this.filename);
			this.FileWriteLine("%PDF-1.4");

			// Ecrit toutes les parties fixes ou variables.
			foreach ( string part in this.parts )
			{
				if ( part[0] == 'F' )  // texte fixe ?
				{
					this.FileWriteString(part.Substring(1));
				}

				if ( part[0] == 'D' )  // d�finition d'un objet ?
				{
					Object obj = this.dictionary[part.Substring(1)] as Object;
					obj.Offset = this.streamOffset;
					this.FileWriteString(Writer.ToString(obj.Id));
				}

				if ( part[0] == 'R' )  // r�f�rence � un objet ?
				{
					Object obj = this.dictionary[part.Substring(1)] as Object;
					this.FileWriteString(Writer.ToString(obj.Id));
				}
			}

			// Ecrit l'objet xref final.
			int startXref = this.streamOffset;
			this.FileWriteLine(string.Format("xref 0 {0}", Writer.ToString(this.dictionary.Count+1)));
			this.FileWriteLine("0000000000 65535 f");
			for ( int i=0 ; i<this.dictionary.Count ; i++ )
			{
				Object obj = this.DictionarySearch(i+1);
				System.Diagnostics.Debug.Assert(obj != null);
				System.Diagnostics.Debug.Assert(obj.Defined, "PDF.Writer: Object never defined");
				this.FileWriteLine(string.Format("{0} 00000 n", PDF.Writer.ToStringD10(obj.Offset)));
			}
			this.FileWriteLine(string.Format("trailer << /Size {0} /Root 1 0 R >>", Writer.ToString(this.dictionary.Count+1)));
			this.FileWriteLine("startxref");
			this.FileWriteLine(string.Format("{0}", startXref));
			this.FileWriteLine("%%EOF");

			this.FileClose();

			this.parts.Clear();
			this.dictionary.Clear();
			this.flushed = true;
		}

		// Cherche un objet dans le dictionnaire d'apr�s son identificateur.
		protected Object DictionarySearch(int id)
		{
			foreach ( Object obj in this.dictionary.Values )
			{
				if ( obj.Id == id )  return obj;
			}
			return null;
		}



		// Ouvre le fichier PDF.
		protected void FileOpen(string filename)
		{
			this.streamIO = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew);
			this.streamOffset = 0;
		}

		// Ecrit une string suivie d'une fin de ligne.
		protected void FileWriteLine(string line)
		{
			line += "\r\n";
			this.FileWriteString(line);
		}

		// Ecrit juste une string telle quelle.
		protected void FileWriteString(string text)
		{
			System.Text.Encoding e = System.Text.Encoding.GetEncoding(1252);
			byte[] array = e.GetBytes(text);
			this.streamIO.Write(array, 0, array.Length);
			this.streamOffset += array.Length;
		}

		// Ferme le fichier PDF.
		protected void FileClose()
		{
			this.streamIO.Close();
		}


		// Conversion d'un entier en cha�ne.
		protected static string ToString(int value)
		{
			return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		// Conversion d'un entier en cha�ne.
		protected static string ToStringD10(int value)
		{
			return value.ToString("D10", System.Globalization.CultureInfo.InvariantCulture);
		}


		// Objet PDF.
		protected class Object
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


		protected string						filename;
		protected bool							flushed;
		protected System.Collections.ArrayList	parts;
		protected System.Collections.Hashtable	dictionary;
		protected int							objectNextId;
		protected System.IO.FileStream			streamIO;
		protected int							streamOffset;
	}
}
