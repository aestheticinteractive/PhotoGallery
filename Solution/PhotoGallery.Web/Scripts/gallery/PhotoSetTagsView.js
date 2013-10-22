﻿/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSetTagsView(pPhotoSetTags, pSelector) {
	this.photoSetTags = pPhotoSetTags;
	this.selector = pSelector;
	this.photoSetTags.events.listen('dataLoaded', this, this.buildView);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTagsView.prototype.buildView = function() {
	var hold = $(this.selector).html('');
	var pst = this.photoSetTags;
	var tags = pst.getTags();

	if ( tags.length == 0 ) {
		$(this.selector).hide();
		return;
	}

	var onTagClick = function(pEvent) {
		var t = $(this);
		var id = t.attr('data-id');
		pst.onTagClick(id);

		tagDivs.each(function() {
			var tagId = $(this).attr('data-id');
			$(this).attr('class', 'tag'+(pst.isTagActive(tagId) ? ' tagActive' : ''));
		});
	};

	var buildRow = function(pTag) {
		return $('<div>')
			.attr('title', pTag.Disamb)
			.attr('class', 'tag')
			.attr('data-id', pTag.Id)
			.html(pTag.Name)
			.append($('<span>')
				.attr('class', 'number')
				.html(pTag.PhotoIds.length+'')
			)
			.click(pTag.Id, onTagClick);
	};

	var buildList = function() {
		var div = $('<div>');

		for ( var i = 0 ; i < tags.length ; ++i ) {
			div.append(buildRow(tags[i]));
		}

		return div;
	};

	$(hold).append(buildList());

	var tagDivs = $('#Tags .tag');
};

/*----------------------------------------------------------------------------------------------------* /
PhotoSetTagsView.prototype.buildView2 = function() {
	var hold = $(this.selector).html('');
	var pst = this.photoSetTags;
	var tags = pst.getTags();

	var buildRow = function(pTag) {
		return $('<tr>')
			.css('height', '30px')
			.css('margin-bottom', '1px')
			.append($('<td>')
				.append($('<div>')
					.css('width', (pst.getTagWeight(pTag.Id)*100)+'%')
					.css('height', '22px')
					.css('margin-bottom', '-22px')
					.css('background-color', '#ccc')
					.css('background-color', 'rgba(51, 136, 204, 0.2)')
				)
				.append($('<p>')
					.css('margin-left', '4px')
					.html(pTag.Name)
					.append($('<span>')
						.css('font-size', '11px')
						.css('margin-left', '6px')
						.css('color', 'rgba(0, 0, 0, 0.5)')
						.html(pTag.PhotoIds.length+'')
					)
				)
			);
	};

	var buildList = function() {
		var tbody = $('<tbody>');

		for ( var i = 0 ; i < tags.length ; ++i ) {
			tbody.append(buildRow(tags[i]));
		}

		return tbody;
	};

	$(hold)
		.css('max-height', '300px')
		.css('overflow', 'auto')
		.append($('<table>')
			.css('margin', '0')
			.append(buildList())
		);
};*/
