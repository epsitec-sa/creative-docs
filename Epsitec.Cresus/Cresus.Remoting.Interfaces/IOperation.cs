//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// L'interface IOperation permet de contr�ler l'ex�cution d'une op�ration
	/// (qui peut �tre de longue dur�e).
	/// </summary>
	public interface IOperation : IProgressInformation
	{
		void CancelOperation();
		void CancelOperation(out IProgressInformation progress_information);
	}
}
