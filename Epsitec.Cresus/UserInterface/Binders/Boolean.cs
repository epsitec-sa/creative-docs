//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.UserInterface.Binders
{
	/// <summary>
	/// Summary description for Boolean.
	/// </summary>
	public class Boolean : IBinder
	{
		public Boolean()
		{
			BinderFactory.RegisterBinder ("boolean", this);
		}
		
		#region IBinder Members


		#endregion
	}
}
