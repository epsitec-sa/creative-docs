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
									case "BUNDLE":
										break;
									default:
										throw new ResourceException (string.Format ("Bundle does not start with root <BUNDLE>, but <{0}>", reader.Name));
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
