using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindControlsMenu : Submenu
{
    private static RebindControlsMenu instance = null;

    public static RebindControlsMenu Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<RebindControlsMenu>();
            return instance;
        }
    }

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.LogWarning("Duplicate instance of singleton found: " + gameObject.name + ", destroying.");
            Destroy(gameObject);
            return;
        }

        instance = gameObject.GetComponent<RebindControlsMenu>();
    }



    public enum InputID
    {
        FORWARD,
        BACK,
        LEFT,
        RIGHT,
        JUMP,
        FALL,
        SHOOT,
        DETONATE,
        INTERACT,
        RESTART
    }

    [System.Serializable]
    public class InputIDToActionRef
    {
        [field: SerializeField] public InputID InputID { get; private set; }
        [field: SerializeField] public InputActionReference Action { get; private set; }

        [field: SerializeField] public string Name { get; set; }

        public InputIDToActionRef(InputID _inputID)
        {
            InputID = _inputID;
            Action = null;
            Name = null;
        }
    }

    private bool isOpen = false;

    [SerializeField] private GameObject parent;
    [Space(10)]

    [SerializeField] public List<InputIDToActionRef> actionsMap = new List<InputIDToActionRef>();
    [SerializeField] private List<RebindButton> rebindButtons;

    private string startingBinds;

#if UNITY_EDITOR
    private void OnValidate()
    {
        int count = Enum.GetNames(typeof(InputID)).Length;

        if (actionsMap == null || actionsMap.Count < count)
        {
            actionsMap = new List<InputIDToActionRef>();
            for (int i = 0; i < count; i++)
            {
                actionsMap.Add(new InputIDToActionRef((InputID)i));
            }
        }
    }
#endif

    public void CloseMenu(bool _saveChanges)
    {
        if (!_saveChanges)
        {
            //TODO: Check to see if player wants to discard changes
            playerInput.actions.LoadBindingOverridesFromJson(startingBinds);
        }
        else
        {
            //Save Changes
            string rebinds = playerInput.actions.SaveBindingOverridesAsJson();

            SaveData.CurrSaveData.ReboundControls = rebinds;
            SaveData.Instance.Save();
        }

        ToLastSubmenu();
    }

    public override void OnSubmenuSelected()
    {
        OpenPopup();
    }
    public override void OnSubmenuClosed()
    {
        ClosePopup();
    }

    public void Start()
    {
        foreach (InputIDToActionRef actionRef in actionsMap)
            UpdateButtonText(actionRef);
    }

    private void OpenPopup()
    {
        if (isOpen)
            return;

        isOpen = true;
        parent.SetActive(true);

        startingBinds = playerInput.actions.SaveBindingOverridesAsJson();

        foreach (InputIDToActionRef actionRef in actionsMap)
            UpdateButtonText(actionRef);
    }

    private void ClosePopup()
    {
        if (!isOpen)
            return;

        isOpen = false;
        parent.SetActive(false);
    }







    [SerializeField] private GameObject waitingForInputObject = null;
    [SerializeField] private TextMeshProUGUI waitForInputLabel;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject eventSystem;
    private Selectable lastSelected;
    InputIDToActionRef currRebinding = null;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    public void StartRebinding(InputID _inputID, Selectable _lastSelected)
    {
        if (currRebinding != null)
        {
            Debug.LogError("Already rebinding!!");
            return;
        }

        int inputIndex = (int)_inputID;

        waitForInputLabel.text = "Press any key to rebind \"" + _inputID.ToString() + "\"";

        waitingForInputObject.SetActive(true);

        currRebinding = actionsMap[inputIndex];
        lastSelected = _lastSelected;
        lastSelected.interactable = false;

        playerInput.SwitchCurrentActionMap("UI");


        if (inputIndex <= (int)InputID.RIGHT)
        {
            rebindingOperation = currRebinding.Action.action.PerformInteractiveRebinding(inputIndex + 1)
                //.WithControlsExcluding("Mouse")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete())
                .Start();
        }
        else
        {
            rebindingOperation = currRebinding.Action.action.PerformInteractiveRebinding()
                //.WithControlsExcluding("Mouse")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete())
                .Start();
        }
    }

    private void RebindComplete()
    {
        UpdateButtonText(currRebinding);

        rebindingOperation.Dispose();

        lastSelected.interactable = true;
        lastSelected.Select();
        lastSelected = null;
        waitingForInputObject.SetActive(false);

        currRebinding = null;

        playerInput.SwitchCurrentActionMap("Player");

        StartCoroutine(ReloadRoutine());
    }

    private IEnumerator ReloadRoutine()
    {
        eventSystem.SetActive(false);

        yield return null;

        eventSystem.SetActive(true);
    }

    private void UpdateButtonText(InputIDToActionRef actionRef)
    {
        RebindButton currButton = rebindButtons[(int)actionRef.InputID];
        InputActionReference currAction = actionRef.Action;

        int bindingIndex;
        if ((int)actionRef.InputID <= (int)InputID.RIGHT)
            bindingIndex = currAction.action.GetBindingIndexForControl(currAction.action.controls[(int)actionRef.InputID]);
        else
            bindingIndex = currAction.action.GetBindingIndexForControl(currAction.action.controls[0]);

        string bindingName = InputControlPath.ToHumanReadableString(
            currAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice).ToLower()
            .Replace("left button", "lmb")
            .Replace("right button", "rmb")
            .Replace("control", "ctrl")
            .Replace("left ", "l ")
            .Replace("right ", "r ");

        actionRef.Name = bindingName;

        currButton.SetLabel(bindingName);
    }

    public static string GetNameOfBinding(InputID _button)
    {
        return instance.actionsMap[(int)_button].Name;
    }
}
