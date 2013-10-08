using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PhotoGallery.Web.Models {

	/*================================================================================================*/
	public class DataPage {

		public int ItemIndex { get; private set; }
		public int Size { get; private set; }
		public int TotalItems { get; private set; }
		public Func<int, ActionResult> GetActionForPageIndex { get; private set; }
		public string ContentView { get; set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public DataPage(int pItemIndex, int pSize, int pTotalItems, 
								Func<int, ActionResult> pGetActionForPageIndex, string pContentView) {
			ItemIndex = pItemIndex;
			Size = pSize;
			TotalItems = pTotalItems;
			GetActionForPageIndex = pGetActionForPageIndex;
			ContentView = pContentView;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public bool HasPrevPage() {
			return (ItemIndex > 0);
		}

		/*--------------------------------------------------------------------------------------------*/
		public int GetIndexForPrevPage() {
			return Math.Max(0, ItemIndex-Size);
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public bool HasNextPage() {
			return (ItemIndex+Size <= TotalItems);
		}

		/*--------------------------------------------------------------------------------------------*/
		public int GetIndexForNextPage() {
			return ItemIndex+Size;
		}


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public int GetCurrentPageNumber() {
			return ItemIndex/Size;
		}

		/*--------------------------------------------------------------------------------------------*/
		public int GetLastPageNumber() {
			return (int)Math.Ceiling(TotalItems/(double)Size)-1;
		}

		/*--------------------------------------------------------------------------------------------*/
		public int GetIndexForPageNumber(int pPageNum) {
			return pPageNum*Size;
		}

		/*--------------------------------------------------------------------------------------------*/
		public IList<int> GetNearestPageNumbers(int pCount, bool pIncludeLast, out int pSelected) {
			var list = new List<int>();
			int size = Size;
			int up = GetCurrentPageNumber();
			int dn = GetCurrentPageNumber();

			list.Add(up);
			pSelected = 0;

			while ( true ) {
				bool added = false;

				if ( list.Count >= pCount ) {
					break;
				}

				if ( (up+1)*size < TotalItems ) {
					list.Add(++up);
					added = true;
				}

				if ( list.Count >= pCount ) {
					break;
				}

				if ( dn > 0 ) {
					list.Insert(0, --dn);
					added = true;
					++pSelected;
				}

				if ( !added ) {
					break;
				}
			}

			if ( pIncludeLast ) {
				int last = GetLastPageNumber();

				if ( up == last-1 ) {
					list.RemoveAt(0);
					list.Add(last);
					--pSelected;
				}
				else if ( up != last ) {
					list.RemoveRange(list.Count-2, 2);
					list.Add(-1);
					list.Add(last);
				}
			}

			return list;
		}

	}
	
	/*================================================================================================*/
	public class DataPage<T> : DataPage {

		public IList<T> Items { get; private set; }


		////////////////////////////////////////////////////////////////////////////////////////////////
		/*--------------------------------------------------------------------------------------------*/
		public DataPage(int pItemIndex, int pSize, IList<T> pItems, int pTotalItems,
							Func<int, ActionResult> pGetActionForPageIndex, string pContentView) :
							base(pItemIndex, pSize, pTotalItems, pGetActionForPageIndex, pContentView) {
			Items = pItems;
		}

	}
}