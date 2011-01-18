//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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

				this.OnSavingBundle (manager, bundle, mode);
				manager.SetBundle (bundle, mode);
				this.OnBundleSaved (manager, bundle, mode);
			}

			this.bundles.Clear ();
			this.blackList.Clear ();
		}

		/// <summary>
		/// Gets an enumeration of all the live bundles.
		/// </summary>
		/// <returns>An enumeration of all the live bundles.</returns>
		public IEnumerable<ResourceBundle> GetLiveBundles()
		{
			foreach (KeyValuePair<ResourceBundle, ResourceSetMode> pair in this.bundles)
			{
				ResourceBundle  bundle = pair.Key;
				ResourceSetMode mode   = pair.Value;
				
				if (mode != ResourceSetMode.Remove)
				{
					yield return bundle;
				}
			}
		}

		private void OnSavingBundle(ResourceManager manager, ResourceBundle bundle, ResourceSetMode mode)
		{
			if (this.SavingBundle != null)
			{
				this.SavingBundle (manager, bundle, mode);
			}
		}

		private void OnBundleSaved(ResourceManager manager, ResourceBundle bundle, ResourceSetMode mode)
		{
			if (this.BundleSaved != null)
			{
				this.BundleSaved (manager, bundle, mode);
			}
		}

		public event ResourceBundleSaver SavingBundle;
		public event ResourceBundleSaver BundleSaved;

		private Dictionary<ResourceBundle, ResourceSetMode> bundles;
		private List<ResourceBundle> blackList;
	}
}
