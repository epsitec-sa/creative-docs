//	Copyright � 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeCompilation;
using Epsitec.Common.Support.CodeGeneration;

using System.Collections.Generic;

namespace Epsitec.Common.Support.CodeCompilation
{
	public class BuildDriver
	{
		public BuildDriver()
		{
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

		public void Build(CodeProject project)
		{
			string workingDirectory = this.BuildDirectory;
			string outputDirectory  = project.GetItem (TemplateItem.DebugOutputDirectory);
			string assemblyName     = project.GetItem (TemplateItem.AssemblyName);
			string assemblyFileName = string.Concat (assemblyName, ".dll");
			string assemblyFilePath = System.IO.Path.Combine (outputDirectory, assemblyFileName);
			string projectFileName  = string.Concat (assemblyName, ".csproj");
			string projectFilePath  = System.IO.Path.Combine (workingDirectory, projectFileName);

			if (!System.IO.Directory.Exists (workingDirectory))
			{
				System.IO.Directory.CreateDirectory (workingDirectory);
			}
			
			if (System.IO.File.Exists (assemblyFilePath))
			{
				System.IO.File.Delete (assemblyFilePath);
			}

			System.IO.File.WriteAllText (projectFilePath, project.CreateProjectSource (), System.Text.Encoding.UTF8);

			string msbuildPath = System.IO.Path.Combine (Paths.V35_Framework, "msbuild.exe");
			string msbuildArgs = "/ToolsVersion:3.5 /nologo /noautoresponse /verbosity:quiet /t:Build /p:Configuration=Debug";
			string msbuildOutput;
			string msbuildErrors;

			BuildDriver.Execute (msbuildPath, msbuildArgs, workingDirectory, out msbuildOutput, out msbuildErrors);

			System.Diagnostics.Debug.WriteLine ("Output: " + msbuildOutput);
			System.Diagnostics.Debug.WriteLine ("Errors: " + msbuildErrors);
		}

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

		private static bool VerifyBuildInstallation()
		{
			if ((System.IO.Directory.Exists (Paths.V20_Framework)) &&
				(System.IO.Directory.Exists (Paths.V30_Framework)) &&
				(System.IO.Directory.Exists (Paths.V35_Framework)) &&
				(System.IO.Directory.Exists (Paths.V30_ReferenceAssemblies)) &&
				(System.IO.Directory.Exists (Paths.V35_ReferenceAssemblies)))
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
				yield return Paths.V35_ReferenceAssemblies;
				yield return Paths.V30_ReferenceAssemblies;
				yield return Paths.V20_Framework;
			}
		}
		
		private static IEnumerable<string> FrameworkPaths
		{
			get
			{
				yield return Paths.V35_Framework;
				yield return Paths.V30_Framework;
				yield return Paths.V20_Framework;
			}
		}

		private static bool Execute(string program, string arguments, string workingDirectory, out string output, out string errors)
		{
			System.Diagnostics.Process process = new System.Diagnostics.Process ();
			
			process.StartInfo.FileName = program;
			process.StartInfo.Arguments = arguments;
			process.StartInfo.WorkingDirectory = workingDirectory;
			process.StartInfo.UseShellExecute = false;				//	needed to redirect I/O
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.StandardErrorEncoding = System.Text.Encoding.GetEncoding (System.Globalization.CultureInfo.InstalledUICulture.TextInfo.OEMCodePage);
			process.StartInfo.StandardOutputEncoding = System.Text.Encoding.GetEncoding (System.Globalization.CultureInfo.InstalledUICulture.TextInfo.OEMCodePage);
			process.StartInfo.CreateNoWindow = true;				//	needed to avoid the empty window syndrome

			if (process.Start ())
			{
				output = process.StandardOutput.ReadToEnd ();
				errors = process.StandardError.ReadToEnd ();

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

		[System.Runtime.InteropServices.DllImport ("kernel32.dll")]
		private static extern int GetConsoleOutputCP();

		#region Paths Static Class

		internal static class Paths
		{
			static Paths()
			{
				Paths.v30_ReferenceAssemblies = System.IO.Path.Combine (Paths.ReferenceAssemblies, "v3.0");
				Paths.v35_ReferenceAssemblies = System.IO.Path.Combine (Paths.ReferenceAssemblies, "v3.5");
				Paths.v35_Framework = System.IO.Path.Combine (Paths.Framework, "v3.5");
				Paths.v30_Framework = System.IO.Path.Combine (Paths.Framework, "v3.0");
				Paths.v20_Framework = System.IO.Path.Combine (Paths.Framework, "v2.0.50727");
			}

			public static string V35_ReferenceAssemblies
			{
				get
				{
					return Paths.v35_ReferenceAssemblies;
				}
			}

			public static string V35_Framework
			{
				get
				{
					return Paths.v35_Framework;
				}
			}

			public static string V30_ReferenceAssemblies
			{
				get
				{
					return Paths.v30_ReferenceAssemblies;
				}
			}

			public static string V30_Framework
			{
				get
				{
					return Paths.v30_Framework;
				}
			}

			public static string V20_Framework
			{
				get
				{
					return Paths.v20_Framework;
				}
			}

			private static readonly string ReferenceAssemblies	= System.IO.Path.Combine (Globals.Directories.ProgramFiles, @"Reference Assemblies\Microsoft\Framework");
			private static readonly string Framework			= System.IO.Path.Combine (Globals.Directories.Windows, @"Microsoft.Net\Framework");

			public const string BuildBin	= "bin";
			public const string	BuildTemp	= "temp";

			private static readonly string v30_ReferenceAssemblies;
			private static readonly string v35_ReferenceAssemblies;
			private static readonly string v35_Framework;
			private static readonly string v30_Framework;
			private static readonly string v20_Framework;
		}

		#endregion

		private bool? hasValidFrameworkVersions;
		private string buildDirectory;
	}
}
