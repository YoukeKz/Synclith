using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
	// --------------
	// --- consts ---
	private const float MoveFrame = 10.0f;

	// ------------
	// --- enum ---
	// ステート。
	enum eState{
		Wait	, //!< 待機。
		Move	, //!< 移動。
		Fall	, //!< 落下。
		End		, //!< 終了待ち。
	};

	// オブジェクトの種類。
	enum eObjType{
		Normal	, //!< 通常。
		Hole	, //!< 穴。
		Target	, //!< 穴に落とす対象。
	};

	// -----------------
	// --- variables ---
	private Vector2 move = new Vector2( 0, 0 );
	private Vector2 moved = new Vector2( 0, 0 );
	private int moveRange = 0;
	private StateObj<eState> State;
	private float MoveSpd = 0.1f;

	[SerializeField] private int MoveCell		= 5;
	[SerializeField] private eObjType ObjType	= eObjType.Normal;

	// ----------------------
	// --- public methods ---
    // Start is called before the first frame update
    void Start()
    {
		State.Init( eState.Wait );
		MoveSpd = MoveCell / 10.0f;

		// ゲームマネージャに登録。
		_GameMgr.Regist( this );

		// 穴オブジェクトの場合は、穴を登録させる。
		if( ObjType == eObjType.Hole ){
			_GameMgr.RegistChip( Util.GetVec2I( transform.position ), GameMgr.eChip.Hole );
		}else{
			_GameMgr.MapColl_On( Util.GetVec3I( transform.position ) );
		}
    }

    // 手動更新用関数。
    public void UpdateManual()
    {
		State.Update();
		switch( State.Now ){
		case eState.Wait: _StateWait(); break;
		case eState.Move: _StateMove(); break;
		case eState.Fall: _StateFall(); break;
		case eState.End	: _StateEnd();	break;
		}

    }

	// 移動が終了したか。
	public bool IsMoveEnd(){
		return ( State.Now == eState.Wait || State.Now == eState.End );
	}

	// 穴に落とす対象か。
	public bool IsTarget(){
		return ObjType == eObjType.Target;
	}

	// 移動先を決める。
	public bool SetMove( GameMgr.eMove kind ){
		if( State.Now != eState.Wait ){ return false; }

		switch( kind ){
		case GameMgr.eMove.Up		: move.y = +MoveSpd; break;
		case GameMgr.eMove.Down		: move.y = -MoveSpd; break;
		case GameMgr.eMove.Left		: move.x = -MoveSpd; break;
		case GameMgr.eMove.Right	: move.x = +MoveSpd; break;
		}

		State.ChangeState( eState.Move );
		return true;
	}

	// -----------------------
	// --- private methods ---

	// ステート/待機。
	private void _StateWait(){}

	// ステート/移動。
	private void _StateMove(){
		if( State.IsFirst() ){
			Vector3Int vecI = Util.GetVec3I( transform.position );
			if( ObjType != eObjType.Hole ){
				_GameMgr.MapColl_Off( vecI );
			}

			// 移動距離を求める。0
			Vector3Int vNext	= vecI;
			Vector3Int vMove	= Util.GetVec3I( move.normalized );
			for( int i = 0; i < MoveCell; ++i ){
				Vector3Int vTmp = vNext + vMove;
				if( _GameMgr.MapColl_Is( vTmp ) ){
					break;
				}
				moveRange = i+1;
				vNext = vTmp;
			}

			// 移動先にコリジョンを設定。
			if( ObjType != eObjType.Hole && !_GameMgr.IsChip( Util.GetVec2I( vNext ), GameMgr.eChip.Hole ) ){
				_GameMgr.MapColl_On( vNext );
			}

			// 移動距離が0以下ならば、移動せずに終わる。
			if( moveRange <= 0 ){
				_StateMove_Term();
				State.ChangeState( eState.Wait );
			}
		}

		Vector3 vec = transform.position;
		Vector2 old = new Vector2( vec.x, vec.y );

		vec.x += move.x;
		vec.y += move.y;

		moved += move;

		this.transform.position = vec;

		// 目的地まで移動していた。
		if( Mathf.Abs( moved.x ) >= moveRange ||
			Mathf.Abs( moved.y ) >= moveRange )
		{
			_StateMove_Term();
			return;
		}
	}

	// ステート/落下。
	private void _StateFall(){
#if false
		var vRot	= transform.rotation;
		
		vRot.z += 3.0f;

		transform.rotation	 = vRot;
#endif
		const float frameMax = 25.0f;
		var vScale	= transform.localScale;
		float newScale = Mathf.Cos( Mathf.Deg2Rad * 90.0f * State.Cnt / frameMax );
		vScale.Set( newScale, newScale, 1.0f );
		transform.localScale = vScale;

		if( State.Cnt >= frameMax ){
			_GameMgr.DestroyObj( this );
			State.ChangeState( eState.End );
		}
	}

	// ステート/終了。
	private void _StateEnd(){}

	// GameMgrを取得する。
	private GameMgr _GameMgr{
		get{ return GameMgr.Instance; }
	}

	// 移動に関するパラメータの初期化。
	private void _StateMove_Term(){
		move.Set( 0.0f, 0.0f );
		moved.Set( 0.0f, 0.0f );
		moveRange = 0;

		// 位置をキリの良い値に書き換える。
		Vector3Int vec = Util.GetVec3I( transform.position );
		transform.position = Util.GetVec3( vec );

		// 自分の位置に穴があったら、落下に移行。
		if( ObjType != eObjType.Hole && _GameMgr.IsChip( Util.GetVec2I( vec ), GameMgr.eChip.Hole ) ){
			State.ChangeState( eState.Fall );
		// 通常は待機に移行。
		}else{
			State.ChangeState( eState.Wait );
		}
	}
}
