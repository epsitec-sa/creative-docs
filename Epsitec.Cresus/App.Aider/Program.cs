//	Copyright © 2011-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Text;

namespace Epsitec.Aider
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[System.STAThread]
		static void Main(string[] args)
		{
            // CoreCLR need this to support codepage 850
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Common.IO.ConsoleCreator.Initialize ();
            Epsitec.Aider.Processors.Processor.Setup ();

			AiderProgram.Main (args);
		}
	}
}
