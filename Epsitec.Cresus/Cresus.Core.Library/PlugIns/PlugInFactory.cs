//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.PlugIns
{
	/// <summary>
	/// The <c>PlugInFactory</c> is used to load and instantiate plug-ins.
	/// </summary>
	public class PlugInFactory
	{
		public PlugInFactory(CoreApp application)
		{
			this.records = new List<PlugInRecord> ();
			this.application = application;

			this.DiscoverPlugIns ();
		}


		public CoreApp							Application
		{
			get
			{
				return this.application;
			}
		}

		
		public IEnumerable<PlugInAttribute> GetPlugInAttributeList()
		{
			return this.records.Select (record => record.Attribute);
		}

		public ICorePlugIn CreatePlugIn(string name)
		{
			var record = this.records.Where (x => x.Attribute.Name == name).FirstOrDefault ();

			if (record == null)
			{
				return null;
			}
			else
			{
				return record.CreatePlugIn (this);
			}
		}

		private void DiscoverPlugIns()
		{
			var constructorArgumentTypes = new System.Type[] { typeof (PlugInFactory) };
			var assemblies = AssemblyLoader.LoadMatching ("*", System.IO.SearchOption.TopDirectoryOnly, loadMode: AssemblyLoadMode.LoadOnlyEpsitecSigned, subfolder: "plugins");

			if (assemblies.Count == 0)
			{
				return;
			}

			var records = from assembly in assemblies
						  from type in assembly.GetTypes ()
						  where type.IsClass
						     && !type.IsAbstract
							 && type.ContainsInterface<ICorePlugIn> ()
							 && type.GetConstructor (constructorArgumentTypes) != null
							 && type.GetCustomAttributes<PlugInAttribute> ().Any ()
						  select new PlugInRecord (type);

			this.records.AddRange (records);

		}

		#region PlugInRecord Class

		private sealed class PlugInRecord
		{
			public PlugInRecord (System.Type type)
			{
				this.type = type;
				this.allocator = DynamicCodeFactory.CreateAllocator<ICorePlugIn, PlugInFactory> (type);
				this.attribute = this.type.GetCustomAttributes<PlugInAttribute> (false).First ();
			}
			
			public PlugInAttribute Attribute
			{
				get
				{
					return this.attribute;
				}
			}

			public ICorePlugIn CreatePlugIn(PlugInFactory factory)
			{
				return this.allocator (factory);
			}

			private readonly System.Type type;
			private readonly Allocator<ICorePlugIn, PlugInFactory> allocator;
			private readonly PlugInAttribute attribute;
		}

		#endregion

		private readonly List<PlugInRecord>		records;
		private readonly CoreApp				application;
	}
}
