/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSetMetaView(pPhotoSetMeta, pSelector) {
	this.photoSetMeta = pPhotoSetMeta;
	this.selector = pSelector;
	this.hold = $(this.selector);
	this.photoSetMeta.events.listen('dataLoaded', this, this.buildView);
	this.photoSetMeta.photoSet.events.listen("filterChanged", this, this.buildView);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMetaView.prototype.buildView = function() {
	//var cf = this.photoSetMeta.photoSet.getPhotoCount(true);
	//var ct = this.photoSetMeta.photoSet.getPhotoCount(false);

	this.loaded = true;
	this.hold.html('');
	var psm = this.photoSetMeta;

	var buildCell = function(pName, pValues, pStrFunc) {
		var avg = psm.getAvg(pValues);
		var buckets = psm.getBuckets(pValues, 16);
		var count = buckets.length;
		var bucketDiv = $('<div>');
		var buckMax = 0;

		for ( var i = 0 ; i < count ; ++i ) {
			buckMax = Math.max(buckMax, buckets[i].count);
		}

		for ( var i = 0 ; i < count ; ++i ) {
			var b = buckets[i];
			var weight = 1-b.count/buckMax;
			var col = pusher.color("#b66").hue('+'+(weight*225));
				//.alpha(weight == 1 ? 0 : 1);
				//.alpha((1-weight)*0.5+0.5);
				//.saturation((1-weight)*75+25);

			bucketDiv
				.append($('<div>')
					.attr('class', 'histBar')
					.attr('title', pStrFunc(b.min)+' to '+pStrFunc(b.max)+' ('+b.count+')')
					.css('background-color', col.html())
					.css('width', (100/count)+'%')
				);
		}

		return $('<div>')
			.attr('class', 'metaCell')
			.append($('<div>')
				.attr('class', 'avg')
				.html(pStrFunc(avg))
			)
			.append($('<div>')
				.attr('class', 'name')
				.html(pName)
			)
			.append(bucketDiv)
			.append($('<div>')
				.css('clear', 'left')
			);
	};

	var expVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.exposure; });
	var fnmVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.fNumber; });
	var focVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.focalLen; });
	var isoVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.isoSpeed; });

	var expCell = buildCell('Avg. Shutter Speed', expVals, this.photoSetMeta.getExposureStr);
	var fnmCell = buildCell('Avg. F-Number', fnmVals, this.photoSetMeta.getFNumberStr);
	var focCell = buildCell('Avg. Focal Length', focVals, this.photoSetMeta.getFocalLengthStr);
	var isoCell = buildCell('Avg. ISO Speed', isoVals, this.photoSetMeta.getIsoSpeedStr);
	
	var flaVals = this.photoSetMeta.getValues(
		function(pPhotoData) { return (pPhotoData.flash == true ? 1 : 0); });
	var flaAvg = this.photoSetMeta.getAvg(flaVals);

	this.hold
		.append($('<div>')
			.attr('class', 'content')
			.append($('<div>')
				.attr('class', 'row')
				.append($('<div>')
					.attr('class', 'large-6 columns')
					.append(expCell)
				)
				.append($('<div>')
					.attr('class', 'large-6 columns')
					.append(fnmCell)
				)
			)
			.append($('<div>')
				.attr('class', 'row')
				.append($('<div>')
					.attr('class', 'large-6 columns')
					.append(focCell)
				)
				.append($('<div>')
					.attr('class', 'large-6 columns')
					.append(isoCell)
				)
			)
			.append($('<div>')
				.attr('class', 'row')
				.append($('<div>')
					.attr('class', 'large-12 columns')
					.append($('<div>')
						.attr('class', 'metaCell')
						.css('text-align', 'center')
						.append($('<div>')
							.attr('class', 'avg')
							.html(this.photoSetMeta.getFlashUsageStr(flaAvg))
						)
						.append($('<div>')
							.attr('class', 'name')
							.html('Flash Usage')
						)
					)
				)
			)
		);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMetaView.prototype.isVisible = function() {
	return $(this.selector).is(':visible');
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMetaView.prototype.show = function() {
	$(this.selector).show();

	if ( !this.loaded ) {
		this.photoSetMeta.loadData();
	}
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMetaView.prototype.hide = function() {
	$(this.selector).hide();
};
