//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;

namespace Epsitec.Common.Reporting
{
	public sealed class DataNavigator
	{
		public DataNavigator(DataView view)
		{
			if (view == null)
			{
				throw new System.ArgumentNullException ();
			}

			this.view = view;
		}

		public bool IsValid
		{
			get
			{
				return this.path != null;
			}
		}

		public IDataItem CurrentDataItem
		{
			get
			{
				//	TODO: ...

				return null;
			}
		}

		public string CurrentDataPath
		{
			get
			{
				return this.path;
			}
		}

		
		public void Reset()
		{
			throw new System.NotImplementedException ();
		}

		
		public bool NavigateTo(string dataPath)
		{
			this.path = path;

			return this.IsValid;
		}

		public bool NavigateToNext()
		{
			if (this.IsValid)
			{
				//	TODO: ...
			}

			return this.IsValid;
		}

		public bool NavigateToPrevious()
		{
			if (this.IsValid)
			{
				//	TODO: ...
			}

			return this.IsValid;
		}

		public bool NavigateToFirstChild()
		{
			if (this.IsValid)
			{
				//	TODO: ...
			}

			return this.IsValid;
		}

		public bool NavigateToParent()
		{
			if (this.IsValid)
			{
				//	TODO: ...
			}

			return this.IsValid;
		}

		
		
		public void RequestBreak()
		{
			this.EnsureValid ();

			throw new System.NotImplementedException ();
		}

		public void RequestBreak(IEnumerable<CellSplitInfo> collection, int index)
		{
			this.EnsureValid ();

			this.currentSplitInfos = collection;
			this.currentSplitIndex = index;
		}


		private void EnsureValid()
		{
			if (this.IsValid)
			{
				return;
			}

			throw new System.InvalidOperationException ("Invalid state for navigator");
		}


		private readonly DataView view;
		private string path;
		private IEnumerable<CellSplitInfo> currentSplitInfos;
		private int currentSplitIndex;
	}
}
