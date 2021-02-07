using GLTFast;
using System.Threading.Tasks;
using UnityEngine;

namespace WebXRInputProfile
{
  [DefaultExecutionOrder(-10)]
  public class TimeBudgetDeferAgent : MonoBehaviour, IDeferAgent
  {
    float lastTime;

    private int shouldDeferCount = 0;

    public bool ShouldDefer()
    {
      return true;
    }

    public bool ShouldDefer(float duration)
    {
      return false;
    }

    public async Task BreakPoint()
    {
      await Task.Yield();
    }

    public async Task BreakPoint(float duration)
    {
      await Task.Yield();
    }
  }
}
