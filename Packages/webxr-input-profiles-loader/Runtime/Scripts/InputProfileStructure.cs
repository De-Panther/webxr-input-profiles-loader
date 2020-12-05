using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace WebXRInputProfile
{
  [Serializable]
  public class Profile
  {
    public string profileId = string.Empty;
    public string[] fallbackProfileIds = new string[0];
    public Layouts layouts;
  }

  [Serializable]
  public class Layouts
  {
    public Layout none;
    public Layout left;
    public Layout right;
    [JsonProperty("left-right")]
    public Layout left_right;
    [JsonProperty("left-right-none")]
    public Layout left_right_none;
  }

  [Serializable]
  public class Layout
  {
    public string selectComponentId;
    public Dictionary<string, Component> components;
    public string rootNodeName;
    public string assetPath;
  }

  [Serializable]
  public class Component
  {
    public GamepadIndices gamepadIndices;
    public string rootNodeName;
    public Dictionary<string, VisualResponse> visualResponses;
  }

  [Serializable]
  public class GamepadIndices
  {
    public int? button;
    public int? xAxis;
    public int? yAxis;
  }

  [Serializable]
  public class VisualResponse
  {
    public ComponentPropertyTypes componentProperty;
    public ValueNodePropertyTypes valueNodeProperty;
    public string valueNodeName;
    public string minNodeName;
    public string maxNodeName;
  }

  public enum ComponentPropertyTypes
  {
    button,
    xAxis,
    yAxis,
    state
  }

  public enum ValueNodePropertyTypes
  {
    transform,
    visibility
  }

  [Serializable]
  public class LayoutRouting
  {
    public string rootNodeName;
    public string assetPath;
    public VisualResponse[] buttons = new VisualResponse[7];
    public VisualResponse[] axes = new VisualResponse[4];
  }

  [Serializable]
  public class LayoutTransforms
  {
    public LayoutTransform[] buttons = new LayoutTransform[7];
    public LayoutTransform[] axes = new LayoutTransform[4];
  }

  [Serializable]
  public class LayoutTransform
  {
    public Transform valueNode;
    public Transform minNode;
    public Transform maxNode;
  }

  [Serializable]
  public class ProfileInfo
  {
    public string path = string.Empty;
    public bool deprecated = false;
  }
}