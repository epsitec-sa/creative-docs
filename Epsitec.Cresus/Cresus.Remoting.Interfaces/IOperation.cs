//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IOperation permet de contrôler l'exécution d'une opération
	/// (qui peut être de longue durée).
	/// </summary>
	public interface IOperation : IProgressInformation
	{
		void CancelOperation();
		void CancelOperation(out IProgressInformation progress_information);
	}
}
