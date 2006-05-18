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
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			GC.SuppressFinalize (this);
		}
		#endregion
		
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
				return 0;
			}
		}
		
		public static int			CycleDelta
		{
			get
			{
				return 1;
			}
		}
		
		
		static Library				instance = new Library ();
	}
}