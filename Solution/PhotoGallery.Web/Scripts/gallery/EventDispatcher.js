/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function EventDispatcher(pName) {
	this.name = (pName ? pName : '???');
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
	if ( !this.listeners ) {
		this.listeners = {};
	}

	var list = this.listeners[pEventName];
	var n = (list ? list.length : 0);
	console.log('EventDispatcher['+this.name+'].send('+pEventName+'): '+n+' listener(s)');

	for ( var i = 0 ; i < n ; ++i ) {
		list[i].call();
	}
};
