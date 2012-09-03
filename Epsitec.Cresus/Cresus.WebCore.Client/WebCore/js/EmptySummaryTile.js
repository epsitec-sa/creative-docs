Ext.define('Epsitec.cresus.webcore.EmptySummaryTile', {
  extend: 'Epsitec.cresus.webcore.CollectionSummaryTile',
  alternateClassName: ['Epsitec.EmptySummaryTile'],
  alias: 'widget.epsitec.emptysummarytile',

  /* Config */

  html: 'Empty',

  /* Constructor */

  constructor: function(options) {
    var newOptions = {
      hideRemoveButton: true
    };
    Ext.applyIf(newOptions, options);

    this.callParent([newOptions]);
    return this;
  },

  /* Additional methods */

  bodyClickHandler: function() {
    this.addEntityHandler();
  }
});
