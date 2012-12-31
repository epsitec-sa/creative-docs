// We need this subclass in order to override the readRecords method. The
// problem is that the same setting is used to configure where the tree store
// should look for children within a node and for nodes within the result of a
// request. So here we change the location of the nodes within the result of the
// request to have them at the same location as the children within a node.

Ext.define('Epsitec.cresus.webcore.plugins.AiderGroupReader', {
  extend: 'Ext.data.reader.Json',
  alternateClassName: ['Epsitec.AiderGroupReader'],

  /* Additional methods */

  readRecords: function(data) {
    return this.callParent([{
      success: data.success,
      groups: data.content.data.groups
    }]);
  }
});
