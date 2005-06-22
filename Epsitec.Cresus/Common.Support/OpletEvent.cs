//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// La classe OpletEventArgs d�crit l'�v�nement li� � l'ex�cution d'un
	/// objet d�riv� de IOplet.
	/// </summary>
	public class OpletEventArgs : System.EventArgs
	{
		public OpletEventArgs(IOplet oplet, OpletEvent oplet_event)
		{
			this.oplet       = oplet;
			this.oplet_event = oplet_event;
		}
		
		
		public IOplet							Oplet
		{
			get
			{
				return this.oplet;
			}
		}
		
		public OpletEvent						Event
		{
			get
			{
				return this.oplet_event;
			}
		}
		
		
		private IOplet							oplet;
		private OpletEvent						oplet_event;
	}
	
	public delegate void OpletEventHandler(object sender, OpletEventArgs e);
	
	public enum OpletEvent
	{
		UndoExecuted,
		RedoExecuted,
	}
}
