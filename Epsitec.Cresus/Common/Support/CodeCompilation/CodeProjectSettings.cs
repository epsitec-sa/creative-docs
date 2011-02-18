//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeCompilation;
using Epsitec.Common.Support.CodeGeneration;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeCompilation
{
	/// <summary>
	/// The <c>CodeProjectSettings</c> class defines the settings for a
	/// compilation project.
	/// </summary>
	public class CodeProjectSettings
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CodeProjectSettings"/> class.
		/// </summary>
		public CodeProjectSettings()
		{
			this.references = new List<CodeProjectReference> ();
			this.sources = new List<CodeProjectSource> ();
		}

		
		public System.Guid ProjectGuid
		{
			get
			{
				return this.projectGuid;
			}
			set
			{
				this.projectGuid = value;
			}
		}

		public string RootNamespace
		{
			get
			{
				return this.rootNamespace;
			}
			set
			{
				this.rootNamespace = value;
			}
		}

		public string AssemblyName
		{
			get
			{
				return this.assemblyName;
			}
			set
			{
				this.assemblyName = value;
			}
		}

		public string OutputDirectory
		{
			get
			{
				return this.outputDirectory;
			}
			set
			{
				this.outputDirectory = value;
			}
		}

		public string TemporaryDirectory
		{
			get
			{
				return this.temporaryDirectory;
			}
			set
			{
				this.temporaryDirectory = value;
			}
		}

		public IList<CodeProjectReference> References
		{
			get
			{
				return this.references;
			}
		}

		public IList<CodeProjectSource> Sources
		{
			get
			{
				return this.sources;
			}
		}

		#region Internal Methods

		internal void DefineSettings(CodeProject project)
		{
			List<CodeProjectReference> references = new List<CodeProjectReference> ();
			List<CodeProjectSource> sources = new List<CodeProjectSource> ();

			foreach (CodeProjectReference item in this.references)
			{
				if (references.Contains (item))
				{
					continue;
				}

				if (item.IsFrameworkAssembly ())
				{
					project.Add (TemplateItem.ReferenceInsertionPoint, item.ToSimpleString ());
				}
				else
				{
					project.Add (TemplateItem.ReferenceInsertionPoint, item.ToString ());
				}

				references.Add (item);
			}
			
			foreach (CodeProjectSource item in this.sources)
			{
				if (sources.Contains (item))
				{
					continue;
				}

				project.Add (TemplateItem.CompileInsertionPoint, item.ToString ());

				sources.Add (item);
			}

			if (this.projectGuid != System.Guid.Empty)
			{
				project.Add (TemplateItem.ProjectGuid, this.projectGuid.ToString ("D"));
			}
			if (!string.IsNullOrEmpty (this.rootNamespace))
			{
				project.Add (TemplateItem.RootNamespace, this.rootNamespace);
			}
			if (!string.IsNullOrEmpty (this.assemblyName))
			{
				project.Add (TemplateItem.AssemblyName, this.assemblyName);
			}
			if (!string.IsNullOrEmpty (this.outputDirectory))
			{
				project.Add (TemplateItem.DebugOutputDirectory, this.outputDirectory);
				project.Add (TemplateItem.ReleaseOutputDirectory, this.outputDirectory);
			}
			if (!string.IsNullOrEmpty (this.temporaryDirectory))
			{
				project.Add (TemplateItem.DebugTemporaryDirectory, this.temporaryDirectory);
				project.Add (TemplateItem.ReleaseTemporaryDirectory, this.temporaryDirectory);
			}
		}

		#endregion

		private List<CodeProjectReference> references;
		private List<CodeProjectSource> sources;
		private System.Guid projectGuid;
		private string rootNamespace;
		private string assemblyName;
		private string outputDirectory;
		private string temporaryDirectory;
	}
}
