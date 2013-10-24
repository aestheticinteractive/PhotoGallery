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
	this.liveSearch.events.listen("itemHighlighted", this, this.onHighlight);
}

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.initView = function(pInputPlaceholder, pShowClose) {
	var keyClosure = function(pScope) {
		return function(pEvent) {
			pScope.onKeyUp(pEvent);
		};
	};

	var closeClosure = function(pScope) {
		return function() {
			pScope.onClose();
		};
	};

	this.input = $('<input>')
		.attr('type', 'text')
		.attr('class', 'liveSearch')
		.css('margin-bottom', '0')
		.css('padding-right', (pShowClose ? '30px' : '0'))
		.attr('placeholder', (pInputPlaceholder ? pInputPlaceholder : ''))
		.keyup(keyClosure(this));

	var close = $('<div>')
		.attr('class', 'close')
		.html('x')
		.click(closeClosure(this));

	this.loading = $('<div>')
		.attr('class', 'loading')
		.css('background-color', 'yellow')
		.html('LOADING (0)')
		.hide();

	var inputHold = $('<div>')
		.append(this.input)
		.append(close)
		.append(this.loading);
		
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
		.append(inputHold)
		.append(this.scroller);

	this.updateResults();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.updateResults = function() {
	var text = this.input.val();

	if ( text == '' ) {
		this.scroller.hide();
		return;
	}

	this.scroller.show();
	this.tbody.html('');

	var results = this.liveSearch.getResults();
	var n = results.length;

	if ( n == 0 ) {
		this.tbody
			.append($('<tr>')
				.append($('<td>')
					.css('font-style', 'italic')
					.css('color', 'rgba(0, 0, 0, 0.5)')
					.html('No results found for "'+text+'".')
				)
			);

		return;
	}

	var selectClosure = function(pScope, pArtifactId) {
		return function() {
			pScope.onSelect(pArtifactId);
		};
	};
	
	var highClosure = function(pScope, pArtifactId) {
		return function() {
			pScope.liveSearch.onHighlight(pArtifactId);
		};
	};

	for ( var i = 0 ; i < n ; ++i ) {
		var t = results[i];
		var aid = t.ArtifactId;

		var td = $('<td>')
			.attr('data-id', aid+'')
			.attr('title', '['+aid+']'+(t.Note ? ' '+t.Note : ''))
			.html(t.Name)
			.click(selectClosure(this, aid))
			.mouseenter(highClosure(this, aid));

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
LiveSearchView.prototype.onKeyUp = function(pEvent) {
	if ( !this.isVisible() ) {
		return;
	}

	this.liveSearch.updateText(this.input.val());

	switch ( pEvent.which ) {
		case ENTER:
			this.onSelect(this.liveSearch.getHighlightId());
			break;

		case UP_ARROW:
			this.liveSearch.shiftHighlight(-1);
			break;

		case DOWN_ARROW:
			this.liveSearch.shiftHighlight(1);
			break;
	}
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSearchStart = function() {
	this.loading.show();
	this.scroller.hide();
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
LiveSearchView.prototype.onSelect = function(pArtifactId) {
	this.liveSearch.onSelect(pArtifactId);
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onClose = function() {
	this.liveSearch.onClose();
	this.hide();
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onHighlight = function() {
	var id = this.liveSearch.getHighlightId();

	$(this.selector+' td').each(function() {
		var high = ($(this).attr('data-id') == id);
		$(this).css('background-color', (high ? '#38c' : 'transparent'));
	});
};
