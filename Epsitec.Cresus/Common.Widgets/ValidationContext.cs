//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Common.Widgets
{
	public class ValidationContext : DependencyObject
	{
		public ValidationContext()
		{
			this.uniqueId = System.Threading.Interlocked.Increment (ref ValidationContext.nextUniqueId);
		}

		public long UniqueId
		{
			get
			{
				return this.uniqueId;
			}
		}

		public void Refresh(Visual root)
		{
			foreach (Visual child in root.GetAllChildren ())
			{
				Widget w;
				
			}
		}
		
		
		public static ValidationContext GetContext(DependencyObject obj)
		{
			return (ValidationContext) obj.GetValue (ValidationContext.ContextProperty);
		}

		public static void SetContext(DependencyObject obj, ValidationContext context)
		{
			obj.SetValue (ValidationContext.ContextProperty, context);
		}

		public static void ClearContext(DependencyObject obj)
		{
			obj.ClearValueBase (ValidationContext.ContextProperty);
		}


		public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached ("Context", typeof (ValidationContext), typeof (ValidationContext));

		private static long nextUniqueId;
		
		private long uniqueId;
	}
}
