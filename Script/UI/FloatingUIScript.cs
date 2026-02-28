using UnityEngine;

public class FloatingUIScript : MonoBehaviour
{
    [SerializeField] private GameObject TARGET;
    
    private void Start()
    {
        MonsterBase mosnterBase = GetComponentInParent<MonsterBase>();
        Debug.Assert(mosnterBase, $"Cannot found monster base [{gameObject.name}]");
        mosnterBase.Status.IsEnemeyFoundPlayer.AddListener(ToggleOn);
        TARGET.SetActive(false);
    }

    public void ToggleOn(bool toggle)
    {
        Debug.Assert(toggle, "Toggle Cannot be false");
        TARGET.SetActive(true);

    }

    public void DisableUnityEvent()
    {
        TARGET.SetActive(false);
    }
}
