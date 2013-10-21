/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
function PhotoSetMeta(pPhotoSet, pMetas) {
	this.photoSet = pPhotoSet;
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.setMetas = function(pMetas) {
	for ( var i = 0 ; i < pMetas.length ; ++i ) {
		var pm = pMetas[i];
		var d = this.photoSet.getData(pm.PhotoId);

		if ( d ) {
			d.setMeta(pm.Exposure, pm.FNumber, pm.FocalLen, pm.IsoSpeed, pm.UsesFlash);
		}
	}
};


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getValues = function(pProp) {
	var cl = this.photoSet.currentList();
	var vals = [];

	for ( var i = 0 ; i < cl.length ; ++i ) {
		var d = cl[i];
		var val = pProp(d);

		if ( val != null ) {
			vals.push(val);
		}
	}

	return vals;
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getAvgValue = function(pProp) {
	var vals = this.getValues(pProp);
	var sum = 0.0;
	var count = 0;

	for ( var i = 0 ; i < vals.length ; ++i ) {
		sum += vals[i];
		count++;
	}

	return (count == 0 ? null : sum/count);
};


////////////////////////////////////////////////////////////////////////////////////////////////////////
/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getFNumberAvgStr = function() {
	var val = this.getAvgValue(function(pPhotoData) { return pPhotoData.fNumber; });
	return (val == null ? '' : 'f/'+val.toFixed(2));
};
		
/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getIsoSpeedAvgStr = function() {
	var val = this.getAvgValue(function(pPhotoData) { return pPhotoData.isoSpeed; });
	return (val == null ? '' : val.toFixed(1));
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getExposureAvgStr = function() {
	var val = this.getAvgValue(function(pPhotoData) { return pPhotoData.exposure; });

	if ( val == null ) {
		return '';
	}

	if ( val > 0.25 ) {
		return val.toFixed(2)+' sec';
	}

	return '1/'+(1/val).toFixed(2)+' sec';
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getFocalLengthAvgStr = function() {
	var val = this.getAvgValue(function(pPhotoData) { return pPhotoData.focalLen; });
	return (val == null ? '' : val.toFixed(1)+' mm');
};

/*----------------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.getFlashUsageAvgStr = function() {
	var val = this.getAvgValue(function(pPhotoData) { return (pPhotoData.flash == true ? 1 : 0); });
	return (val == null ? '' : (val*100).toFixed(1)+'%');
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
PhotoSetMeta.prototype.buildHistogram = function(pSelector, pValues) {
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
