mergeInto(LibraryManager.library, {

  SyncFs: function () {
    FS.syncfs(false, function (err) { });
  }

});
