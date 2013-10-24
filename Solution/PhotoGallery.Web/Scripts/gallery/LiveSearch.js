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
	if ( this.text == pText ) {
		return;
	}

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
	this.resultsMap = {};
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.appendResults = function(pResults) {
	this.newResults = pResults;
	var n = pResults.length;

	for ( var i = 0 ; i < n ; ++i ) {
		var item = pResults[i];
		var id = this.uniqueIdFunc(item);

		if ( !this.resultsMap[id] ) {
			this.resultsMap[id] = this.results.length;
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
	this.selectId = null;
	this.highlightId = null;
	this.clearResults();
	this.events.send('searchStarted');

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


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.onSelect = function(pUniqueId) {
	this.selectId = pUniqueId;
	this.abortSearch();
	this.events.send('itemSelected');
	alert('Selected! '+pUniqueId);
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.getSelectId = function() {
	return this.selectId;
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.getSelectItem = function() {
	var index = this.resultsMap[this.selectId];
	return this.results[index];
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.onHighlight = function(pUniqueId) {
	this.highlightId = pUniqueId;
	this.events.send('itemHighlighted');
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.shiftHighlight = function(pDir) {
	var n = this.results.length;

	if ( this.highlightId == null ) {
		if ( pDir > 0 && n > 0 ) {
			this.highlightId = this.uniqueIdFunc(this.results[0]);
			this.events.send('itemHighlighted');
		}

		return;
	}

	var index = this.resultsMap[this.highlightId]+pDir;

	if ( index < 0 || index >= n ) {
		return;
	}

	this.highlightId = this.uniqueIdFunc(this.results[index]);
	this.events.send('itemHighlighted');
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.getHighlightId = function() {
	return this.highlightId;
};

/*--------------------------------------------------------------------------------------------*/
LiveSearch.prototype.getHighlightItem = function() {
	var index = this.resultsMap[this.highlightId];
	return this.results[index];
};
