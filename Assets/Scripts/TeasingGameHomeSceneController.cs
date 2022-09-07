using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace TeasingGame
{
    public enum TeasingGameScene :int 
    {
        Home,
        Game,
    }
public class TeasingGameHomeSceneController : MonoBehaviour
{
    public TeasingGameScene SceneForButton;

    public void GoToGameScene()
   {
       SceneManager.LoadScene((int)SceneForButton);
   }
}
}