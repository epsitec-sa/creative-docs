namespace Epsitec.Common.Drawing
{
	/// <summary>
	/// La classe Graphics encapsule le contexte graphique utilisé pour peindre.
	/// </summary>
	public class Graphics : System.IDisposable
	{
		public Graphics()
		{
			this.transform = new Transform ();
		}

		~ Graphics()
		{
			this.Dispose (false);
		}
		
		public Transform			Transform
		{
			get 
			{
				return this.transform;
			}
			set
			{
			}
		}
		
		public void AttachHandle(System.IntPtr handle)
		{
			System.Diagnostics.Debug.Assert (this.handle == System.IntPtr.Zero);
			this.handle = handle;
		}
		
		public void DetachHandle()
		{
			System.Diagnostics.Debug.Assert (this.handle != System.IntPtr.Zero);
			this.handle = System.IntPtr.Zero;
		}
		
		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (true);
		}

		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	TODO: libère objets locaux
			}
			
			//	TODO: libère ressources non managed
			
			if (this.handle != System.IntPtr.Zero)
			{
				this.DetachHandle ();
			}
		}
		
		
		System.IntPtr				handle;
		Transform					transform;
	}
}
