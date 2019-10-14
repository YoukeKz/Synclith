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
		Wait		, //!< 待機。
		Move		, //!< 移動。
		MoveAfter	, //!< 移動後。
		Clear		, //!< クリアに移行。
	};	

	// 移動方向。
	public enum eMove {
		None	, //!< なし。
		Up		, //!< 上から。
		Down	, //!< 下から。
		Left	, //!< 左から。
		Right	, //!< 右から。
	};

	// チップの種類。
	public enum eChip{
		None	, //!< なし。
		Hole	, //!< 穴。
	};

	// --------------
	// --- struct ---
	struct CHIP_DATA{
		public eChip Chip;
		public Vector2Int Pos;
		public void Init( Vector2Int vec, eChip chip ){
			Chip	= chip;
			Pos		= vec;
		}
	};

	// ----------------
	// --- variable ---
	private StateObj<eState> State;
	private eMove MoveDir = eMove.None;
	private CMap Cmap = new CMap();
	private CMap.MAP_DATA mapdata;
	private List<Object> ObjList = new List<Object>();
	private List<Object> DestroyList = new List<Object>();
	private List<CHIP_DATA> ChipList = new List<CHIP_DATA>();

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
		case eState.Wait		: _StateWait();			break;
		case eState.Move		: _StateMove();			break;
		case eState.MoveAfter	: _StateMoveAfter();	break;
		case eState.Clear		: _StateClear();		break;
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
	// オブジェクトの破棄。
	public void DestroyObj( Object obj ){
		DestroyList.Add( obj );
	}
	// チップの登録。
	public void RegistChip( Vector2Int vec, eChip chip ){
		var data = new CHIP_DATA();
		data.Init( vec, chip );
		ChipList.Add( data );
	}
	// チップを参照。
	public bool IsChip( Vector2Int vec, eChip chip ){
		foreach( CHIP_DATA data in ChipList ){
			if( data.Pos.x != vec.x || 
				data.Pos.y != vec.y ){
				continue;
			}

			if( data.Chip == chip ){
				return true;
			}
		}
		return false;
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

			// ターゲットの座標を取得する。
			var PosList = new List<Vector2Int>();
			foreach( Object obj in ObjList ){
				if( !obj.IsTarget() ){ continue; }
				PosList.Add( Util.GetVec2I( obj.transform.position ) );
			}

			// 移動を開始してもらう。
			foreach( Object obj in ObjList ){
				var vPosI = Util.GetVec2I( obj.transform.position );
				bool fgUpdate = true;
				foreach( var vTgtPos in PosList ){
					switch( MoveDir ){
						case eMove.Up		: // no_break;
						case eMove.Down		:{
							if( vPosI.x != vTgtPos.x ){ fgUpdate = false; }
							break;
						}
						case eMove.Left		: // no_break;
						case eMove.Right	:{
							if( vPosI.y != vTgtPos.y ){ fgUpdate = false; }
							break;
						}
					}
				}
				if( fgUpdate ){
					obj.SetMove( MoveDir );
				}
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
			State.ChangeState( eState.MoveAfter );
		}
	}

	// ステート/移動後。
	private void _StateMoveAfter(){
		foreach( Object obj in DestroyList ){
			ObjList.Remove( obj );
			Destroy( obj );
		}
		DestroyList.Clear();

		bool fgClear = true;
		foreach( Object obj in ObjList ){
			if( obj.IsTarget() ){
				fgClear = false;
				break;
			}
		}

		// クリア条件を満たしていたら、リザルトに移動する。
		if( fgClear ){
			State.ChangeState( eState.Clear );
		// それ以外は、待機に戻る。
		}else{
			State.ChangeState( eState.Wait );
		}
	}

	// ステート/クリアに移行。
	private void _StateClear(){
		if( State.Cnt > 30 ){
			ButtonEvent btn = new ButtonEvent();
			btn.ButtonToResult_Click();
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
