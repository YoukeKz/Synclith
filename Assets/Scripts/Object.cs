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
	enum eState{
		Wait	, //!< 待機。
		Move	, //!< 移動。
	};

	// -----------------
	// --- variables ---
	private Vector2 move = new Vector2( 0, 0 );
	private Vector2 moved = new Vector2( 0, 0 );
	private int moveRange = 0;
	private StateObj<eState> State;
	private float MoveSpd = 0.1f;

	[SerializeField]
	public int MoveCell = 5;

	// ----------------------
	// --- public methods ---
    // Start is called before the first frame update
    void Start()
    {
		State.Init( eState.Wait );
		MoveSpd = MoveCell / 10.0f;
		_GameMgr.MapColl_On( Util.GetVec3I( transform.position ) );

		// ゲームマネージャに登録。
		_GameMgr.Regist( this );
    }

    // 手動更新用関数。
    public void UpdateManual()
    {
		State.Update();
		switch( State.Now ){
		case eState.Wait: _StateWait(); break;
		case eState.Move: _StateMove(); break;
		}
    }

	// 移動が終了したか。
	public bool IsMoveEnd()
	{
		return ( State.Now == eState.Wait );
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
			_GameMgr.MapColl_Off( vecI );

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
			_GameMgr.MapColl_On( vNext );

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

		// ステートを変更。
		State.ChangeState( eState.Wait );
	}
}
