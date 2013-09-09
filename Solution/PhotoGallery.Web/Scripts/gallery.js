/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
$(document).ready(function() {
	setupThumbs();
	cropThumbs();
});

/*--------------------------------------------------------------------------------------------* /
$(window).resize(function() {
	cropThumbs();
});


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
//stackoverflow.com/questions/8348017/
//	how-to-tell-when-an-image-is-already-in-browser-cache-in-ie9
//stackoverflow.com/questions/3877027/
//  jquery-callback-on-image-load-even-when-the-image-is-cached
function setupThumbs() {
	$("img.thumb")
		.css("display", "none")
		.one("load", function() {
			onThumbLoad(this);
		})
		.each(function() {
			if ( this.complete ) {
				$(this).load();
			}
		});
}

/*--------------------------------------------------------------------------------------------*/
function onThumbLoad(pThumb) {
	captureImageSize(pThumb);
	resizeThumb(pThumb);
}

/*--------------------------------------------------------------------------------------------*/
function cropThumbs() {
	$("img.thumb")
		.each(function () {
			resizeThumb(this);
		});
}

/*--------------------------------------------------------------------------------------------*/
function captureImageSize(pThumb) {
	$(pThumb).css("display", "block");
	$(pThumb).css("position", "absolute");
	$(pThumb).data("origW", $(pThumb).width());
	$(pThumb).data("origH", $(pThumb).height());
	$(pThumb).css("position", "inherit");
	
	/*var imgW = $(pThumb).data("origW");
	var imgH = $(pThumb).data("origH");
	console.log(imgW+"/"+imgH+" ... "+$(pThumb).width()+"/"+$(pThumb).height());*/
}

/*--------------------------------------------------------------------------------------------*/
function resizeThumb(pThumb) {
	var par = $(pThumb).parent();
	var crop = $(par).parent();

	var cropW = $(crop).width();
	$(crop).css("height", cropW+"px");
	
	var imgW = $(pThumb).data("origW");
	var imgH = $(pThumb).data("origH");
	console.log("resize: "+imgW+" / "+imgH+" / "+par+" / "+crop+" ... "+cropW);
	if ( !imgW ) { return; }

	if ( imgH/imgW < 1.0 ) {
		$(pThumb).height(cropW);

		var mx = -(imgW*(cropW/imgH)-cropW)/2;
		$(par).css("margin-left", mx+"px");
		$(par).css("margin-right", mx+"px");
		$(par).css("margin-top", "0px");
	}
	else {
		$(par).css("margin-top", -(cropW*0.08)+"px");
		$(par).css("margin-left", "0px");
		$(par).css("margin-right", "0px");
	}
}