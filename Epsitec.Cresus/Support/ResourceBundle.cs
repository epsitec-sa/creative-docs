//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	using System.Collections;
	
	/// <summary>
	/// La classe ResourceBundle donne accès aux données d'une ressource XML sous
	/// une forme simplifiée.
	/// </summary>
	public class ResourceBundle
	{
		public ResourceBundle(string name)
		{
			this.name = name;
		}
		
		
		public string					Name
		{
			get { return this.name; }
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
		
		
		public bool Contains(string field)
		{
			if (this.fields != null)
			{
				return this.fields.Contains (field);
			}
			
			return false;
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
			if (data is byte[])
			{
				return ResourceFieldType.Binary;
			}
			if (data is System.Collections.IList)
			{
				return ResourceFieldType.BundleList;
			}
			
			throw new ResourceException ("Invalid field type in bundle");
		}
		
		public string GetFieldString(string field)
		{
			return this[field] as string;
		}
		
		public byte[] GetFieldBinary(string field)
		{
			return this[field] as byte[];
		}
		
		public ResourceBundle GetFieldBundle(string field)
		{
			return this[field] as ResourceBundle;
		}
		
		public System.Collections.IList GetFieldBundleList(string field)
		{
			return this[field] as System.Collections.IList;
		}
		
		public int GetFieldBundleListLength(string field)
		{
			System.Collections.IList list = this[field] as System.Collections.IList;
			return (list == null) ? 0 : list.Count;
		}
		
		public ResourceBundle GetFieldBundleListItem(string field, int index)
		{
			System.Collections.IList list = this[field] as System.Collections.IList;
			return (list == null) ? null : list[index] as ResourceBundle;
		}
		
		
		public Drawing.Bitmap GetBitmap(string image_name)
		{
			string field_name = "i." + image_name;
			
			if (this.bitmap_cache == null)
			{
				if (this.GetFieldType ("image.data") != ResourceFieldType.Binary)
				{
					throw new ResourceException ("Bundle does not contain image");
				}
				
				byte[] image_data = this.GetFieldBinary ("image.data");
				string image_args = this.GetFieldString ("image.size");
				
				Drawing.Size size = Drawing.Size.Parse (image_args);
				
				this.bitmap_cache = Drawing.Bitmap.FromData (image_data, Drawing.Point.Empty, size);
			}
			
			System.Diagnostics.Debug.Assert (this.bitmap_cache != null);
			
			if (this.Contains (field_name))
			{
				//	Une image est définie par un champ 'i.name' qui contient une chaîne composée
				//	de 'x;y;dx;dy;ox;oy' définissant l'origine dans l'image mère, la taille et
				//	l'offset de l'origine dans la sous-image. 'oy;oy' sont facultatifs.
				
				string[] args = this.GetFieldString (field_name).Split (';', ':');
				
				if ((args.Length != 4) && (args.Length != 6))
				{
					throw new ResourceException (string.Format ("Invalid image specification for '{0}', {1} arguments", image_name, args.Length));
				}
				
				Drawing.Point rect_pos = Drawing.Point.Parse (args[0] + ";" + args[1]);
				Drawing.Size  rect_siz = Drawing.Size.Parse (args[2] + ";" + args[3]);
				Drawing.Point origin   = Drawing.Point.Empty;
				
				if (args.Length >= 6)
				{
					origin = Drawing.Point.Parse (args[4] + ";" + args[5]);
				}
				
				return Drawing.Bitmap.FromLargerImage (this.bitmap_cache, new Drawing.Rectangle (rect_pos, rect_siz), origin);
			}
			
			return null;
		}
		
		public void Compile(byte[] data)
		{
			this.Compile (data, null, ResourceLevel.Merged, 0);
		}
		
		public void Compile(byte[] data, string default_prefix, ResourceLevel level)
		{
			this.Compile (data, default_prefix, level, 0);
		}
		
		public void Compile(byte[] data, string default_prefix, ResourceLevel level, int recursion)
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
		
		
		public static bool SplitTarget(string target, out string target_bundle, out string target_field)
		{
			int pos = target.IndexOf ("#");
			
			target_bundle = target;
			target_field  = null;
			
			if (pos >= 0)
			{
				target_bundle = target.Substring (0, pos);
				target_field  = target.Substring (pos+1);
				
				return true;
			}
			
			return false;
		}
		
		
		protected void ParseXml(System.Xml.XmlTextReader reader, string default_prefix, ResourceLevel level, int reader_depth, int recursion)
		{
			//	Analyse un fragment de XML.
			
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
						//	une référence <ref target='target'/>, soit un élément commençant une liste, soit
						//	un élément <bundle> commençant un sous-bundle, soit un élément définissant un
						//	champ 'string' du bundle.
						
						if (name == "ref")
						{
							ResourceBundle new_bundle;
							
							this.ParseXmlRef (reader, default_prefix, level, reader_depth, recursion, element_name, buffer, out new_bundle);
							
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
									
									System.Diagnostics.Debug.Assert (element_name != null);
									
									ResourceBundle child_bundle = new ResourceBundle (string.Format ("{0}#{1}", this.name, element_name));
									child_bundle.ParseXml (reader, default_prefix, level, 1, recursion+1);
									this.AddChildBundle (element_name, child_bundle);
									
									//	Comme le ParseXml récursif a aussi consommé le </bundle>, on saute à
									//	la suite sans changer le niveau d'imbrication.
									
									continue;
								
								default:
									throw new ResourceException ("Illegal bundle depth");
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
							throw new ResourceException (string.Format ("Bundle does not start with root <bundle>, but <{0}>", name));
						}
						else
						{
							throw new ResourceException (string.Format ("Malformed XML bundle, unknown tag '{0}' found", name));
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
								((this.fields[element_name] is ResourceBundle) ||
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
				throw new ResourceException (string.Format ("Found field at depth {0}.", reader_depth));
			}
			
			string name = reader.GetAttribute ("name");
			
			if (name == null)
			{
				throw new ResourceException ("Field has no name");
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
				throw new ResourceException (string.Format ("Found list at depth {0}.", reader_depth));
			}
			
			System.Diagnostics.Debug.Assert (element_name == null);
			
			element_name = reader.GetAttribute ("name");
			
			if (element_name == null)
			{
				throw new ResourceException ("List has no name");
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
							ResourceBundle child_bundle = new ResourceBundle (string.Format ("{0}#{1}[{2}]", this.name, element_name, list.Count));
							child_bundle.ParseXml (reader, default_prefix, level, 1, recursion+1);
							list.Add (child_bundle);
						}
						else if (name == "ref")
						{
							ResourceBundle child_bundle;
							this.ParseXmlRef (reader, default_prefix, level, 1, recursion, null, null, out child_bundle);
							
							if (child_bundle == null)
							{
								throw new ResourceException ("Illegal reference in list");
							}
							
							list.Add (child_bundle);
						}
						else
						{
							throw new ResourceException (string.Format ("Found tag <{0}> in list.", name));
						}
						break;
					
					case System.Xml.XmlNodeType.EndElement:
						if (name == "list")
						{
							stop_loop = true;
						}
						else
						{
							throw new ResourceException (string.Format ("Found end tag </{0}> in list.", name));
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
		
		protected void ParseXmlRef(System.Xml.XmlTextReader reader, string default_prefix, ResourceLevel level, int reader_depth, int recursion, string element_name, System.Text.StringBuilder buffer, out ResourceBundle new_bundle)
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
							throw new ResourceException (string.Format ("Binary target '{0}' cannot be resolved", target));
						}
						
						this.AddBinaryField (element_name, data);
						return;
					}
					
					throw new ResourceException (string.Format ("Illegal reference to binary target '{0}' at depth {1}", target, reader_depth));
				}
				else
				{
					throw new ResourceException (string.Format ("Target '{0}' has unsupported type '{1}'", target, type));
				}
			}
			
			ResourceBundle bundle = Resources.GetBundle (target_bundle, level, recursion+1);
			
			if (bundle == null)
			{
				throw new ResourceException (string.Format ("Reference to target '{0}' cannot be resolved", target));
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
					else if (field_data is ResourceBundle)
					{
						child_bundle = field_data as ResourceBundle;
					}
					else
					{
						throw new ResourceException (string.Format ("Target bundle '{0}' contains unsupported data of type '{1}'", target_bundle, field_data.GetType ().ToString ()));
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
				current_bundle = new ResourceBundle (string.Format ("{0}#{1}", this.name, element_name));
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
		
		protected void Merge(ResourceBundle bundle)
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
		
		
		protected string				name;
		protected Hashtable				fields;
		protected Drawing.Bitmap		bitmap_cache;
		
		protected const int				max_recursion = 50;
	}
}
