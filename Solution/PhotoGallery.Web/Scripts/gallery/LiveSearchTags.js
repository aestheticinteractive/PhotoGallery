/// <reference path="~/Scripts/jquery-2.0.3-vsdoc.js" />


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
LiveSearchTags.prototype = new LiveSearch();

/*--------------------------------------------------------------------------------------------*/
function LiveSearchTags(pSearchUrl) {
	var urlFunc = function(pText, pFirst) {
		return pSearchUrl+pText+(pFirst ? '?first=true' : '');
	};

	var idFunc = function(pItem) {
		return pItem.ArtifactId;
	};

	LiveSearch.prototype.init.call(this, urlFunc, idFunc);
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
LiveSearchTags.prototype.appendResults = function(pResults) {
	LiveSearch.prototype.appendResults.call(this, pResults);
	var text = this.searchText.toLowerCase();

	this.results.sort(function(a, b) {
		var an = a.Name.toLowerCase();

		if ( an == text ) {
			return -1;
		}
		
		var bn = b.Name.toLowerCase();

		if ( bn == text ) {
			return 1;
		}

		if ( an == bn ) {
			return (a.Disamb.toLowerCase() > b.Disamb.toLowerCase());
		}

		return (an > bn);
	});

	/*n = this.results.length;
	console.log('---- '+n+' Result(s) ----');

	for ( i = 0 ; i < n ; ++i ) {
		t = this.results[i];
		console.log(' * '+i+': '+t.Name+' ('+t.Disamb+')');
	}*/
};


////////////////////////////////////////////////////////////////////////////////////////////////
/*--------------------------------------------------------------------------------------------*/
LiveSearchTags.prototype.onTagSelect = function(pArtifactId) {
	this.selectedTag = this.tagMap[pArtifactId];

	alert('Selected! '+this.selectedTag.ArtifactId+"\n\n"+
		this.selectedTag.Name+' ('+this.selectedTag.Disamb+')');

	this.abortSearch();
	this.events.send('tagSelected');
};

/*--------------------------------------------------------------------------------------------*/
LiveSearchTags.prototype.getSelectedTagArtifactId = function() {
	return this.selectedTag.ArtifactId;
};
