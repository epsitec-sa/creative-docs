//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	using System.Collections;
	
	/// <summary>
	/// Implémentation d'un ResourceBundle basé sur une interprétation du source
	/// XML et un stockage de l'information non XML.
	/// </summary>
	public class ResourceBundleXmlReader : ResourceBundle
	{
		public ResourceBundleXmlReader(string name) : base(name)
		{
		}
		
		
		public override int				CountFields
		{
			get { return (this.fields == null) ? 0 : this.fields.Count; }
		}
		
		public override string[]		FieldNames
		{
			get
			{
				int n = this.CountFields;
				string[] names = new string[n];
				this.fields.Keys.CopyTo (names, 0);
				return names;
			}
		}
		
		public override object			this[string field]
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
		
		
		public override bool Contains(string field)
		{
			if (this.fields != null)
			{
				return this.fields.Contains (field);
			}
			
			return false;
		}
		
		public override void Compile(byte[] data, string default_prefix, ResourceLevel level, int recursion)
		{
			//	La compilation des données part du principe que le bundle XML est "well formed",
			//	c'est-à-dire qu'il comprend un seul bloc à la racine (<bundle>..</bundle>), et
			//	que son contenu est valide (l'en-tête <?xml ...?> n'est pas requis).
			
			//	Les divers éléments ne peuvent pas contenir d'autres éléments à leur tour. Ceci signifie
			//	que pour stocker du code XML dans un bundle, il faut l'encapsuler dans une section CDATA
			//	(voir http://www.w3.org/TR/2000/REC-xml-20001006#sec-cdata-sect). Cette encapsulation ne
			//	peut se faire que sur un niveau.
			
			if (data != null)
			{
				System.IO.MemoryStream   stream = new System.IO.MemoryStream (data, false);
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
		
		
		protected string XmlErrorContext(System.Xml.XmlTextReader reader)
		{
			return string.Format ("Line {0}, position {1}", reader.LineNumber, reader.LinePosition);
		}
		
		protected void ParseXml(System.Xml.XmlTextReader reader, string default_prefix, ResourceLevel level, int reader_depth, int recursion)
		{
			//	Analyse un fragment de XML.
			
			if (recursion > ResourceBundle.MaxRecursion)
			{
				throw new ResourceException (string.Format ("Bundle is too complex, giving up. {0}.", this.XmlErrorContext (reader)));
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
						//	une référence <ref target='target'/>, soit un élément commençant une liste, soit
						//	un élément <bundle> commençant un sous-bundle, soit un élément définissant un
						//	champ 'string' du bundle.
						
						if (name == "ref")
						{
							ResourceBundleXmlReader new_bundle;
							RefRecord ref_record = new RefRecord (element_name);
							
							ref_record.ParseBegin (reader, buffer);
							this.ParseXmlRef (reader, default_prefix, level, reader_depth, recursion, element_name, buffer, out new_bundle);
							ref_record.ParseEnd (buffer);
							
							if (new_bundle != null)
							{
								//	Gère la référence à un bundle externe : transfère le contenu du
								//	bundle externe dans notre propre bundle.
								
								this.Merge (new_bundle);
							}
							
							//	On ne change pas la profondeur (reader_depth) ici, car c'est un
							//	tag <ref ../> qui ne contient aucune donnée :
							
							continue;
						}
						
						if (name == "list")
						{
							this.ParseXmlList (reader, default_prefix, level, reader_depth, recursion, ref element_name, buffer);
							
							//	On ne change pas la profondeur (reader_depth) ici, car on a terminé
							//	sur un tag </list>.
							
							continue;
						}
						
						if (name == "bundle")
						{
							ResourceBundleXmlReader child_bundle;
							string local_name;
							
							switch (reader_depth)
							{
								case 0:
									//	On vient de trouver le tag bundle de départ. C'est normal.
									break;
								
								case 1:
									local_name = reader.GetAttribute ("name");
									
									if (local_name == null)
									{
										//	Quelqu'un a voulu placer un sous-bundle directement dans le bundle.
										//	Ce n'est pas permis, car ce serait un bundle anonyme !
										
										throw new ResourceException (string.Format ("Sub-bundle in bundle cannot be anonymous. {0}.", this.XmlErrorContext (reader)));
									}
									else
									{
										child_bundle = new ResourceBundleXmlReader (string.Format ("{0}#{1}", this.name, local_name));
										child_bundle.ParseXml (reader, default_prefix, level, 1, recursion+1);
										this.AddChildBundle (local_name, child_bundle);
									}
									
									//	Comme le ParseXml récursif a aussi consommé le </bundle>, on saute à
									//	la suite sans changer le niveau d'imbrication.
									
									continue;
								
								case 2:
									//	On vient de trouver un sous-bundle dans le bundle courant. Travaille
									//	de manière récursive pour lire ce bundle. Notons au passage que comme
									//	le tag <bundle> a déjà été lu, le parser n'a pas besoin de valider le
									//	bundle comme tel (reader_depth = 1).
									
									System.Diagnostics.Debug.Assert (element_name != null);
									
									child_bundle = new ResourceBundleXmlReader (string.Format ("{0}#{1}", this.name, element_name));
									child_bundle.ParseXml (reader, default_prefix, level, 1, recursion+1);
									this.AddChildBundle (element_name, child_bundle);
									
									//	Comme le ParseXml récursif a aussi consommé le </bundle>, on saute à
									//	la suite sans changer le niveau d'imbrication.
									
									continue;
								
								default:
									throw new ResourceException (string.Format ("Illegal bundle depth. {0}.", this.XmlErrorContext (reader)));
							}
							
							reader_depth++;
							continue;
						}
						
						if (name == "field")
						{
							this.ParseXmlField (reader, reader_depth, out element_name);
							reader_depth++;
							continue;
						}
						
						if (reader_depth == 0)
						{
							throw new ResourceException (string.Format ("Bundle does not start with root <bundle>, but <{0}>. {1}.", name, this.XmlErrorContext (reader)));
						}
						else
						{
							throw new ResourceException (string.Format ("Malformed XML bundle, unknown tag '{0}' found. {1}.", name, this.XmlErrorContext (reader)));
						}
					
					
					
					case System.Xml.XmlNodeType.EndElement:
						
						System.Diagnostics.Debug.Assert (reader_depth > 0);
						
						reader_depth--;
						
						if (reader_depth == 0)
						{
							//	Dès que le niveau d'imbrication retombe à zéro, on arrête l'analyse,
							//	parce que dans le cas d'un fragment, il ne faut pas lire au-delà de
							//	sa fin (ça consommerait des éléments utiles à l'appelant).
							
							return;
						}
						
						//	Les assertions qui suivent sont redondantes, car en principe, XmlTextReader ne doit
						//	pas laisser passer ce genre d'erreurs.
						
						System.Diagnostics.Debug.Assert (element_name != null);
						System.Diagnostics.Debug.Assert (name == "field");
						
						if (reader_depth == 1)
						{
							if ((this.fields.Contains (element_name)) &&
								((this.fields[element_name] is ResourceBundleXmlReader) ||
								(this.fields[element_name] is byte[]) ||
								(this.fields[element_name] is System.Collections.IList)))
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
					
					
					
					default:
						break;
				}
			}
		}
		
		protected void ParseXmlField(System.Xml.XmlTextReader reader, int reader_depth, out string element_name)
		{
			if (reader_depth != 1)
			{
				throw new ResourceException (string.Format ("Found field at depth {0}. {1}.", reader_depth, this.XmlErrorContext (reader)));
			}
			
			string name = reader.GetAttribute ("name");
			
			if (name == null)
			{
				throw new ResourceException (string.Format ("Field has no name. {0}.", this.XmlErrorContext (reader)));
			}
			
			element_name = name;
		}
		
		protected void ParseXmlList(System.Xml.XmlTextReader reader, string default_prefix, ResourceLevel level, int reader_depth, int recursion, ref string element_name, System.Text.StringBuilder buffer)
		{
			//	Une liste est identifée par <list>...</list> et peut contenir exclusivement :
			//
			//	- des bundles ou
			//	- des références à des bundles
			//
			//	On ne peut pas (encore) faire des listes de listes, ou des listes de textes.
			
			if (reader_depth != 1)
			{
				throw new ResourceException (string.Format ("Found list at depth {0}. {1}.", reader_depth, this.XmlErrorContext (reader)));
			}
			
			System.Diagnostics.Debug.Assert (element_name == null);
			
			element_name = reader.GetAttribute ("name");
			
			if (element_name == null)
			{
				throw new ResourceException (string.Format ("List has no name. {0}.", this.XmlErrorContext (reader)));
			}
			
			System.Collections.ArrayList list = new System.Collections.ArrayList (); 
			
			bool stop_loop = false;
			
			while (reader.Read ())
			{
				string name = reader.Name;
				
				switch (reader.NodeType)
				{
					case System.Xml.XmlNodeType.Element:
						if (name == "bundle")
						{
							ResourceBundleXmlReader child_bundle = new ResourceBundleXmlReader (string.Format ("{0}#{1}[{2}]", this.name, element_name, list.Count));
							child_bundle.ParseXml (reader, default_prefix, level, 1, recursion+1);
							list.Add (child_bundle);
						}
						else if (name == "ref")
						{
							ResourceBundleXmlReader child_bundle;
							this.ParseXmlRef (reader, default_prefix, level, 1, recursion, null, null, out child_bundle);
							
							if (child_bundle == null)
							{
								throw new ResourceException (string.Format ("Illegal reference in list. {0}.", this.XmlErrorContext (reader)));
							}
							
							list.Add (child_bundle);
						}
						else
						{
							throw new ResourceException (string.Format ("Found tag <{0}> in list. {1}.", name, this.XmlErrorContext (reader)));
						}
						break;
					
					case System.Xml.XmlNodeType.EndElement:
						if (name == "list")
						{
							stop_loop = true;
						}
						else
						{
							throw new ResourceException (string.Format ("Found end tag </{0}> in list. {1}.", name, this.XmlErrorContext (reader)));
						}
						break;
					
					default:
						break;
				}
				
				if (stop_loop)
				{
					break;
				}
			}
			
			this.AddChildList (element_name, list);
		}
		
		protected void ParseXmlRef(System.Xml.XmlTextReader reader, string default_prefix, ResourceLevel level, int reader_depth, int recursion, string element_name, System.Text.StringBuilder buffer, out ResourceBundleXmlReader new_bundle)
		{
			new_bundle = null;
			
			//	On vient de trouver une référence à une autre ressource. En fonction du
			//	niveau auquel on se trouve, cette référence va soit retourner le bundle
			//	trouvé (niveau 1), soit simplement insérer les données dans le buffer
			//	courant (niveau 2).
			
			string target = reader.GetAttribute ("target");
			string type   = reader.GetAttribute ("type");
			
			if (target == null)
			{
				throw new ResourceException (string.Format ("Reference has no target. {1}.", this.XmlErrorContext (reader)));
			}
			
			if (Resources.ExtractPrefix (target) == null)
			{
				if (default_prefix == null)
				{
					throw new ResourceException (string.Format ("No default prefix specified, target '{0}' cannot be resolved. {1}.", target, this.XmlErrorContext (reader)));
				}
				
				target = default_prefix + ":" + target;
			}
			
			//	La cible peut être composée de deux parties: un nom de bundle et un nom de champ;
			//	le séparateur est le "#", donc target = "bundle#field" ou target = "bundle".
			
			string target_bundle;
			string target_field;
			
			ResourceBundle.SplitTarget (target, out target_bundle, out target_field);
			
			//	La cible peut avoir une spécification de type. La seule spécification actuellement
			//	autorisée est "binary" qui indique que la cible n'est pas une ressource standard,
			//	mais une ressource binaire dont il ne faut pas interpréter le contenu.
			
			if (type != null)
			{
				if (type == "binary")
				{
					if ((reader_depth == 2) &&
						(element_name != null))
					{
						byte[] data = Resources.GetBinaryData (target_bundle, level);
						
						if (data == null)
						{
							throw new ResourceException (string.Format ("Binary target '{0}' cannot be resolved. {1}.", target, this.XmlErrorContext (reader)));
						}
						
						this.AddBinaryField (element_name, data);
						return;
					}
					
					throw new ResourceException (string.Format ("Illegal reference to binary target '{0}' at depth {1}. {2}.", target, reader_depth, this.XmlErrorContext (reader)));
				}
				else
				{
					throw new ResourceException (string.Format ("Target '{0}' has unsupported type '{1}'. {2}.", target, type, this.XmlErrorContext (reader)));
				}
			}
			
			ResourceBundleXmlReader bundle = Resources.GetBundle (target_bundle, level, recursion+1) as ResourceBundleXmlReader;
			
			if (bundle == null)
			{
				throw new ResourceException (string.Format ("Reference to target '{0}' cannot be resolved. {1}.", target, this.XmlErrorContext (reader)));
			}
			
			if (reader_depth == 1)
			{
				System.Diagnostics.Debug.Assert (element_name == null);
				System.Diagnostics.Debug.Assert (target_field == null);
				
				new_bundle = bundle;
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
				
				ResourceBundleXmlReader child_bundle = null;
				
				if (target_field != null)
				{
					if (!bundle.Contains (target_field))
					{
						throw new ResourceException (string.Format ("Target bundle '{0}' does not contain field '{1}'. {2}.", target_bundle, target_field, this.XmlErrorContext (reader)));
					}
					
					object field_data = bundle[target_field];
					
					if (field_data is string)
					{
						buffer.Append (field_data);
					}
					else if (field_data is ResourceBundleXmlReader)
					{
						child_bundle = field_data as ResourceBundleXmlReader;
					}
					else
					{
						throw new ResourceException (string.Format ("Target bundle '{0}' contains unsupported data of type '{1}'. {2}.", target_bundle, field_data.GetType ().ToString (), this.XmlErrorContext (reader)));
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
				throw new ResourceException (string.Format ("Illegal reference to '{0}' at level {1}. {2}.", target, reader_depth, this.XmlErrorContext (reader)));
			}
		}
		
		
		protected void AddChildBundle(string element_name, ResourceBundleXmlReader child_bundle)
		{
			ResourceBundleXmlReader current_bundle;
			
			if (this.fields.Contains (element_name))
			{
				current_bundle = this.fields[element_name] as ResourceBundleXmlReader;
			}
			else
			{
				current_bundle = new ResourceBundleXmlReader (string.Format ("{0}#{1}", this.name, element_name));
				this.fields[element_name] = current_bundle;
			}
			
			System.Diagnostics.Debug.Assert (current_bundle != null);
			
			current_bundle.Merge (child_bundle);
		}
		
		protected void AddChildList(string element_name, System.Collections.ArrayList child_list)
		{
			System.Collections.ArrayList current_list;
			
			if (this.fields.Contains (element_name))
			{
				current_list = this.fields[element_name] as System.Collections.ArrayList;
			}
			else
			{
				current_list = new System.Collections.ArrayList ();
				this.fields[element_name] = current_list;
			}
			
			System.Diagnostics.Debug.Assert (current_list != null);
			
			current_list.AddRange (child_list);
		}
		
		protected void AddBinaryField(string element_name, byte[] data)
		{
			System.Diagnostics.Debug.Assert (element_name != null);
			System.Diagnostics.Debug.Assert (data != null);
			
			this.fields[element_name] = data;
		}
		
		protected void Merge(ResourceBundleXmlReader bundle)
		{
			//	Fusionne le bundle passé en entrée avec le bundle actuel.
			
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
				//	TODO: ajouter le support pour copier les champs qui sont différents
				//	du type string. En particulier, il faut fusionner les sous-bundles
				//	avec ceux déjà existants (récursivement); idem pour les listes.
				
				this.fields[entry.Key] = entry.Value;
			}
		}
		
		
		public class RefRecord
		{
			public RefRecord(string element_name)
			{
				this.element_name = element_name;
			}
			
			
			public void ParseBegin(System.Xml.XmlTextReader reader, System.Text.StringBuilder buffer)
			{
				for (int i = 0; i < reader.AttributeCount; i++)
				{
					reader.MoveToAttribute (i);
					
					string arg_name  = reader.Name;
					string arg_value = reader.Value;
					
					this.arguments[arg_name] = arg_value;
				}
				
				reader.MoveToFirstAttribute ();
				
				this.buffer_offset = buffer.Length;
			}
			
			public void ParseEnd(System.Text.StringBuilder buffer)
			{
				this.buffer_insert = buffer.Length - this.buffer_offset;
			}
			
			
			public void DefineBuffer(int offset, int length)
			{
				this.buffer_offset = offset;
				this.buffer_insert = length;
			}
			
			public string				ElementName
			{
				get { return this.element_name; }
			}
			
			public int					BufferOffset
			{
				get { return this.buffer_offset; }
			}
			
			public int					BufferInsert
			{
				get { return this.buffer_insert; }
			}
			
			public Hashtable			Arguments
			{
				get { return this.arguments; }
			}
			
			
			public override string ToString()
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				buffer.Append ("<ref");
				
				foreach (string key in this.arguments.Keys)
				{
					buffer.Append (" ");
					buffer.Append (key);
					buffer.Append (@"=""");
					buffer.Append (this.arguments[key]);
					buffer.Append (@"""");
				}
				
				buffer.Append ("/>");
				
				return buffer.ToString ();
			}

			
			
			private string				element_name;
			private Hashtable			arguments = new Hashtable ();
			private int					buffer_offset;
			private int					buffer_insert;
		}
		
		
		protected Hashtable				fields;
	}
}
