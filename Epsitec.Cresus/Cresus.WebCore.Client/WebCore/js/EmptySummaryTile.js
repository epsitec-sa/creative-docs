Ext.require([
  'Epsitec.cresus.webcore.CollectionSummaryTile',
  'Epsitec.cresus.webcore.Texts'
],
function() {
  Ext.define('Epsitec.cresus.webcore.EmptySummaryTile', {
    extend: 'Epsitec.cresus.webcore.CollectionSummaryTile',
    alternateClassName: ['Epsitec.EmptySummaryTile'],
    alias: 'widget.epsitec.emptysummarytile',

    /* Config */

    html: Epsitec.Texts.getEmptySummaryText(),

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
});
