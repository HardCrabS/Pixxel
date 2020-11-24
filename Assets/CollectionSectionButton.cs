using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CollectionSectionButton : MonoBehaviour
{
    [SerializeField] GameObject collectionPanel;

    Animator animator;
    Toggle toggle;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        toggle = GetComponent<Toggle>();
    }

    public void ToggleSection()
    {
        animator.SetBool("Pressed", toggle.isOn);
        if (collectionPanel != null)
            collectionPanel.SetActive(toggle.isOn);
    }
}