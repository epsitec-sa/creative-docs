//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface IAttachable permet de s'attacher/d�tacher d'une table d'une
	/// base de donn�es.
	/// </summary>
	public interface IAttachable
	{
		void Attach(DbInfrastructure infrastructure, DbTable table);
		void Detach();
	}
}
