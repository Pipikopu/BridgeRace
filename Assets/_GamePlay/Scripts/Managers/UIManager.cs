using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Numerics;

public enum UIID
{
    UICGamePlay = 0,
    UICBlockRaycast = 1,
    UICMainMenu = 2,
    UICPause = 3,
    UICFail = 4,
    UICVictory = 5,
    UICEndGame = 6,
    UICLoading = 7,
}


public class UIManager : Singleton<UIManager>
{

    private Dictionary<UIID, UICanvas> UICanvas = new Dictionary<UIID, UICanvas>();

    public Transform CanvasParentTF;

    private void Awake()
    {
        //DontDestroyOnLoad(CanvasParentTF.gameObject);
    }

    #region Canvas

    public bool IsOpenedUI(UIID ID)
    {
        return UICanvas.ContainsKey(ID) && UICanvas[ID] != null && UICanvas[ID].gameObject.activeInHierarchy;
    }

    public UICanvas GetUI(UIID ID)
    {
        if (!UICanvas.ContainsKey(ID) || UICanvas[ID] == null)
        {
            UICanvas canvas = Instantiate(Resources.Load<UICanvas>("UI/" + ID.ToString()), CanvasParentTF);
            UICanvas[ID] = canvas;
        }

        return UICanvas[ID];
    }

    public T GetUI<T>(UIID ID) where T : UICanvas
    {
        return GetUI(ID) as T;
    }

    public UICanvas OpenUI(UIID ID)
    {
        UICanvas canvas = GetUI(ID);

        canvas.Setup();
        canvas.Open();

        return canvas;
    }

    public T OpenUI<T>(UIID ID) where T : UICanvas
    {
        return OpenUI(ID) as T;
    }

    public bool IsOpened(UIID ID)
    {
        return UICanvas.ContainsKey(ID) && UICanvas[ID] != null;
    }

    #endregion

}
