using System;
using System.Runtime.InteropServices;

namespace Epsitec.Common.Drawing.Agg
{
	public class Library : System.IDisposable
	{
		Library()
		{
			AntiGrain.Interface.Initialise ();
		}
		
		~Library()
		{
			this.Dispose (false);
		}
		
		
		public void Dispose()
		{
			this.Dispose (true);
			GC.SuppressFinalize (this);
		}
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
			
			Library.instance = null;
			AntiGrain.Interface.ShutDown ();
		}
		
		public static Library		Current
		{
			get
			{
				return Library.instance;
			}
		}
		
		public static long			Cycles
		{
			get
			{
				return AntiGrain.Interface.DebugGetCycles ();
			}
		}
		
		public static int			CycleDelta
		{
			get
			{
				return AntiGrain.Interface.DebugGetCycleDelta ();
			}
		}
		
		
		public static void TrapZeroPointer()
		{
			AntiGrain.Interface.DebugTrapZeroPointer ();
		}
		
		static Library				instance = new Library ();
	}
}