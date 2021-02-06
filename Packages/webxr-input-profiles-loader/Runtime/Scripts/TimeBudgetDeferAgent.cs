using GLTFast;
using System.Threading.Tasks;
using UnityEngine;

namespace WebXRInputProfile
{
  [DefaultExecutionOrder(-10)]
  public class TimeBudgetDeferAgent : MonoBehaviour, IDeferAgent
  {
    public float timeBudget = 0.009f;
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
      return !FitsInCurrentFrame(0);
    }

    public bool ShouldDefer(float duration)
    {
      return !FitsInCurrentFrame(duration);
    }

    bool FitsInCurrentFrame(float duration)
    {
      return duration <= timeBudget - (Time.realtimeSinceStartup - lastTime);
    }

    public async Task BreakPoint()
    {
      if (ShouldDefer())
      {
        await Task.Yield();
      }
    }

    public async Task BreakPoint(float duration)
    {
      if (ShouldDefer(duration))
      {
        await Task.Yield();
      }
    }
  }
}
