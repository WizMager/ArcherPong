using Controllers.Interfaces;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SingleUIController : IStart, ICleanup
{
    private readonly Button _leave;

    public SingleUIController(Button leave)
    {
        _leave = leave;
    }
    
    public void Start()
    {
        _leave.onClick.AddListener(LeftRoom);
    }

    private void LeftRoom()
    {
        SceneManager.LoadScene(0);
    }
    
    public void Cleanup()
    {
        _leave.onClick.RemoveListener(LeftRoom);
    }
}