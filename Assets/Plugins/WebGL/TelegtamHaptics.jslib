mergeInto(LibraryManager.library, {
    TriggerHapticFeedback: function(type) {
        if (window.Telegram && window.Telegram.WebApp) {
            Telegram.WebApp.HapticFeedback.notificationOccurred("success");
        }
    }
});