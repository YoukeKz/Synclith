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
		Wait	,
		Move	,
	};

	// -----------------
	// --- variables ---
	private Grid grid = null;
	private Vector2 move = new Vector2( 0, 0 );
	private Vector2 moved = new Vector2( 0, 0 );
	private StateObj<eState> State;
	private float moveSpd = 0.0f;

	[SerializeField]
	public int MoveCell = 5;

	// ----------------------
	// --- public methods ---
    // Start is called before the first frame update
    void Start()
    {
        GameObject parent = transform.root.gameObject;
		if( parent == null ){ Destroy( this ); return; }
		grid = parent.GetComponent<Grid>();
		if( grid == null ){ Destroy( this ); return; }
		State.Init( eState.Wait );
		moveSpd = MoveCell / MoveFrame;
    }

    // Update is called once per frame
    void Update()
    {
		State.Update();
		switch( State.Now() ){
		case eState.Wait: _StateWait(); break;
		case eState.Move: _StateMove(); break;
		}
    }

	private void LateUpdate()
	{
	}

	// -----------------------
	// --- private methods ---
	private void _StateWait(){
		bool fgMove = true;
        if( Input.GetKey( KeyCode.UpArrow ) ){
			move.y = moveSpd;
		}else if( Input.GetKey( KeyCode.DownArrow ) ){
			move.y = -moveSpd;
		}else if( Input.GetKey( KeyCode.RightArrow ) ){
			move.x = moveSpd;
		}else if( Input.GetKey( KeyCode.LeftArrow ) ){
			move.x = -moveSpd;
		}else{
			fgMove = false;
		}

		if( fgMove ){
			State.ChangeState( eState.Move );
			_StateMove();
		}
	}
	private void _StateMove(){
		Vector3 vec = this.transform.localPosition;
		Vector2 old = new Vector2( vec.x, vec.y );

		vec.x += move.x;
		vec.y += move.y;

		moved += move;

		if( Mathf.Abs( moved.x ) >= MoveCell ||
			Mathf.Abs( moved.y ) >= MoveCell )
		{
			vec.Set( Mathf.Round( vec.x ), Mathf.Round( vec.y ), vec.z );
			move.Set( 0.0f, 0.0f );
			moved.Set( 0.0f, 0.0f );
			State.ChangeState( eState.Wait );
		}

		this.transform.position = vec;
	}
}
