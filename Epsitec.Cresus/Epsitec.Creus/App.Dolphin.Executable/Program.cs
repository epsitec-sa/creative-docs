//	Copyright © 2007, OPaC bright ideas, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.App.Dolphin
{
	static class Program
	{
		[System.STAThread]
		static void Main()
		{
			System.Reflection.Assembly assembly = typeof (Program).Assembly;

			string assemblyPath = System.IO.Path.GetDirectoryName (assembly.Location);

			string[] stripList = new string[] { @"\bin\Debug", @"\bin\Release" };

			foreach (string strip in stripList)
			{
				if (assemblyPath.EndsWith (strip))
				{
					assemblyPath = assemblyPath.Substring (0, assemblyPath.Length - strip.Length);
					break;
				}
			}

			string rootPath = System.IO.Path.Combine (assemblyPath, "Executable");
			string shellExe = System.IO.Path.Combine (rootPath, "shell.exe");
			System.Diagnostics.Process.Start (shellExe, System.Environment.CommandLine);
		}
	}
}