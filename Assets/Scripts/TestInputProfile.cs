using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WebXRInputProfile
{
  public class TestInputProfile : MonoBehaviour
  {
    public InputProfileModel inputProfileModel;
    public InputProfileLoader inputProfileLoader;
    public InputField profileNameInput;
    public Button loadNone;
    public Button loadLeft;
    public Button loadRight;
    public Slider[] buttonsSliders;
    public Slider[] axesSliders;
    public Slider[] rotationsSliders;
    private string lastDownloadedProfile;

    public void LoadProfile()
    {
      lastDownloadedProfile = this.profileNameInput.text;
      loadNone.interactable = false;
      loadLeft.interactable = false;
      loadRight.interactable = false;
      inputProfileLoader.LoadProfile(lastDownloadedProfile, OnProfileLoaded);
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
