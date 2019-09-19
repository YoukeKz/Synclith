using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMgr : MonoBehaviour
{
    void OnGUI()
    {
		Util.SetFontSize( 32 );

        float x = 128;
        float y = 32;
        float h = ( Screen.width - x );
        float w = ( Screen.height - y );
		Util.GUILabel( x, y, w, h, "Title" );
		
        x += 60;
        if( GUI.Button( new Rect( x, y, w, h ), "START" ) ){
			SceneManager.CreateScene( "SceneMain" );
		}
    }
}
