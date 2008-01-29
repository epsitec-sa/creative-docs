//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.FormEngine
{
	using Keywords=CodeHelper.Keywords;

	public sealed class CodeGenerator
	{
		public CodeGenerator(ResourceManager resourceManager, IEnumerable<ResourceBundle> bundles)
		{
			this.formatter = new CodeFormatter ();
			this.bundles = new List<ResourceBundle> (bundles);
			
			this.resourceManager = resourceManager;
			this.resourceManagerPool = this.resourceManager.Pool;
			this.resourceModuleInfo = this.resourceManagerPool.GetModuleInfo (this.resourceManager.DefaultModulePath);
			this.sourceNamespace = this.resourceModuleInfo.SourceNamespace;
		}

		public void Emit()
		{
			CodeHelper.EmitHeader (this.formatter);

			this.formatter.WriteBeginNamespace (this.sourceNamespace);
			this.formatter.WriteBeginClass (CodeHelper.StaticClassAttributes, "FormIds");

			foreach (ResourceBundle bundle in this.bundles)
			{
				Druid  id   = bundle.Id;
				string name = bundle.Caption;

				this.formatter.WriteField (CodeHelper.StaticReadOnlyFieldAttributes,
					/**/				   Keywords.Druid, " ", name, " = ",
					/**/				   Keywords.New, " ", Keywords.Druid, " (",
					/**/				   id.Module.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ",
					/**/				   id.Developer.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ",
					/**/				   id.Local.ToString (System.Globalization.CultureInfo.InvariantCulture), ");");
			}

			this.formatter.WriteEndClass ();
			this.formatter.WriteEndNamespace ();

			this.Formatter.Flush ();
		}

		public CodeFormatter Formatter
		{
			get
			{
				return this.formatter;
			}
		}

		private static string CreateFormsNamespace(string name)
		{
			return string.Concat (name, ".", Keywords.Forms);
		}


				
		private readonly CodeFormatter formatter;
		private readonly List<ResourceBundle> bundles;
		private readonly ResourceManager resourceManager;
		private readonly ResourceManagerPool resourceManagerPool;
		private readonly ResourceModuleInfo resourceModuleInfo;
		private readonly string sourceNamespace;
	}
}
