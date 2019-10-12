using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{
	public void ButtonToTitle_Click()
	{
		Debug.Log( "タイトルへ移行" );

		SceneManager.LoadScene( "SceneTitle" );
	}	

	public void ButtonToMain_Click()
	{
		Debug.Log( "メインへ移行" );

		SceneManager.LoadScene( "SceneMain" );
	}
	public void ButtonToResult_Click()
	{
		Debug.Log( "リザルトへ移行" );

		SceneManager.LoadScene( "SceneResult" );
	}
}