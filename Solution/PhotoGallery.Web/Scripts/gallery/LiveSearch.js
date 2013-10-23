/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function LiveSearch() {
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.init = function(pSearchUrlFunc, pUniqueIdFunc) {
	this.searchUrlFunc = pSearchUrlFunc;
	this.uniqueIdFunc = pUniqueIdFunc;
	this.events = new EventDispatcher('LiveSearch');
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.updateText = function(pText) {
	this.text = pText;

	var searchClosure = function(pScope) {
		return function() {
			pScope.doSearch();
		};
	};

	clearTimeout(this.timer);
	this.timer = setTimeout(searchClosure(this), 500);
	this.events.send('textUpdated');
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.getResults = function() {
	return this.results;
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.getNewResults = function() {
	return this.newResults;
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.clearResults = function() {
	this.results = [];
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.appendResults = function(pResults) {
	this.newResults = pResults;
	this.idMap = {};

	var items = this.results.concat(pResults);
	var n = items.length;

	for ( var i = 0 ; i < n ; ++i ) {
		var item = items[i];
		var id = this.uniqueIdFunc(item);

		if ( !this.idMap[id] ) {
			this.idMap[id] = item;
			this.results.push(item);
		}
	}
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.doSearch = function() {
	if ( this.text == this.searchText ) {
		return;
	}
	
	if ( this.request ) {
		this.abortSearch();
	}

	this.searchText = this.text+'';
	this.events.send('searchStarted');
	this.clearResults();

	if ( this.text == '' ) {
		this.onSearchFinish();
		return;
	}

	this.continueSearch(true);
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.continueSearch = function(pFirst) {
	if ( this.text != this.searchText ) {
		return;
	}

	var url = this.searchUrlFunc(this.searchText, pFirst);

	var dataClosure = function(pScope) {
		return function(pData) {
			pScope.onSearchData(pData);
		};
	};

	if ( !pFirst ) {
		this.events.send('searchContinued');
	}

	this.request = jQuery.post(url, null, dataClosure(this));
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.onSearchData = function(pData) {
	if ( this.text != this.searchText ) {
		return;
	}

	if ( !pData || !pData.length ) {
		this.onSearchFinish();
		return;
	}

	this.appendResults(pData);
	this.events.send('searchDataReceived');
	this.continueSearch(false);
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.onSearchFinish = function() {
	this.events.send('searchFinished');
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.abortSearch = function() {
	if ( !this.request ) {
		return;
	}
	
	this.request.abort();
	this.request = null;
	this.events.send('searchAborted');
};
