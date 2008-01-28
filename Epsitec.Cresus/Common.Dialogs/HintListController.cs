//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Types;

using System.Collections.Generic;

[assembly: DependencyClass (typeof (HintListController))]

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>HintListController</c> class manages a hint list associated with
	/// a dialog or form.
	/// </summary>
	public sealed class HintListController : DependencyObject
	{
		public HintListController()
		{
			this.searchController = new DialogSearchController ();
			this.searchController.SuggestionChanged += this.HandleSearchControllerSuggestionChanged;

			DialogSearchController.GlobalSearchContextChanged += this.HandleGlobalSearchContextChanged;
		}


		public DialogSearchController SearchController
		{
			get
			{
				return this.searchController;
			}
		}

		public ISearchContext ActiveSearchContext
		{
			get
			{
				return this.activeSearchContext;
			}
			private set
			{
				if (this.activeSearchContext != value)
				{
					this.activeSearchContext = value;
					this.OnActiveSearchContextChanged ();
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.searchController.SuggestionChanged -= this.HandleSearchControllerSuggestionChanged;
				DialogSearchController.GlobalSearchContextChanged -= this.HandleGlobalSearchContextChanged;
			}

			base.Dispose (disposing);
		}

		private void OnActiveSearchContextChanged()
		{
			//	TODO: the active search context changed, refresh the UI list
		}


		
		private void HandleGlobalSearchContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			System.Diagnostics.Debug.Write (e.ToString ());

			ISearchContext context = e.NewValue as ISearchContext;

			if ((context != null) &&
				(context.SearchController == this.searchController))
			{
				this.activeSearchContext = context;
			}
		}

		private void HandleSearchControllerSuggestionChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
		}


		private readonly DialogSearchController searchController;
		private ISearchContext activeSearchContext;
	}
}
