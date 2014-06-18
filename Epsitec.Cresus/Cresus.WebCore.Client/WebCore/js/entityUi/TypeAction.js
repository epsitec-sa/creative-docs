// This class is the action window that is used for actions that are not related
// to some entities but to an entity type.

Ext.require([
  'Epsitec.cresus.webcore.entityUi.Action',
  'Epsitec.cresus.webcore.entityUi.BrickWallParser',
  'Epsitec.cresus.webcore.tools.Tools',
  'Epsitec.cresus.webcore.tools.ViewMode'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityUi.TypeAction', {
    extend: 'Epsitec.cresus.webcore.entityUi.Action',
    alternateClassName: ['Epsitec.TypeAction'],

    /* Methods */

    getFormUrl: function(options) {
      return 'proxy/entity/action/type/' + options.viewMode + '/' +
          options.viewId + '/' + options.entityTypeId;
    },

    handleSave: function(json) {
      var entityId = json.content.entityId;
      this.callback.execute([entityId]);
    },

    /* Static methods */

    statics: {
      showDialog: function(viewId, typeId, inQueue, callback) {
        var url = 'proxy/layout/type/' + Epsitec.ViewMode.brickCreation + '/' +
            viewId + '/' + typeId;

        Epsitec.Action.showDialog(url, 'Epsitec.TypeAction', inQueue, callback);
      }
    }
  });
});
