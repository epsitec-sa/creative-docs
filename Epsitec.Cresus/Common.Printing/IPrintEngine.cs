//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Printing
{
	/// <summary>
	/// L'interface IPrintEngine.
	/// </summary>
	public interface IPrintEngine
	{
		void StartingPrintJob();
		void FinishingPrintJob();
		
		void PrepareNewPage(PageSettings settings);
		PrintEngineStatus PrintPage(PrintPort port);
	}
	
	public enum PrintEngineStatus
	{
		MorePages,
		FinishJob,
		CancelJob,
	}
}
