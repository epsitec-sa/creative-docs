﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceModule : ResourceNode, IEnumerable<ResourceBundle>
	{
		public static ResourceModule Load(string moduleInfoFilePath, ProjectResource project = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (File.Exists (moduleInfoFilePath))
			{
				cancellationToken.ThrowIfCancellationRequested ();
				var info = new ResourceModuleInfo (moduleInfoFilePath);
				cancellationToken.ThrowIfCancellationRequested ();

				var moduleBundles = new List<ResourceBundle> ();
				var module = new ResourceModule (info, moduleBundles, project);
				var bundles = ResourceModule.LoadBundles (Path.GetDirectoryName (moduleInfoFilePath), module, cancellationToken).ToList ();
				if (bundles.Any())
				{
					moduleBundles.AddRange (bundles);
					return module;
				}
			}
			return null;
		}

		public ResourceModule(ResourceModule module, IEnumerable<ResourceBundle> bundles)
			: this (module.Info, bundles.ToList (), module.project)
		{
		}

		private ResourceModule(ResourceModuleInfo info, IEnumerable<ResourceBundle> bundles, ProjectResource project)
		{
			this.project		 = project;
			this.info            = info;
			this.bundles         = bundles;
			this.byNameFirst     = new Lazy<Dictionary<string, Dictionary<CultureInfo, ResourceBundle>>> (this.ByNameFirstFactory);
			this.byCultureFirst  = new Lazy<Dictionary<CultureInfo, Dictionary<string, ResourceBundle>>> (this.ByCultureFirstFactory);
		}

		public ProjectResource					Project
		{
			get
			{
				return this.project;
			}
		}
		public ResourceModuleInfo				Info
		{
			get
			{
				return this.info;
			}
		}

		public IEnumerable<string> TouchedFilePathes()
		{
			yield return this.Info.FilePath;
			foreach (var bundle in this)
			{
				yield return bundle.FilePath;
			}
		}

		public IReadOnlyDictionary<string, Dictionary<CultureInfo, ResourceBundle>> ByNameFirst
		{
			get
			{
				return this.byNameFirst.Value;
			}
		}

		public IReadOnlyDictionary<CultureInfo, Dictionary<string, ResourceBundle>> ByCultureFirst
		{
			get
			{
				return this.byCultureFirst.Value;
			}
		}

		public ResourceBundle GetNeutralCultureBundle(ResourceBundle bundle)
		{
			return this.ByNameFirst[bundle.Name].Values.FirstOrDefault ();
		}

		#region Object Overrides
		
		public override string ToString()
		{
			return this.Info.ToString ();
		}

		#endregion
	
		#region ResourceNode Overrides

		public override ResourceNode Accept(ResourceVisitor visitor)
		{
			return visitor.VisitModule (this);
		}

		#endregion

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

		private Dictionary<string, Dictionary<CultureInfo, ResourceBundle>> ByNameFirstFactory()
		{
			return
			(
				from n in this
				group n by n.Name into ng
				select new
				{
					Name = ng.Key,
					NameGroups =
					(
						from c in ng
						group c by c.Culture into cg
						select new
						{
							Culture = cg.Key,
							CultureGroups = cg
						}
					).ToDictionary (a => a.Culture, a => a.CultureGroups.Single ())
				}
			).ToDictionary (a => a.Name, a => a.NameGroups);
		}

		private Dictionary<CultureInfo, Dictionary<string, ResourceBundle>> ByCultureFirstFactory()
		{
			return
			(
				from c in this
				group c by c.Culture into cg
				select new
				{
					Culture = cg.Key,
					CultureGroups =
					(
						from n in cg
						group n by n.Name into ng
						select new
						{
							Name = ng.Key,
							NameGroups = ng
						}
					).ToDictionary (a => a.Name, a => a.NameGroups.Single ())
				}
			).ToDictionary (a => a.Culture, a => a.CultureGroups);
		}

		private static IEnumerable<ResourceBundle> LoadBundles(string directory, ResourceModule module, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested ();
			return
				ResourceModule.EnumerateBundles (directory, module)
				.Do (_ => cancellationToken.ThrowIfCancellationRequested ());
		}

		private static IEnumerable<ResourceBundle> EnumerateBundles(string directory, ResourceModule module)
		{
			var fileNames = Directory.EnumerateFiles (directory, "*.??.resource").Select (fn => fn.ToLower ());
			if (fileNames.Any ())
			{
				var byNameFirst =
					from fn in fileNames
					let fnwe = Path.GetFileNameWithoutExtension (fn)
					let name = Path.GetFileNameWithoutExtension (fnwe)
					group new
					{
						Name = name,
						Culture = Path.GetExtension (fnwe).Substring (1),
						FileName = fn
					} by name into nameGroups
					select new
					{
						Name = nameGroups.Key,
						NameGroups =
							from c in nameGroups
							orderby c.Culture ascending
							select new
							{
								Culture = c.Culture,
								Name = c.Name,
								FileName = c.FileName
							}
					};

				foreach (var nameGrouping in byNameFirst)
				{
					var name = nameGrouping.Name;
					var neutralGroup = nameGrouping.NameGroups.First ();
					var otherGroups = nameGrouping.NameGroups.Skip (1);

					var neutralBundle = ResourceBundle.Load (neutralGroup.FileName, module, null);
					if (neutralBundle != null)
					{
						yield return neutralBundle;

						foreach (var otherGroup in otherGroups)
						{
							var otherBundle = ResourceBundle.Load (otherGroup.FileName, module, neutralBundle);
							if (otherBundle != null)
							{
								yield return otherBundle;
							}
						}
					}
				}
			}
		}


		private readonly ProjectResource project;
		private readonly ResourceModuleInfo info;
		private readonly IEnumerable<ResourceBundle> bundles;
		private readonly Lazy<Dictionary<string, Dictionary<CultureInfo, ResourceBundle>>> byNameFirst;
		private readonly Lazy<Dictionary<CultureInfo, Dictionary<string, ResourceBundle>>> byCultureFirst;
	}
}
