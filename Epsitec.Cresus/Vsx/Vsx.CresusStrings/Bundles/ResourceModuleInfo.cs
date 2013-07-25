using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Epsitec.Cresus.Strings.Bundles
{
	public class ResourceModuleInfo
	{
		public static ResourceModuleInfo Load(string fileName)
		{
			var doc = XDocument.Load (fileName);
			return new ResourceModuleInfo (doc.Root);
		}

		public ResourceModuleInfo(XElement element)
		{
			this.id = element.Attribute ("id").GetString ();
			this.name = element.Attribute ("name").GetString ();
			this.layer = element.Attribute ("layer").GetString ();
			this.textMode = element.Attribute ("textMode").GetString ();
			this.assemblies = element.Attribute ("assemblies").GetString ();
			this.@namespace = element.Attribute ("namespace").GetString ();
			this.namespaceRes = element.Attribute ("namespaceRes").GetString ();
			this.versions = new ResourceVersions (element.Element ("Versions"));
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public string Layer
		{
			get
			{
				return this.layer;
			}
		}

		public string TextMode
		{
			get
			{
				return this.textMode;
			}
		}

		public string Assemblies
		{
			get
			{
				return this.assemblies;
			}
		}

		public string Namespace
		{
			get
			{
				return this.@namespace;
			}
		}

		public string NamespaceRes
		{
			get
			{
				return this.namespaceRes;
			}
		}

		public ResourceVersions Versions
		{
			get
			{
				return this.versions;
			}
		}

		private readonly string id;
		private readonly string name;
		private readonly string layer;
		private readonly string textMode;
		private readonly string assemblies;
		private readonly string @namespace;
		private readonly string namespaceRes;
		private readonly ResourceVersions versions;
	}
}
