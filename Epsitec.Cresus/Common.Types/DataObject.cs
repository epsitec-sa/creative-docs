//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Types.DataObject))]

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DataObject</c> is only used as a support for the <c>DataContext</c>
	/// property.
	/// </summary>
	public abstract class DataObject : DependencyObject
	{
		/// <summary>
		/// Gets the data context (an instance of <see cref="Binding"/>) associated with an object.
		/// </summary>
		/// <param name="o">The object to query.</param>
		/// <returns>The data context.</returns>
		public static Binding GetDataContext(DependencyObject o)
		{
			return o.GetValue (DataObject.DataContextProperty) as Binding;
		}

		/// <summary>
		/// Sets the data context (an instance of <see cref="Binding"/>) associated with an object.
		/// </summary>
		/// <param name="o">The object.</param>
		/// <param name="value">The data context.</param>
		public static void SetDataContext(DependencyObject o, Binding value)
		{
			o.SetValue (DataObject.DataContextProperty, value);
		}

		/// <summary>
		/// Clears the data context associated with an object.
		/// </summary>
		/// <param name="o">The object to clear.</param>
		public static void ClearDataContext(DependencyObject o)
		{
			o.ClearValue (DataObject.DataContextProperty);
		}

		public static readonly DependencyProperty DataContextProperty = DependencyProperty.RegisterAttached ("DataContext", typeof (Binding), typeof (DataObject), new DependencyPropertyMetadataWithInheritance ());
	}
}
