//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeCompilation;
using Epsitec.Common.Support.CodeGeneration;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeCompilation
{
	/// <summary>
	/// The <c>BuildDriver</c> class is used to compile source projects based
	/// on the Microsoft <c>msbuild.exe</c> tool.
	/// </summary>
	public class BuildDriver : System.IDisposable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BuildDriver"/> class.
		/// </summary>
		public BuildDriver()
		{
			string directory = string.Concat ("BuildDriver.", System.DateTime.Now.Ticks, ".", System.Diagnostics.Process.GetCurrentProcess ().Id);
			this.BuildDirectory = System.IO.Path.Combine (System.IO.Path.GetTempPath (), directory);
		}
		
		~BuildDriver()
		{
			this.Dispose (false);
		}


		/// <summary>
		/// Gets a value indicating whether the correct .NET Framework versions are installed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the correct .NET Framework versions are installed; otherwise, <c>false</c>.
		/// </value>
		public bool HasValidFrameworkVersions
		{
			get
			{
				if (!this.hasValidFrameworkVersions.HasValue)
				{
					this.hasValidFrameworkVersions = BuildDriver.VerifyBuildInstallation ();
				}

				return this.hasValidFrameworkVersions.Value;
			}
		}

		/// <summary>
		/// Gets or sets the build directory.
		/// </summary>
		/// <value>The build directory.</value>
		public string BuildDirectory
		{
			get
			{
				return this.buildDirectory;
			}
			set
			{
				this.buildDirectory = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the build directory
		/// should be deleted on disposal of this instance.
		/// </summary>
		/// <value>
		/// <c>true</c> if the build directory should be deleted on disposal;
		/// otherwise, <c>false</c>.
		/// </value>
		public bool DeleteOnDispose
		{
			get
			{
				return this.deleteOnDispose;
			}
			set
			{
				this.deleteOnDispose = value;
			}
		}

		/// <summary>
		/// Creates the build directory, if needed.
		/// </summary>
		public void CreateBuildDirectory()
		{
			if (!System.IO.Directory.Exists (this.buildDirectory))
			{
				System.IO.Directory.CreateDirectory (this.buildDirectory);
				this.deleteBuildDirectoryOnDispose = true;
			}
		}

		/// <summary>
		/// Compiles the specified project.
		/// </summary>
		/// <param name="project">The project.</param>
		/// <returns>
		/// <c>true</c> if the assembly was successfully built; otherwise, <c>false</c>.
		/// </returns>
		public bool Compile(CodeProject project)
		{
			string workingDirectory = this.BuildDirectory;
			string outputDirectory  = project.GetItem (TemplateItem.DebugOutputDirectory);
			string tempDirectory    = project.GetItem (TemplateItem.DebugTemporaryDirectory);
			string assemblyName     = project.GetItem (TemplateItem.AssemblyName);
			string assemblyFileName = string.Concat (assemblyName, ".dll");
			string assemblyFilePath = System.IO.Path.Combine (outputDirectory, assemblyFileName);
			string projectFileName  = string.Concat (assemblyName, ".csproj");
			string projectFilePath  = System.IO.Path.Combine (workingDirectory, projectFileName);

			if (System.IO.File.Exists (assemblyFilePath))
			{
				System.IO.File.Delete (assemblyFilePath);
			}

			System.IO.File.WriteAllText (projectFilePath, project.CreateProjectSource (), System.Text.Encoding.UTF8);

			string msbuildPath = System.IO.Path.Combine (Paths.V35Framework, "msbuild.exe");
			string msbuildArgs = "/ToolsVersion:3.5 /nologo /noautoresponse /verbosity:quiet /t:Build /p:Configuration=Debug";
			string msbuildOutput;
			string msbuildErrors;

			this.buildMessages = null;
			this.buildAssemblyDllPath = null;
			this.buildAssemblyPdbPath = null;

			try
			{
				if (BuildDriver.Execute (msbuildPath, msbuildArgs, workingDirectory, out msbuildOutput, out msbuildErrors))
				{
					System.Diagnostics.Debug.Assert (msbuildErrors.Length == 0);

					this.buildMessages = msbuildOutput;

					if (System.IO.File.Exists (assemblyFilePath))
					{
						this.buildAssemblyDllPath = assemblyFilePath;
						this.buildAssemblyPdbPath = string.Concat (assemblyFilePath.Remove (assemblyFilePath.Length-3), "pdb");

						return true;
					}
				}

				return false;
			}
			finally
			{
				if (System.IO.Directory.Exists (tempDirectory))
				{
					System.IO.Directory.Delete (tempDirectory, true);
				}
			}
		}

		/// <summary>
		/// Gets the build messages, if any.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<string> GetBuildMessages()
		{
			if (this.buildMessages != null)
			{
				string[] lines = this.buildMessages.Split (new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

				foreach (string line in lines)
				{
					yield return line;
				}
			}
		}

		/// <summary>
		/// Gets the path to the compiled assembly.
		/// </summary>
		/// <returns>The path to the compiled assembly.</returns>
		public string GetCompiledAssemblyPath()
		{
			return this.buildAssemblyDllPath;
		}

		/// <summary>
		/// Gets the path to the debug information for the compiled assembly.
		/// </summary>
		/// <returns>The path to the debug information for the compiled assembly.</returns>
		public string GetCompiledAssemblyDebugInfoPath()
		{
			return this.buildAssemblyPdbPath;
		}

		/// <summary>
		/// Creates default project settings in order to build an assembly with the
		/// specified name.
		/// </summary>
		/// <param name="assemblyName">Name of the expected assembly.</param>
		/// <returns>The default project settings.</returns>
		public CodeProjectSettings CreateSettings(string assemblyName)
		{
			System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (this.BuildDirectory) == false);

			CodeProjectSettings settings = new CodeProjectSettings ();

			settings.AssemblyName       = assemblyName;
			settings.ProjectGuid        = System.Guid.NewGuid ();
			settings.OutputDirectory    = System.IO.Path.Combine (this.BuildDirectory, Paths.BuildBin);
			settings.TemporaryDirectory = System.IO.Path.Combine (this.BuildDirectory, Paths.BuildTemp);
			settings.References.Add (CodeProjectReference.FromAssembly (typeof (int).Assembly));

			return settings;
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}

		#endregion

		/// <summary>
		/// Finds the reference assembly path.
		/// </summary>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <returns>The path to the reference assembly or <c>null</c> if it
		/// cannot be found.</returns>
		public static string FindReferenceAssemblyPath(string assemblyName)
		{
			if (assemblyName == "System")
			{
				assemblyName = "mscorlib";
			}
			
			string assemblyFileName = BuildDriver.GetAssemblyFileName (assemblyName);

			foreach (string path in BuildDriver.ReferenceAssembliesPaths)
			{
				string filePath = System.IO.Path.Combine (path, assemblyFileName);

				if (System.IO.File.Exists (filePath))
				{
					return filePath;
				}
			}

			return null;
		}

		/// <summary>
		/// Determines whether the specified assembly path belongs to one of the
		/// frameworks.
		/// </summary>
		/// <param name="assemblyPath">The assembly path.</param>
		/// <returns>
		/// 	<c>true</c> if the specified assembly path belongs to one of the frameworks; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsFrameworkPath(string assemblyPath)
		{
			string assemblyDirectory = System.IO.Path.GetDirectoryName (assemblyPath).ToLowerInvariant ();

			foreach (string path in BuildDriver.ReferenceAssembliesPaths)
			{
				if (assemblyDirectory.StartsWith (path.ToLowerInvariant ()))
				{
					return true;
				}
			}
			foreach (string path in BuildDriver.FrameworkPaths)
			{
				if (assemblyDirectory.StartsWith (path.ToLowerInvariant ()))
				{
					return true;
				}
			}

			return false;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (this.deleteOnDispose)
			{
				if (this.deleteBuildDirectoryOnDispose)
				{
					System.IO.Directory.Delete (this.buildDirectory, true);
					this.deleteBuildDirectoryOnDispose = false;
				}
			}
		}

		private static bool VerifyBuildInstallation()
		{
			if ((System.IO.Directory.Exists (Paths.V20Framework)) &&
				(System.IO.Directory.Exists (Paths.V30Framework)) &&
				(System.IO.Directory.Exists (Paths.V35Framework)) &&
				(System.IO.Directory.Exists (Paths.V30ReferenceAssemblies)) &&
				(System.IO.Directory.Exists (Paths.V35ReferenceAssemblies)))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private static string GetAssemblyFileName(string assemblyName)
		{
			return string.Concat (assemblyName, ".dll");
		}

		private static IEnumerable<string> ReferenceAssembliesPaths
		{
			get
			{
				yield return Paths.V35ReferenceAssemblies;
				yield return Paths.V30ReferenceAssemblies;
				yield return Paths.V20Framework;
			}
		}
		
		private static IEnumerable<string> FrameworkPaths
		{
			get
			{
				yield return Paths.V35Framework;
				yield return Paths.V30Framework;
				yield return Paths.V20Framework;
			}
		}

		private static bool Execute(string program, string arguments, string workingDirectory, out string output, out string errors)
		{
			System.Diagnostics.Process process = new System.Diagnostics.Process ();

			int oemCodePage = System.Globalization.CultureInfo.InstalledUICulture.TextInfo.OEMCodePage;
			
			process.StartInfo.FileName = program;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.WorkingDirectory = workingDirectory;
			process.StartInfo.UseShellExecute = false;				//	needed to redirect I/O
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.StandardErrorEncoding = System.Text.Encoding.GetEncoding (oemCodePage);
			process.StartInfo.StandardOutputEncoding = System.Text.Encoding.GetEncoding (oemCodePage);
			process.StartInfo.CreateNoWindow = true;				//	needed to avoid the empty window syndrome

			if (process.Start ())
			{
				output = process.StandardOutput.ReadToEnd ().Trim ();
				errors = process.StandardError.ReadToEnd ().Trim ();

				process.WaitForExit ();

				return true;
			}
			else
			{
				output = null;
				errors = null;

				return false;
			}
		}


		#region Paths Static Class

		internal static class Paths
		{
			static Paths()
			{
				Paths.v30ReferenceAssemblies = System.IO.Path.Combine (Paths.ReferenceAssemblies, "v3.0");
				Paths.v35ReferenceAssemblies = System.IO.Path.Combine (Paths.ReferenceAssemblies, "v3.5");
				Paths.v35Framework = System.IO.Path.Combine (Paths.Framework, "v3.5");
				Paths.v30Framework = System.IO.Path.Combine (Paths.Framework, "v3.0");
				Paths.v20Framework = System.IO.Path.Combine (Paths.Framework, "v2.0.50727");
			}

			public static string V35ReferenceAssemblies
			{
				get
				{
					return Paths.v35ReferenceAssemblies;
				}
			}

			public static string V35Framework
			{
				get
				{
					return Paths.v35Framework;
				}
			}

			public static string V30ReferenceAssemblies
			{
				get
				{
					return Paths.v30ReferenceAssemblies;
				}
			}

			public static string V30Framework
			{
				get
				{
					return Paths.v30Framework;
				}
			}

			public static string V20Framework
			{
				get
				{
					return Paths.v20Framework;
				}
			}

			private static readonly string ReferenceAssemblies	= System.IO.Path.Combine (Globals.Directories.ProgramFiles, @"Reference Assemblies\Microsoft\Framework");
			private static readonly string Framework			= System.IO.Path.Combine (Globals.Directories.Windows, @"Microsoft.Net\Framework");

			public const string BuildBin	= "bin";
			public const string	BuildTemp	= "temp";

			private static readonly string v30ReferenceAssemblies;
			private static readonly string v35ReferenceAssemblies;
			private static readonly string v35Framework;
			private static readonly string v30Framework;
			private static readonly string v20Framework;
		}

		#endregion

		private bool? hasValidFrameworkVersions;
		private bool deleteBuildDirectoryOnDispose;
		private bool deleteOnDispose;
		private string buildDirectory;
		private string buildMessages;
		private string buildAssemblyDllPath;
		private string buildAssemblyPdbPath;
	}
}
