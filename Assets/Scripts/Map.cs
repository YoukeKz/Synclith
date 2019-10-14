using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CMap
{
	// マップデータ型。
	public struct MAP_DATA{
		public int w, h;
		public int[] Cells;

		// データが正常か。
		public bool IsCorrect(){
			if( Util.Assert( Cells.Length != w*h, string.Format("Cmap: cells length is wrong size. / [w,h]:(%d,%d), [Cells.Length]:(%d)", w, h, Cells.Length ) ) ){
				return false;
			}
			return true;
		}

		// マップのサイズを取得。
		public int Length{
			get{ return Cells.Length; }
		}

		// 要素を取得。
		public int Cell( int x, int y ){
			return Cells[ x*h + y ];
		}
	};

	// データが正常かチェック。
	public bool IsCorrect( int mapIdx = -1 ){
		// 引数なしなら全チェック。
		if( mapIdx == -1 ){
			foreach( MAP_DATA data in Data ){
				if( !data.IsCorrect() ){
					return false;
				}
			}
		// 引数がある場合は、特定のマップだけチェック。
		}else{
			if( Util.Assert( mapIdx >= Data.Length, string.Format( "CMap: mapIdx is out of range. [mapIdx]:(%d), [Data.Length]:(%d)", mapIdx, Data.Length ) ) ){
				return false;
			}else if( !Data[mapIdx].IsCorrect() ){
				return false;
			}
		}
		return true;
	}

	// マップデータの定義。
	public MAP_DATA[] Data = new MAP_DATA[]{ 
		new MAP_DATA{
			w = 9,
			h = 9,
			Cells = new int[]{
				1, 1, 1, 1, 1, 1, 1, 1, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 0, 1, 1, 1, 1, 1, 0, 1,
				1, 0, 0, 1, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 1, 0, 0, 0, 1,

				1, 0, 0, 0, 0, 1, 0, 0, 1,
				1, 0, 0, 0, 0, 0, 1, 0, 1,
				1, 0, 0, 0, 0, 0, 0, 0, 1,
				1, 1, 1, 1, 1, 1, 1, 1, 1,
			}
		},
		new MAP_DATA{
			w = 8,
			h = 8,
			Cells = new int[]{
				1, 1, 1, 1, 1, 1, 1, 1,
				1, 0, 0, 0, 0, 0, 1, 1,
				1, 0, 1, 0, 0, 0, 0, 1,
				1, 0, 1, 0, 1, 0, 0, 1,
				1, 0, 1, 0, 1, 0, 0, 1,

				1, 0, 1, 0, 0, 0, 0, 1,
				1, 0, 0, 0, 0, 0, 1, 1,
				1, 1, 1, 1, 1, 1, 1, 1,
			}
		},
	};
}
