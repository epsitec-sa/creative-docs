Ext.require([
  'Epsitec.cresus.webcore.entityList.EntityListPanel',
  'Epsitec.cresus.webcore.entityUi.EntityColumn'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityUi.SetColumn', {
    extend: 'Epsitec.cresus.webcore.entityUi.EntityColumn',
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
          enableCreate: options.displayDatabase.enableCreate,
          enableDelete: options.displayDatabase.enableDelete,
          pickDatabaseDefinition: options.pickDatabase,
          multiSelect: true,
          onSelectionChange: null
        }
      });
    }
  });
});
