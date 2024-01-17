using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace WebXRInputProfile
{
  public class InputProfileLoader : MonoBehaviour
  {
    private const string DEFAULT_PROFILES_URL = "https://cdn.jsdelivr.net/npm/@webxr-input-profiles/assets/dist/profiles/";

    public enum Handedness
    {
      none = 0,
      left = 1,
      right = 2
    }

    // For cases where asked to download controllers separately
    private Dictionary<string, Profile> downloadedProfiles = new Dictionary<string, Profile>();
    private Dictionary<string, LayoutRouting[]> cachedLayoutRoutings = new Dictionary<string, LayoutRouting[]>();
    private Dictionary<string, string> profilesPaths = null;

    public Dictionary<string, string> GetProfilesPaths()
    {
      return profilesPaths;
    }

    private string profilesUrl = DEFAULT_PROFILES_URL;

    UnityWebRequest profilesListWebRequest = null;
    UnityWebRequest profileWebRequest = null;

    public void LoadProfilesList(System.Action<Dictionary<string, string>> callback = null, string profilesUrl = null)
    {
      if (profilesPaths == null || profilesPaths.Count == 0 || (profilesUrl != null && profilesUrl != this.profilesUrl))
      {
        this.profilesUrl = !string.IsNullOrEmpty(profilesUrl) ? profilesUrl : DEFAULT_PROFILES_URL;
        StartCoroutine(DownloadProfilesList(callback));
      }
      else
      {
        callback(profilesPaths);
      }
    }

    IEnumerator DownloadProfilesList(System.Action<Dictionary<string, string>> callback = null)
    {
      if (profilesListWebRequest == null)
      {
        profilesListWebRequest = UnityWebRequest.Get(profilesUrl + "profilesList.json");
        yield return profilesListWebRequest.SendWebRequest();
      }
      while (profilesListWebRequest != null && !profilesListWebRequest.isDone)
      {
        yield return null;
      }

      if (profilesListWebRequest != null)
      {
        if (!profilesListWebRequest.isNetworkError && !profilesListWebRequest.isHttpError)
        {
          var profilesList = JsonConvert.DeserializeObject<Dictionary<string, ProfileInfo>>(profilesListWebRequest.downloadHandler.text);
          if (profilesList != null)
          {
            profilesPaths = new Dictionary<string, string>();
            foreach (var keyValPair in profilesList)
            {
              profilesPaths.Add(keyValPair.Key, keyValPair.Value.path);
            }
          }
        }
        profilesListWebRequest.Dispose();
        profilesListWebRequest = null;
      }

      if (callback != null)
      {
        callback(profilesPaths);
      }
    }

    public void LoadProfile(string[] profileNames, System.Action<bool> callback = null)
    {
      if (profileNames == null || profileNames.Length == 0)
      {
        Debug.LogError("No profile name");
        callback?.Invoke(false);
        return;
      }
      if (DownloadedProfileExist(profileNames))
      {
        callback?.Invoke(true);
        return;
      }
      StartCoroutine(DownloadProfile(profileNames, callback));
    }

    IEnumerator DownloadProfile(string[] profileNames, System.Action<bool> callback)
    {
      if (profilesPaths == null)
      {
        yield return DownloadProfilesList();
      }
      string profilePath = string.Empty;
      string profileName = string.Empty;
      int profilesIndex = -1;
      if (profilesPaths != null)
      {
        for (int i = 0; i < profileNames.Length; i++)
        {
          if (profilesPaths.ContainsKey(profileNames[i]))
          {
            profileName = profileNames[i];
            profilePath = profilesPaths[profileName];
            profilesIndex = i;
            break;
          }
        }
      }
      if (profilesIndex == -1)
      {
        callback(false);
        yield break;
      }
      // if no path for profiles, group them together with the first profile that has path
      string[] similarProfiles = new string[profilesIndex + 1];
      for (int i = 0; i < similarProfiles.Length; i++)
      {
        similarProfiles[i] = profileNames[i];
      }

      if (profileWebRequest == null)
      {
        profileWebRequest = UnityWebRequest.Get(profilesUrl + profilePath);
        yield return profileWebRequest.SendWebRequest();
      }

      while (profileWebRequest != null && !profileWebRequest.isDone)
      {
        yield return null;
      }

      if (profileWebRequest != null)
      {
        if (!profileWebRequest.isNetworkError && !profileWebRequest.isHttpError)
        {
          HandleProfileText(similarProfiles, profileWebRequest.downloadHandler.text, callback);
        }
        else if (callback != null)
        {
          callback(false);
        }
        profileWebRequest.Dispose();
        profileWebRequest = null;
      }
      else
      {
        callback?.Invoke(DownloadedProfileExist(similarProfiles));
      }
    }

    private void HandleProfileText(string[] profileNames, string profileText, System.Action<bool> callback)
    {
      if (DownloadedProfileExist(profileNames))
      {
        callback?.Invoke(true);
        return;
      }
      var profile = JsonConvert.DeserializeObject<Profile>(profileText);
      var layoutRoutings = CreateLayoutRoutings(profile);
      for (int i = 0; i < profileNames.Length; i++)
      {
        downloadedProfiles.Add(profileNames[i], profile);
        cachedLayoutRoutings.Add(profileNames[i], layoutRoutings);
      }
      callback?.Invoke(true);
    }

    private bool DownloadedProfileExist(string[] profileNames)
    {
      for (int i = 0; i < profileNames.Length; i++)
      {
        if (downloadedProfiles.ContainsKey(profileNames[i]))
        {
          for (int j = 0; j < i; j++)
          {
            downloadedProfiles.Add(profileNames[j], downloadedProfiles[profileNames[i]]);
            cachedLayoutRoutings.Add(profileNames[j], cachedLayoutRoutings[profileNames[i]]);
          }
          return true;
        }
      }
      return false;
    }

    private LayoutRouting[] CreateLayoutRoutings(Profile profile)
    {
      var layoutRoutings = new LayoutRouting[3];
      if (profile.layouts.left_right_none != null)
      {
        var layoutRouting = GetLayoutRouting(profile.layouts.left_right_none); ;
        layoutRoutings[(int)Handedness.none] = layoutRouting;
        layoutRoutings[(int)Handedness.left] = layoutRouting;
        layoutRoutings[(int)Handedness.right] = layoutRouting;

      }
      else if (profile.layouts.left_right != null)
      {
        var layoutRouting = GetLayoutRouting(profile.layouts.left_right); ;
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

      return layoutRoutings;
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
#if UNITY_EDITOR
      var modelObject = new GameObject(layoutRoutings[(int)handedness].rootNodeName);
#else
      var modelObject = new GameObject();
#endif
      var inputProfileModel = modelObject.AddComponent<InputProfileModel>();
      string assetPath = profilesUrl + downloadedProfiles[profileName].profileId + "/" + layoutRoutings[(int)handedness].assetPath;
      inputProfileModel.Init(layoutRoutings[(int)handedness], assetPath, callback);
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
