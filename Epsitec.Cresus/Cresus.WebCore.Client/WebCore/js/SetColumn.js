Ext.require([
  'Epsitec.cresus.webcore.EntityColumn',
  'Epsitec.cresus.webcore.EntityListPanel'
],
function() {
  Ext.define('Epsitec.cresus.webcore.SetColumn', {
    extend: 'Epsitec.cresus.webcore.EntityColumn',
    alternateClassName: ['Epsitec.SetColumn'],

    /* Config */

    border: true,
    width: 400,
    height: '100%',
    layout: 'fit',
    resizable: true,
    resizeHandles: 'e',

    /* Properties */

    displayDatabase: null,
    pickDatabase: null,

    /* Constructor */

    constructor: function(options) {
      var newOptions = {
        items: [this.createEntityListPanel(options)]
      };
      Ext.applyIf(newOptions, options);

      this.callParent([newOptions]);
      return this;
    },

    /* Additional methods */

    createEntityListPanel: function(options) {
      return Ext.create('Epsitec.EntityListPanel', {
        container: {
          border: false
        },
        list: {
          entityListTypeName: 'Epsitec.SetEditableEntityList',
          setColumn: this,
          viewId: options.viewId,
          entityId: options.entityId,
          columnDefinitions: options.displayDatabase.columns,
          sorterDefinitions: options.displayDatabase.sorters,
          pickDatabaseDefinition: options.pickDatabase,
          multiSelect: true,
          onSelectionChange: null
        }
      });
    }
  });
});
