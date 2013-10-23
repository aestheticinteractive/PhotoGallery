/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function LiveSearchView(pLiveSearch, pSelector) {
	this.liveSearch = pLiveSearch;
	this.selector = pSelector;

	this.liveSearch.events.listen("searchStarted", this, this.onSearchStart);
	this.liveSearch.events.listen("searchDataReceived", this, this.onSearchData);
	this.liveSearch.events.listen("searchAborted", this, this.onSearchStop);
	this.liveSearch.events.listen("searchFinished", this, this.onSearchStop);
}

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.initView = function(pInputPlaceholder) {
	var keyClosure = function(pScope) {
		return function() {
			pScope.onKeyUp();
		};
	};

	this.input = $('<input>')
		.attr('type', 'text')
		.attr('class', 'liveSearch')
		.css('margin-bottom', '0')
		.attr('placeholder', (pInputPlaceholder ? pInputPlaceholder : ''))
		.keyup(keyClosure(this));

	this.loading = $('<div>')
		.html('LOADING (0)')
		.css('background-color', 'yellow')
		.hide();

	$(this.selector)
		.html('')
		.append(this.input)
		.append(this.loading);
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.isVisible = function() {
	return $(this.selector).is(':visible');
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.show = function() {
	$(this.selector).show();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.hide = function() {
	$(this.selector).hide();
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onKeyUp = function() {
	this.liveSearch.updateText(this.input.val());
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSearchStart = function() {
	this.loading.show();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSearchData = function() {
	this.loading.html('LOADING ('+this.liveSearch.getResults().length+')');
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSearchStop = function() {
	this.loading.hide();
};
