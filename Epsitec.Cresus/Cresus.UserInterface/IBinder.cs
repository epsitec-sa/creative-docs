//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier/PA

namespace Epsitec.Cresus.UserInterface
{
	public interface IBinder
	{
		void CreateBinding(object ui_object, DataLayer.DataStore root, string binding, Database.DbColumn db_column);
	}
}
