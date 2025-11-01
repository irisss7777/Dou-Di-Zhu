mergeInto(LibraryManager.library, {
	OnGameLoaded: function(){
		window.dispatchReactUnityEvent(
			"OnGameLoaded"
		);
	},
	OnGameStarted: function(){
		window.dispatchReactUnityEvent(
			"OnGameStarted"
		);
	},
	OnTimed: function(){
		window.dispatchReactUnityEvent(
			"OnTimed"
		);
	},
	OnGameExit: function(){
		window.dispatchReactUnityEvent(
			"OnGameExit"
		);
	},
	OnScoreUpdate: function(isWin, scoreAdded, scoreTotal){
		window.dispatchReactUnityEvent(
			"OnScoreUpdate",
			isWin,
			scoreAdded,
			scoreTotal
		);
	}
});