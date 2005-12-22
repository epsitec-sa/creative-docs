//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.IO
{
	/// <summary>
	/// La classe GenericDeserializationBinder permet de désérialiser des données
	/// en ignorant explicitement la révision du type correspondant.
	/// </summary>
	public class GenericDeserializationBinder : System.Runtime.Serialization.SerializationBinder
	{
		public GenericDeserializationBinder()
		{
		}
		
		
		public override System.Type BindToType(string assembly_name, string type_name) 
		{
			string full_name = string.Concat (type_name, ", ", assembly_name);
			System.Type type = GenericDeserializationBinder.type_cache[full_name] as System.Type;
			
			if (type == null)
			{
				//	Premier essai: trouve le type exact correspondant à ce qui est
				//	demandé par la désérialisation.
				
				type = System.Type.GetType (full_name);
				
				if (type == null)
				{
					//	Second essai: trouve le type équivalent dans l'assembly avec la
					//	version courante, plutôt que celle avec la version spécifiée.
					
					type = this.FindReplacementType (assembly_name, type_name);
				}
				
				if (type == null)
				{
					//	Troisième essai: trouve le type équivalent dans une autre
					//	assembly, avec une heuristique maison.
					
					if (assembly_name.IndexOf ("Common.Drawing") >= 0)
					{
						type = this.FindReplacementType (assembly_name.Replace ("Common.Drawing", "Common.Drawing.Agg"), type_name);
					}
				}
				
				System.Diagnostics.Debug.Assert (type != null);
				GenericDeserializationBinder.type_cache[full_name] = type;
			}
			
			return type;
		}
		
		
		protected virtual System.Type FindReplacementType(string assembly_name, string type_name)
		{
			string prefix = assembly_name.Substring (0, assembly_name.IndexOf (' '));
					
			foreach (System.Reflection.Assembly assembly in GenericDeserializationBinder.assemblies)
			{
				if (assembly.FullName.StartsWith (prefix))
				{
					string full_name = string.Concat (type_name, ", ", assembly.FullName);
					return System.Type.GetType (full_name);
				}
			}
			
			return null;
		}
		
		
		static GenericDeserializationBinder()
		{
			GenericDeserializationBinder.assemblies = System.AppDomain.CurrentDomain.GetAssemblies ();
			GenericDeserializationBinder.type_cache = new System.Collections.Hashtable ();
		}
		
		
		static System.Reflection.Assembly[]		assemblies;
		static System.Collections.Hashtable		type_cache;
	}
}
