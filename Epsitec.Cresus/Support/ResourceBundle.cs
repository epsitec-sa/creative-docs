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
		
		public string					this[string field]
		{
			get
			{
				if (this.fields != null)
				{
					return this.fields[field] as string;
				}
				return null;
			}
		}
		
		
		public bool Contains(string field)
		{
			if (this.fields != null)
			{
				return this.fields.Contains (field);
			}
			
			return false;
		}
		
		
		public void Compile(System.IO.Stream stream)
		{
			this.Compile (stream, null, ResourceLevel.Merged);
		}
		
		public void Compile(System.IO.Stream stream, string default_prefix, ResourceLevel level)
		{
			//	La compilation des données part du principe que le bundle XML est "well formed",
			//	c'est-à-dire qu'il comprend un seul bloc à la racine (<bundle>..</bundle>), et
			//	que son contenu est valide (l'en-tête <?xml ...?> n'est pas requis).
			
			//	Les divers éléments ne peuvent pas contenir d'autres éléments à leur tour. Ceci signifie
			//	que pour stocker du code XML dans un bundle, il faut l'encapsuler dans une section CDATA
			//	(voir http://www.w3.org/TR/2000/REC-xml-20001006#sec-cdata-sect). Cette encapsulation ne
			//	peut se faire que sur un niveau.
			
			if (stream != null)
			{
				System.Xml.XmlTextReader reader = new System.Xml.XmlTextReader (stream);
				
				if (this.fields == null)
				{
					this.fields = new System.Collections.Hashtable ();
				}
				
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				string element_name = null;
				int    reader_depth = 0;
				
				while (reader.Read ())
				{
					string name  = reader.Name;
					
					switch (reader.NodeType)
					{
						case System.Xml.XmlNodeType.Element:
							if (name == "ref")
							{
								//	On vient de trouver une référence à une autre ressource. En fonction du
								//	niveau auquel on se trouve, cette référence va soit réaliser une fusion
								//	avec un autre bundle (niveau 1), soit simplement insérer les données dans
								//	le buffer courant (niveau 2).
								
								string target = reader.GetAttribute ("t");
								
								if (target == null)
								{
									throw new ResourceException ("Reference has no target");
								}
								
								if (Resources.ExtractPrefix (target) == null)
								{
									if (default_prefix == null)
									{
										throw new ResourceException (string.Format ("No default prefix specified, target '{0}' cannot be resolved", target));
									}
									
									target = default_prefix + ":" + target;
								}
								
								if (reader_depth == 1)
								{
									//	Gère la référence à un bundle externe : charge le bundle et transfère
									//	son contenu dans notre propre bundle.
									
									ResourceBundle bundle = Resources.GetBundle (target, level);
									
									if (bundle == null)
									{
										throw new ResourceException (string.Format ("Reference to target '{0}' cannot be resolved", target));
									}
									
									this.Merge (bundle);
								}
								else if (reader_depth == 2)
								{
									//	Gère la référence à un champ d'un bundle externe : charge le bundle, puis
									//	insère la valeur de la cible dans notre buffer interne.
									
									//	La cible est composée de deux parties: un nom de bundle et un nom de champ;
									//	le séparateur est le "#", donc on a target = "bundle#field".
									
									int pos = target.IndexOf ("#");
									
									if ((pos < 1) || (pos+1 == target.Length))
									{
										throw new ResourceException (string.Format ("Target '{0}' is not of the form 'bundle#field'", target));
									}
									
									string target_bundle = target.Substring (0, pos);
									string target_field  = target.Substring (pos+1);
									
									ResourceBundle bundle = Resources.GetBundle (target_bundle, level);
									
									if (bundle == null)
									{
										throw new ResourceException (string.Format ("Reference to target '{0}' cannot be resolved", target_bundle));
									}
									
									if (!bundle.Contains (target_field))
									{
										throw new ResourceException (string.Format ("Target bundle '{0}' does not contain field '{1}'", target_bundle, target_field));
									}

									buffer.Append (bundle[target_field]);
								}
								else
								{
									throw new ResourceException (string.Format ("Illegal reference to '{0}' at level {1}", target, reader_depth));
								}
								
								continue;
							}
							
							if (reader_depth == 0)
							{
								switch (name)
								{
									case "bundle":
										break;
									default:
										throw new ResourceException (string.Format ("Bundle does not start with root <bundle>, but <{0}>", name));
								}
							}
							else if (reader_depth == 1)
							{
								System.Diagnostics.Debug.Assert (element_name == null);
								element_name = name;
							}
							else
							{
								throw new ResourceException (string.Format ("Malformed XML bundle at {0} in {1}", name, element_name));
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
								System.Diagnostics.Debug.Assert (element_name == name);
								
								this.fields[element_name] = buffer.ToString ();
								
								buffer       = new System.Text.StringBuilder ();
								element_name = null;
							}
							break;
						
						case System.Xml.XmlNodeType.Text:
						case System.Xml.XmlNodeType.CDATA:
							System.Diagnostics.Debug.Assert (reader_depth == 2);
							System.Diagnostics.Debug.Assert (element_name != null);
							buffer.Append (reader.Value);
							break;
					}
				}
				
				reader.Close ();
			}
		}
		
		public void Merge(ResourceBundle bundle)
		{
			if ((bundle == null) ||
				(bundle.CountFields == 0))
			{
				return;
			}
			
			if (this.fields == null)
			{
				this.fields = new System.Collections.Hashtable ();
			}
			
			foreach (System.Collections.DictionaryEntry entry in bundle.fields)
			{
				this.fields[entry.Key] = entry.Value;
			}
		}
		
		
		protected Hashtable				fields;
	}
}
