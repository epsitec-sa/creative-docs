//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support.EntityEngine
{
	using Assembly=System.Reflection.Assembly;

	/// <summary>
	/// The <c>EntityResolver</c> class is used to allocate entity instances
	/// based on entity ids. The mapping between entity id and entity class
	/// must be marked with the <see cref="EntityAttribute"/> attribute, at
	/// the <c>assembly</c> level.
	/// </summary>
	public static class EntityResolver
	{
		/// <summary>
		/// Creates an empty entity instance.
		/// </summary>
		/// <param name="id">The entity id.</param>
		/// <returns>The new entity instance or <c>null</c> if the id
		/// cannot be resolved.</returns>
		public static AbstractEntity CreateEmptyEntity(Druid id)
		{
			Record record;

			if (EntityResolver.types.TryGetValue (id, out record))
			{
				return record.CreateInstance ();
			}
			else
			{
				return null;
			}
		}

		public static void Setup()
		{
		}

		#region Setup and Run-Time Analysis Methods

		static EntityResolver()
		{
			EntityResolver.domain     = System.AppDomain.CurrentDomain;
			EntityResolver.assemblies = new List<Assembly> ();
			EntityResolver.types      = new Dictionary<Druid, Record> ();

			Assembly[] assemblies = EntityResolver.domain.GetAssemblies ();

			EntityResolver.domain.AssemblyLoad += new System.AssemblyLoadEventHandler (EntityResolver.HandleDomainAssemblyLoad);

			foreach (Assembly assembly in assemblies)
			{
				EntityResolver.Analyse (assembly);
			}
		}

		private static void Analyse(Assembly assembly)
		{
			foreach (EntityAttribute attribute in EntityAttribute.GetRegisteredAttributes (assembly))
			{
				string name = attribute.EntityType.Name;
				string suffix = "Entity";
				Record record = new Record (attribute.EntityType);

				if (name.EndsWith (suffix))
				{
					EntityResolver.types[attribute.EntityId] = record;
				}
				else
				{
					System.Diagnostics.Debug.WriteLine (string.Format ("Type '{0}' specifies ControllerAttribute but does not follow naming conventions", name));
				}
			}
		}

		private static void HandleDomainAssemblyLoad(object sender, System.AssemblyLoadEventArgs args)
		{
			if (!args.LoadedAssembly.ReflectionOnly)
			{
				EntityResolver.assemblies.Add (args.LoadedAssembly);
				EntityResolver.Analyse (args.LoadedAssembly);
			}
		}

		#endregion

		#region Private Record Structure

		private struct Record
		{
			public Record(System.Type type)
			{
				this.exclusion = new object ();
				this.allocator = null;
				this.type = type;
			}

			public AbstractEntity CreateInstance()
			{
				if (this.allocator == null)
				{
					lock (this.exclusion)
					{
						if (this.allocator == null)
						{
							this.allocator = Support.DynamicCodeFactory.CreateAllocator<AbstractEntity> (this.type);
						}
					}
				}

				return this.allocator ();
			}

			private readonly object exclusion;
			private Support.Allocator<AbstractEntity> allocator;
			private System.Type type;
		}

		#endregion

		private static System.AppDomain domain;
		private static List<Assembly> assemblies;
		private static Dictionary<Druid, Record> types;
	}
}
