//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

namespace Epsitec.Common.Support
{
	using System.Collections;
	
	/// <summary>
	/// La classe ObjectBundler s'occupe de déballer des bundles pour en
	/// faire des objets.
	/// </summary>
	public class ObjectBundler
	{
		private ObjectBundler()
		{
		} 
		
		static ObjectBundler()
		{
			ObjectBundler.classes = new System.Collections.Hashtable ();
			
			//	TODO: énumérer toutes les assembly chargées et identifier toutes les classes
			//	qui implémentent directement l'interface IBundleSupport, puis appeler Register
			//	avec une instance créée dynamiquement pour chacune.
		}
		
		public static void Register(IBundleSupport bundle_support)
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
		
		
		public static object CreateFromBundle(ResourceBundle bundle)
		{
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
			
			obj_type.GetMembers ();
			
			foreach (System.Attribute obj_attr in obj_type.Attributes)
			{
				BundleAttribute attribute = obj_attr as BundleAttribute;
				
				if (attribute != null)
				{
					
				}
			}
			
			//	TODO: restore automatique en fonction des attributs
			
			obj.RestoreFromBundle (bundle);
			
			return obj;
		}
		
		protected static Hashtable		classes;
	}
}
