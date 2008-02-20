//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
