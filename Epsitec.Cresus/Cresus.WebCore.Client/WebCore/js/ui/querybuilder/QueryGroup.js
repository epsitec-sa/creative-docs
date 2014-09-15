Ext.require([
],
function() {
  Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryGroup', {
    extend: 'Ext.Panel',
    alternateClassName: ['Epsitec.QueryGroup'],

    /* Properties */

    components: null,

    /* Constructor */

    constructor: function(options) {
      var first   = options.isFirstElement(options.itemId);
      var composer = options.composer;

      this.components = [];
      var config = {
        itemId: options.itemId,
        minHeight: 50,
        minWidth: 300,
        layout: 'hbox',
        border: false,
        closable: !first,
        parent: builder,
        style: {
          borderRight: '1px solid #99BCE8',
          borderBottom: '1px solid #99BCE8',
          borderLeft: '1px solid #99BCE8'
        },
        bodyCls: 'tile',
        title: 'Groupe ' + options.itemId,
        tools: [{
          type: 'plus',
          tooltip: 'Ajouter un groupe',
          handler: composer.onAddGroup,
          scope: composer
        }],
        items: this.components,
        listeners: {
          close: composer.onRemoveGroup,
          scope: composer
        }
      };

      Ext.applyIf(config, options);
      this.callParent([config]);

      return this;
    },

    /* Methods */
    getGroup: function () {
      return {
      };
    }
  });
});
