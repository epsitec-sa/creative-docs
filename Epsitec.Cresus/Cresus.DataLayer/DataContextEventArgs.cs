//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;

namespace Epsitec.Cresus.DataLayer
{
	public class DataContextEventArgs : EventArgs
	{
		public DataContextEventArgs(DataContext dataContext)
		{
			this.dataContext = dataContext;
		}


		public DataContext DataContext
		{
			get
			{
				return this.dataContext;
			}
		}


		private readonly DataContext dataContext;
	}
}
