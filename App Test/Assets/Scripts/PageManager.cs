using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    [SerializeField] private Page activePage;
    [SerializeField] private Page[] registeredPages;

    public void GoToPage(Page page)
    {
        activePage.Deactivate(); //TODO: implement transition animations
        activePage = page;
        activePage.Activate(); //TODO: implement transition animations
    }

    private void Start()
    {
        foreach(Page page in registeredPages)
        {
            page.Deactivate();
        }

        GoToPage(activePage);
    }
}
