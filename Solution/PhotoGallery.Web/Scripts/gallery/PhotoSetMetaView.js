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
		var buckets = psm.getBuckets(pValues, 20);
		var avg = psm.getAvg(pValues);
		var count = buckets.length;
		var buckMax = 0;

		for ( var i = 0 ; i < count ; ++i ) {
			buckMax = Math.max(buckMax, buckets[i].count);
		}

		var histDiv = $('<div>')
			.attr('class', 'hist');
		var histH = 40;

		for ( var i = 0 ; i < count ; ++i ) {
			var b = buckets[i];
			var weight = 1-b.count/buckMax;
			var h = (1-weight)*histH;
			var col = pusher.color("#855").hue('+'+(weight*225));

			histDiv
				.append($('<div>')
					.attr('title', pStrFunc(b.min)+' to '+pStrFunc(b.max)+' ('+b.count+')')
					.attr('class', 'slot')
					//.css('background-color', col.alpha(0.25).html())
					.css('width', (100/count)+'%')
					.append($('<div>')
						.attr('class', 'bar')
						//.css('border-top', '2px solid #333')
						.css('background-color', col.html())
						.css('height', h+'px')
						.css('margin-top', (histH-h)+'px')
					)
				);
		}

		return $('<div>')
			.attr('class', 'metaCell')
			.append($('<div>')
				.attr('class', 'val')
				.html(pStrFunc(avg))
			)
			.append($('<div>')
				.attr('class', 'name')
				.html(pName)
			)
			.append(histDiv)
			.append($('<div>')
				.css('clear', 'left')
			);
	};

	var buildBasic = function(pName, pValue) {
		return $('<div>')
			.attr('class', 'metaCell')
			.append($('<div>')
				.attr('class', 'val')
				.html(pValue)
			)
			.append($('<div>')
				.attr('class', 'name')
				.html(pName)
			);
	};

	var expVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.exposure; });
	var fnmVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.fNumber; });
	var focVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.focalLen; });
	var isoVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.isoSpeed; });

	var expCell = buildCell('Avg. Shutter Speed', expVals, psm.getExposureStr);
	var fnmCell = buildCell('Avg. F-Number', fnmVals, psm.getFNumberStr);
	var focCell = buildCell('Avg. Focal Length', focVals, psm.getFocalLengthStr);
	var isoCell = buildCell('Avg. ISO Speed', isoVals, psm.getIsoSpeedStr);
	
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
					.attr('class', 'large-6 columns')
					.append(buildBasic("Photos", this.photoSetMeta.photoSet.dataList.length))
				)
				.append($('<div>')
					.attr('class', 'large-6 columns')
					.append(buildBasic("Flash Usage", this.photoSetMeta.getFlashUsageStr(flaAvg)))
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
