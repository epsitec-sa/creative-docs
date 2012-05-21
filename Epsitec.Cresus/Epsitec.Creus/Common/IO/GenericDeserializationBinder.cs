//	Copyright © 2005-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.IO
{
	/// <summary>
	/// La classe GenericDeserializationBinder permet de désérialiser des données
	/// en ignorant explicitement la révision du type correspondant.
	/// </summary>
	public sealed class GenericDeserializationBinder : System.Runtime.Serialization.SerializationBinder
	{
		public GenericDeserializationBinder(System.Func<string, string, System.Type> bindToTypeFunc = null)
		{
			this.bindToTypeFunc = bindToTypeFunc;
		}
		
		
		public override System.Type BindToType(string assemblyName, string typeName)
		{
			System.Type type;
			
			if (this.bindToTypeFunc != null)
			{
				//	If there is a binding function, call it first; if it provides a type,
				//	use it, otherwise, apply the default binding logic.

				type = this.bindToTypeFunc (assemblyName, typeName);

				if (type != null)
				{
					return type;
				}
			}

			//	Tons of old documents still reference old assembly names; we have to
			//	map names such as "Common.Drawing.Agg" to "Common" :
			
			assemblyName = GenericDeserializationBinder.FixAssemblyName (assemblyName);

			string fullName = GenericDeserializationBinder.GetFullName (assemblyName, typeName);

			lock (GenericDeserializationBinder.typeCache)
			{
				GenericDeserializationBinder.typeCache.TryGetValue (fullName, out type);
			}
			
			if (type == null)
			{
				type = GenericDeserializationBinder.SafeGetType (fullName)
					?? this.FindReplacementType (assemblyName, typeName);
				
				System.Diagnostics.Debug.Assert (type != null);
				
				lock (GenericDeserializationBinder.typeCache)
				{
					GenericDeserializationBinder.typeCache[fullName] = type;
				}
			}
			
			return type;
		}


		private static string GetFullName(string assemblyName, string typeName)
		{
			return string.Concat (typeName, ", ", assemblyName);
		}

		private static string FixAssemblyName(string assemblyFullName)
		{
			int pos = assemblyFullName.IndexOf (',');

			var assemblyName = assemblyFullName.Substring (0, pos);
			var assemblyInfo = assemblyFullName.Substring (pos);

			switch (assemblyName)
			{
				case "Common.Drawing":
				case "Common.Drawing.Agg":
				case "Common.Types":
				case "Common.Support":
				case "Common.Widgets":
				case "Common.OpenType":
					return "Common" + assemblyInfo;
				
				default:
					return assemblyFullName;
			}
		}

		[System.Diagnostics.DebuggerHidden]
		private static System.Type SafeGetType(string name)
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

			return TypeEnumerator.Instance.GetLoadedAssemblies ()
				.Where (assembly => assembly.FullName.StartsWith (prefix))
				.Select (assembly => GenericDeserializationBinder.GetFullName (assembly.FullName, typeName))
				.FirstOrDefault ();
		}
		
		static readonly Dictionary<string, System.Type>	typeCache = new Dictionary<string, System.Type> ();

		private readonly System.Func<string, string, System.Type> bindToTypeFunc;
	}
}
