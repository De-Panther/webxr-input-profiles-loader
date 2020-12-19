using GLTFast;
using UnityEngine;

namespace WebXRInputProfile
{
  public class TimeBudgetDeferAgent : MonoBehaviour, IDeferAgent
  {
    public float timeBudget = 0.001f;
    float lastTime;

    void Awake()
    {
      Reset();
    }

    void Update()
    {
      Reset();
    }

    void Reset()
    {
      lastTime = Time.realtimeSinceStartup;
    }

    public bool ShouldDefer()
    {
      float now = Time.realtimeSinceStartup;
      if (now - lastTime > timeBudget)
      {
        lastTime = now;
        return true;
      }
      return false;
    }
  }
}
