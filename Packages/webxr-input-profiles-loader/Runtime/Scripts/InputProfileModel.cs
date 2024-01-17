using UnityEngine;
using GLTFast;

namespace WebXRInputProfile
{
  public class InputProfileModel : MonoBehaviour
  {
    private System.Action<bool> onLoadCallback;
    private float[] buttons = new float[7];
    private float[] axes = new float[4];

    private LayoutRouting layoutRouting;

    private LayoutTransforms layoutTransforms;

    private GltfAsset gltfAsset;
    private WebGLDeferAgent deferAgent;

    private GameObject gltfChild;
    private Transform gltfTransform;

    public async void Init(LayoutRouting layoutRouting, string url, System.Action<bool> callback = null)
    {
      this.layoutRouting = layoutRouting;
      onLoadCallback = callback;
      if (gltfChild == null)
      {
#if UNITY_EDITOR
        gltfChild = new GameObject("gltfChild");
#else
        gltfChild = new GameObject();
#endif
        gltfAsset = gltfChild.AddComponent<GltfAsset>();
        deferAgent = gltfChild.AddComponent<WebGLDeferAgent>();
        gltfTransform = gltfChild.transform;
        gltfTransform.parent = transform;
        gltfTransform.localPosition = Vector3.zero;
        gltfTransform.localEulerAngles = new Vector3(0, 180, 0);
      }
      gltfAsset.LoadOnStartup = false;
      gltfAsset.Url = url;
      var loadResult = await gltfAsset.Load(gltfAsset.Url, null, deferAgent);
      OnGltfLoaded(loadResult);
    }

    private void OnGltfLoaded(bool success)
    {
      if (!success)
      {
        onLoadCallback?.Invoke(false);
        return;
      }
      if (layoutRouting != null)
      {
        layoutTransforms = new LayoutTransforms();
        for (int i = 0; i < layoutRouting.buttons.Length; i++)
        {
          if (layoutRouting.buttons[i] != null)
          {
            layoutTransforms.buttons[i] = new LayoutTransform();
            layoutTransforms.buttons[i].valueNode = TransformFindRecursive(gltfTransform, layoutRouting.buttons[i].valueNodeName);
            layoutTransforms.buttons[i].minNode = TransformFindRecursive(gltfTransform, layoutRouting.buttons[i].minNodeName);
            layoutTransforms.buttons[i].maxNode = TransformFindRecursive(gltfTransform, layoutRouting.buttons[i].maxNodeName);
            SetButtonValue(i, buttons[i]);
          }
        }
        for (int i = 0; i < layoutRouting.axes.Length; i++)
        {
          if (layoutRouting.axes[i] != null)
          {
            layoutTransforms.axes[i] = new LayoutTransform();
            layoutTransforms.axes[i].valueNode = TransformFindRecursive(gltfTransform, layoutRouting.axes[i].valueNodeName);
            // In WebXR, yAxis is inverted. Not sure if here's the place to switch
            if (layoutRouting.axes[i].componentProperty == ComponentPropertyTypes.yAxis)
            {
              layoutTransforms.axes[i].minNode = TransformFindRecursive(gltfTransform, layoutRouting.axes[i].maxNodeName);
              layoutTransforms.axes[i].maxNode = TransformFindRecursive(gltfTransform, layoutRouting.axes[i].minNodeName);
            }
            else
            {
              layoutTransforms.axes[i].minNode = TransformFindRecursive(gltfTransform, layoutRouting.axes[i].minNodeName);
              layoutTransforms.axes[i].maxNode = TransformFindRecursive(gltfTransform, layoutRouting.axes[i].maxNodeName);
            }
            SetAxisValue(i, Mathf.Lerp(-1f, 1f, axes[i]));
          }
        }
        onLoadCallback?.Invoke(true);
      }
      else
      {
        onLoadCallback?.Invoke(false);
      }
    }

    private Transform TransformFindRecursive(Transform parent, string value)
    {
      Transform result = parent.Find(value);
      if (result != null)
      {
        return result;
      }
      foreach (Transform child in parent)
      {
        result = TransformFindRecursive(child, value);
        if (result != null)
        {
          break;
        }
      }
      return result;
    }

    public Transform GetChildTransform(string transformName)
    {
      return TransformFindRecursive(gltfTransform, transformName);
    }

    public bool SetButtonValue(int index, float value)
    {
      if (index < 0 || index >= buttons.Length)
      {
        Debug.LogError ("Index error");
        return false;
      }
      buttons[index] = Mathf.Clamp01(value);
      if (layoutTransforms == null
          || layoutTransforms.buttons[index] == null
          || layoutTransforms.buttons[index].valueNode == null
          || layoutTransforms.buttons[index].minNode == null
          || layoutTransforms.buttons[index].maxNode == null)
      {
        return false;
      }
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
      if (layoutTransforms == null
          || layoutTransforms.axes[index] == null
          || layoutTransforms.axes[index].valueNode == null
          || layoutTransforms.axes[index].minNode == null
          || layoutTransforms.axes[index].maxNode == null)
      {
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
