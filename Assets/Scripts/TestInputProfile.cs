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
    public InputField profileName;
    public Button loadNone;
    public Button loadLeft;
    public Button loadRight;
    public Slider[] buttonsSliders;
    public Slider[] axesSliders;

    public void LoadProfile()
    {
      inputProfileLoader.LoadProfile(profileName.text, OnProfileLoaded);
    }

    private void OnProfileLoaded(bool success)
    {
      loadNone.interactable = success;
      loadLeft.interactable = success;
      loadRight.interactable = success;
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
      inputProfileModel = inputProfileLoader.LoadModelForHand(handedness);
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

  }
}
