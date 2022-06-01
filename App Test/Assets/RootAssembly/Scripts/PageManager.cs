using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManager : MonoBehaviour
{
    [SerializeField] private Page activePage;
    [SerializeField] private Page[] registeredPages;

    [SerializeField] GameObject[] PageObject;

    public void GoToPage(Page page)
    {
        activePage.Deactivate();
        activePage = page;
        activePage.Activate();
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
