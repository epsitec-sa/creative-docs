//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	using System.Collections;
	
	using PropertyInfo   = System.Reflection.PropertyInfo;
	using MemberInfo     = System.Reflection.MemberInfo;
	using MemberTypes    = System.Reflection.MemberTypes;
	using BindingFlags   = System.Reflection.BindingFlags;
	
	using TypeDescriptor = System.ComponentModel.TypeDescriptor;
	using TypeConverter  = System.ComponentModel.TypeConverter;
	
	#region BundlingEvent & BundlingPropertyEvent...
	public class BundlingEventArgs : System.EventArgs
	{
		public BundlingEventArgs(ResourceBundle bundle, object obj)
		{
			this.obj    = obj;
			this.bundle = bundle;
		}
		
		
		public object					Object
		{
			get { return this.obj; }
		}
		
		public ResourceBundle			Bundle
		{
			get { return this.bundle; }
		}
		
		
		protected object				obj;
		protected ResourceBundle		bundle;
	}
	
	public class BundlingPropertyEventArgs : BundlingEventArgs
	{
		public BundlingPropertyEventArgs(ResourceBundle bundle, object obj, PropertyInfo prop_info, string prop_name, object prop_value, object prop_default) : base(bundle, obj)
		{
			this.prop_info    = prop_info;
			this.prop_name    = prop_name;
			this.prop_value   = prop_value;
			this.prop_default = prop_default;
		}
		
		
		public string					PropertyName
		{
			get { return this.prop_name; }
		}
		
		public object					PropertyValue
		{
			get { return this.prop_value; }
		}
		
		public object					PropertyDefault
		{
			get { return this.prop_default; }
		}
		
		public string					PropertyData
		{
			get { return this.prop_data; }
			set { this.prop_data = value; }
		}
		
		public PropertyInfo				PropertyInfo
		{
			get { return this.prop_info; }
		}
		
		public System.Type				PropertyType
		{
			get { return this.prop_info.PropertyType; }
		}
		
		public bool						SuppressProperty
		{
			get { return this.suppress; }
			set { this.suppress = value; }
		}
		
		
		protected string				prop_name;
		protected object				prop_value;
		protected string				prop_data;
		protected object				prop_default;
		protected bool					suppress;
		protected PropertyInfo			prop_info;
	}
	
	
	public delegate void BundlingEventHandler(object sender, BundlingEventArgs e);
	public delegate void BundlingPropertyEventHandler(object sender, BundlingPropertyEventArgs e);
	#endregion
	
	/// <summary>
	/// La classe ObjectBundler s'occupe de déballer des bundles pour en
	/// faire des objets.
	/// </summary>
	public class ObjectBundler
	{
		public ObjectBundler() : this (false)
		{
		}
		
		public ObjectBundler(bool store_mapping)
		{
			if (store_mapping)
			{
				this.EnableMapping ();
			}
		}
		
		
		public void EnableMapping()
		{
			if (this.store_mapping == false)
			{
				this.store_mapping = true;
				this.obj_to_bundle = new System.Collections.Hashtable ();
			}
		}
		
		
		static ObjectBundler()
		{
			ObjectBundler.classes = new System.Collections.Hashtable ();
			
			System.AppDomain             domain     = System.AppDomain.CurrentDomain;
			System.Reflection.Assembly[] assemblies = domain.GetAssemblies ();
			
			for (int i = 0; i < assemblies.Length; i++)
			{
				ObjectBundler.RegisterAssembly (assemblies[i]);
			}
		}
		
		static public void Initialise()
		{
			//	En appelant cette méthode statique, on peut garantir que le constructeur
			//	statique de ObjectBundler a bien été exécuté.
		}
		
		protected static void RegisterAssembly(System.Reflection.Assembly assembly)
		{
			System.Type[] assembly_types = assembly.GetTypes ();
			
			foreach (System.Type type in assembly_types)
			{
				if ((type.IsClass) &&
					(!type.IsAbstract))
				{
					if (type.GetInterface ("IBundleSupport") != null)
					{
						if (type.IsDefined (typeof (SuppressBundleSupportAttribute), false))
						{
							//	La classe ne doit pas être pise en considération: le fait qu'elle
							//	implémente IBundleSupport ne peut pas être pris comme indication
							//	de son support. Utile pour des classes spéciales (à usage interne
							//	uniquement).
							
							continue;
						}
						
						try
						{
							IBundleSupport bundle_support = System.Activator.CreateInstance (type, true) as IBundleSupport;
							ObjectBundler.Register (bundle_support);
							bundle_support.Dispose ();
						}
						catch (System.Exception ex)
						{
							System.Diagnostics.Debug.WriteLine ("Could not register " + type.Name + " : " + ex.Message);
						}
					}
				}
			}
		}
		
		protected static void Register(IBundleSupport bundle_support)
		{
			System.Diagnostics.Debug.Assert (bundle_support != null);
			System.Diagnostics.Debug.Assert (bundle_support.PublicClassName != null);
			
			string      name = bundle_support.PublicClassName;
			System.Type type = bundle_support.GetType ();
			
			if (ObjectBundler.classes.Contains (name))
			{
				//	Zut, nous avons déjà une classe avec ce nom. Vérifions si ce n'est pas dû
				//	à un héritage intempestif, auquel cas il faut choisir la classe qui est le
				//	plus près de la racine.
				
				System.Type cur_type = ObjectBundler.classes[name] as System.Type;
				System.Type new_type = type;
				
				if (cur_type == new_type)
				{
					return;
				}
				if (new_type.IsSubclassOf (cur_type))
				{
					//	Le nouveau type est dérivé de celui qui est connu. On garde donc
					//	l'ancien.
					
					return;
				}
				if (cur_type.IsSubclassOf (new_type))
				{
					//	On va remplacer le type anciennement stocké par le nouveau, qui est
					//	un parent de l'ancien...
				}
				else
				{
					throw new System.Exception (string.Format ("Class {0} and class {1} share the name {2}", cur_type.Name, new_type.Name, name));
				}
			}
			
			ObjectBundler.classes[name] = type;
		}
		
		
		public object CopyObject(object source)
		{
			IBundleSupport src_obj = source as IBundleSupport;
			
			if (src_obj == null)
			{
				return null;
			}
			
			object         copy    = System.Activator.CreateInstance (source.GetType (), true);
			IBundleSupport dst_obj = copy as IBundleSupport;
			
			System.Diagnostics.Debug.Assert (dst_obj != null);
			
			foreach (MemberInfo member_info in source.GetType ().GetMembers (BindingFlags.Public | BindingFlags.Instance))
			{
				//	Passe en revue tous les membres publics. Ceux qui sont des propriétés
				//	vont nécessiter une attention toute particulière...
				
				if (member_info.MemberType == MemberTypes.Property)
				{
					System.Type  type      = source.GetType ();
					PropertyInfo prop_info = member_info as PropertyInfo;
					
					System.Diagnostics.Debug.Assert (prop_info != null);
					
					while (this.CopyProperty (source, copy, prop_info) == false)
					{
						type = type.BaseType;
						
						if (type == null)
						{
							break;
						}
						
						prop_info = type.GetProperty (prop_info.Name, prop_info.PropertyType);
						
						if (prop_info == null)
						{
							break;
						}
					}
				}
			}
			
			return copy;
		}
		
		public object CreateFromBundle(ResourceBundle bundle)
		{
			//	A partir d'une description stockée dans un bundle, crée un objet de toutes pièces
			//	et initialise les propriétés connues.
			
			//	Si l'objet est inconnu (ou que le bundle ne décrit pas un objet), alors on retourne
			//	simplement null sans générer d'exception.
			
			if (bundle == null)
			{
				return null;
			}
			
			string name = bundle["class"].AsString;
			
			if (name == null)
			{
				return null;
			}
			
			System.Type obj_type = ObjectBundler.classes[name] as System.Type;
			
			if (obj_type == null)
			{
				return null;
			}
			
			IBundleSupport obj = System.Activator.CreateInstance (obj_type, true) as IBundleSupport;
			
			foreach (MemberInfo member_info in obj_type.GetMembers (BindingFlags.Public | BindingFlags.Instance))
			{
				//	Passe en revue tous les membres publics. Ceux qui sont des propriétés
				//	vont nécessiter une attention toute particulière...
				
				if (member_info.MemberType == MemberTypes.Property)
				{
					System.Type type = obj_type;
					PropertyInfo prop_info = member_info as PropertyInfo;
					
					System.Diagnostics.Debug.Assert (prop_info != null);
					
					while (this.RestoreProperty (bundle, obj, prop_info) == false)
					{
						type = type.BaseType;
						
						if (type == null)
						{
							break;
						}
						
						prop_info = type.GetProperty (prop_info.Name, prop_info.PropertyType);
						
						if (prop_info == null)
						{
							break;
						}
					}
				}
			}
			
			obj.RestoreFromBundle (this, bundle);
			
			//	Si le ObjectBundler a été configuré de manière à se souvenir des objets créés,
			//	on prend note de l'objet et de son bundle associé.
			
			if (this.store_mapping)
			{
				this.obj_to_bundle[obj] = bundle;
			}
			
			this.OnObjectUnbundled (new BundlingEventArgs (bundle, obj));
			
			return obj;
		}
		
		public bool FillBundleFromObject(ResourceBundle bundle, object source)
		{
			//	A partir d'un objet supportant l'interface IBundleSupport, remplit un bundle avec
			//	les informations nécessaires à recréer cet objet.
			
			//	Si l'objet ne supporte pas IBundleSupport, alors on retourne simplement false sans
			//	générer d'exception.
			
			IBundleSupport obj = source as IBundleSupport;
			
			if (obj == null)
			{
				return false;
			}
			
			System.Type obj_type  = source.GetType ();
			string      obj_class = null;
			
			foreach (System.Collections.DictionaryEntry entry in ObjectBundler.classes)
			{
				if (entry.Value == obj_type)
				{
					obj_class = entry.Key as string;
					break;
				}
			}
			
			if (obj_class == null)
			{
				return false;
			}
			
			System.Diagnostics.Debug.Assert (ObjectBundler.xmldoc != null);
			System.Diagnostics.Debug.Assert (ObjectBundler.xmldoc.ChildNodes.Count == 0);
			
			this.BundleAddDataField (bundle, "class", obj_class);
			
			object obj_default = System.Activator.CreateInstance (obj_type, true);
			
			foreach (MemberInfo member_info in obj_type.GetMembers (BindingFlags.Public | BindingFlags.Instance))
			{
				//	Passe en revue tous les membres publics. Ceux qui sont des propriétés
				//	vont nécessiter une attention toute particulière...
				
				if (member_info.MemberType == MemberTypes.Property)
				{
					System.Type type = obj_type;
					PropertyInfo prop_info = member_info as PropertyInfo;
					
					System.Diagnostics.Debug.Assert (prop_info != null);
					
					while (this.SerialiseProperty (bundle, obj, obj_default, prop_info) == false)
					{
						type = type.BaseType;
						
						if (type == null)
						{
							break;
						}
						
						prop_info = type.GetProperty (prop_info.Name, prop_info.PropertyType);
						
						if (prop_info == null)
						{
							break;
						}
					}
				}
			}
			
			System.IDisposable obj_default_disposable = obj_default as System.IDisposable;
			
			if (obj_default_disposable != null)
			{
				obj_default_disposable.Dispose ();
			}
			
//			obj.RestoreFromBundle (this, bundle);
//			this.OnObjectUnbundled (obj, bundle);
			
			System.Diagnostics.Debug.Assert (ObjectBundler.xmldoc.ChildNodes.Count == 0);
			
			return true;
		}
		
		
		protected void BundleAddDataField(ResourceBundle bundle, string name, string value)
		{
			System.Xml.XmlElement   xmlnode = ObjectBundler.xmldoc.CreateElement ("data");
			System.Xml.XmlAttribute xmlattr = ObjectBundler.xmldoc.CreateAttribute ("name");
			
			xmlattr.Value = name;
			xmlnode.Attributes.Append (xmlattr);
			xmlnode.InnerText = value;
			
			bundle.Add (new ResourceBundle.Field (bundle, xmlnode));
		}
		
		protected bool SerialiseProperty(ResourceBundle bundle, object obj, object obj_default, string property_name)
		{
			PropertyInfo prop_info = this.FindPropertyInfo (obj, property_name);
			
			if (prop_info == null)
			{
				return false;
			}
			
			return this.SerialiseProperty (bundle, obj, obj_default, prop_info);
		}
		
		protected bool SerialiseProperty(ResourceBundle bundle, object obj, object obj_default, PropertyInfo prop_info)
		{
			//	Pout un objet donné, fait un "get" de la propriété spécifiée et stocke les
			//	données dans le champ correspondant du bundle.
			
			if ((prop_info != null) &&
				(prop_info.CanRead) &&
				(prop_info.IsDefined (typeof (BundleAttribute), true)))
			{
				//	C'est bien une propriété qui peut être lue et qui a l'attribut [Bundle] défini.
				
				object[] attributes = prop_info.GetCustomAttributes (typeof (BundleAttribute), true);
				
				System.Diagnostics.Debug.Assert (attributes.Length == 1);
				
				BundleAttribute attr = attributes[0] as BundleAttribute;
				
				object prop_value = prop_info.GetValue (obj, null);
				object prop_def   = prop_info.GetValue (obj_default, null);
				
				BundlingPropertyEventArgs e = new BundlingPropertyEventArgs (bundle, obj, prop_info, attr.PropertyName, prop_value, prop_def);
				
				e.PropertyData     = TypeDescriptor.GetConverter (prop_info.PropertyType).ConvertToInvariantString (prop_value);
				e.SuppressProperty = this.IsPropertyEqual (obj_default, prop_info, e.PropertyData);
				
				this.OnPropertyBundled (e);
				
				if (e.SuppressProperty == false)
				{
					this.BundleAddDataField (bundle, e.PropertyName, e.PropertyData);
				}
				
				return true;
			}
			
			return false;
		}
		
		
		public bool IsPropertyEqual(object obj, string property_name, string ref_value)
		{
			//	Compare la valeur de référence à la valeur de la propriété de l'objet spécié.
			//	Retourne true si les deux sont égaux. Cela peut être utile pour savoir si une
			//	valeur diffère de la valeur par défaut d'une propriété (on passe alors comme
			//	objet en entrée un objet fraîchement construit).
			
			PropertyInfo prop_info = this.FindPropertyInfo (obj, property_name);
			
			if (prop_info == null)
			{
				return false;
			}
			
			return this.IsPropertyEqual (obj, prop_info, ref_value);
		}
		
		public bool IsPropertyEqual(object obj, PropertyInfo prop_info, string ref_value)
		{
			if ((prop_info != null) &&
				(prop_info.CanRead) &&
				(prop_info.IsDefined (typeof (BundleAttribute), true)))
			{
				//	C'est bien une propriété qui peut être lue et qui a l'attribut [Bundle] défini.
				
				System.Type prop_type   = prop_info.PropertyType;
				object      prop_object = prop_info.GetValue (obj, null);
				string      prop_value  = TypeDescriptor.GetConverter (prop_type).ConvertToInvariantString (prop_object);
				
				return prop_value == ref_value;
			}
			
			return false;
		}
		
		
		public bool RestoreProperty(ResourceBundle bundle, object obj, string property_name)
		{
			PropertyInfo prop_info = this.FindPropertyInfo (obj, property_name);
			
			if (prop_info == null)
			{
				return false;
			}
			
			return this.RestoreProperty (bundle, obj, prop_info);
		}
		
		public bool RestoreProperty(ResourceBundle bundle, object obj, PropertyInfo prop_info)
		{
			//	Pout un objet donné, fait un "set" de la propriété spécifiée en se
			//	basant sur les données stockées dans le champ correspondant du bundle.
			
			bool ok = false;
			
			if ((prop_info != null) &&
				(prop_info.CanRead) &&
				(prop_info.IsDefined (typeof (BundleAttribute), true)))
			{
				//	C'est bien une propriété qui peut être lue et qui a l'attribut [Bundle] défini.
				
				object[] attributes = prop_info.GetCustomAttributes (typeof (BundleAttribute), true);
				
				System.Diagnostics.Debug.Assert (attributes.Length == 1);
				
				BundleAttribute attr = attributes[0] as BundleAttribute;
				string     prop_name = attr.PropertyName;
				
				switch (bundle[prop_name].Type)
				{
					case ResourceFieldType.Bundle:
						prop_info.SetValue (obj, this.CreateFromBundle (bundle[prop_name].AsBundle), null);
						ok = true;
						break;
					
					case ResourceFieldType.Data:
						
						//	La valeur source trouvée dans le bundle est un texte. Il faut faire
						//	en sorte que cette valeur puisse être affectée à la propriété.
						
						string str_value = bundle[prop_name].AsString;
						
						if ((str_value != null) && (str_value != ""))
						{
							System.Type prop_type = prop_info.PropertyType;
							
							if (prop_type == typeof (string))
							{
								//	C'est un texte. Comme nous avons déjà une 'string' extraite du bundle,
								//	aucune conversion supplémentaire n'est nécessaire.
								
								prop_info.SetValue (obj, str_value, null);
								ok = true;
							}
							else if (prop_type == typeof (double))
							{
								//	C'est une valeur numérique qu'il faut convertir... On ne supporte pas
								//	de valeur par défaut ici, donc on fait simplement un "Parse".
								
								double num_value = System.Double.Parse (str_value, System.Globalization.CultureInfo.InvariantCulture);
								prop_info.SetValue (obj, num_value, null);
								ok = true;
							}
							else if (prop_type == typeof (int))
							{
								int num_value = System.Int32.Parse (str_value, System.Globalization.CultureInfo.InvariantCulture);
								prop_info.SetValue (obj, num_value, null);
								ok = true;
							}
							else if (prop_type == typeof (long))
							{
								long num_value = System.Int64.Parse (str_value, System.Globalization.CultureInfo.InvariantCulture);
								prop_info.SetValue (obj, num_value, null);
								ok = true;
							}
							else if (prop_type == typeof (decimal))
							{
								decimal num_value = System.Decimal.Parse (str_value, System.Globalization.CultureInfo.InvariantCulture);
								prop_info.SetValue (obj, num_value, null);
								ok = true;
							}
							else if (prop_type.IsSubclassOf (typeof (System.Enum)))
							{
								//	C'est une énumération. On va tenter de convertir la valeur 'string' en
								//	une valeur correspond à l'énumération.
								
								object enum_value = System.Enum.Parse (prop_type, str_value, true);
								prop_info.SetValue (obj, enum_value, null);
								ok = true;
							}
							else if (prop_type == typeof (Drawing.Size))
							{
								//	C'est une taille. On va tenter de convertir la valeur 'string' en une
								//	taille, laquelle est stockée sous la forme 'x;y' où x et/ou y peuvent
								//	être remplacés par "*" (ce qui signifie: utiliser la valeur par défaut)
								
								Drawing.Size def_size = (Drawing.Size) prop_info.GetValue (obj, null);
								prop_info.SetValue (obj, Drawing.Size.Parse (str_value, System.Globalization.CultureInfo.InvariantCulture, def_size), null);
								ok = true;
							}
							else if (prop_type == typeof (Drawing.Point))
							{
								//	C'est un point. On va tenter de convertir la valeur 'string' en un
								//	point, lequel est stocké sous la forme 'x;y' où x et/ou y peuvent
								//	être remplacés par "*" (ce qui signifie: utiliser la valeur par défaut)
								
								Drawing.Point def_point = (Drawing.Point) prop_info.GetValue (obj, null);
								prop_info.SetValue (obj, Drawing.Point.Parse (str_value, System.Globalization.CultureInfo.InvariantCulture, def_point), null);
								ok = true;
							}
							else if (prop_type == typeof (Drawing.Rectangle))
							{
								//	C'est un rectangle. On va tenter de convertir la valeur 'string' en un
								//	rectangle, lequel est stocké sous la forme 'x;y;dx;dy' où les arguments
								//	peuvent être remplacés par "*" (valeur par défaut).
								
								Drawing.Rectangle def_rect = (Drawing.Rectangle) prop_info.GetValue (obj, null);
								prop_info.SetValue (obj, Drawing.Rectangle.Parse (str_value, System.Globalization.CultureInfo.InvariantCulture, def_rect), null);
								ok = true;
							}
							else if (prop_type == typeof (Drawing.Margins))
							{
								//	Ce sont des marges. On va tenter de convertir la valeur 'string' en des
								//	marges, lequelles sont stockées sous la forme 'x1;x2;y1;y2' où les arguments
								//	peuvent être remplacés par "*" (ce qui signifie: utiliser la valeur par défaut)
								
								Drawing.Margins def_margins = (Drawing.Margins) prop_info.GetValue (obj, null);
								prop_info.SetValue (obj, Drawing.Margins.Parse (str_value, System.Globalization.CultureInfo.InvariantCulture, def_margins), null);
								ok = true;
							}
							else if (prop_type == typeof (bool))
							{
								//	C'est un booléen. Accepte "0/1", "off/on", "no/yes", "false/true".
								
								switch (str_value)
								{
									case "0":
									case "false":
									case "no":
									case "off":
										prop_info.SetValue (obj, false, null);
										break;
									case "1":
									case "true":
									case "yes":
									case "on":
										prop_info.SetValue (obj, true, null);
										break;
									default:
										throw new System.ArgumentException (string.Format ("{0} cannot be mapped to bool", str_value));
								}
								
								ok = true;
							}
							else
							{
								//	TODO: gérer les autres cas fréquents ici... (types numériques, couleur, etc.)
							}
						}
						break;
					
					default:
						break;
				}
			}
			
			return ok;
		}
		
		
		public bool CopyProperty(object source, object copy, PropertyInfo prop_info)
		{
			if ((prop_info.CanRead) &&
				(prop_info.CanWrite) &&
				(prop_info.IsDefined (typeof (BundleAttribute), true)))
			{
				//	C'est bien une propriété qui peut être lue et écrite, et qui a l'attribut
				//	[Bundle] défini.
				
				object        data = prop_info.GetValue (source, null);
				TypeConverter conv = TypeDescriptor.GetConverter (prop_info.PropertyType);
				string        text = conv.ConvertToInvariantString (data);
				
				data = conv.ConvertFromInvariantString (text);
				
				prop_info.SetValue (copy, data, null);
				
				return true;
			}
			
			return false;
		}
		
		
		public PropertyInfo FindPropertyInfo(object obj, string property_name)
		{
			//	Parcourt les propriétés publiques de l'objet à la recherche de celle
			//	qui a un attribut [Bundle] avec le PropertyName spécifié.
			
			foreach (MemberInfo member_info in obj.GetType ().GetMembers (BindingFlags.Public | BindingFlags.Instance))
			{
				if (member_info.MemberType == MemberTypes.Property)
				{
					PropertyInfo prop_info = member_info as PropertyInfo;
					
					System.Diagnostics.Debug.Assert (prop_info != null);
					
					if ((prop_info != null) &&
						(prop_info.CanRead) &&
						(prop_info.IsDefined (typeof (BundleAttribute), true)))
					{
						//	On vient de trouver une propriété qui pourrait faire l'affaire. Vérifions
						//	encore que le nom corresponde.
						
						object[] attributes = prop_info.GetCustomAttributes (typeof (BundleAttribute), true);
						
						System.Diagnostics.Debug.Assert (attributes.Length == 1);
						
						BundleAttribute attr = attributes[0] as BundleAttribute;
						
						if (attr.PropertyName == property_name)
						{
							//	Cette propriété est la bonne !
							
							return prop_info;
						}
					}
				}
			}
			
			return null;
		}
		
		
		public ResourceBundle FindBundleFromObject(object obj)
		{
			//	Trouve le bundle qui a servi à générer un objet.
			
			if (this.store_mapping)
			{
				return this.obj_to_bundle[obj] as ResourceBundle;
			}
			
			return null;
		}
		
		public object[] FindObjectsFromBundleName(string name)
		{
			//	Trouve les objets qui ont été générés par le bundle dont le nom est
			//	spécifié. Comme un bundle peut servir à générer plusieurs objets, on
			//	retrourne un array.
			
			if (this.store_mapping)
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();
				
				foreach (System.Collections.DictionaryEntry entry in this.obj_to_bundle)
				{
					ResourceBundle bundle = entry.Value as ResourceBundle;
					
					if (bundle.Name == name)
					{
						list.Add (entry.Key);
					}
				}
				
				object[] objs = new object[list.Count];
				list.CopyTo (objs);
				
				return objs;
			}
			
			return new object[0];
		}
		
		
		protected virtual void OnObjectUnbundled(BundlingEventArgs e)
		{
			if (this.ObjectUnbundled != null)
			{
				this.ObjectUnbundled (this, e);
			}
		}
		
		protected virtual void OnPropertyBundled(BundlingPropertyEventArgs e)
		{
			if (this.PropertyBundled != null)
			{
				this.PropertyBundled (this, e);
			}
		}
		
		
		public static System.Xml.XmlDocument		XmlDocument
		{
			get
			{
				System.Diagnostics.Debug.Assert (ObjectBundler.xmldoc != null);
				System.Diagnostics.Debug.Assert (ObjectBundler.xmldoc.ChildNodes.Count == 0);
				
				return ObjectBundler.xmldoc;
			}
		}
		
		
		public event BundlingEventHandler			ObjectUnbundled;
		public event BundlingPropertyEventHandler	PropertyBundled;
		
		protected static Hashtable					classes;
		protected static System.Xml.XmlDocument		xmldoc = new System.Xml.XmlDocument ();
		
		protected bool								store_mapping;
		protected Hashtable							obj_to_bundle;			//	lien entre noms de bundles et objets
	}
}
