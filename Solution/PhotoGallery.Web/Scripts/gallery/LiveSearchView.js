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
		.attr('class', 'loading')
		.css('background-color', 'yellow')
		.html('LOADING (0)')
		.hide();
		
	var table = $('<table>')
		.attr('class', 'liveSearch')
		.css('margin', '0');

	this.scroller = $('<div>')
		.attr('class', 'scroller')
		.css('max-height', '220px')
		.css('overflow', 'auto')
		.append(table);

	this.tbody = $('<tbody>')
		.appendTo(table);

	$(this.selector)
		.html('')
		.append(this.input)
		.append(this.loading)
		.append(this.scroller);
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
	if ( !this.isVisible() ) {
		return;
	}

	this.liveSearch.updateText(this.input.val());

	/*switch ( key ) {
		case ENTER:
			this.onSelect(this.highArtifactId);
			break;

		case UP_ARROW:
			this.onHighlight(-1);
			break;

		case DOWN_ARROW:
			this.onHighlight(1);
			break;
	}*/
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSearchStart = function() {
	this.loading.show();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSearchData = function() {
	this.loading.html('LOADING ('+this.liveSearch.getResults().length+')');
	this.updateResults();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSearchStop = function() {
	this.loading.hide();
	this.updateResults();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSelect = function(pArtifactId, pFromClick) {
	this.liveSearch.onTagSelect(pArtifactId);
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.updateResults = function() {
	var results = this.liveSearch.getResults();
	var n = results.length;

	var selectClosure = function(pScope, pArtifactId) {
		return function() {
			pScope.onSelect(pArtifactId, true);
		};
	};

	this.tbody.html('');

	for ( var i = 0 ; i < n ; ++i ) {
		var t = results[i];

		var td = $('<td>')
			.attr('data-id', t.ArtifactId+'')
			.attr('title', '['+t.ArtifactId+']'+(t.Note ? ' '+t.Note : ''))
			.html(t.Name)
			.click(selectClosure(this, t.ArtifactId));

		if ( t.Disamb ) {
			var span = $('<span>')
				.attr('class', 'disamb')
				.css('font-size', '12px')
				.css('color', 'rgba(0, 0, 0, 0.5)')
				.html(t.Disamb);

			td.append('<br/>').append(span);
		}

		var tr = $('<tr>').append(td);
		this.tbody.append(tr);
	}
};

//TODO: highlighting
