using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static class Scene{
        public static string MainPage = "MainPage";
        public static string CreateTaskPage = "CreateTaskPage";
        public static string CreateAppointmentPage = "CreateAppointmentPage";
    }

    public static void Load(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
