//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Common.Types
{
	/// <summary>
	/// The <c>DependencyPropertyChangedEventArgs</c> class provides information for
	/// the <c>INotifyPropertyChanged.PropertyChanged</c> event.
	/// </summary>
	public class DependencyPropertyChangedEventArgs : Support.EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyPropertyChangedEventArgs"/> class.
		/// </summary>
		/// <param name="property">The property which changes.</param>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		public DependencyPropertyChangedEventArgs(DependencyProperty property, object oldValue, object newValue)
		{
			this.property = property;
			this.propertyName = property.Name;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyPropertyChangedEventArgs"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property which changes.</param>
		/// <param name="oldValue">The old value.</param>
		/// <param name="newValue">The new value.</param>
		public DependencyPropertyChangedEventArgs(string propertyName, object oldValue, object newValue)
		{
			this.propertyName = propertyName;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DependencyPropertyChangedEventArgs"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property which changes.</param>
		public DependencyPropertyChangedEventArgs(string propertyName)
		{
			this.propertyName = propertyName;
			this.oldValue = null;
			this.newValue = null;
		}

		/// <summary>
		/// Gets the property which changes.
		/// </summary>
		/// <value>The property which changes or <c>null</c> if it is not
		/// a <see cref="DependencyProperty"/>.</value>
		public DependencyProperty				Property
		{
			get
			{
				return this.property;
			}
		}

		/// <summary>
		/// Gets the name of the property which changes.
		/// </summary>
		/// <value>The name of the property.</value>
		public string							PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		/// <summary>
		/// Gets the old value, before the property change.
		/// </summary>
		/// <value>The old value.</value>
		public object							OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		/// <summary>
		/// Gets the new value, after the property change.
		/// </summary>
		/// <value>The new value.</value>
		public object							NewValue
		{
			get
			{
				return this.newValue;
			}
		}


		public override string ToString()
		{
			return string.Concat ("Changed ",
				/* */			  this.PropertyName ?? (this.property == null ? "?" : this.property.Name),
				/* */			  " from ",
				/* */			  this.oldValue == null ? "<null>" : this.oldValue.ToString (),
				/* */			  " to ",
				/* */			  this.newValue == null ? "<null>" : this.newValue.ToString ());
		}
		
		private DependencyProperty				property;
		private string							propertyName;
		private object							oldValue;
		private object							newValue;
	}
}
