//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// La classe ImmediateProgress est utile pour représenter le progrès d'une
	/// opération de durée nulle (ou presque, d'où le nom de "immédiat").
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
