
Ext.require([],
  function() {
    Ext.define('Epsitec.cresus.webcore.ui.querybuilder.QueryComposerPanel', {
      extend: 'Ext.Panel',
      alternateClassName: ['Epsitec.QueryComposerPanel'],

      /* Properties */
      builder: null,
      itemIdGenerator: null,
      /* Constructor */

      constructor: function(builder) {
        this.itemIdGenerator = 1;
        this.builder = builder;
        var config = {
          region: 'top',
          title: 'Compositeur',
          items: [],
          layout: {
            type: 'vbox'
          }
        };
        this.callParent([config]);

        return this;
      },

      /* Methods */
      createGroup: function (values) {
        return Ext.create(
          'Epsitec.QueryGroup', {
            itemId: this.itemIdGenerator
          });
      },

      getGroups: function () {

      },

      closeGroups: function ()
      {

      },

      resetGroups: function ()
      {

      },

      loadGroups: function (query) {

      },

      onAddGroup: function(event, toolEl, panel) {

      },

      onRemoveGroup: function(panel, e) {

      }
    });
  });
