//	Copyright � 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.IO
{
	/// <summary>
	/// La classe TemporaryFile permet de cr�er et d�truire automatiquement des
	/// fichiers temporaires. A chaque instance correspond un fichier temporaire.
	/// </summary>
	public class TemporaryFile : System.IDisposable
	{
		public TemporaryFile()
		{
			this.name = System.IO.Path.GetTempFileName ();
		}
		
		~ TemporaryFile()
		{
			this.Dispose (false);
		}
		
		
		public string							Path
		{
			get
			{
				return this.name;
			}
		}
		
		
		public void Delete()
		{
			//	Tente de supprimer le fichier tout de suite. Si on n'y r�ussit pas,
			//	ce sera le 'finalizer' qui s'en chargera...
			
			this.RemoveFile ();
			
			if (this.name == null)
			{
				//	Fichier d�truit, plus besoin d'ex�cuter le 'finalizer'.
				
				System.GC.SuppressFinalize (this);
			}
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//	rien � faire de plus...
			}
			
			this.RemoveFile ();
		}
		
		protected virtual void RemoveFile()
		{
			if (this.name != null)
			{
				try
				{
					if (System.IO.File.Exists (this.name))
					{
						System.IO.File.Delete (this.name);
						this.name = null;
					}
				}
				catch (System.Exception ex)
				{
					System.Diagnostics.Debug.WriteLine ("Could not remove file " + this.name + ";\n" + ex.ToString ());
				}
			}
		}
		
		
		private string						name;
	}
}
