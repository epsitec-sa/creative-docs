// This class is a json data reader that is used by the AiderGroupPicker
// window.

// We need this subclass in order to override the readRecords method. The
// problem is that the path from the root to the top level nodes should be the
// same as the path from within a node to its sub nodes. This is a limitation in
// the json reader of Ext JS.
// The server gives us back an object like :
// {
//   success: ...
//   content: {
//     data: {
//       groups: [{
//         id: ...,
//         summary: ...,
//         groups: [...]
//       }
//     }]
//   }
// }
// We transform it in something like :
// {
//   success: ...
//   groups: [{
//     id: ...,
//     summary: ...,
//     groups: [...]
//   }
// }]
// So from the root, we can get to the top level nodes with root.groups and from
// a node, we can get its sub nodes with node.groups, wich is the same path.

Ext.define('Epsitec.cresus.webcore.plugins.AiderGroupReader', {
  extend: 'Ext.data.reader.Json',
  alternateClassName: ['Epsitec.AiderGroupReader'],

  /* Methods */

  readRecords: function(data) {
    return this.callParent([{
      success: data.success,
      groups: data.content.data.groups
    }]);
  }
});
