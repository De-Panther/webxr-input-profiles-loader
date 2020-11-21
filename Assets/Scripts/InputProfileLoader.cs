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
    private const string DEFAULT_PROFILES_URL = "https://cdn.jsdelivr.net/npm/@webxr-input-profiles/assets/dist/profiles/{0}/{1}";

    public enum Handedness
    {
      none = 0,
      left = 1,
      right = 2
    }

    // For cases where asked to download controllers separately
    private static Dictionary<string, Profile> downloadedProfiles = new Dictionary<string, Profile>();
    private static Dictionary<string, LayoutRouting[]> cachedLayoutRoutings = new Dictionary<string, LayoutRouting[]>();

    private string profilesUrl = DEFAULT_PROFILES_URL;

    public void LoadProfile(string profileName, System.Action<bool> callback = null, string profilesUrl = null)
    {
      this.profilesUrl = !string.IsNullOrEmpty(profilesUrl) ? profilesUrl : DEFAULT_PROFILES_URL;
      if (string.IsNullOrEmpty(profileName))
      {
        Debug.LogError("No profile name");
        callback?.Invoke(false);
        return;
      }
      if (downloadedProfiles.ContainsKey(profileName))
      {
        callback?.Invoke(true);
        return;
      }
      StartCoroutine(DownloadProfile(profileName, callback));
    }

    IEnumerator DownloadProfile(string profileName, System.Action<bool> callback)
    {
      using (UnityWebRequest webRequest = UnityWebRequest.Get(string.Format(profilesUrl, profileName, "profile.json")))
      {
        yield return webRequest.SendWebRequest();
        while (!webRequest.isDone)
        {
          yield return null;
        }

        if (!webRequest.isNetworkError && !webRequest.isHttpError)
        {
          HandleProfileText(profileName, webRequest.downloadHandler.text, callback);
        }
        else if (callback != null)
        {
          callback(false);
        }
      }
    }

    private void HandleProfileText(string profileName, string profileText, System.Action<bool> callback)
    {
      if (downloadedProfiles.ContainsKey(profileName))
      {
        callback?.Invoke(true);
        return;
      }
      var profile = JsonConvert.DeserializeObject<Profile>(profileText);
      downloadedProfiles.Add(profileName ,profile);
      SetLayoutRoutings(profileName ,profile);
      callback?.Invoke(true);
    }

    private void SetLayoutRoutings(string profileName, Profile profile)
    {
      var layoutRoutings = new LayoutRouting[3];
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

      cachedLayoutRoutings.Add(profileName, layoutRoutings);
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

    public InputProfileModel LoadModelForHand(string profileName, Handedness handedness, System.Action<bool> callback = null)
    {
      if (!downloadedProfiles.ContainsKey(profileName))
      {
        Debug.LogError("No profile");
        return null;
      }
      var layoutRoutings = cachedLayoutRoutings[profileName];
      if (layoutRoutings[(int)handedness] == null)
      {
        Debug.LogError("No model for hand");
        return null;
      }
      var modelObject = new GameObject(layoutRoutings[(int)handedness].rootNodeName);
      var inputProfileModel = modelObject.AddComponent<InputProfileModel>();
      inputProfileModel.Init(layoutRoutings[(int)handedness], string.Format(profilesUrl, profileName, layoutRoutings[(int)handedness].assetPath), callback);
      return inputProfileModel;
    }

    public bool HasModelForHand(string profileName, Handedness handedness)
    {
      if (cachedLayoutRoutings.ContainsKey(profileName))
      {
        return cachedLayoutRoutings[profileName][(int)handedness] != null;
      }
      return false;
    }
  }
}
