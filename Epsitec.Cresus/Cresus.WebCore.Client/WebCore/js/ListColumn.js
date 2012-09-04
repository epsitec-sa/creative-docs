Ext.define('Epsitec.cresus.webcore.ListColumn', {
  extend: 'Ext.grid.column.Column',
  alias: ['widget.listcolumn'],
  alternateClassName: ['Epsitec.ListColumn'],

  /* Config */
  loadingText: Epsitec.Texts.getLoadingText(),

  /* Properties */

  store: null,
  refreshPending: false,

  /* Constructor */

  constructor: function(options) {
    var newOptions = {
      store: Epsitec.Enumeration.getStore(options.enumerationName)
    };
    Ext.applyIf(newOptions, options);

    this.callParent([newOptions]);
    return this;
  },

  /* Additional methods */

  defaultRenderer: function(value, data, record, colIdx, cellIdx, store, view) {

    // If the store is not yet loaded, we can't dispay anything, so we return a
    // dummy text and schedule a refresh for later on. Unfortunately, for now,
    // the refresh will refresh the whole table, as I have not found a way to
    // refresh only the current column.

    if (this.store.isLoading()) {
      this.requestRefresh(view);
      return this.loadingText;
    }

    return this.store.findRecord('id', value).get('text');
  },

  requestRefresh: function(view) {
    if (this.refreshPending) {
      return;
    }
    this.refreshPending = true;
    this.store.on('load', function() { this.refresh(view); }, this);
  },

  refresh: function(view) {
    this.refreshPending = false;

    // The view might have been hidden or destroyed while we where loading the
    // store, so we have to check it we can refresh it before doing it.

    if (view.isVisible()) {
      view.refresh();
    }
  }
});
