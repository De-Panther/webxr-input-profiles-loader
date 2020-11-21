using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GLTFast;
using Newtonsoft.Json;

namespace WebXRInputProfile
{
  public class InputProfileLoader : MonoBehaviour
  {
    public enum Handedness
    {
      none = 0,
      left = 1,
      right = 2
    }

    public string profileName;
    private Profile profile;

    private string profiles_url = "https://cdn.jsdelivr.net/npm/@webxr-input-profiles/assets/dist/profiles/{0}/{1}";

    private LayoutRouting[] layoutRoutings = new LayoutRouting[3];

    [ContextMenu("Load ProfileName")]
    private void LoadProfile()
    {
      LoadProfile(profileName);
    }

    [ContextMenu("Load None Hand")]
    private void LoadNoneHand()
    {
      LoadModelForHand(Handedness.none);
    }

    [ContextMenu("Load Left Hand")]
    private void LoadLeftHand()
    {
      LoadModelForHand(Handedness.left);
    }

    [ContextMenu("Load Right Hand")]
    private void LoadRightHand()
    {
      LoadModelForHand(Handedness.right);
    }

    public void LoadProfile(string profileName, System.Action<bool> callback = null)
    {
      this.profileName = profileName;
      StartCoroutine(DownloadProfile(callback));
    }

    IEnumerator DownloadProfile(System.Action<bool> callback)
    {
      using (UnityWebRequest webRequest = UnityWebRequest.Get(string.Format(profiles_url, profileName, "profile.json")))
      {
        yield return webRequest.SendWebRequest();
        while (!webRequest.isDone)
        {
          yield return null;
        }

        if (!webRequest.isNetworkError && !webRequest.isHttpError)
        {
          HandleProfileText(webRequest.downloadHandler.text, callback);
        }
        else if (callback != null)
        {
          callback(false);
        }
      }
    }

    private void HandleProfileText(string profileText, System.Action<bool> callback)
    {
      profile = JsonConvert.DeserializeObject<Profile>(profileText);

      layoutRoutings = new LayoutRouting[3];
      if (profile.layouts.left_right_none != null)
      {
        var layoutRouting = GetLayoutRouting(profile.layouts.left_right_none);;
        layoutRoutings[(int)Handedness.none] = layoutRouting;
        layoutRoutings[(int)Handedness.left] = layoutRouting;
        layoutRoutings[(int)Handedness.right] = layoutRouting;

      }
      else if (profile.layouts.left_right != null)
      {
        var layoutRouting = GetLayoutRouting(profile.layouts.left_right);;
        layoutRoutings[(int)Handedness.left] = layoutRouting;
        layoutRoutings[(int)Handedness.right] = layoutRouting;
      }
      else
      {
        if (profile.layouts.left != null)
        {
          layoutRoutings[(int)Handedness.left] = GetLayoutRouting(profile.layouts.left);
        }
        if (profile.layouts.right != null)
        {
          layoutRoutings[(int)Handedness.right] = GetLayoutRouting(profile.layouts.right);
        }
      }

      if (profile.layouts.none != null)
      {
        layoutRoutings[(int)Handedness.none] = GetLayoutRouting(profile.layouts.none);
      }

      callback?.Invoke(true);
    }

    private LayoutRouting GetLayoutRouting(Layout layout)
    {
      var layoutRouting = new LayoutRouting();
      layoutRouting.rootNodeName = layout.rootNodeName;
      layoutRouting.assetPath = layout.assetPath;
      foreach (var component in layout.components)
      {
        foreach (var visualResponse in component.Value.visualResponses)
        {
          switch (visualResponse.Value.componentProperty)
          {
            case ComponentPropertyTypes.button:
              layoutRouting.buttons[component.Value.gamepadIndices.button.Value] = visualResponse.Value;
              break;
            case ComponentPropertyTypes.xAxis:
              layoutRouting.axes[component.Value.gamepadIndices.xAxis.Value] = visualResponse.Value;
              break;
            case ComponentPropertyTypes.yAxis:
              layoutRouting.axes[component.Value.gamepadIndices.yAxis.Value] = visualResponse.Value;
              break;
          }
        }
      }
      return layoutRouting;
    }

    public InputProfileModel LoadModelForHand(Handedness handedness)
    {
      if (profile == null)
      {
        Debug.LogError("No profile");
        return null;
      }
      if (layoutRoutings[(int)handedness] == null)
      {
        Debug.LogError("No model for hand");
        return null;
      }
      var modelObject = new GameObject(layoutRoutings[(int)handedness].rootNodeName);
      var inputProfileModel = modelObject.AddComponent<InputProfileModel>();
      inputProfileModel.Init(layoutRoutings[(int)handedness], string.Format(profiles_url, profileName, layoutRoutings[(int)handedness].assetPath));
      return inputProfileModel;
    }


  }
}
