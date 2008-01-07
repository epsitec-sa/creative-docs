//	Copyright © 2006-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;

namespace Epsitec.Common.UI.Controllers
{
	using Assembly=System.Reflection.Assembly;

	/// <summary>
	/// The <c>Controllers.Factory</c> class is used to create and configure
	/// controller instances used by the <see cref="Placeholder"/> class.
	/// </summary>
	public static class Factory
	{
		/// <summary>
		/// Creates the controller based on its name and parameter.
		/// </summary>
		/// <param name="name">The controller name.</param>
		/// <param name="parameters">The optional controller parameters.</param>
		/// <returns>An object implementing <see cref="IController"/> or
		/// <c>null</c> if the specified controller cannot be found.</returns>
		public static IController CreateController(string name, ControllerParameters parameters)
		{
			Record record;
			
			if (Factory.types.TryGetValue (name, out record))
			{
				return record.CreateController (parameters);
			}
			
			return null;
		}

		/// <summary>
		/// Gets the default controller and its parameters based on the binding.
		/// The information is derived from the data type.
		/// </summary>
		/// <param name="binding">The binding.</param>
		/// <param name="controllerName">The controller name.</param>
		/// <param name="controllerParameter">The controller parameter.</param>
		/// <returns><c>true</c> if a controller definition could be derived
		/// from the binding; otherwise, <c>false</c>.</returns>
		public static bool GetDefaultController(BindingExpression binding, out string controllerName, out string controllerParameter)
		{
			controllerName = null;
			controllerParameter = null;
			
			if (binding == null)
			{
				return false;
			}

			INamedType type = binding.GetSourceNamedType ();

			if (type == null)
			{
				return false;
			}

			controllerName = type.DefaultController;
			controllerParameter = type.DefaultControllerParameters;
			
			return string.IsNullOrEmpty (controllerName) ? false : true;
		}

		public static void Setup()
		{
		}

		#region Setup and Run-Time Analysis Methods

		static Factory()
		{
			Factory.domain     = System.AppDomain.CurrentDomain;
			Factory.assemblies = new List<Assembly> ();
			Factory.types      = new Dictionary<string, Record> ();

			Assembly[] assemblies = Factory.domain.GetAssemblies ();

			Factory.domain.AssemblyLoad += new System.AssemblyLoadEventHandler (Factory.HandleDomainAssemblyLoad);

			foreach (Assembly assembly in assemblies)
			{
				Factory.Analyse (assembly);
			}
		}

		private static void Analyse(Assembly assembly)
		{
			foreach (System.Type type in ControllerAttribute.GetRegisteredTypes (assembly))
			{
				string name = type.Name;
				string suffix = "Controller";
				Record record = new Record (type);

				if (name.EndsWith (suffix))
				{
					name = name.Substring (0, name.Length-suffix.Length);
					Factory.types[name] = record;
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
				Factory.assemblies.Add (args.LoadedAssembly);
				Factory.Analyse (args.LoadedAssembly);
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
			
			public IController CreateController(ControllerParameters parameters)
			{
				if (this.allocator == null)
				{
					lock (this.exclusion)
					{
						if (this.allocator == null)
						{
							this.allocator = Support.DynamicCodeFactory.CreateAllocator<IController, ControllerParameters> (this.type);
						}
					}
				}

				return this.allocator (parameters);
			}

			private readonly object exclusion;
			private Support.Allocator<IController, ControllerParameters> allocator;
			private System.Type type;
		}

		#endregion

		private static readonly System.AppDomain domain;
		private static readonly List<Assembly> assemblies;
		private static readonly Dictionary<string, Record> types;
	}
}
