using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public Dictionary<string, List<GameObject>> objectPoolDictionary = new Dictionary<string, List<GameObject>>();
    private static ObjectPool _instance;
    public static ObjectPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ObjectPool>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    _instance = singletonObject.AddComponent<ObjectPool>();
                    singletonObject.name = typeof(ObjectPool).ToString() + " (Singleton)";
                }
            }

            return _instance;
        }
    }
    // 從對象池中獲取對象
    public GameObject Get(string objectType , Vector3 SpawnPlace)
    {
        if (objectPoolDictionary[objectType].Count <=5)
        {
            Preload(objectType, objectPoolDictionary[objectType][0], 10); // 當池中沒有物件時，擴展10個
        }
        if (objectPoolDictionary.ContainsKey(objectType) && objectPoolDictionary[objectType].Count > 0)
        {
            GameObject obj = objectPoolDictionary[objectType][0];
            objectPoolDictionary[objectType].Remove(obj);
            obj.SetActive(true);
            obj.transform.position = SpawnPlace;
            return obj;
        }
        else
        {
            return null;
        }
    }

    // 將對象返回到對象池
    public void Return(string objectType, GameObject obj)
    {
        obj.SetActive(false);
        if (objectPoolDictionary.ContainsKey(objectType))
        {
            objectPoolDictionary[objectType].Add(obj);
        }
        else
        {
            objectPoolDictionary.Add(objectType, new List<GameObject> { obj });
            Debug.LogWarning($"Key '{objectType}' does not exist in the pool.");
        }
    }

    // 預先生成一定數量的對象
    public void Preload(string objectType, GameObject prefab, int count)
    {
        //如果沒有這個池子則創立一個list
        if (!objectPoolDictionary.ContainsKey(objectType))
        {
            objectPoolDictionary.Add(objectType, new List<GameObject>());
        }
        //生成指定數量的單位
        for (int i = 0; i < count; i++)
        {
            GameObject obj = Instantiate(prefab, transform);
            obj.name = GameManager.Instance.unitNumber.ToString();
            GameManager.Instance.unitNumber++;
            obj.SetActive(false);
            objectPoolDictionary[objectType].Add(obj);
        }
    }
}
