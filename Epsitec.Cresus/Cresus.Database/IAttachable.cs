//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IAttachable permet de s'attacher/détacher d'une table d'une
	/// base de données.
	/// </summary>
	public interface IAttachable
	{
		void Attach(DbInfrastructure infrastructure, DbTable table);
		void Detach();
	}
}
