//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;

namespace Epsitec.Common.UI.Controllers
{
	/// <summary>
	/// Summary description for AbstractConstrainedController.
	/// </summary>
	public abstract class AbstractConstrainedController : AbstractController
	{
		protected AbstractConstrainedController()
		{
		}
		
		
		public Types.IDataConstraint			Constraint
		{
			get
			{
				return this.constraint;
			}
			set
			{
				this.constraint = value;
			}
		}
		
		
		protected bool CheckConstraint(object value)
		{
			if ((this.constraint == null) ||
				(this.constraint.CheckConstraint (value)))
			{
				return true;
			}
			
			return false;
		}
		
		
		private Types.IDataConstraint			constraint;
	}
}
