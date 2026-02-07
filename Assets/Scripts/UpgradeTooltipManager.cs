using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeTooltipManager : MonoBehaviour
{

    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMPro.TextMeshProUGUI tooltipText;

    private InputAction selectAction;
    private InputAction deselectAction;
    private InputAction cursorLocationAction;
    private RectTransform tooltipRect;
    private GameObject upgradeOption1Prefab;
    private GameObject upgradeOption2Prefab;

    private UpgradeableObject currObjectSelected;

    private void Awake()
    {
        tooltipRect = tooltipPanel.GetComponent<RectTransform>();

        selectAction = InputSystem.actions.FindAction("Select");
        deselectAction = InputSystem.actions.FindAction("Deselect");
        cursorLocationAction = InputSystem.actions.FindAction("CursorLocation");
    }

    private void OnEnable()
    {
        selectAction.performed += OnSelect;
        deselectAction.performed += OnDeselect;
    }

    private void OnDisable()
    {
        selectAction.performed -= OnSelect;
        deselectAction.performed -= OnDeselect;
    }

    private void OnSelect(InputAction.CallbackContext ctx)
    {
        //TODO only if we have a buildable tile selected, if so grab values from UpgradeableObject and show

        Vector2 screenPos = cursorLocationAction.ReadValue<Vector2>();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null && (hit.CompareTag("EconomyTower")))
        {
            // Clicked on a collider with the matching tag
            currObjectSelected = hit.gameObject.GetComponent<UpgradeableObject>();
            SetupTooltip();
        }
        else
        {
            Debug.Log("Didn't find currently implemented selectable tag");
            return;
        }

        tooltipPanel.SetActive(true);

        if (tooltipPanel.activeSelf)
            PositionAtCursor();
    }

    private void OnDeselect(InputAction.CallbackContext ctx)
    {
        tooltipPanel.SetActive(false);

        if (tooltipPanel.activeSelf)
            PositionAtCursor();
    }

    private void PositionAtCursor()
    {
        Vector2 screenPos = cursorLocationAction.ReadValue<Vector2>();

        //TODO use the location of the nearest selected tile not the mouse location directly
        tooltipRect.position = screenPos;
    }

    private void SetupTooltip()
    {
        tooltipText.text = currObjectSelected.tooltipText;
        upgradeOption1Prefab = currObjectSelected.upgradeOption1;
        upgradeOption2Prefab = currObjectSelected.upgradeOption2;
        // TODO this needs to be created more intelligently to show what values we will upgrade to, probably on hover of the buttons
    }

    public void UpgradeOption1()
    {
        ImplementUpgrade(currObjectSelected.gameObject, upgradeOption1Prefab);
    }

    public void UpgradeOption2()
    {
        ImplementUpgrade(currObjectSelected.gameObject, upgradeOption2Prefab);
    }

    private void ImplementUpgrade(GameObject from, GameObject to)
    {
        GameObject.Instantiate(to, from.gameObject.transform.position, from.gameObject.transform.rotation);
    }
}
