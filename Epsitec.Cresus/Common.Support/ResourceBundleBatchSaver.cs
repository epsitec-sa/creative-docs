//	Copyright � 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>ResourceBundleBatchSaver</c> class is used to save <see
	/// cref="ResourceBundle"/> instances through the resource accessors,
	/// postponing all operations until its <c>Execute</c> method gets
	/// called.
	/// </summary>
	public class ResourceBundleBatchSaver
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceBundleBatchSaver"/> class.
		/// </summary>
		public ResourceBundleBatchSaver()
		{
			this.bundles = new Dictionary<ResourceBundle, ResourceSetMode> ();
			this.blackList = new List<ResourceBundle> ();
		}

		/// <summary>
		/// Remember that the bundle has to be saved, using the specified mode.
		/// The operation will be performed when <c>Execute</c> is called.
		/// </summary>
		/// <param name="manager">The resource manager.</param>
		/// <param name="bundle">The resource bundle.</param>
		/// <param name="mode">The save mode.</param>
		public void DelaySave(ResourceManager manager, ResourceBundle bundle, ResourceSetMode mode)
		{
			System.Diagnostics.Debug.Assert (manager != null);
			System.Diagnostics.Debug.Assert (bundle != null);
			System.Diagnostics.Debug.Assert (bundle.ResourceManager == manager);

			switch (mode)
			{
				case ResourceSetMode.CreateOnly:
					if (this.blackList.Contains (bundle))
					{
						//	The bundle was on a black list (it was deleted before),
						//	but since we re-create it now, we can safely remove it
						//	from the black list :

						this.blackList.Remove (bundle);
					}
					
					this.bundles[bundle] = ResourceSetMode.Write;
					break;
				
				case ResourceSetMode.UpdateOnly:
				case ResourceSetMode.Write:
					if (this.blackList.Contains (bundle))
					{
						//	Nothing to do, the bundle is on a black list and should
						//	never be written back to disk...
					}
					else
					{
						this.bundles[bundle] = ResourceSetMode.Write;
					}
					break;

				case ResourceSetMode.Remove:
					if (this.blackList.Contains (bundle))
					{
						//	Nothing to do, the bundle has already been added to
						//	the black list, so it must have been deleted before.
					}
					else
					{
						this.blackList.Add (bundle);
						this.bundles[bundle] = ResourceSetMode.Remove;
					}
					break;

				default:
					throw new System.NotImplementedException ();
			}
		}

		/// <summary>
		/// Executes all pending save operations.
		/// </summary>
		public void Execute()
		{
			foreach (KeyValuePair<ResourceBundle, ResourceSetMode> pair in this.bundles)
			{
				ResourceBundle  bundle  = pair.Key;
				ResourceSetMode mode    = pair.Value;
				ResourceManager manager = bundle.ResourceManager;
				
				manager.SetBundle (bundle, mode);
			}

			this.bundles.Clear ();
			this.blackList.Clear ();
		}

		Dictionary<ResourceBundle, ResourceSetMode> bundles;
		List<ResourceBundle> blackList;
	}
}
