//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.Support
{
	using System.Collections;
	
	/// <summary>
	/// La classe ResourceBundle donne accès aux données d'une ressource XML sous
	/// une forme simplifiée.
	/// </summary>
	public class ResourceBundle
	{
		public ResourceBundle()
		{
		}
		
		
		public bool						IsEmpty
		{
			get { return this.CountFields == 0; }
		}
		
		public int						CountFields
		{
			get { return (this.fields == null) ? 0 : this.fields.Count; }
		}
		
		public string[]					FieldNames
		{
			get
			{
				int n = this.CountFields;
				string[] names = new string[n];
				this.fields.Keys.CopyTo (names, 0);
				return names;
			}
		}
		
		public string					this[string name]
		{
			get
			{
				if (this.fields != null)
				{
					return this.fields[name] as string;
				}
				return null;
			}
		}
		
		
		public void Compile(System.IO.Stream stream)
		{
			//	La compilation des données part du principe que le bundle XML est "well formed",
			//	c'est-à-dire qu'il comprend un seul bloc à la racine (<bundle>..</bundle>), et
			//	que son contenu est valide (l'en-tête <?xml ...?> n'est pas requis).
			
			//	Les divers éléments ne peuvent pas contenir d'autres éléments à leur tour. Ceci signifie
			//	que pour stocker du code XML dans un bundle, il faut l'êncapsuler dans une section CDATA
			//	(voir http://www.w3.org/TR/2000/REC-xml-20001006#sec-cdata-sect). Cette encapsulation ne
			//	peut se faire que sur un niveau.
			
			if (stream != null)
			{
				System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader (stream);
				
				if (this.fields == null)
				{
					this.fields = new System.Collections.Hashtable ();
				}
				
				string element_name = null;
				int    reader_depth = 0;
				
				while (reader.Read ())
				{
					switch (reader.NodeType)
					{
						case System.Xml.XmlNodeType.Element:
							if (reader_depth == 0)
							{
								switch (reader.Name)
								{
									case "bundle":
										break;
									default:
										throw new ResourceException (string.Format ("Bundle does not start with root <bundle>, but <{0}>", reader.Name));
								}
							}
							else if (reader_depth == 1)
							{
								System.Diagnostics.Debug.Assert (element_name == null);
								element_name = reader.Name;
							}
							else
							{
								throw new ResourceException (string.Format ("Malformed XML bundle at {0} in {1}", reader.Name, element_name));
							}
							
							reader_depth++;
							break;
						
						case System.Xml.XmlNodeType.EndElement:
							
							System.Diagnostics.Debug.Assert (reader_depth > 0);
							
							reader_depth--;
							
							if (reader_depth == 1)
							{
								//	Les assertions qui suivent sont redondantes, car en principe, XmlTextReader ne doit
								//	pas laisser passer ce genre d'erreurs.
								
								System.Diagnostics.Debug.Assert (element_name != null);
								System.Diagnostics.Debug.Assert (element_name == reader.Name);
								element_name = null;
							}
							break;
						
						case System.Xml.XmlNodeType.Text:
						case System.Xml.XmlNodeType.CDATA:
							System.Diagnostics.Debug.Assert (reader_depth == 2);
							System.Diagnostics.Debug.Assert (element_name != null);
							this.fields[element_name] = reader.Value;
							break;
					}
				}
				
				reader.Close ();
			}
		}
		
		
		protected Hashtable				fields;
	}
}
