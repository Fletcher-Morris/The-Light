using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LayerTools
{
    public static LayerMask CreateLayerMask(string[] _layers)
    {
        LayerMask mask = new LayerMask();
        foreach(string layer in _layers)
        {
            mask = mask | 1 << LayerMask.NameToLayer(layer);
        }
        return mask;
    }

    public static LayerMask CreateLayerMask(this LayerMask _layerMask, string[] _layers)
    {
        return CreateLayerMask(_layers);
    }

    public static LayerMask AllLayers()
    {
        return ~0;
    }

    public static LayerMask AllLayers(this LayerMask _layerMask)
    {
        return AllLayers();
    }

    public static LayerMask AddLayer(this LayerMask _layerMask, string _layer)
    {
        return _layerMask | 1 << LayerMask.NameToLayer(_layer);
    }

    public static LayerMask RemoveLayer(this LayerMask _layerMask, string _layer)
    {
        return _layerMask | 1 >> LayerMask.NameToLayer(_layer);
    }
}