//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	using System.Collections;
	
	public delegate void BundlingEventHandler(object sender, object obj, ResourceBundle bundle);
	
	/// <summary>
	/// La classe ObjectBundler s'occupe de d�baller des bundles pour en
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
			
			//	TODO: �num�rer toutes les assembly charg�es et identifier toutes les classes
			//	qui impl�mentent directement l'interface IBundleSupport, puis appeler Register
			//	avec une instance cr��e dynamiquement pour chacune.
		}
		
		public static void RegisterAssembly(System.Reflection.Assembly assembly)
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
							//	La classe ne doit pas �tre pise en consid�ration: le fait qu'elle
							//	impl�mente IBundleSupport ne peut pas �tre pris comme indication
							//	de son support. Utile pour des classes sp�ciales (� usage interne
							//	uniquement).
							
							continue;
						}
						
						try
						{
							IBundleSupport bundle_support = System.Activator.CreateInstance (type) as IBundleSupport;
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
		
		public static void Register(IBundleSupport bundle_support)
		{
			System.Diagnostics.Debug.Assert (bundle_support != null);
			System.Diagnostics.Debug.Assert (bundle_support.PublicClassName != null);
			
			string      name = bundle_support.PublicClassName;
			System.Type type = bundle_support.GetType ();
			
			if (ObjectBundler.classes.Contains (name))
			{
				//	Zut, nous avons d�j� une classe avec ce nom. V�rifions si ce n'est pas d�
				//	� un h�ritage intempestif, auquel cas il faut choisir la classe qui est le
				//	plus pr�s de la racine.
				
				System.Type cur_type = ObjectBundler.classes[name] as System.Type;
				System.Type new_type = type;
				
				if (cur_type == new_type)
				{
					return;
				}
				if (new_type.IsSubclassOf (cur_type))
				{
					//	Le nouveau type est d�riv� de celui qui est connu. On garde donc
					//	l'ancien.
					
					return;
				}
				if (cur_type.IsSubclassOf (new_type))
				{
					//	On va remplacer le type anciennement stock� par le nouveau, qui est
					//	un parent de l'ancien...
				}
				else
				{
					throw new System.Exception (string.Format ("Class {0} and class {1} share the name {2}", cur_type.Name, new_type.Name, name));
				}
			}
			
			ObjectBundler.classes[name] = type;
		}
		
		
		public object CreateFromBundle(ResourceBundle bundle)
		{
			//	A partir d'une description stock�e dans un bundle, cr�e un objet de toutes pi�ces
			//	et initialise les propri�t�s connues.
			
			//	Si l'objet est inconnu (ou que le bundle ne d�crit pas un objet), alors on retourne
			//	simplement null sans g�n�rer d'exception.
			
			if (bundle == null)
			{
				return null;
			}
			
			string name = bundle.GetFieldString ("class");
			
			if (name == null)
			{
				return null;
			}
			
			System.Type obj_type = ObjectBundler.classes[name] as System.Type;
			
			if (obj_type == null)
			{
				return null;
			}
			
			IBundleSupport obj = System.Activator.CreateInstance (obj_type) as IBundleSupport;
			
			foreach (System.Reflection.MemberInfo member_info in obj_type.GetMembers (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
			{
				//	Passe en revue tous les membres publics. Ceux qui sont des propri�t�s
				//	vont n�cessiter une attention toute particuli�re...
				
				if (member_info.MemberType == System.Reflection.MemberTypes.Property)
				{
					System.Type type = obj_type;
					System.Reflection.PropertyInfo prop_info = member_info as System.Reflection.PropertyInfo;
					
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
			
			//	Si le ObjectBundler a �t� configur� de mani�re � se souvenir des objets cr��s,
			//	on prend note de l'objet et de son bundle associ�.
			
			if (this.store_mapping)
			{
				this.obj_to_bundle[obj] = bundle;
			}
			
			this.OnObjectUnbundled (obj, bundle);
			
			return obj;
		}
		
		
		public bool IsPropertyEqual(object obj, string property_name, string ref_value)
		{
			//	Compare la valeur de r�f�rence � la valeur de la propri�t� de l'objet sp�ci�.
			//	Retourne true si les deux sont �gaux. Cela peut �tre utile pour savoir si une
			//	valeur diff�re de la valeur par d�faut d'une propri�t� (on passe alors comme
			//	objet en entr�e un objet fra�chement construit).
			
			System.Reflection.PropertyInfo prop_info = this.FindPropertyInfo (obj, property_name);
			
			if (prop_info == null)
			{
				return false;
			}
			
			return this.IsPropertyEqual (obj, prop_info, ref_value);
		}
		
		public bool IsPropertyEqual(object obj, System.Reflection.PropertyInfo prop_info, string ref_value)
		{
			if ((prop_info != null) &&
				(prop_info.CanRead) &&
				(prop_info.IsDefined (typeof (BundleAttribute), true)))
			{
				//	C'est bien une propri�t� qui peut �tre lue et qui a l'attribut [Bundle] d�fini.
				
				string obj_value = prop_info.GetValue (obj, null).ToString  ();
				
				return obj_value == ref_value;
			}
			
			return false;
		}
		
		
		public bool RestoreProperty(ResourceBundle bundle, object obj, string property_name)
		{
			System.Reflection.PropertyInfo prop_info = this.FindPropertyInfo (obj, property_name);
			
			if (prop_info == null)
			{
				return false;
			}
			
			return this.RestoreProperty (bundle, obj, prop_info);
		}
		
		public bool RestoreProperty(ResourceBundle bundle, object obj, System.Reflection.PropertyInfo prop_info)
		{
			//	Pout un objet donn�, fait un "set" de la propri�t� sp�cifi�e en se
			//	basant sur les donn�es stock�es dans le champ correspondant du bundle.
			
			bool ok = false;
			
			if ((prop_info != null) &&
				(prop_info.CanRead) &&
				(prop_info.IsDefined (typeof (BundleAttribute), true)))
			{
				//	C'est bien une propri�t� qui peut �tre lue et qui a l'attribut [Bundle] d�fini.
				
				object[] attributes = prop_info.GetCustomAttributes (typeof (BundleAttribute), true);
				
				System.Diagnostics.Debug.Assert (attributes.Length == 1);
				
				BundleAttribute attr = attributes[0] as BundleAttribute;
				string     prop_name = attr.PropertyName;
				
				switch (bundle.GetFieldType (prop_name))
				{
					case ResourceFieldType.Bundle:
						prop_info.SetValue (obj, this.CreateFromBundle (bundle.GetFieldBundle (prop_name)), null);
						ok = true;
						break;
					
					case ResourceFieldType.String:
						
						//	La valeur source trouv�e dans le bundle est une string. Il faut faire
						//	en sorte que cette valeur puisse �tre affect�e � la propri�t�.
						
						string str_value = bundle.GetFieldString (prop_name);
						
						if ((str_value != null) && (str_value != ""))
						{
							System.Type prop_type = prop_info.PropertyType;
							
							if (prop_type == typeof (string))
							{
								//	C'est un texte. Comme nous avons d�j� une 'string' extraite du bundle,
								//	aucune conversion suppl�mentaire n'est n�cessaire.
								
								prop_info.SetValue (obj, str_value, null);
								ok = true;
							}
							else if (prop_type == typeof (double))
							{
								//	C'est une valeur num�rique qu'il faut convertir... On ne supporte pas
								//	de valeur par d�faut ici, donc on fait simplement un "Parse".
								
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
								//	C'est une �num�ration. On va tenter de convertir la valeur 'string' en
								//	une valeur correspond � l'�num�ration.
								
								object enum_value = System.Enum.Parse (prop_type, str_value, true);
								prop_info.SetValue (obj, enum_value, null);
								ok = true;
							}
							else if (prop_type == typeof (Drawing.Size))
							{
								//	C'est une taille. On va tenter de convertir la valeur 'string' en une
								//	taille, laquelle est stock�e sous la forme 'x;y' o� x et/ou y peuvent
								//	�tre remplac�s par "*" (ce qui signifie: utiliser la valeur par d�faut)
								
								Drawing.Size def_size = (Drawing.Size) prop_info.GetValue (obj, null);
								prop_info.SetValue (obj, Drawing.Size.Parse (str_value, System.Globalization.CultureInfo.InvariantCulture, def_size), null);
								ok = true;
							}
							else if (prop_type == typeof (Drawing.Point))
							{
								//	C'est un point. On va tenter de convertir la valeur 'string' en un
								//	point, lequel est stock� sous la forme 'x;y' o� x et/ou y peuvent
								//	�tre remplac�s par "*" (ce qui signifie: utiliser la valeur par d�faut)
								
								Drawing.Point def_point = (Drawing.Point) prop_info.GetValue (obj, null);
								prop_info.SetValue (obj, Drawing.Point.Parse (str_value, System.Globalization.CultureInfo.InvariantCulture, def_point), null);
								ok = true;
							}
							else if (prop_type == typeof (Drawing.Rectangle))
							{
								//	C'est un rectangle. On va tenter de convertir la valeur 'string' en un
								//	rectangle, lequel est stock� sous la forme 'x;y;dx;dy' o� les arguments
								//	peuvent �tre remplac�s par "*" (valeur par d�faut).
								
								Drawing.Rectangle def_rect = (Drawing.Rectangle) prop_info.GetValue (obj, null);
								prop_info.SetValue (obj, Drawing.Rectangle.Parse (str_value, System.Globalization.CultureInfo.InvariantCulture, def_rect), null);
								ok = true;
							}
							else if (prop_type == typeof (Drawing.Margins))
							{
								//	Ce sont des marges. On va tenter de convertir la valeur 'string' en des
								//	marges, lequelles sont stock�es sous la forme 'x1;x2;y1;y2' o� les arguments
								//	peuvent �tre remplac�s par "*" (ce qui signifie: utiliser la valeur par d�faut)
								
								Drawing.Margins def_margins = (Drawing.Margins) prop_info.GetValue (obj, null);
								prop_info.SetValue (obj, Drawing.Margins.Parse (str_value, def_margins), null);
								ok = true;
							}
							else if (prop_type == typeof (bool))
							{
								//	C'est un bool�en. Accepte "0/1", "off/on", "no/yes", "false/true".
								
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
								//	TODO: g�rer les autres cas fr�quents ici... (types num�riques, couleur, etc.)
							}
						}
						break;
					
					default:
						break;
				}
			}
			
			return ok;
		}
		
		
		public System.Reflection.PropertyInfo FindPropertyInfo(object obj, string property_name)
		{
			//	Parcourt les propri�t�s publiques de l'objet � la recherche de celle
			//	qui a un attribut [Bundle] avec le PropertyName sp�cifi�.
			
			foreach (System.Reflection.MemberInfo member_info in obj.GetType ().GetMembers (System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
			{
				if (member_info.MemberType == System.Reflection.MemberTypes.Property)
				{
					System.Reflection.PropertyInfo prop_info = member_info as System.Reflection.PropertyInfo;
					
					System.Diagnostics.Debug.Assert (prop_info != null);
					
					if ((prop_info != null) &&
						(prop_info.CanRead) &&
						(prop_info.IsDefined (typeof (BundleAttribute), true)))
					{
						//	On vient de trouver une propri�t� qui pourrait faire l'affaire. V�rifions
						//	encore que le nom corresponde.
						
						object[] attributes = prop_info.GetCustomAttributes (typeof (BundleAttribute), true);
						
						System.Diagnostics.Debug.Assert (attributes.Length == 1);
						
						BundleAttribute attr = attributes[0] as BundleAttribute;
						
						if (attr.PropertyName == property_name)
						{
							//	Cette propri�t� est la bonne !
							
							return prop_info;
						}
					}
				}
			}
			
			return null;
		}
		
		
		public ResourceBundle FindBundleFromObject(object obj)
		{
			//	Trouve le bundle qui a servi � g�n�rer un objet.
			
			if (this.store_mapping)
			{
				return this.obj_to_bundle[obj] as ResourceBundle;
			}
			
			return null;
		}
		
		public object[] FindObjectsFromBundleName(string name)
		{
			//	Trouve les objets qui ont �t� g�n�r�s par le bundle dont le nom est
			//	sp�cifi�. Comme un bundle peut servir � g�n�rer plusieurs objets, on
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
		
		
		protected virtual void OnObjectUnbundled(object obj, ResourceBundle bundle)
		{
			if (this.ObjectUnbundled != null)
			{
				this.ObjectUnbundled (this, obj, bundle);
			}
		}
		
		
		public event BundlingEventHandler	ObjectUnbundled;
		
		protected static Hashtable			classes;
		
		protected bool						store_mapping;
		protected Hashtable					obj_to_bundle;			//	lien entre noms de bundles et objets
	}
}
