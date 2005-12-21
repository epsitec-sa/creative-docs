using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document.PDF
{
	/// <summary>
	/// La classe Writer gère la création du fichier PDF.
	/// Les objets PDF sont nommés par des objectName, qui seront remplacés par
	/// des numéros dans le fichier PDF. Supposons le fichier suivant:
	/// 
	/// << /Resources 6 0 R >>  % référence à un objet qui n'existe pas encore
	/// ...
	/// 6 0 obj  % définition de l'objet
	/// << /ProcSet [/PDF /Text] >>
	/// endobj
	/// 
	/// Pour générer ce fichier, il suffit d'écrire:
	/// 
	/// writer.WriteString("<< /Resources ");
	/// writer.WriteObjectRef("Tralala");  // référence à un objet qui n'existe pas encore
	/// writer.WriteString(">>");
	/// ...
	/// writer.WriteObjectDef("Tralala");  // définition de l'objet
	/// writer.WriteString("<< /ProcSet [/PDF /Text] >> endobj");
	/// </summary>
	public class Writer
	{
		public Writer(string filename)
		{
			//	Constructeur qui reçoit le nom du fichier.
			//	En fait, le fichier n'est écrit qu'au moment du Flush().
			//	Il n'est pas nécessaire de se soucier du "%PDF-1.4" en début de fichier,
			//	ni des tables "xref..startxref..%%EOF" en fin de fichier.
			this.filename = filename;
			this.parts = new System.Collections.ArrayList();
			this.dictionary = new System.Collections.Hashtable();
			this.objectNextId = 1;  // premier identificateur d'objet
			this.flushed = false;
		}

		public void WriteObjectDef(string objectName)
		{
			//	Ecrit une définition d'objet sous la forme "n 0 obj".
			//	L'objet racine doit être nommé "Root" et être le premier défini et référencé.
			this.WriteObject(objectName, " 0 obj ", "D");  // définition
		}

		public void WriteObjectRef(string objectName)
		{
			//	Ecrit une référence à un objet sous la forme "n 0 R".
			this.WriteObject(objectName, " 0 R ", "R");  // référence
		}

		protected void WriteObject(string objectName, string ending, string type)
		{
			//	Ecrit une définition ou une référence d'objet.
			System.Diagnostics.Debug.Assert(!this.flushed);

			if ( !this.dictionary.ContainsKey(objectName) )
			{
				Object obj = new Object(this.objectNextId++, 0);
				this.dictionary.Add(objectName, obj);
			}

			if ( type == "D" )  // définition ?
			{
				//	On vérifie qu'un objet n'est pas défini 2 fois:
				Object obj = this.dictionary[objectName] as Object;
				System.Diagnostics.Debug.Assert(obj != null);
				System.Diagnostics.Debug.Assert(!obj.Defined, "PDF.Writer: Attempt to redefine a object");
				System.Diagnostics.Debug.Assert(obj.Id == 1 || objectName != "Root", "PDF.Writer: Root objet must have number 1");
				obj.Defined = true;
			}

			this.parts.Add(string.Format("{0}{1}", type, objectName));
			this.WriteString(ending);
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
			System.Diagnostics.Debug.Assert(!this.flushed);
			this.parts.Add(string.Format("F{0}", text));  // texte fixe
		}

		public void Flush()
		{
			//	Ecrit effectivement le fichier.
			//	Les objectName sont remplacés par des numéros.
			//	Les tables "xref..startxref..%%EOF" en fin de fichier sont créées.
			System.Diagnostics.Debug.Assert(!this.flushed);

			this.FileOpen(this.filename);
			this.FileWriteLine("%PDF-1.4");

			//	Ecrit toutes les parties fixes ou variables.
			foreach ( string part in this.parts )
			{
				if ( part[0] == 'F' )  // texte fixe ?
				{
					this.FileWriteString(part.Substring(1));
				}

				if ( part[0] == 'D' )  // définition d'un objet ?
				{
					Object obj = this.dictionary[part.Substring(1)] as Object;
					obj.Offset = this.streamOffset;
					this.FileWriteString(Writer.ToString(obj.Id));
				}

				if ( part[0] == 'R' )  // référence à un objet ?
				{
					Object obj = this.dictionary[part.Substring(1)] as Object;
					this.FileWriteString(Writer.ToString(obj.Id));
				}
			}

			//	Ecrit l'objet xref final.
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

		protected Object DictionarySearch(int id)
		{
			//	Cherche un objet dans le dictionnaire d'après son identificateur.
			foreach ( Object obj in this.dictionary.Values )
			{
				if ( obj.Id == id )  return obj;
			}
			return null;
		}



		protected void FileOpen(string filename)
		{
			//	Ouvre le fichier PDF.
			this.streamIO = new System.IO.FileStream(filename, System.IO.FileMode.CreateNew);
			this.streamOffset = 0;
		}

		protected void FileWriteLine(string line)
		{
			//	Ecrit une string suivie d'une fin de ligne.
			line += "\r\n";
			this.FileWriteString(line);
		}

		protected void FileWriteString(string text)
		{
			//	Ecrit juste une string telle quelle.
			System.Text.Encoding e = System.Text.Encoding.GetEncoding(1252);
			byte[] array = e.GetBytes(text);
			this.streamIO.Write(array, 0, array.Length);
			this.streamOffset += array.Length;
		}

		protected void FileClose()
		{
			//	Ferme le fichier PDF.
			this.streamIO.Close();
		}


		protected static string ToString(int value)
		{
			//	Conversion d'un entier en chaîne.
			return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
		}

		protected static string ToStringD10(int value)
		{
			//	Conversion d'un entier en chaîne.
			return value.ToString("D10", System.Globalization.CultureInfo.InvariantCulture);
		}


		//	Objet PDF.
		protected class Object
		{
			public Object(int id, int offset)
			{
				this.Id     = id;
				this.Offset = offset;
			}

			public int		Id;			// identificateur unique 1..n
			public int		Offset;		// offset de la définition dans le fichier
			public bool		Defined;	// true -> objet défini
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
