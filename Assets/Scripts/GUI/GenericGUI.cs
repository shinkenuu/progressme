using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Assets.Scripts.Model;
using Assets.Scripts;

public abstract class GenericGUI : MonoBehaviour {

    #region References

    protected NetManagerScript NetMng;
    protected GameManagerScript GameMng;
    protected RepositoryReader RepReader;

    #endregion



    protected virtual void OnEnable()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Offline":
                StartCoroutine(LookForGameManager());
                break;
            case "Panel":
                StartCoroutine(LookForGameManager());
                StartCoroutine(LookForStorage());
                break;
            case "User":
                StartCoroutine(LookForGameManager());
                StartCoroutine(LookForStorage());
                break;
        }

        gameObject.GetComponent<Canvas>().worldCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
    }


    #region Look For

    protected IEnumerator LookForGameManager()
    {
        GameObject mngGo;

        //While the GameObject has not been Instantiated yet by the Scene
        while (true)
        {
            mngGo = GameObject.Find("MngGO");

            if (mngGo != null)
            {
                GameMng = mngGo.GetComponent<GameManagerScript>();
                NetMng = mngGo.GetComponent<NetManagerScript>();
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    protected IEnumerator LookForStorage()
    {
        GameObject dataGo;

        //While the GameObject has not been Instantiated yet by the Scene
        while (true)
        {
            dataGo = GameObject.FindWithTag("Storage");

            if (dataGo != null)
            {
                RepReader = dataGo.GetComponent<RepositoryReader>();
                yield break;
            }

            yield return new WaitForEndOfFrame();
        }

    }

    #endregion
    
}
