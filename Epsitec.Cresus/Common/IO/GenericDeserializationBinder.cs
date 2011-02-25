//	Copyright © 2005-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

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
			assemblyName = GenericDeserializationBinder.FixAssemblyName (assemblyName);

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

		private static string FixAssemblyName(string assemblyFullName)
		{
			int pos = assemblyFullName.IndexOf (',');
			string assemblyName = assemblyFullName.Substring (0, pos);
			string assemblyInfo = assemblyFullName.Substring (pos);
			bool replace = false;

			if (assemblyName.StartsWith ("Common.Drawing"))
			{
				replace = true;
			}
			else
			{
				switch (assemblyName)
				{
					case "Common.Types":
					case "Common.Support":
					case "Common.Widgets":
					case "Common.OpenType":
						replace = true;
						break;
				}
			}

			if (replace)
			{
				return "Common" + assemblyInfo;
			}
			else
			{
				return assemblyFullName;
			}
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
		
		private System.Type FindReplacementType(string assemblyName, string typeName)
		{
			string prefix = assemblyName.Substring (0, assemblyName.IndexOf (' '));

			if (typeName.Contains ("`"))
			{
				//	Ouch, this is a generic type name and it will have more than
				//	one embedded assembly reference. Cut the type specification
				//	into manageable "[...]" chunks and replace the assembly versions
				//	with the loaded ones.

				int end;
				int pos = typeName.IndexOf ('[');

				System.Text.StringBuilder name = new System.Text.StringBuilder ();
				name.Append (typeName.Substring (0, pos));

				while (pos < typeName.Length)
				{
					while (typeName[pos] == '[')
					{
						name.Append ('[');
						pos++;
					}

					end = typeName.IndexOf (']', pos);

					string part = typeName.Substring (pos, end-pos);
					string partType = part.Substring (0, part.IndexOf (','));
					string partAssembly = part.Substring (part.IndexOf (',')+2);
					string partPrefix = partAssembly.Substring (0, partAssembly.IndexOf (' '));
					string replacement = GenericDeserializationBinder.FindReplacementTypeName (partPrefix, partType);

					name.Append (replacement);

					pos = end;
					
					while ((pos < typeName.Length)
						&& (typeName[pos] != '['))
					{
						name.Append (typeName[pos]);
						pos++;
					}
				}

				typeName = name.ToString ();
			}
			
			string fullName = GenericDeserializationBinder.FindReplacementTypeName (prefix, typeName);
			return fullName == null ? null : GenericDeserializationBinder.SafeGetType (fullName);
		}

		private static string FindReplacementTypeName(string prefix, string typeName)
		{
			if (prefix.Contains (","))
			{
				prefix = GenericDeserializationBinder.FixAssemblyName (prefix);
			}

			foreach (System.Reflection.Assembly assembly in GenericDeserializationBinder.assemblies)
			{
				if (assembly.FullName.StartsWith (prefix))
				{
					return string.Concat (typeName, ", ", assembly.FullName);
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
			if (!e.LoadedAssembly.ReflectionOnly)
			{
				GenericDeserializationBinder.assemblies.Add (e.LoadedAssembly);
			}
		}
		
		
		static List<System.Reflection.Assembly>	assemblies;
		static Dictionary<string, System.Type>	typeCache;
	}
}
