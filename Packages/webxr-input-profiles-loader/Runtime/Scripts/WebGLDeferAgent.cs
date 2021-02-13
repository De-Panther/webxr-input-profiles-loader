using GLTFast;
using System.Threading.Tasks;
using UnityEngine;

namespace WebXRInputProfile
{
  public class WebGLDeferAgent : MonoBehaviour, IDeferAgent
  {
    public bool ShouldDefer()
    {
      return false;
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
