/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSetTagsView(pPhotoSetTags, pSelector) {
	this.photoSetTags = pPhotoSetTags;
	this.selector = pSelector;
	this.photoSetTags.events.listen('dataLoaded', this, this.buildView);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetTagsView.prototype.buildView = function() {
	this.hold = $(this.selector).html('');

	var pst = this.photoSetTags;
	var tags = pst.getTags();

	if ( tags.length == 0 ) {
		$(this.selector).hide();
		return;
	}

	var onTagClick = function() {
		var t = $(this);
		var id = t.attr('data-id');
		pst.onTagClick(id);

		tagDivs.each(function() {
			var tagId = $(this).attr('data-id');
			var act = pst.isTagActive(tagId);

			$(this)
				.attr('class', 'tag'+(act ? ' tagActive' : ''))
				.mouseleave();
		});
	};

	var buildRow = function(pTag) {
		var isPer = pst.isPersonTag(pTag.Id);
		var w = 1-pst.getTagWeight(pTag.Id);
		var col = pusher.color("#b66").hue('+'+(w*w*w*225));
		var bgCol = col.alpha(0.55*(1-w*w*w)+0.2).html();

		return $('<div>')
			.attr('title', pTag.Disamb)
			.attr('class', 'tag')
			.attr('data-id', pTag.Id)
			.css('border', '1px solid '+col.hex6())
			.css('border', '1px solid '+col.html())
			.css('background-color', bgCol)
			.hover(
				function() {
					var hovCol = ($(this).attr('class') == 'tag' ?
						col.alpha(0.75).html() : col.shade(0.2).hex6());
					$(this).css('background-color', hovCol);
				},
				function() {
					var hovCol = ($(this).attr('class') == 'tag' ?
						bgCol : col.shade(0.2).hex6());
					$(this).css('background-color', hovCol);
				}
			)
			.append($('<div>')
				.attr('class', 'number')
				.css('background-color', col.shade(0.2).hex6())
				.html(pTag.PhotoIds.length+'')
			)
			.append($('<span>')
				.css('text-decoration', (isPer ? 'underline' : 'inherit'))
				.html(pTag.Name)
			)
			.click(pTag.Id, onTagClick);
	};

	var buildList = function() {
		var div = $('<div>')
			.attr('class', 'tagCell');

		for ( var i = 0 ; i < tags.length ; ++i ) {
			div.append(buildRow(tags[i]));
		}

		return div;
	};
	
	this.hold
		.append($('<div>')
			.attr('class', 'content')
			.append($('<div>')
				.attr('class', 'row')
				.append($('<div>')
					.attr('class', 'large-12 columns')
					.append(buildList())
				)
			)
		);

	var tagDivs = $('#Tags .tag');

	this.hold.trigger('heightChanged');
};

/*--------------------------------------------------------------------------------------------*/
PhotoSetTagsView.prototype.isVisible = function() {
	return $(this.selector).is(':visible');
};

/*--------------------------------------------------------------------------------------------*/
PhotoSetTagsView.prototype.show = function() {
	$(this.selector).show();
	this.photoSetTags.loadData();
};

/*--------------------------------------------------------------------------------------------*/
PhotoSetTagsView.prototype.hide = function() {
	$(this.selector).hide();
};