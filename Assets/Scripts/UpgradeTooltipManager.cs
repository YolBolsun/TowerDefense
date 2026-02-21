using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UpgradeTooltipManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI currTowerText;
    [SerializeField] private Image currTowerImage;

    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TMPro.TextMeshProUGUI tooltipText;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;

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
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPos);

        //warning about this being where the pointer was on the previous frame - should be fine for our purposes
        Debug.unityLogger.filterLogType = LogType.Error;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            // Don't click through ui elements
            return;
        }
        Debug.unityLogger.filterLogType = LogType.Log;
        // loop through so we don't get absorbed by a projectile collider or any other object collider
        foreach (Collider2D hit in hits)
        {
            if (hit != null && (hit.CompareTag("EconomyTower") || hit.CompareTag("OffensiveTower")))
            {
                // Clicked on a collider with the matching tag
                currObjectSelected = hit.gameObject.GetComponent<UpgradeableObject>();
                SetupTooltip();

                tooltipPanel.SetActive(true);
                PositionAtCursor();
            }
        }
    }

    private void OnDeselect(InputAction.CallbackContext ctx)
    {
        HideTooltip();   
    }

    private void HideTooltip()
    {
        tooltipPanel.SetActive(false);

        if (tooltipPanel.activeSelf)
            PositionAtCursor();
    }

    private void PositionAtCursor()
    {
        Vector2 screenPos = cursorLocationAction.ReadValue<Vector2>();

        Debug.Log(screenPos);

        float tooltipRectX = -1f;//-tooltipRect.rect.width;
        float tooltipRectY = 0f;
        if(screenPos.y > Screen.height - tooltipRect.rect.height)
        {
            tooltipRectY = 1f;// tooltipRect.rect.height;
        }
        if (screenPos.x > Screen.width - 2*tooltipRect.rect.width)
        {
            tooltipRectX = 1f;// tooltipRect.rect.width;
        }

        tooltipRect.pivot = new Vector2(tooltipRectX, tooltipRectY);

        //TODO use the location of the nearest selected tile not the mouse location directly
        tooltipRect.position = screenPos;
    }

    private void SetupTooltip()
    {
        currTowerImage.sprite = currObjectSelected.GetComponent<SpriteRenderer>().sprite;
        currTowerImage.color = currObjectSelected.GetComponent<SpriteRenderer>().color;
        OffensiveTower offTower = currObjectSelected.GetComponent<OffensiveTower>();
        if(offTower != null)
        {
            currTowerText.text = offTower.GetUpgradeableObjectTooltip();
        }
        else
        {
            currTowerText.text = currObjectSelected.currentTowerDescription;
        }
            
        if (currObjectSelected.upgradeOption1 == null)
        {
            tooltipText.text = "Fully Upgraded!";
            button1.interactable = false;
            button2.interactable = false;
            return;
        }
        else
        {
            button1.interactable = true;
            button2.interactable = true;
        }
        StopHover();
        button1.image.sprite = currObjectSelected.upgradeOption1.GetComponent<SpriteRenderer>().sprite;
        button1.image.color = currObjectSelected.upgradeOption1.GetComponent<SpriteRenderer>().color;
        button2.image.sprite = currObjectSelected.upgradeOption2.GetComponent<SpriteRenderer>().sprite;
        button2.image.color = currObjectSelected.upgradeOption2.GetComponent<SpriteRenderer>().color;
        upgradeOption1Prefab = currObjectSelected.upgradeOption1;
        upgradeOption2Prefab = currObjectSelected.upgradeOption2;
        // TODO this needs to be created more intelligently to show what values we will upgrade to, probably on hover of the buttons
    }

    public void UpgradeOption1()
    {
        if (currObjectSelected.cost1 <= EcoManager.instance.CurrGold)
        {
            EcoManager.instance.CurrGold -= currObjectSelected.cost1;
            ImplementUpgrade(currObjectSelected.gameObject, upgradeOption1Prefab);
        }
        else
        {
            Debug.Log("play ui error sound");
        }
            
    }

    public void HoverOption1()
    {
        if (currObjectSelected.upgradeOption1 == null)
        {
            tooltipText.text = "Fully Upgraded!";
            return;
        }
        OffensiveTower offTower = currObjectSelected.upgradeOption1.GetComponent<OffensiveTower>();
        if (offTower != null)
        {
            tooltipText.text = offTower.GetUpgradeableObjectTooltip();
        }
        else
        {
            tooltipText.text = currObjectSelected.tooltipText1;
        }
        tooltipText.text += $"\n Cost: {currObjectSelected.cost1}";
            
    }

    public void UpgradeOption2()
    {
        
        if (currObjectSelected.cost2 <= EcoManager.instance.CurrGold)
        {
            EcoManager.instance.CurrGold -= currObjectSelected.cost2;
            ImplementUpgrade(currObjectSelected.gameObject, upgradeOption2Prefab);
        }
        else
        {
            Debug.Log("play ui error sound");
        }
    }
    public void HoverOption2()
    {
        if (currObjectSelected.upgradeOption2 == null)
        {
            tooltipText.text = "Fully Upgraded!";
            return;
        }
        OffensiveTower offTower = currObjectSelected.upgradeOption2.GetComponent<OffensiveTower>();
        if (offTower != null)
        {
            tooltipText.text = offTower.GetUpgradeableObjectTooltip();
        }
        else
        {
            tooltipText.text = currObjectSelected.tooltipText2;
        }
        tooltipText.text += $"\n Cost: {currObjectSelected.cost1}";
    }

    public void StopHover()
    {
        if (currObjectSelected.upgradeOption1 == null)
        {
            tooltipText.text = "Fully Upgraded!";
            return;
        }
        tooltipText.text = "Hover an upgrade option to see details!";
    }

    private void ImplementUpgrade(GameObject from, GameObject to)
    {

        GameObject.Instantiate(to, from.gameObject.transform.position, from.gameObject.transform.rotation);
        Destroy(from);
        HideTooltip();
    }
}
