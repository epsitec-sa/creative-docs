Ext.define('Epsitec.Cresus.Core.Static.EmptySummaryTile', {
  extend: 'Epsitec.Cresus.Core.Static.CollectionSummaryTile',
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
