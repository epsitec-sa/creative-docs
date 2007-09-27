//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
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

		public bool IsValidInstallation
		{
			get
			{
				if (!this.isValidInstallation.HasValue)
				{
					this.isValidInstallation = this.VerifyBuildInstallation ();
				}

				return this.isValidInstallation.Value;
			}
		}



		private bool VerifyBuildInstallation()
		{
			if ((System.IO.Directory.Exists (Paths.V30_Framework)) &&
				(System.IO.Directory.Exists (Paths.V35_Framework)) &&
				(System.IO.Directory.Exists (Paths.V35_ReferenceAssemblies)))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		#region Paths Static Class

		internal static class Paths
		{
			static Paths()
			{
				Paths.v35_ReferenceAssemblies = System.IO.Path.Combine (Paths.ReferenceAssemblies, "v3.5");
				Paths.v35_Framework = System.IO.Path.Combine (Paths.Framework, "v3.5");
				Paths.v30_Framework = System.IO.Path.Combine (Paths.Framework, "v3.0");
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

			public static string V30_Framework
			{
				get
				{
					return Paths.v30_Framework;
				}
			}

			public const string ReferenceAssemblies = @"%ProgramFiles%\Reference Assemblies\Microsoft\Framework";
			public const string Framework			= @"%windir%\Microsoft.Net\Framework";

			private static readonly string v35_ReferenceAssemblies;
			private static readonly string v35_Framework;
			private static readonly string v30_Framework;
		}

		#endregion

		private bool? isValidInstallation;
	}
}
