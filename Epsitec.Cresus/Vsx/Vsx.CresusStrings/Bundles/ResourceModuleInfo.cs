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
			this.element = element;
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

		public string Namespace
		{
			get
			{
				return this.element.Attribute ("namespace").GetString ();
			}
		}

		public string NamespaceRes
		{
			get
			{
				return this.element.Attribute ("namespaceRes").GetString ();
			}
		}

		public ResourceVersions Versions
		{
			get
			{
				return this.versions;
			}
		}

		private readonly XElement element;
		private readonly ResourceVersions versions;
	}
}
