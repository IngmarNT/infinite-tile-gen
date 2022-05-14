using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool<T> : IPool<T> where T : MonoBehaviour, IPoolable<T> {
    private Action<T> pullObject;
    private Action<T> pushObject;
    private Stack<T> pooledObjects = new Stack<T>();
    private GameObject prefab;
    public int pooledCount {
        get {
            return pooledObjects.Count;
        }
    }

    public ObjectPool(GameObject pooledObject, int numToSpawn = 0) {
        this.prefab = pooledObject;
        Spawn(numToSpawn);
    }

    public ObjectPool(GameObject pooledObject, Action<T> pullObject, Action<T> pushObject, int numToSpawn = 0) {
        this.prefab = pooledObject;
        this.pullObject = pullObject;
        this.pushObject = pushObject;
    }

    private void Spawn(int number) {
        T t;

        for (int i = 0; i < number; i++) {
            t = GameObject.Instantiate(prefab).GetComponent<T>();
            pooledObjects.Push(t);
            t.gameObject.SetActive(false);
        }
    }

    public T Pull() {
        T t;
        if (pooledCount > 0)
        {
            t = pooledObjects.Pop();
        }
        else {
            t = GameObject.Instantiate(prefab).GetComponent<T>();
        }

        t.gameObject.SetActive(true);
        t.Initialize(Push);

        pullObject?.Invoke(t);

        return t;
    }

    public T Pull(Vector3 position) {
        T t = Pull();
        t.transform.position = position;
        return t;
    }

    public T Pull(Vector3 position, Quaternion rotation) {
        T t = Pull();
        t.transform.position = position;
        t.transform.rotation = rotation;
        return t;
    }

    public GameObject PullGameObject() {
        return Pull().gameObject;
    }

    public GameObject PullGameObject(Vector3 position)
    {
        GameObject go = Pull().gameObject;
        go.transform.position = position;
        return go;
    }

    public GameObject PullGameObject(Vector3 position, Quaternion rotation)
    {
        GameObject go = Pull().gameObject;
        go.transform.position = position;
        go.transform.rotation = rotation;
        return go;
    }

    public void Push(T t) {
        pooledObjects.Push(t);

        pushObject?.Invoke(t);

        t.gameObject.SetActive(false);
    }
}

public class PoolObject : MonoBehaviour, IPoolable<PoolObject> {
    private Action<PoolObject> returnToPool;

    private void OnDisable() {
        ReturnToPool();
    }

    public void Initialize(Action<PoolObject> returnAction) {
        this.returnToPool = returnAction;
    }

    public void ReturnToPool() {
        returnToPool?.Invoke(this);
    }
}

public interface IPool<T> {
    T Pull();
    void Push(T t);
}

public interface IPoolable<T> {
    void Initialize(Action<T> returnAction);
    void ReturnToPool();
}