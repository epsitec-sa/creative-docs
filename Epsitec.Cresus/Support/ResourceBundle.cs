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
		
		public object					this[string field]
		{
			get
			{
				if (this.fields != null)
				{
					return this.fields[field];
				}
				return null;
			}
		}
		
		
		public ResourceFieldType GetFieldType(string field)
		{
			object data = this[field];
			
			if (data == null)
			{
				return ResourceFieldType.None;
			}
			if (data is string)
			{
				return ResourceFieldType.String;
			}
			if (data is ResourceBundle)
			{
				return ResourceFieldType.Bundle;
			}
			
			throw new ResourceException ("Invalid field type in bundle");
		}
		
		public string GetFieldString(string field)
		{
			return this[field] as string;
		}
		
		public ResourceBundle GetFieldBundle(string field)
		{
			return this[field] as ResourceBundle;
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
			this.Compile (stream, null, ResourceLevel.Merged, 0);
		}
		
		public void Compile(System.IO.Stream stream, string default_prefix, ResourceLevel level)
		{
			this.Compile (stream, default_prefix, level, 0);
		}
		
		public void Compile(System.IO.Stream stream, string default_prefix, ResourceLevel level, int recursion)
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
				
				try
				{
					this.ParseXml (reader, default_prefix, level, 0, recursion);
				}
				
				finally
				{
					reader.Close ();
					stream.Close ();
				}
			}
		}
		
		
		protected void ParseXml(System.Xml.XmlTextReader reader, string default_prefix, ResourceLevel level, int reader_depth, int recursion)
		{
			if (recursion > ResourceBundle.max_recursion)
			{
				throw new ResourceException ("Bundle is too complex, giving up");
			}
			
			if (this.fields == null)
			{
				this.fields = new System.Collections.Hashtable ();
			}
				
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			string element_name = null;
			
			while (reader.Read ())
			{
				string name = reader.Name;
						
				switch (reader.NodeType)
				{
					case System.Xml.XmlNodeType.Element:
						
						//	Un élément peut être soit un élément racine <bundle>, soit un élément signalant
						//	une référence <ref t='target'/>, soit un élément définissant un champ du bundle.
						
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
							
							//	La cible peut être composée de deux parties: un nom de bundle et un nom de champ;
							//	le séparateur est le "#", donc target = "bundle#field" ou target = "bundle".
							
							int pos = target.IndexOf ("#");
							
							string target_bundle = target;
							string target_field  = null;
							
							if (pos >= 0)
							{
								target_bundle = target.Substring (0, pos);
								target_field  = target.Substring (pos+1);
							}
							
							ResourceBundle bundle = Resources.GetBundle (target_bundle, level, recursion+1);
							
							if (bundle == null)
							{
								throw new ResourceException (string.Format ("Reference to target '{0}' cannot be resolved", target));
							}
							
							if (reader_depth == 1)
							{
								System.Diagnostics.Debug.Assert (element_name == null);
								
								//	Gère la référence à un bundle externe : transfère le contenu du
								//	bundle externe dans notre propre bundle.
								
								this.Merge (bundle);
							}
							else if (reader_depth == 2)
							{
								System.Diagnostics.Debug.Assert (element_name != null);
								
								//	Gère la référence à un champ d'un bundle externe. Il faut distinguer ici deux
								//	cas différents:
								//
								//	1) La référence pointe sur un champ de type texte. Il faut alors simplement
								//	   ajouter le texte dans le buffer courant.
								//
								//	2) La référence pointe sur un autre bundle. Il faut alors fusionner le bundle
								//	   avec le bundle courant.
								
								ResourceBundle child_bundle = null;
								
								if (target_field != null)
								{
									if (!bundle.Contains (target_field))
									{
										throw new ResourceException (string.Format ("Target bundle '{0}' does not contain field '{1}'", target_bundle, target_field));
									}
									
									object field_data = bundle[target_field];
									
									if (field_data is string)
									{
										buffer.Append (field_data);
									}
									else
									{
										child_bundle = field_data as ResourceBundle;
									}
								}
								else
								{
									child_bundle = bundle;
								}
								
								if (child_bundle != null)
								{
									this.AddChildBundle (element_name, child_bundle);
								}
							}
							else
							{
								throw new ResourceException (string.Format ("Illegal reference to '{0}' at level {1}", target, reader_depth));
							}
							
							//	On ne change pas la profondeur (reader_depth) ici, car c'est un
							//	 tag <ref ../> qui ne contient aucune donnée :
							
							continue;
						}
						
						
						if (name == "bundle")
						{
							switch (reader_depth)
							{
								case 0:
									//	On vient de trouver le tag bundle de départ. C'est normal.
									break;
								
								case 1:
									//	Quelqu'un a voulu placer un sous-bundle directement dans le bundle.
									//	Ce n'est pas permis, car ce serait un bundle anonyme !
									
									throw new ResourceException ("Sub-bundle in bundle cannot be anonymous");
								
								case 2:
									//	On vient de trouver un sous-bundle dans le bundle courant. Travaille
									//	de manière récursive pour lire ce bundle. Notons au passage que comme
									//	le tag <bundle> a déjà été lu, le parser n'a pas besoin de valider le
									//	bundle comme tel (reader_depth = 1).
									
									ResourceBundle child_bundle = new ResourceBundle ();
									child_bundle.ParseXml (reader, default_prefix, level, 1, recursion+1);
									this.AddChildBundle (element_name, child_bundle);
									
									//	Comme le ParseXml récursif a aussi consommé le </bundle>, on saute à
									//	la suite sans changer le niveau d'imbrication.
									
									continue;
								
								default:
									throw new ResourceException ("Illegal bundle depth");
							}
						}
						else if (reader_depth == 1)
						{
							System.Diagnostics.Debug.Assert (element_name == null);
							element_name = name;
						}
						else
						{
							if (reader_depth == 0)
							{
								throw new ResourceException (string.Format ("Bundle does not start with root <bundle>, but <{0}>", name));
							}
							else
							{
								throw new ResourceException (string.Format ("Malformed XML bundle at {0} in {1}", name, element_name));
							}
						}
						
						reader_depth++;
						break;
					
					case System.Xml.XmlNodeType.EndElement:
						
						System.Diagnostics.Debug.Assert (reader_depth > 0);
						
						reader_depth--;
						
						if (reader_depth == 0)
						{
							return;
						}
						
						//	Les assertions qui suivent sont redondantes, car en principe, XmlTextReader ne doit
						//	pas laisser passer ce genre d'erreurs.
						
						System.Diagnostics.Debug.Assert (element_name != null);
						System.Diagnostics.Debug.Assert (element_name == name);
						
						if (reader_depth == 1)
						{
							if (this.fields.Contains (element_name) && (this.fields[element_name] is ResourceBundle))
							{
								System.Diagnostics.Debug.Assert (buffer.Length == 0);
							}
							else
							{
								this.fields[element_name] = buffer.ToString ();
								buffer.Length = 0;
							}
							
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
		}
		
		protected void AddChildBundle(string element_name, ResourceBundle child_bundle)
		{
			ResourceBundle current_bundle;
			
			if (this.fields.Contains (element_name))
			{
				current_bundle = this.fields[element_name] as ResourceBundle;
			}
			else
			{
				current_bundle = new ResourceBundle ();
				this.fields[element_name] = current_bundle;
			}
			
			System.Diagnostics.Debug.Assert (current_bundle != null);
			
			current_bundle.Merge (child_bundle);
		}
		
		protected void Merge(ResourceBundle bundle)
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
		
		protected const int				max_recursion = 50;
	}
}
