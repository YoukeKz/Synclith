using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonEvent : MonoBehaviour
{
	public void TitleButton_Click()
	{
		Debug.Log( "メインへ移行" );

		SceneManager.LoadScene( "SceneMain" );
	}	

	public void MainButton_Click()
	{
		Debug.Log( "タイトルへ移行" );

		SceneManager.LoadScene( "SceneTitle" );
	}

}