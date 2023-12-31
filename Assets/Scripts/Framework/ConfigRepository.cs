﻿using UnityEngine;
using System.Collections;
using UnityEngine.Assertions;
using System.Linq;

public class ConfigRepository : MonoBehaviour
{
    private static ConfigRepository instance;

    [SerializeField] private Object[] configs;

    private void Awake()
    {
        Assert.IsNull(instance, "There can only be one ConfigRepository");
        instance = this;
    }

    public static T GetConfig<T>()
        where T : Object
    {
        T settings = instance.configs.Where(s => s is T).FirstOrDefault() as T;
        Assert.IsNotNull(settings, typeof(T).ToString() + " does not exist");

        return settings;
    }
}
