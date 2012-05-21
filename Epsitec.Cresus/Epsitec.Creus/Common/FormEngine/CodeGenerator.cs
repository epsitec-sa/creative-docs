//	Copyright © 2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.FormEngine
{
	using Keywords=CodeHelper.Keywords;

	/// <summary>
	/// The <c>CodeGenerator</c> class produces a C# source code wrapper for
	/// the forms defined in the resource files.
	/// </summary>
	public sealed class CodeGenerator
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeGenerator"/> class.
		/// </summary>
		/// <param name="resourceManager">The resource manager.</param>
		public CodeGenerator(ResourceManager resourceManager)
		{
			this.formatter = new CodeFormatter ();
			
			this.resourceManager = resourceManager;
			this.resourceManagerPool = this.resourceManager.Pool;
			this.resourceModuleInfo = this.resourceManagerPool.GetModuleInfo (this.resourceManager.DefaultModulePath);
			this.sourceNamespaceForms = this.resourceModuleInfo.SourceNamespaceForms;
		}

		/// <summary>
		/// Gets the formatter used when generating code.
		/// </summary>
		/// <value>The formatter.</value>
		public CodeFormatter Formatter
		{
			get
			{
				return this.formatter;
			}
		}

		/// <summary>
		/// Emits the code for the specified form bundles.
		/// </summary>
		/// <param name="bundles">The form bundles.</param>
		public void Emit(IEnumerable<ResourceBundle> bundles)
		{
			CodeHelper.EmitHeader (this.formatter);

			this.formatter.WriteBeginNamespace (this.sourceNamespaceForms);
			this.formatter.WriteBeginClass (CodeHelper.FormIdsClassAttributes, "FormIds");

			foreach (ResourceBundle bundle in bundles)
			{
				if (bundle != null)
				{
					Druid  id   = bundle.Id;
					string name = bundle.Caption;

					this.formatter.WriteField (CodeHelper.PublicStaticReadOnlyFieldAttributes,
						/**/				   Keywords.Druid, " ", name, " = ",
						/**/				   Keywords.New, " ", Keywords.Druid, " (",
						/**/				   id.Module.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ",
						/**/				   id.DeveloperAndPatchLevel.ToString (System.Globalization.CultureInfo.InvariantCulture), ", ",
						/**/				   id.Local.ToString (System.Globalization.CultureInfo.InvariantCulture), ");");
				}
			}

			this.formatter.WriteEndClass ();
			this.formatter.WriteEndNamespace ();

			this.Formatter.Flush ();
		}

		
		private readonly CodeFormatter formatter;
		private readonly ResourceManager resourceManager;
		private readonly ResourceManagerPool resourceManagerPool;
		private readonly ResourceModuleInfo resourceModuleInfo;
		private readonly string sourceNamespaceForms;
	}
}
