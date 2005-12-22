//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.IO
{
	/// <summary>
	/// La classe GenericDeserializationBinder permet de d�s�rialiser des donn�es
	/// en ignorant explicitement la r�vision du type correspondant.
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
				//	Premier essai: trouve le type exact correspondant � ce qui est
				//	demand� par la d�s�rialisation.
				
				type = System.Type.GetType (full_name);
				
				if (type == null)
				{
					//	Second essai: trouve le type �quivalent dans l'assembly avec la
					//	version courante, plut�t que celle avec la version sp�cifi�e.
					
					type = this.FindReplacementType (assembly_name, type_name);
				}
				
				if (type == null)
				{
					//	Troisi�me essai: trouve le type �quivalent dans une autre
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
