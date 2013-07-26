using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.Strings.Bundles
{
	public class ResourceModule : IEnumerable<ResourceBundle>
	{
		public static ResourceModule Load(string moduleInfoFilePath)
		{
			var info      = ResourceModuleInfo.Load (moduleInfoFilePath);
			var fileNames = Directory.EnumerateFiles (Path.GetDirectoryName (moduleInfoFilePath), "*.??.resource");
			var bundles   = fileNames.Select (fn => ResourceBundle.Load (fn)).ToList ();

			return new ResourceModule (info, bundles);
		}

		private ResourceModule(ResourceModuleInfo info, List<ResourceBundle> bundles)
		{
			this.info           = info;
			this.bundles        = bundles;
			this.byNameFirst    = new Lazy<Dictionary<string, Dictionary<string, ResourceBundle>>> (this.ByNameFirstFactory);
			this.byCultureFirst = new Lazy<Dictionary<string, Dictionary<string, ResourceBundle>>> (this.ByCultureFirstFactory);
		}

		public ResourceModuleInfo Info
		{
			get
			{
				return this.info;
			}
		}

		public Dictionary<string, Dictionary<string, ResourceBundle>> ByNameFirst
		{
			get
			{
				return this.byNameFirst.Value;
			}
		}

		public Dictionary<string, Dictionary<string, ResourceBundle>> ByCultureFirst
		{
			get
			{
				return this.byCultureFirst.Value;
			}
		}

		#region IEnumerable<Bundle> Members

		public IEnumerator<ResourceBundle> GetEnumerator()
		{
			return this.bundles.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.bundles.GetEnumerator ();
		}

		#endregion

		private Func<Dictionary<string, Dictionary<string, ResourceBundle>>> ByNameFirstFactory
		{
			get
			{
				return () =>
					(from n in this.bundles
					 group n by n.Name into ng
					 select new
					 {
						 Name = ng.Key,
						 NameGroups =
							(from c in ng
							 group c by c.Culture into cg
							 select new
							 {
								 Culture = cg.Key,
								 CultureGroups = cg
							 }).ToDictionary (a => a.Culture, a => a.CultureGroups.Single ())
					 }).ToDictionary (a => a.Name, a => a.NameGroups);
			}
		}

		private Func<Dictionary<string, Dictionary<string, ResourceBundle>>> ByCultureFirstFactory
		{
			get
			{
				return () =>
					(from c in this.bundles
					 group c by c.Culture into cg
					 select new
					 {
						 Culture = cg.Key,
						 CultureGroups =
						(from n in cg
						 group n by n.Name into ng
						 select new
						 {
							 Name = ng.Key,
							 NameGroups = ng
						 }).ToDictionary (a => a.Name, a => a.NameGroups.Single ())
					 }).ToDictionary (a => a.Culture, a => a.CultureGroups);
			}
		}

		private readonly ResourceModuleInfo info;
		private readonly List<ResourceBundle> bundles;
		private readonly Lazy<Dictionary<string, Dictionary<string, ResourceBundle>>> byNameFirst;
		private readonly Lazy<Dictionary<string, Dictionary<string, ResourceBundle>>> byCultureFirst;
	}
}
