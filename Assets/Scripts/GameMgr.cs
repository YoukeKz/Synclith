using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameMgr : MonoBehaviour
{
	// ---------------
	// --- statics ---
	// 適当にシングルトンを実装する。
	private static GameMgr Inst = null;
	public static GameMgr Instance{ get{ return Inst; } }
	public static int sUniqId = 0;
	public int uniqId = sUniqId++;

	// ------------
	// --- enum ---
	// ステート。
	enum eState{
		Wait	, //!< 待機。
		Move	, //!< 移動。
	};	

	// 移動方向。
	public enum eMove {
		None	, //!< なし。
		Up		, //!< 上から。
		Down	, //!< 下から。
		Left	, //!< 左から。
		Right	, //!< 右から。
	};

	// ----------------
	// --- variable ---
	private StateObj<eState> State;
	private eMove MoveDir = eMove.None;
	private CMap Cmap = new CMap();
	private CMap.MAP_DATA mapdata;
	private List<Object> ObjList = new List<Object>();

	[SerializeField] private Tilemap Tmap = new Tilemap();

#if false	// 未使用変数。
	private int mapIdx = 0;
	[SerializeField]private Grid grid = null;
	[SerializeField]private TileBase[] Tbase = null;
#endif

	// -----------------------
	// --- override method ---
	private void Awake()
	{
		if( Inst == null ){
			Inst = this;
		}

		State.Init( eState.Wait );
#if false
		if( !Cmap.IsCorrect( mapIdx ) ){ return; }
		mapdata = Cmap.Data[mapIdx];

		// マップの生成。
		for( int w = 0; w < mapdata.w; w++ ){
			for( int h = 0; h < mapdata.h; h++ ){
				int chip = mapdata.Cell( w, h );
				if( chip < Tbase.Length ){
					Tmap.SetTile( new Vector3Int( w, h, 0 ), Tbase[chip].() );
				}
			}
		}

		Instantiate( Tmap, grid.transform );
#endif
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		State.Update();
		switch(State.Now){
		case eState.Wait	: _StateWait();	break;
		case eState.Move	: _StateMove();	break;
		}

		if( Input.GetKeyDown( KeyCode.D ) ){
			var bound = Tmap.cellBounds;
			string str = "";
			for (int y = bound.max.y - 1; y >= bound.min.y; --y){
				for (int x = bound.min.x; x < bound.max.x; ++x){
					var tile = Tmap.GetColliderType( new Vector3Int( x, y, 0 ) );
					str += tile != Tile.ColliderType.None ? "■" : "□";
				}
				str += "\n";
			}
			Debug.Log( str );
		}
    }

	// 破棄されたとき。
	private void OnDestroy()
	{
		if( Inst == this ){
			Inst = null;
		}
	}


	// --------------------
	// --- loacl method ---

	// オブジェクトの登録。
	public void Regist( Object obj ){
		ObjList.Add( obj );
	}

	// 動いても良いか。
	public bool IsMoveEnable(){
		return State.Now == eState.Wait;
	}

	// ステート/待機。
	private void _StateWait(){
		// オブジェクトを更新。
		foreach( Object obj in ObjList ){
	        obj.UpdateManual();
		}

		// キーが押されたら、移動ステートに移行。
		if(		 Input.GetKeyDown( KeyCode.UpArrow )	){ MoveDir = eMove.Up;		}
		else if( Input.GetKeyDown( KeyCode.DownArrow )	){ MoveDir = eMove.Down;	}
		else if( Input.GetKeyDown( KeyCode.LeftArrow )	){ MoveDir = eMove.Left;	}
		else if( Input.GetKeyDown( KeyCode.RightArrow )	){ MoveDir = eMove.Right;	}
		if( MoveDir != eMove.None ){
			State.ChangeState( eState.Move );
		}
	}

	// ステート/移動。
	private void _StateMove(){
		// 初回のみ、ソートをする。
		if( State.IsFirst() ){
			switch( MoveDir ){
			case eMove.Up		: ObjList.Sort( (a, b) => (int)( b.transform.position.y - a.transform.position.y ) );	break;
			case eMove.Down		: ObjList.Sort( (a, b) => (int)( a.transform.position.y - b.transform.position.y ) );	break;
			case eMove.Left		: ObjList.Sort( (a, b) => (int)( a.transform.position.x - b.transform.position.x ) );	break;
			case eMove.Right	: ObjList.Sort( (a, b) => (int)( b.transform.position.x - a.transform.position.x ) );	break;
			default:																									break;
			}

			// 移動を開始してもらう。
			foreach( Object obj in ObjList ){
				obj.SetMove( MoveDir );
			}
			MoveDir = eMove.None;
		}
		
		// オブジェクトを更新。
		bool fgUpdateEnd = true;
		foreach( Object obj in ObjList ){
	        obj.UpdateManual();
			if( !obj.IsMoveEnd() ){
				fgUpdateEnd = false;
			}
		}

		// すべての移動が終了したら待機に戻る。
		if( fgUpdateEnd ){
			State.ChangeState( eState.Wait );
		}
	}

	// マップにコリジョンを設置する。
	public void MapColl_On( Vector3Int vec ){
		_MapColl_Set( vec, Tile.ColliderType.Grid );
	}
	public void MapColl_Off( Vector3Int vec ){
		_MapColl_Set( vec, Tile.ColliderType.None );
	}
	private void _MapColl_Set( Vector3Int vec, Tile.ColliderType coll ){
		if( !Tmap.HasTile( vec ) ){ return; }
		Tmap.SetColliderType( vec, coll );
	}
	// マップのコリジョンを取得する。
	public bool MapColl_Is( Vector3Int vec ){
		if( !Tmap.HasTile( vec ) ){ return true; }
		Tile.ColliderType tile = Tmap.GetColliderType( vec );
		Tile.ColliderType tile2 = Tmap.GetColliderType( new Vector3Int( -5, -5, 0 ) );
		return tile != Tile.ColliderType.None;
	}
}
