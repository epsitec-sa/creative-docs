//	Copyright © 2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
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
			this.searchController.Resolved += this.HandleSearchControllerResolved;

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
				this.searchController.Resolved -= this.HandleSearchControllerResolved;
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
			AbstractEntity suggestion = e.NewValue as AbstractEntity;
		}

		private void HandleSearchControllerResolved(object sender)
		{
			EntityResolverResult resolverResult = this.searchController.GetResolverResult (this.activeSearchContext);

			System.Diagnostics.Debug.WriteLine (string.Format ("Found {0} results", resolverResult.AllResults.Count));
		}

		private readonly DialogSearchController searchController;
		private ISearchContext activeSearchContext;
	}
}
