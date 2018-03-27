using UnityEngine;
using UnityEngine.UI;

public class Debugger : MonoBehaviour
{
    public Text UserMemory;
    public Text UserFPS;

    void Start()
    {
        timeleft = updateInterval;
    }
    void Update()
    {
        UpdateUsed();
        UpdateFPS();
    }
    private string sUserMemory;
    private uint AllMemory;
    void UpdateUsed()
    {
        sUserMemory = "";
        // MonoUsedM = Profiler.GetMonoUsedSize() / 1000000;
        AllMemory = Profiler.GetTotalAllocatedMemory() / 1000000;
        // sUserMemory += "MonoUsed:" + MonoUsedM + "M" + "\n";
        sUserMemory += "AllMemory:" + AllMemory + "M" + "\n";
        // sUserMemory += "UnUsedReserved:" + Profiler.GetTotalUnusedReservedMemory() / 1000000 + "M" + "\n";
       // s = "";
       // s += " MonoHeap:" + Profiler.GetMonoHeapSize() / 1000 + "k";
      //  s += " MonoUsed:" + Profiler.GetMonoUsedSize() / 1000 + "k";
      //  s += " Allocated:" + Profiler.GetTotalAllocatedMemory() / 1000 + "k";
      //  s += " Reserved:" + Profiler.GetTotalReservedMemory() / 1000 + "k";
      //  s += " UnusedReserved:" + Profiler.GetTotalUnusedReservedMemory() / 1000 + "k";
      //  s += " UsedHeap:" + Profiler.usedHeapSize / 1000 + "k";
        UserMemory.text = sUserMemory;
    }

    float updateInterval = 0.5f;
    private float accum = 0.0f;
    private float frames = 0;
    private float timeleft;
    private float fps;
    private string FPSAAA;
    void UpdateFPS()
    {
        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        if (timeleft <= 0.0)
        {
            fps = accum / frames;
            FPSAAA = "FPS: " + fps.ToString("f2");
            timeleft = updateInterval;
            accum = 0.0f;
            frames = 0;
        }
        UserFPS.text = FPSAAA;
    }
}