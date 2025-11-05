using UnityEngine;


// IInteractibleを中継するためのクラス
public class InteractProxy : MonoBehaviour, IInteractable
{
    [SerializeField] private IInteractable realTarget; // 呼び出したい相手
    [SerializeField] private string realTargetName; // 呼び出したい相手の名前


    #region IInteractable

    public string GetInteractText() => realTarget.GetInteractText();

    public bool ShowInteractText => realTarget.ShowInteractText;
    public bool ActivateCrosshair => realTarget.ActivateCrosshair;

    public void Interact(GameObject targetObject)
    {
        realTarget.Interact(targetObject); 
    }

    #endregion



    private void Start()
    {
        realTarget = GameObject.Find(realTargetName).GetComponent<IInteractable>();
        Debug.Log("InteractProxyが" + realTarget + "を見つけました。");
    }
}
