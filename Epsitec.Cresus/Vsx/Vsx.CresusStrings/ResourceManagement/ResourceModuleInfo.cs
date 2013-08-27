using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epsitec.Cresus.ResourceManagement
{
	public class ResourceModuleInfo
	{
		public ResourceModuleInfo(string filePath)
		{
			var doc = XDocument.Load (filePath);
			this.filePath = filePath;
			this.element = doc.Root;
			this.versions = new ResourceVersions (element.Element ("Versions"));
		}

		public string Id
		{
			get
			{
				return this.element.Attribute ("id").GetString ();
			}
		}

		public string Name
		{
			get
			{
				return this.element.Attribute ("name").GetString ();
			}
		}

		public string Layer
		{
			get
			{
				return this.element.Attribute ("layer").GetString ();
			}
		}

		public string TextMode
		{
			get
			{
				return this.element.Attribute ("textMode").GetString ();
			}
		}

		public string Assemblies
		{
			get
			{
				return this.element.Attribute ("assemblies").GetString ();
			}
		}

		public string DefaultNamespace
		{
			get
			{
				return this.element.Attribute ("namespace").GetString ();
			}
		}

		public string ResourceNamespace
		{
			get
			{
				var resourceNamespace = this.element.Attribute ("namespaceRes").GetString ();
				return resourceNamespace ?? this.DefaultNamespace;
			}
		}

		public ResourceVersions Versions
		{
			get
			{
				return this.versions;
			}
		}

		public string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		#region Object Overrides

		public override string ToString()
		{
			return string.Format ("Id : {0}, Folder : {1}", this.Id, System.IO.Path.GetDirectoryName(this.FilePath));
		}
		#endregion

		private readonly string filePath;
		private readonly XElement element;
		private readonly ResourceVersions versions;
	}
}
