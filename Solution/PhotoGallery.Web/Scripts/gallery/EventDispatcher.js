/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function EventDispatcher() {
};

/*----------------------------------------------------------------------------------------------------*/
EventDispatcher.prototype.listen = function(pEventName, pCallbackScope, pCallback) {
	if ( !this.listeners ) {
		this.listeners = {};
	}

	if ( !this.listeners[pEventName] ) {
		this.listeners[pEventName] = [];
	}

	var closure = function(pScope) {
		return function() {
			pCallback.apply(pScope);
		};
	};

	this.listeners[pEventName].push(closure(pCallbackScope));
};

/*----------------------------------------------------------------------------------------------------*/
EventDispatcher.prototype.send = function(pEventName) {
	var list = this.listeners[pEventName];

	for ( var i = 0 ; i < list.length ; ++i ) {
		list[i].call();
	}
};
