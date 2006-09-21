//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

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
		
		
		public override System.Type BindToType(string assemblyName, string typeName) 
		{
			string fullName = string.Concat (typeName, ", ", assemblyName);
			System.Type type;
			
			if (GenericDeserializationBinder.typeCache.TryGetValue (fullName, out type) == false)
			{
				//	Premier essai: trouve le type exact correspondant à ce qui est
				//	demandé par la désérialisation.

				type = GenericDeserializationBinder.SafeGetType (fullName);
				
				if (type == null)
				{
					//	Second essai: trouve le type équivalent dans l'assembly avec la
					//	version courante, plutôt que celle avec la version spécifiée.
					
					type = this.FindReplacementType (assemblyName, typeName);
				}
				
				if (type == null)
				{
					//	Troisième essai: trouve le type équivalent dans une autre
					//	assembly, avec une heuristique maison.
					
					if (assemblyName.StartsWith ("Common.Drawing,"))
					{
						assemblyName = assemblyName.Replace ("Common.Drawing,", "Common.Drawing.Agg,");
						type = this.FindReplacementType (assemblyName, typeName);
					}
				}
				
				System.Diagnostics.Debug.Assert (type != null);
				GenericDeserializationBinder.typeCache[fullName] = type;
			}
			
			return type;
		}

		[System.Diagnostics.DebuggerHidden]
		public static System.Type SafeGetType(string name)
		{
			try
			{
				return System.Type.GetType (name, false, false);
			}
			catch (System.IO.FileLoadException)
			{
			}

			return null;
		}
		
		protected virtual System.Type FindReplacementType(string assemblyName, string typeName)
		{
			string prefix = assemblyName.Substring (0, assemblyName.IndexOf (' '));
					
			foreach (System.Reflection.Assembly assembly in GenericDeserializationBinder.assemblies)
			{
				if (assembly.FullName.StartsWith (prefix))
				{
					string fullName = string.Concat (typeName, ", ", assembly.FullName);
					return GenericDeserializationBinder.SafeGetType (fullName);
				}
			}
			
			return null;
		}
		
		
		static GenericDeserializationBinder()
		{
			GenericDeserializationBinder.assemblies = new List<System.Reflection.Assembly> ();
			GenericDeserializationBinder.typeCache = new Dictionary<string, System.Type> ();

			GenericDeserializationBinder.assemblies.AddRange (System.AppDomain.CurrentDomain.GetAssemblies ());

			System.AppDomain.CurrentDomain.AssemblyLoad += GenericDeserializationBinder.HandleAssemblyLoad;
		}

		private static void HandleAssemblyLoad(object sender, System.AssemblyLoadEventArgs e)
		{
			GenericDeserializationBinder.assemblies.Add (e.LoadedAssembly);
		}
		
		
		static List<System.Reflection.Assembly>	assemblies;
		static Dictionary<string, System.Type>	typeCache;
	}
}
