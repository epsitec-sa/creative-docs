//	Copyright © 2005-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>OpleEventArgs</c> class describes an event produced by the execution
	/// of an <see cref="IOplet"/> implementation.
	/// </summary>
	public class OpletEventArgs : System.EventArgs
	{
		public OpletEventArgs(IOplet oplet, OpletEvent opletEvent)
		{
			this.oplet       = oplet;
			this.opletEvent  = opletEvent;
		}


		public IOplet Oplet
		{
			get
			{
				return this.oplet;
			}
		}

		public OpletEvent Event
		{
			get
			{
				return this.opletEvent;
			}
		}


		private IOplet							oplet;
		private OpletEvent						opletEvent;
	}
}
