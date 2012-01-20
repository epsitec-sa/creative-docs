//	Copyright © 2007-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Designer
{
	static partial class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main(string[] args)
		{
			//	Start the program in an isolated app domain, with active shadow copying, so
			//	that we don't keep the DLLs open and therefore still allow the original DLLs
			//	to be recompiled/changed while the Designer is running.

			//	The AppDomainStarter class has been included in this project to avoid a premature
			//	reference to the "Common.dll" assembly (which would then be locked) :

			Epsitec.Common.Support.AppDomainStarter.StartInIsolatedAppDomain ("main", args, Program.Start);
		}
	}
}
