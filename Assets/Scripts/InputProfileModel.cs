using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast;

namespace WebXRInputProfile
{
  public class InputProfileModel : MonoBehaviour
  {
    private float[] buttons = new float[7];
    private float[] axes = new float[4];

    private LayoutRouting layoutRouting;

    public LayoutTransforms layoutTransforms;

    private GltfAsset gltfAsset;

    public void Init(LayoutRouting layoutRouting, string url)
    {
      this.layoutRouting = layoutRouting;
      if (gltfAsset == null)
      {
        gltfAsset = gameObject.AddComponent<GLTFast.GltfAsset>();
      }
      gltfAsset.loadOnStartup = false;
      gltfAsset.url = url;
      gltfAsset.onLoadComplete += OnGltfLoaded;
      gltfAsset.Load(gltfAsset.url);
    }

    private void OnGltfLoaded(GltfAssetBase assetBase, bool success)
    {
      gltfAsset.onLoadComplete -= OnGltfLoaded;
      var _transform = transform;
      if (layoutRouting != null)
      {
        layoutTransforms = new LayoutTransforms();
        for (int i = 0; i < layoutRouting.buttons.Length; i++)
        {
          if (layoutRouting.buttons[i] != null)
          {
            layoutTransforms.buttons[i] = new LayoutTransform();
            layoutTransforms.buttons[i].valueNode = TransformFindRecursive(_transform, layoutRouting.buttons[i].valueNodeName);
            layoutTransforms.buttons[i].minNode = TransformFindRecursive(_transform, layoutRouting.buttons[i].minNodeName);
            layoutTransforms.buttons[i].maxNode = TransformFindRecursive(_transform, layoutRouting.buttons[i].maxNodeName);
          }
        }
        for (int i = 0; i < layoutRouting.axes.Length; i++)
        {
          if (layoutRouting.axes[i] != null)
          {
            layoutTransforms.axes[i] = new LayoutTransform();
            layoutTransforms.axes[i].valueNode = TransformFindRecursive(_transform, layoutRouting.axes[i].valueNodeName);
            layoutTransforms.axes[i].minNode = TransformFindRecursive(_transform, layoutRouting.axes[i].minNodeName);
            layoutTransforms.axes[i].maxNode = TransformFindRecursive(_transform, layoutRouting.axes[i].maxNodeName);
          }
        }
      }
    }

    private Transform TransformFindRecursive(Transform parent, string value)
    {
      Transform result = parent.Find(value);
      if (result != null)
      {
        return result;
      }
      foreach (Transform _transform in parent)
      {
        result = TransformFindRecursive(_transform, value);
        if (result != null)
        {
          break;
        }
      }
      return result;
    }

    public bool SetButtonValue(int index, float value)
    {
      if (index < 0 || index >= buttons.Length)
      {
        Debug.LogError ("Index error");
        return false;
      }
      buttons[index] = Mathf.Clamp01(value);
      if (layoutTransforms.buttons[index] == null
          || layoutTransforms.buttons[index].valueNode == null
          || layoutTransforms.buttons[index].minNode == null
          || layoutTransforms.buttons[index].maxNode == null)
      {
        Debug.LogError ("No button");
        return false;
      }
      //layoutTransforms.buttons[index].valueNode.localPosition
      var position = Vector3.Lerp(layoutTransforms.buttons[index].minNode.localPosition,
                                  layoutTransforms.buttons[index].maxNode.localPosition,
                                  buttons[index]);
      layoutTransforms.buttons[index].valueNode.localPosition = position;
      var rotation = Quaternion.Lerp(layoutTransforms.buttons[index].minNode.localRotation,
                                    layoutTransforms.buttons[index].maxNode.localRotation,
                                    buttons[index]);
      layoutTransforms.buttons[index].valueNode.localRotation = rotation;
      return true;
    }

    public bool SetAxisValue(int index, float value)
    {
      if (index < 0 || index >= axes.Length)
      {
        Debug.LogError ("Index error");
        return false;
      }
      axes[index] = Mathf.Clamp(value, -1f, 1f);
      axes[index] = Mathf.InverseLerp(-1f, 1f, axes[index]);
      if (layoutTransforms.axes[index] == null
          || layoutTransforms.axes[index].valueNode == null
          || layoutTransforms.axes[index].minNode == null
          || layoutTransforms.axes[index].maxNode == null)
      {
        Debug.LogError ("No axis");
        return false;
      }
      var position = Vector3.Lerp(layoutTransforms.axes[index].minNode.localPosition,
                                  layoutTransforms.axes[index].maxNode.localPosition,
                                  axes[index]);
      layoutTransforms.axes[index].valueNode.localPosition = position;
      var rotation = Quaternion.Lerp(layoutTransforms.axes[index].minNode.localRotation,
                                    layoutTransforms.axes[index].maxNode.localRotation,
                                    axes[index]);
      layoutTransforms.axes[index].valueNode.localRotation = rotation;
      return true;
    }

  }
}
