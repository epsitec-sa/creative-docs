//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer.Context
{
	
	
	/// <summary>
	/// The <c>DataContextEventArgs</c> holds the data that should be provided to an event handler
	/// when an event in relation to a <see cref="DataContext"/> is fired.
	/// </summary>
	public class DataContextEventArgs : EventArgs
	{
		
		
		/// <summary>
		/// Builds a new <c>DataContextEventArgs</c> holding a reference to <paramref name="dataContext"/>.
		/// </summary>
		/// <param name="dataContext">The <see cref="DataContext"/> targeted by the event.</param>
		public DataContextEventArgs(DataContext dataContext)
		{
			this.dataContext = dataContext;
		}


		/// <summary>
		/// Gets the <see cref="DataContext"/> targeted by the event.
		/// </summary>
		public DataContext DataContext
		{
			get
			{
				return this.dataContext;
			}
		}


		/// <summary>
		/// The reference to the <see cref="DataContext"/> targeted by the event.
		/// </summary>
		private readonly DataContext dataContext;
	
	
	}


}
