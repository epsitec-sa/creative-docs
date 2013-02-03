Ext.require([
  'Epsitec.cresus.webcore.entityUi.Action',
  'Epsitec.cresus.webcore.entityUi.BrickWallParser',
  'Epsitec.cresus.webcore.tools.Tools'
],
function() {
  Ext.define('Epsitec.cresus.webcore.entityUi.EntityAction', {
    extend: 'Epsitec.cresus.webcore.entityUi.Action',
    alternateClassName: ['Epsitec.EntityAction'],

    /* Additional method */

    getFormUrl: function(options) {
      var prefix, viewMode, viewId, entityId, additionalEntityId;

      prefix = 'proxy/entity/action/entity';
      viewMode = options.viewMode;
      viewId = options.viewId;
      entityId = options.entityId;
      additionalEntityId = options.additionalEntityId;

      return Epsitec.EntityAction.getUrl(
          prefix, viewMode, viewId, entityId, additionalEntityId
      );
    },

    handleSave: function(json) {
      this.callback.execute([]);
    },

    /* Static methods */

    statics: {
      showDialog: function(viewMode, viewId, entityId, aEntityId, callback) {
        var prefix, url;

        prefix = 'proxy/layout/entity';
        url = this.getUrl(prefix, viewMode, viewId, entityId, aEntityId);

        Epsitec.Action.showDialog(url, 'Epsitec.EntityAction', callback);
      },

      getUrl: function(prefix, viewMode, viewId, entityId, additionalEntityId) {
        var url = prefix + '/' + viewMode + '/' + viewId + '/' + entityId;

        if (additionalEntityId !== null) {
          url += '/' + additionalEntityId;
        }

        return url;
      }
    }
  });
});
