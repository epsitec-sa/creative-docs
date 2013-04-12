//	Copyright © 2004-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.IO;

namespace Epsitec.Common.Pdf.Engine
{
	using CultureInfo = System.Globalization.CultureInfo;

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
	public sealed class Writer
	{
		public Writer(Stream stream)
		{
			//	Constructeur qui reçoit le stream dans lequel écrire le fichier pdf.
			//	En fait, le stream n'est écrit qu'au moment du Flush().
			//	Il n'est pas nécessaire de se soucier du "%PDF-1.4" en début de fichier,
			//	ni des tables "xref..startxref..%%EOF" en fin de fichier.
			this.parts = new List<Part> ();
			this.dictionary = new Dictionary<string, Object> ();
			this.objectNextId = 1;  // premier identificateur d'objet
			this.streamIO = stream;
			this.streamOffset = 0;

			this.StreamWriteLine ("%PDF-1.4");
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

		private void WriteObject(string objectName, string ending, string type)
		{
			//	Ecrit une définition ou une référence d'objet.
			if ( !this.dictionary.ContainsKey(objectName) )
			{
				Object obj = new Object(this.objectNextId++, 0);
				this.dictionary.Add(objectName, obj);
			}

			if ( type == "D" )  // définition ?
			{
				//	On vérifie qu'un objet n'est pas défini 2 fois:
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
			//	Les objectName sont remplacés par des numéros.

			//	Ecrit toutes les parties fixes ou variables.
			foreach (Part part in this.parts)
			{
				switch (part.Type)
				{
					case "F":	// texte fixe ?
						this.StreamWriteString (part.Text);
						break;

					case "D":	// définition d'un objet ?
						{
							Object obj = this.dictionary[part.Text];
							obj.Offset = this.streamOffset;
							this.StreamWriteString (Writer.ToString (obj.Id));
						}
						break;

					case "R":	// référence à un objet ?
						{
							Object obj = this.dictionary[part.Text];
							this.StreamWriteString (Writer.ToString (obj.Id));
						}
						break;

					default:
						throw new System.NotSupportedException (string.Format ("Part type {0} not supported here", part.Type));
				}
			}

			this.parts.Clear();  // libère les données écrites, afin d'utiliser le moins possible de mémoire
		}

		public void Finish()
		{
			//	Ecrit l'objet xref final.
			//	Les tables "xref..startxref..%%EOF" en fin de fichier sont créées.
			int startXref = this.streamOffset;
			this.StreamWriteLine(string.Format(CultureInfo.InvariantCulture, "xref 0 {0}", Writer.ToString(this.dictionary.Count+1)));
			this.StreamWriteLine("0000000000 65535 f");
			for ( int i=0 ; i<this.dictionary.Count ; i++ )
			{
				Object obj = this.DictionarySearch(i+1);
				System.Diagnostics.Debug.Assert(obj != null);
				System.Diagnostics.Debug.Assert(obj.Defined, "PDF.Writer: Object never defined");
				this.StreamWriteLine(string.Format(CultureInfo.InvariantCulture, "{0} 00000 n", Writer.ToStringD10(obj.Offset)));
			}
			this.StreamWriteLine(string.Format (CultureInfo.InvariantCulture, "trailer << /Size {0} /Root 1 0 R /Info {1} 0 R >>", Writer.ToString(this.dictionary.Count+1), this.GetObjectId("Info")));
			this.StreamWriteLine("startxref");
			this.StreamWriteLine(string.Format(CultureInfo.InvariantCulture, "{0}", startXref));
			this.StreamWriteLine("%%EOF");

			this.streamIO.Flush ();

			this.dictionary.Clear();
		}


		public void WriteHugeString(string text)
		{
			this.Flush ();
			this.StreamWriteString (text);
		}
		
		public void WriteStream(System.IO.Stream stream)
		{
			this.Flush ();
			this.StreamWriteStream (stream);
		}


		private Object DictionarySearch(int id)
		{
			//	Cherche un objet dans le dictionnaire d'après son identificateur.
			foreach ( Object obj in this.dictionary.Values )
			{
				if ( obj.Id == id )  return obj;
			}
			return null;
		}

		private void StreamWriteLine(string line)
		{
			//	Ecrit une string suivie d'une fin de ligne.
			line += "\r\n";
			this.StreamWriteString(line);
		}

		private void StreamWriteString(string text)
		{
			//	Ecrit juste une string telle quelle.
			System.Text.Encoding e = System.Text.Encoding.Default;
			byte[] buffer = e.GetBytes (text);
			this.streamIO.Write (buffer, 0, buffer.Length);
			this.streamOffset += buffer.Length;
		}

		private void StreamWriteStream(System.IO.Stream stream)
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


		private static string ToString(int value)
		{
			//	Conversion d'un entier en chaîne.
			return value.ToString(CultureInfo.InvariantCulture);
		}

		private static string ToStringD10(int value)
		{
			//	Conversion d'un entier en chaîne.
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
			public int		Offset;		// offset de la définition dans le fichier
			public bool		Defined;	// true -> objet défini
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

		private readonly List<Part>					parts;
		private readonly Dictionary<string, Object>	dictionary;
		private int									objectNextId;
		private Stream								streamIO;
		private int									streamOffset;
	}
}
