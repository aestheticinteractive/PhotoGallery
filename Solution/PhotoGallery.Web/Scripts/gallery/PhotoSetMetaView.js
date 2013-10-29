﻿/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSetMetaView(pPhotoSetMeta, pSelector) {
	this.photoSetMeta = pPhotoSetMeta;
	this.selector = pSelector;
	this.photoSetMeta.events.listen('dataLoaded', this, this.buildView);
	this.photoSetMeta.photoSet.events.listen("filterChanged", this, this.buildView);
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMetaView.prototype.buildView = function() {
	this.hold = $(this.selector).html('');
	var cf = this.photoSetMeta.photoSet.getPhotoCount(true);
	var ct = this.photoSetMeta.photoSet.getPhotoCount(false);

	$('<table>')
		.append($('<tbody>')
			.append($('<tr>')
				.append(
					$('<td>').html('Photo Count')
				)
				.append(
					$('<td>').html('<strong>'+cf+(cf != ct ? ' ('+ct+' total)' : '')+'</strong>')
				)
				.append(
					$('<td>')
				)
			)
			.append($('<tr>')
				.append(
					$('<td>').html('Avg. Exposure')
				)
				.append(
					$('<td>').html('<strong>'+this.photoSetMeta.getExposureAvgStr()+'</strong>')
				)
				.append(
					$('<td>').attr('id', 'MetaExpGraph')
				)
			)
			.append($('<tr>')
				.append(
					$('<td>').html('Avg. F-Number')
				)
				.append(
					$('<td>').html('<strong>'+this.photoSetMeta.getFNumberAvgStr()+'</strong>')
				)
				.append(
					$('<td>').attr('id', 'MetaFNumGraph')
				)
			)
			.append($('<tr>')
				.append(
					$('<td>').html('Avg. Focal Length')
				)
				.append(
					$('<td>').html('<strong>'+this.photoSetMeta.getFocalLengthAvgStr()+'</strong>')
				)
				.append(
					$('<td>').attr('id', 'MetaFocalGraph')
				)
			)
			.append($('<tr>')
				.append(
					$('<td>').html('Avg. ISO Speed')
				)
				.append(
					$('<td>').html('<strong>'+this.photoSetMeta.getIsoSpeedAvgStr()+'</strong>')
				)
				.append(
					$('<td>').attr('id', 'MetaIsoGraph')
				)
			)
			.append($('<tr>')
				.append(
					$('<td>').html('Flash Usage')
				)
				.append(
					$('<td>').html('<strong>'+this.photoSetMeta.getFlashUsageAvgStr()+'</strong>')
				)
				.append(
					$('<td>')
				)
			)
		)
		.appendTo(this.hold);

	var expVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.exposure; });
	var fnmVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.fNumber; });
	var focVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.focalLen; });
	var isoVals = this.photoSetMeta.getValues(function(pPhotoData) { return pPhotoData.isoSpeed; });

	this.buildHistogram('#MetaExpGraph', expVals);
	this.buildHistogram('#MetaFNumGraph', fnmVals);
	this.buildHistogram('#MetaFocalGraph', focVals);
	this.buildHistogram('#MetaIsoGraph', isoVals);
};

/*--------------------------------------------------------------------------------------------*/
PhotoSetMetaView.prototype.isVisible = function() {
	return $(this.selector).is(':visible');
};

/*--------------------------------------------------------------------------------------------*/
PhotoSetMetaView.prototype.show = function() {
	$(this.selector).show();
	this.photoSetMeta.loadData();
};

/*--------------------------------------------------------------------------------------------*/
PhotoSetMetaView.prototype.hide = function() {
	$(this.selector).hide();
};


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSetMetaView.prototype.buildHistogram = function(pSelector, pValues) {
	if ( !pValues || !pValues.length ) {
		return;
	}

	var g = {};
	g.selector = pSelector;
	g.width = 60;
	g.height = 15;

	g.xs = d3.scale
		.linear()
		.domain([0, d3.max(pValues)])
		.range([0, g.width]);

	g.data = d3.layout
		.histogram()
		.bins(g.xs.ticks(20))(pValues);

	g.ys = d3.scale
		.linear()
		.domain([0, d3.max(g.data, function (d) { return d.y; })])
		.range([g.height, 0]);

	g.xAxis = d3.svg
		.axis()
		.scale(g.xs)
		.orient('bottom');

	g.svg = d3.select(g.selector)
		.append('svg')
		.attr('width', g.width)
		.attr('height', g.height)
		.append('g');

	////

	g.bar = g.svg.selectAll('.bar')
		.data(g.data)
		.enter()
		.append('g')
			.attr('class', 'miniHistBar')
			.attr('transform', function (d) { return 'translate('+g.xs(d.x)+','+g.ys(d.y)+')'; });

	g.bar.append('rect')
		.attr('width', g.xs(g.data[0].dx))
		.attr('height', function (d) { return g.height-g.ys(d.y); });

	g.svg.append('g')
		.attr('class', 'miniHistXAxis')
		.attr('transform', 'translate(0,'+g.height+')')
		.call(g.xAxis);

	return g;
};
