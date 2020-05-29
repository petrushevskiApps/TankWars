using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private List<UIScreen> screens = new List<UIScreen>();

    private Stack<UIScreen> backStack = new Stack<UIScreen>();

    public static UIController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        //GameManager.Instance.InputController.OnBackPressed.AddListener(OnBack);
    }
    private void OnDestroy()
    {
        //GameManager.Instance.InputController.OnBackPressed.RemoveListener(OnBack);
    }
    public void ShowScreen<T>() where T : UIScreen
    {
        CloseOpenedScreen();

        foreach (UIScreen screen in screens)
        {
            if(screen.GetType() == typeof(T))
            {
                if(backStack.Contains(screen))
                {
                    ClearStackAndOpen(screen);
                }
                else
                {
                    ShowScreen(screen);
                }
            }
        }
    }

    public void OnBack()
    {
        if (backStack.Count > 1)
        {
            backStack.Pop().Close();
            backStack.Peek().Open();
        }
        else
        {
            CloseApplication();
        }
    }

    private void ClearStackAndOpen(UIScreen screen)
    {
        UIScreen stackScreen = backStack.Pop();

        while(stackScreen != null && !stackScreen.Equals(screen))
        {
            if(backStack.Count > 0)
            {
                stackScreen = backStack.Pop();
            }
            else
            {
                CloseApplication();
            }
        }

        if(stackScreen != null)
        {
            ShowScreen(stackScreen);
        }
        else
        {
            CloseApplication();
        }
    }
    
    

    private void ShowScreen(UIScreen screen)
    {
        screen.Open();
        backStack.Push(screen);
    }

    private void CloseOpenedScreen()
    {
        if(backStack.Count > 0)
        {
            UIScreen screen = backStack.Peek();

            if (screen.gameObject.activeSelf)
            {
                screen.Close();
            }
        }
    }

    private void CloseApplication()
    {
        Application.Quit();
    }

}
