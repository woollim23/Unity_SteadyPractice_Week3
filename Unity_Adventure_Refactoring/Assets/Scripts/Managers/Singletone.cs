using UnityEngine;
public class Singletone<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (_instance == null)
                {
                    string tName = typeof(T).ToString(); // 오브젝트 이름 정하기
                    var singletoneObj = new GameObject(tName); // 타입 이름대로 지정되어 생성됨
                    _instance = singletoneObj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null)
            DontDestroyOnLoad(_instance);
    }
}
