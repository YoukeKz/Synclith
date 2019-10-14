using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;

public struct StateObj< T >{
	// -----------------
	// --- variables ---
	int cnt;
	T now;
	T prev;
	T next;
	bool fgInit;
	bool fgFirst;
		
	// ---------------
	// --- methods ---
	// 初期化。
	public void Init( T state ){
		cnt = 0;
		now	 = state;
		prev = state;
		next = state;
		fgInit = true;
		fgFirst = false;
	}
	// 更新。
	public void Update(){
		cnt++;
		fgFirst = false;
		if( fgInit ){
			cnt = 0;
			fgInit = false;
			prev = now;
			now  = next;
			next = default(T);
			fgFirst = true;
		}
	}
	// ステート変更。
	public void ChangeState( T state ){
		fgInit	= true;
		next	= state;
	}

	// ----------------
	// --- accessor ---
	public T Now{ get{ return now; } }
	public T Prev{ get{ return prev; } }
	public T Next{ get{ return next; } }
	public int Cnt{ get{ return cnt; } }
	public bool IsFirst(){ return fgFirst; }
};



/// 様々なユーティリティ.
public class Util {
	public static bool Assert( bool fg, string str = "" ){
		if( fg ){
			UnityEngine.Assertions.Assert.IsTrue( !fg, str );
			return true;
		}
		return false;
	}

	// Vectorの変換。
	public static Vector3Int GetVec3I( Vector3 vec ){ return new Vector3Int( Mathf.RoundToInt( vec.x ), Mathf.RoundToInt( vec.y ), Mathf.RoundToInt( vec.z ) ); }
	public static Vector3Int GetVec3I( Vector2 vec ){ return new Vector3Int( Mathf.RoundToInt( vec.x ), Mathf.RoundToInt( vec.y ), 0 ); }
	public static Vector2Int GetVec2I( Vector3 vec ){ return new Vector2Int( Mathf.RoundToInt( vec.x ), Mathf.RoundToInt( vec.y ) ); }
	public static Vector2Int GetVec2I( Vector2 vec ){ return new Vector2Int( Mathf.RoundToInt( vec.x ), Mathf.RoundToInt( vec.y ) ); }
	public static Vector3 GetVec3( Vector3Int vec ){ return new Vector3( (float)vec.x, (float)vec.y, (float)vec.z ); }
	public static Vector3 GetVec3( Vector2Int vec ){ return new Vector3( (float)vec.x, (float)vec.y, 0.0f ); }
	public static Vector2 GetVec2( Vector3Int vec ){ return new Vector2( (float)vec.x, (float)vec.y ); }
	public static Vector2 GetVec2( Vector2Int vec ){ return new Vector2( (float)vec.x, (float)vec.y ); }

	/// Mathf.Cosの角度指定版.
	public static float CosEx(float Deg) {
		return Mathf.Cos(Mathf.Deg2Rad * Deg);
	}
	/// Mathf.Sinの角度指定版.
	public static float SinEx(float Deg) {
		return Mathf.Sin(Mathf.Deg2Rad * Deg);
	}

	/// 入力方向を取得する.
	public static Vector2 GetInputVector() {
		float x = Input.GetAxisRaw("Horizontal");
		float y = Input.GetAxisRaw("Vertical");
		return new Vector2(x, y).normalized;
	}

	/// トークンを動的生成する.
	public static Token CreateToken(float x, float y, string SpriteFile, string SpriteName, string objName="Token") {
		GameObject obj = new GameObject(objName);
		obj.AddComponent<SpriteRenderer>();
		obj.AddComponent<Rigidbody2D>();
		obj.AddComponent<Token>();

		Token t = obj.GetComponent<Token>();
		// スプライト設定.
		t.SetSprite(GetSprite(SpriteFile, SpriteName));
		// 座標を設定.
		t.X = x;
		t.Y = y;
		// 重力を無効にする.
		t.GravityScale = 0.0f;

		return t;
	}

	/// スプライトをリソースから取得する.
	/// ※スプライトは「Resources/Sprites」以下に配置していなければなりません.
	/// ※fileNameに空文字（""）を指定するとシングルスプライトから取得します.
	public static Sprite GetSprite(string fileName, string spriteName) {
		if(spriteName == "") {
			// シングルスプライト
			return Resources.Load<Sprite>(fileName);
		}
		else {
			// マルチスプライト
			Sprite[] sprites = Resources.LoadAll<Sprite>(fileName);
			return System.Array.Find<Sprite>(sprites, (sprite) =>  sprite.name.Equals(spriteName));
		}
	}

	private static Rect _guiRect = new Rect();
	static Rect GetGUIRect() {
		return _guiRect;
	} 
	private static GUIStyle _guiStyle = null;
	static GUIStyle GetGUIStyle() {
		return _guiStyle ?? (_guiStyle = new GUIStyle());
	}
	/// フォントサイズを設定.
	public static void SetFontSize(int size) {
		GetGUIStyle().fontSize = size;
	}
	/// フォントカラーを設定.
	public static void SetFontColor(Color color) {
		GetGUIStyle().normal.textColor = color;
	}
	/// フォント位置設定
	public static void SetFontAlignment(TextAnchor align)
	{
		GetGUIStyle().alignment = align;
	}
	/// ラベルの描画.
	public static void GUILabel(float x, float y, float w, float h, string text) {
		Rect rect = GetGUIRect();
		rect.x = x;
		rect.y = y;
		rect.width = w;
		rect.height = h;

		GUI.Label(rect, text, GetGUIStyle());
	}
	/// ボタンの配置.
	public static bool GUIButton(float x, float y, float w, float h, string text) {
		Rect rect = GetGUIRect();
		rect.x = x;
		rect.y = y;
		rect.width = w;
		rect.height = h;

		return GUI.Button(rect, text, GetGUIStyle());
	}
}
