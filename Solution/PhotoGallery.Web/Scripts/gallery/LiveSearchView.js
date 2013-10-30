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
LiveSearchView.prototype.buildView = function(pInputPlaceholder, pShowClose) {
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
		.css('padding-right', (pShowClose ? '34px' : '5px'))
		.attr('placeholder', (pInputPlaceholder ? pInputPlaceholder : ''))
		.keyup(keyClosure(this));

	var close = $('<div>')
		.attr('class', 'close')
		.attr('title', 'Cancel')
		.append($('<span>')
			.attr('class', 'fa fa-times')
		)
		.fadeTo(0, 0.5)
		.click(closeClosure(this));

	if ( !isTouch() ) {
		close
			.mouseenter(function() {
				$(this).fadeTo(0, 1.0);
			})
			.mouseleave(function() {
				$(this).fadeTo(0, 0.5);
			});
	}

	this.loading = $('<div>')
		.attr('class', 'loading')
		.append($('<span>')
			.attr('class', 'fa fa-spinner fa-spin')
		)
		.hide();

	var inputHold = $('<div>')
		.attr('class', 'topBar')
		.append(this.input)
		.append(close)
		.append(this.loading);
		
	var table = $('<table>');

	this.scroller = $('<div>')
		.attr('class', 'scroller')
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
					.attr('class', 'noResults')
					.html('No results found for "'+text+'".')
					.append($('<br/>'))
					.append($('<span>')
						.attr('class', 'disamb')
						.html('Please try a different search.')
					)
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
			if ( pScope.shouldIgnoreMoveUnderCursor() ) {
				return;
			}

			pScope.highHover = true;
			pScope.liveSearch.onHighlight(pArtifactId);
			pScope.highHover = false;
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
	this.input.focus();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.hide = function() {
	$(this.selector).hide();
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.beforeMoveUnderCursor = function() {
	this.moveUnder = new Date().getTime();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.shouldIgnoreMoveUnderCursor = function() {
	var ms = new Date().getTime()-this.moveUnder;
	//console.log('LiveSearchView.shouldIgnoreMoveUnderCursor(): '+ms);
	return (ms < 500);
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
			this.beforeMoveUnderCursor();
			this.liveSearch.shiftHighlight(-1);
			break;

		case DOWN_ARROW:
			this.beforeMoveUnderCursor();
			this.liveSearch.shiftHighlight(1);
			break;
	}
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSearchStart = function() {
	this.loading.show();
	this.loadingFirst = true;
	//this.scroller.hide();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSearchData = function() {
	//this.loading.html(this.liveSearch.getResults().length+'');
	this.beforeMoveUnderCursor();
	this.updateResults();

	if ( this.loadingFirst ) {
		this.loadingFirst = false;
		this.scroller.scrollTop(0);
	}

	this.onHighlight();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSearchStop = function() {
	this.loading.hide();
	this.onSearchData();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onSelect = function(pArtifactId) {
	this.liveSearch.onSelect(pArtifactId);
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onClose = function() {
	this.liveSearch.onClose();
	this.hide();
	this.liveSearch.updateText('');
	this.input.val('');
	this.updateResults();
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchView.prototype.onHighlight = function() {
	var id = this.liveSearch.getHighlightId();
	var highTd = null;

	$(this.selector+' td').each(function() {
		var td = $(this);
		var high = (td.attr('data-id') == id);
		$(this).attr('class', (high ? 'highlight' : ''));

		if ( high ) {
			highTd = td;
		}
	});

	if ( highTd == null || this.highHover ) {
		return;
	}

	var rsH = this.scroller.height();
	var rsS = this.scroller.scrollTop();

	var tr = highTd.parent();
	var trH = tr.height();
	var trY = tr.offset().top-this.scroller.offset().top;

	if ( trY < 0 ) {
		this.scroller.scrollTop(rsS+trY);
	}
	else if ( trY+trH > rsH ) {
		this.scroller.scrollTop(rsS+trY-rsH+trH);
	}
};
