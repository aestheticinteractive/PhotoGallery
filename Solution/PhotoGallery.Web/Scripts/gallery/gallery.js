/// <reference path='~/Scripts/jquery-2.0.3-vsdoc.js' />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function isTouch() {
	return !!('ontouchstart' in window);
}

/*--------------------------------------------------------------------------------------------*/
function pagingLoad(pUrl) {
	var onSuccess = function(pData) {
		$('#DataPage .dataHold').stop().fadeIn(200);
		$('#DataPage').html(pData);
	};

	jQuery.get(pUrl, onSuccess);
	$('#DataPage .dataHold').stop().fadeTo(200, 0.333);
	$('#DataPage .wait').fadeIn(200);
}


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
function drawMiniHistogram(pSelector, pValues) {
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
}