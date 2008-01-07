//	Copyright � 2006-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Serialization
{
	using Assembly = System.Reflection.Assembly;
	
	public sealed class DependencyClassManager
	{
		private DependencyClassManager()
		{
			this.domain     = System.AppDomain.CurrentDomain;
			this.assemblies = new List<Assembly> ();
			this.types      = new Dictionary<string, DependencyObjectType> ();
			this.converters = new Dictionary<System.Type, ISerializationConverter> ();

			Assembly[] assemblies = this.domain.GetAssemblies ();
			
			this.domain.AssemblyLoad += new System.AssemblyLoadEventHandler (this.HandleDomainAssemblyLoad);

			foreach (Assembly assembly in assemblies)
			{
				this.Analyse (assembly);
			}
		}

		public static DependencyClassManager	Current
		{
			get
			{
				DependencyClassManager.Setup ();
				return DependencyClassManager.current;
			}
		}

		public static bool						IsReady
		{
			get
			{
				return DependencyClassManager.current != null;
			}
		}

		public static void Setup()
		{
			if (DependencyClassManager.current == null)
			{
				System.Diagnostics.Debug.Assert (DependencyClassManager.isBooting == false);

				DependencyClassManager.isBooting = true;
				DependencyClassManager.current   = new DependencyClassManager ();
				DependencyClassManager.isBooting = false;

				if (DependencyClassManager.bootingActions != null)
				{
					while (DependencyClassManager.bootingActions.Count > 0)
					{
						DependencyClassManager.bootingActions.Dequeue () ();
					}

					DependencyClassManager.bootingActions = null;
				}
			}
		}


		/// <summary>
		/// Finds the object type for the given name.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The object type or <c>null</c> if none can be found.</returns>
		public DependencyObjectType FindObjectType(string name)
		{
			DependencyObjectType objectType;
			
			if (this.types.TryGetValue (name, out objectType))
			{
				return objectType;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Finds the serialization converter for the specified type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>The serialization converter or <c>null</c> if none can be found.</returns>
		public ISerializationConverter FindSerializationConverter(System.Type type)
		{
			ISerializationConverter converter;
			
			if (this.converters.TryGetValue (type, out converter))
			{
				return converter;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Executes the specified initialization code, either synchronously if
		/// no explicit static constructor call was done by the dependency system,
		/// or postponed later.
		/// </summary>
		/// <param name="callback">The initialization code callback.</param>
		public static void ExecuteInitializationCode(Support.SimpleCallback callback)
		{
			if (DependencyClassManager.isBooting)
			{
				if (DependencyClassManager.bootingActions == null)
				{
					DependencyClassManager.bootingActions = new Queue<Support.SimpleCallback> ();
				}

				DependencyClassManager.bootingActions.Enqueue (callback);
			}
			else if (DependencyObjectType.IsExecutingStaticConstructor)
			{
				if (DependencyClassManager.pendingActions == null)
				{
					DependencyClassManager.pendingActions = new Queue<Support.SimpleCallback> ();
				}

				DependencyClassManager.pendingActions.Enqueue (callback);
			}
			else
			{
				callback ();
			}
		}

		/// <summary>
		/// Executes the pending initialization code.
		/// </summary>
		internal static void ExecutePendingInitializationCode()
		{
			if (!DependencyObjectType.IsExecutingStaticConstructor)
			{
				if (DependencyClassManager.pendingActions != null)
				{
					while (DependencyClassManager.pendingActions.Count > 0)
					{
						if (DependencyClassManager.analyseCount > 0)
						{
							break;
						}
						
						DependencyClassManager.pendingActions.Dequeue () ();
					}

					if (DependencyClassManager.pendingActions.Count == 0)
					{
						DependencyClassManager.pendingActions = null;
					}
				}
			}
		}

		private void Analyse(Assembly assembly)
		{
			DependencyClassManager.analyseCount++;

			try
			{
				foreach (System.Type type in DependencyClassAttribute.GetRegisteredTypes (assembly))
				{
					DependencyObjectType depType = DependencyObjectType.FromSystemType (type);
					string name = type.FullName;

					this.types[name] = depType;
				}

				foreach (DependencyClassAttribute attribute in DependencyClassAttribute.GetConverterAttributes (assembly))
				{
					this.converters[attribute.Type] = System.Activator.CreateInstance (attribute.Converter) as ISerializationConverter;
				}
				
				foreach (DependencyConverterAttribute attribute in DependencyConverterAttribute.GetConverterAttributes (assembly))
				{
					this.converters[attribute.Type] = System.Activator.CreateInstance (attribute.Converter) as ISerializationConverter;
				}
			}
			finally
			{
				DependencyClassManager.analyseCount--;
			}

			DependencyClassManager.ExecutePendingInitializationCode ();
		}

		private void HandleDomainAssemblyLoad(object sender, System.AssemblyLoadEventArgs args)
		{
			if (!args.LoadedAssembly.ReflectionOnly)
			{
				this.assemblies.Add (args.LoadedAssembly);
				this.Analyse (args.LoadedAssembly);
			}
		}
		
		private static DependencyClassManager	current;
		private static bool						isBooting;
		
		private System.AppDomain				domain;
		private List<Assembly>					assemblies;
		private Dictionary<string, DependencyObjectType> types;
		private Dictionary<System.Type, ISerializationConverter> converters;

		[System.ThreadStatic]
		private static Queue<Support.SimpleCallback> pendingActions;
		private static Queue<Support.SimpleCallback> bootingActions;

		[System.ThreadStatic]
		private static int analyseCount;
	}
}
