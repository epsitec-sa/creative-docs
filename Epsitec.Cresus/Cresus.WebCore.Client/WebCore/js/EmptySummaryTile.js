Ext.define('Epsitec.cresus.webcore.EmptySummaryTile', {
  extend: 'Epsitec.cresus.webcore.CollectionSummaryTile',
  alternateClassName: ['Epsitec.EmptySummaryTile'],
  alias: 'widget.emptysummarytile',

  /* Config */

  html: 'Empty',

  /* Constructor */

  constructor: function(o) {
    var options = o || {};

    options.hideRemoveButton = true;

    this.callParent([options]);
    return this;
  },

  /* Additional methods */

  // Override
  bodyClicked: function() {
    this.addEntity();
  }
});
