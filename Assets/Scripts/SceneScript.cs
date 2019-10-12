using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScript : MonoBehaviour
{
	enum eType{
		Title	,
		Main	,
		Result	,
	};
	[SerializeField]
	private eType Type = eType.Title;

	public void Update()
	{
		switch( Type ){
		case eType.Title	: UpdateTitle();	break;
		case eType.Main		: UpdateMain();		break;
		case eType.Result	: UpdateResult();	break;
		}
	}

	private void UpdateTitle()
	{
		if( Input.GetKeyDown( KeyCode.Z )			|| 
			Input.GetKeyDown( KeyCode.X )			|| 
			Input.GetKeyDown( KeyCode.Space )		|| 
			Input.GetKeyDown( KeyCode.KeypadEnter )	|| 
			Input.GetKeyDown( KeyCode.Return )		){
			ButtonEvent btn = new ButtonEvent();
			btn.ButtonToMain_Click();
		}
	}
	private void UpdateMain()
	{
		if( Input.GetKeyDown( KeyCode.LeftControl ) && Input.GetKeyDown( KeyCode.Space ) ){
			ButtonEvent btn = new ButtonEvent();
			btn.ButtonToResult_Click();
		}else if(	Input.GetKeyDown( KeyCode.R )		||
					Input.GetKeyDown( KeyCode.Space )	){
			ButtonEvent btn = new ButtonEvent();
			btn.ButtonToMain_Click();
		}else if(	Input.GetKeyDown( KeyCode.Backspace )	||  
					Input.GetKeyDown( KeyCode.Escape )		){
			ButtonEvent btn = new ButtonEvent();
			btn.ButtonToTitle_Click();
		}
			
	}
	private void UpdateResult()
	{
		if( Input.GetKeyDown( KeyCode.Z )			|| 
			Input.GetKeyDown( KeyCode.X )			|| 
			Input.GetKeyDown( KeyCode.Space )		|| 
			Input.GetKeyDown( KeyCode.KeypadEnter )	|| 
			Input.GetKeyDown( KeyCode.Return )		){
			ButtonEvent btn = new ButtonEvent();
			btn.ButtonToTitle_Click();
		}else if( Input.GetKeyDown( KeyCode.R ) ){
			ButtonEvent btn = new ButtonEvent();
			btn.ButtonToMain_Click();
		}
	}
}
