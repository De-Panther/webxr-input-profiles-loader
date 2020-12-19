using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WebXRInputProfile.Viewer
{
  public class InputProfileViewer : MonoBehaviour
  {
    public InputProfileModel inputProfileModel;
    public InputProfileLoader inputProfileLoader;
    public Dropdown profileNameDropdown;
    public Button loadProfile;
    public Button loadNone;
    public Button loadLeft;
    public Button loadRight;
    public Slider[] buttonsSliders;
    public Slider[] axesSliders;
    public Slider[] rotationsSliders;
    private string lastDownloadedProfile;

    void Start()
    {
      profileNameDropdown.ClearOptions();
      var profilesPaths = inputProfileLoader.GetProfilesPaths();
      if (profilesPaths == null || profilesPaths.Count == 0)
      {

        inputProfileLoader.LoadProfilesList(HandleProfilesList);
      }
      else
      {
        HandleProfilesList(profilesPaths);
      }
    }

    void HandleProfilesList(Dictionary<string,string> profilesPaths)
    {
      if (profilesPaths == null || profilesPaths.Count == 0)
      {
        return;
      }
      List<string> profileNames = new List<string>();
      foreach (var keyValPair in profilesPaths)
      {
        profileNames.Add(keyValPair.Key);
      }
      profileNameDropdown.AddOptions(profileNames);
      loadProfile.interactable = true;
    }

    public void LoadProfile()
    {
      lastDownloadedProfile = profileNameDropdown.options[profileNameDropdown.value].text;
      loadNone.interactable = false;
      loadLeft.interactable = false;
      loadRight.interactable = false;
      inputProfileLoader.LoadProfile(new string[]{lastDownloadedProfile}, OnProfileLoaded);
    }

    private void OnProfileLoaded(bool success)
    {
      loadNone.interactable = inputProfileLoader.HasModelForHand(lastDownloadedProfile, InputProfileLoader.Handedness.none);
      loadLeft.interactable = inputProfileLoader.HasModelForHand(lastDownloadedProfile, InputProfileLoader.Handedness.left);
      loadRight.interactable = inputProfileLoader.HasModelForHand(lastDownloadedProfile, InputProfileLoader.Handedness.right);
    }

    public void LoadNone()
    {
      LoadModel(InputProfileLoader.Handedness.none);
    }

    public void LoadLeft()
    {
      LoadModel(InputProfileLoader.Handedness.left);
    }

    public void LoadRight()
    {
      LoadModel(InputProfileLoader.Handedness.right);
    }

    private void LoadModel(InputProfileLoader.Handedness handedness)
    {
      if (inputProfileModel != null)
      {
        Destroy(inputProfileModel.gameObject);
      }
      inputProfileModel = inputProfileLoader.LoadModelForHand(lastDownloadedProfile, handedness);
      UpdateModelRotations();
      for (int i = 0; i < buttonsSliders.Length; i++)
      {
        SetButtonValue(i);
      }
      for (int i = 0; i < axesSliders.Length; i++)
      {
        SetAxisValue(i);
      }
    }

    public void SetButtonValue(int index)
    {
      if (inputProfileModel)
      {
        inputProfileModel.SetButtonValue(index, buttonsSliders[index].value);
      }
    }

    public void SetAxisValue(int index)
    {
      if (inputProfileModel)
      {
        inputProfileModel.SetAxisValue(index, axesSliders[index].value);
      }
    }

    public void UpdateModelRotations()
    {
      if (inputProfileModel)
      {
        inputProfileModel.transform.localEulerAngles = new Vector3(rotationsSliders[0].value,
                                                                  rotationsSliders[1].value,
                                                                  rotationsSliders[2].value);
      }
    }
  }
}
