Ext.require([
  'Epsitec.cresus.webcore.entityUi.Action',
  'Epsitec.cresus.webcore.entityUi.BrickWallParser',
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityUi.TypeAction', {
    extend: 'Epsitec.cresus.webcore.entityUi.Action',
    alternateClassName: ['Epsitec.TypeAction'],

    /* Additional methods */

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
      showDialog: function(viewId, typeId, callback) {
        var url = 'proxy/layout/type/8/' + viewId + '/' + typeId;

        Epsitec.Action.showDialog(url, 'Epsitec.TypeAction', callback);
      }
    }
  });
});
