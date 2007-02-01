//	Copyright � 2004-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// La classe ImmediateProgress est utile pour repr�senter le progr�s d'une
	/// op�ration de dur�e nulle (ou presque, d'o� le nom de "imm�diat").
	/// </summary>
	public sealed class ImmediateProgress : AbstractProgress
	{
		public ImmediateProgress()
		{
			this.SetProgress (100);
		}
		
		
		public override System.TimeSpan			ExpectedDuration
		{
			get
			{
				return new System.TimeSpan (1);
			}
		}
	}
}
