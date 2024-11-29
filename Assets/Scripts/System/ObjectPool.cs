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
    // �q��H���������H
    public GameObject Get(string objectType , Vector3 SpawnPlace)
    {
        if (objectPoolDictionary[objectType].Count <=5)
        {
            Preload(objectType, objectPoolDictionary[objectType][0], 10); // ������S������ɡA�X�i10��
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

    // �N��H��^���H��
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

    // �w���ͦ��@�w�ƶq����H
    public void Preload(string objectType, GameObject prefab, int count)
    {
        //�p�G�S���o�Ӧ��l�h�Хߤ@��list
        if (!objectPoolDictionary.ContainsKey(objectType))
        {
            objectPoolDictionary.Add(objectType, new List<GameObject>());
        }
        //�ͦ����w�ƶq�����
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
