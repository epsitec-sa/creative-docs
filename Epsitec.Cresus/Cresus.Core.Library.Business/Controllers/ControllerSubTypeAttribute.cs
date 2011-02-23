//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers
{
	/// <summary>
	/// The <c>ControllerSubTypeAttribute</c> is used to decorate special versions
	/// of some view controller implementations. This can be used to split the UI
	/// for the edition of large entities into several view controllers.
	/// </summary>
	[System.AttributeUsage (System.AttributeTargets.Class, AllowMultiple=true, Inherited=false)]
	public class ControllerSubTypeAttribute : System.Attribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerSubTypeAttribute"/> class.
		/// The <c>Id</c> will be set to <code>0</code>.
		/// </summary>
		public ControllerSubTypeAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerSubTypeAttribute"/> class.
		/// </summary>
		/// <param name="id">The id of the view controller.</param>
		public ControllerSubTypeAttribute(int id)
		{
			this.id = id;
		}

		/// <summary>
		/// Gets the id of the view controller.
		/// </summary>
		/// <value>The id of the view controller.</value>
		public int Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly int id;
	}
}
