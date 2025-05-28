using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static SerializedSaveData;

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
        FORWARD = 0,
        BACK = 1,
        LEFT = 2,
        RIGHT = 3,
        JUMP = 4,
        FALL = 5,
        SHOOT = 6,
        DETONATE = 7,
        INTERACT = 8,
        RESTART = 9,
        PAUSE = 10
    }

    [System.Serializable]
    public class InputIDToActionRef
    {
        [field: SerializeField] public InputID InputID { get; private set; }
        [field: SerializeField] public InputActionReference Action { get; private set; }

        public InputIDToActionRef(InputID _inputID)
        {
            InputID = _inputID;
            Action = null;
        }
    }

    private bool isOpen = false;

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
        if (parent.activeSelf)
            parent.SetActive(false);
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

    public void OnResetControlsClicked(Selectable button)
    {
        lastSelected = button;
        resetChangesPopup_Parent.SetActive(true);
        resetYesButton.Select();
    }

    public void OnResetClosed(bool resetControls)
    {
        resetChangesPopup_Parent.SetActive(false);
        lastSelected.Select();

        if (resetControls)
        {
            InputHandler.Instance.ResetControls();
            startingBinds = playerInput.actions.SaveBindingOverridesAsJson();

            //Save Changes
            SaveData.CurrSaveData.ReboundControls = null;
            SaveData.Instance.Save();

            foreach (InputIDToActionRef actionRef in actionsMap)
                UpdateButtonText(actionRef);
        }
    }


    public void OnBackClicked(Selectable button)
    {
        if (startingBinds.Equals(playerInput.actions.SaveBindingOverridesAsJson()))
        {
            //No changes
            CloseMenu(false);
            return;
        }

        lastSelected = button;
        unsavedChangesPopup_Parent.SetActive(true);
        yesSaveButton.Select();
    }

    public void OnBackClosed(bool saveControls)
    {
        unsavedChangesPopup_Parent.SetActive(false);
        CloseMenu(saveControls);
    }





    [Header("RebindControlsMenu")]
    [SerializeField] private ListOfTmpSpriteAssets listofTmpSpriteAssets;

    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject unsavedChangesPopup_Parent;
    [SerializeField] private Selectable yesSaveButton;
    [SerializeField] private GameObject resetChangesPopup_Parent;
    [SerializeField] private Selectable resetYesButton;
    [Space(10)]

    [SerializeField] public List<InputIDToActionRef> actionsMap = new List<InputIDToActionRef>();
    [SerializeField] private List<RebindButton> rebindButtons;

    [SerializeField] private GameObject waitingForInputObject = null;
    [SerializeField] private TextMeshProUGUI waitForInputLabel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject eventSystem;
    private Selectable lastSelected;
    InputIDToActionRef currRebinding = null;

    private string startingBinds;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    private bool isControllerMenu = false;

    private const int rebindTime = 5;
    private Coroutine rebindTimerCoroutine = null;

    private IEnumerator StartTimer()
    {
        for (int counter = rebindTime; counter >= 0; counter--)
        {
            timerText.text = counter.ToString();

            yield return new WaitForSeconds(1);
        }

        rebindTimerCoroutine = null;
        RebindComplete();
    }

    public void StartRebinding(InputID _inputID, Selectable _lastSelected)
    {
        if (currRebinding != null)
        {
            Debug.LogError("Already rebinding!!");
            return;
        }

        rebindTimerCoroutine = StartCoroutine(StartTimer());

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
                .OnMatchWaitForAnother(0.1f)
                .WithExpectedControlType("Button")
                .OnCancel(_ => RebindComplete())
                .OnComplete(_ => RebindComplete())
                .Start();
        }
        else
        {
            rebindingOperation = currRebinding.Action.action.PerformInteractiveRebinding()
                .OnMatchWaitForAnother(0.1f)
                .OnCancel(_ => RebindComplete())
                .OnComplete(_ => RebindComplete())
                .Start();
        }
    }

    public void SetIsControllerMenu(bool _isControllerMenu)
    {
        isControllerMenu = _isControllerMenu;
    }


    private void RebindComplete()
    {
        if (rebindTimerCoroutine != null)
        {
            StopCoroutine(rebindTimerCoroutine);
        }

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

        yield return null;

        foreach (InputIDToActionRef actionRef in actionsMap)
            UpdateButtonText(actionRef);
    }

    private void UpdateButtonText(InputIDToActionRef actionRef)
    {
        int index = (int)actionRef.InputID;
        if (index >= rebindButtons.Count)
        {
            Debug.LogWarning($"Index out of range of rebindButtons: {index}");
            return;
        }

        RebindButton currButton = rebindButtons[(int)actionRef.InputID];
        InputActionReference currAction = actionRef.Action;

        /*
        int bindingIndex;
        if ((int)actionRef.InputID <= (int)InputID.RIGHT)
            bindingIndex = currAction.action.GetBindingIndexForControl(currAction.action.controls[(int)actionRef.InputID]);
        else
        {
            if (isControllerMenu)
                bindingIndex = currAction.action.GetBindingIndexForControl(currAction.action.controls[1]);
            else
                bindingIndex = currAction.action.GetBindingIndexForControl(currAction.action.controls[0]);
        }

        string bindingName;
        if (isControllerMenu)
        {
            //Controller binds
            bindingName = Utils.GetSpriteForBinding(currAction.action.bindings[bindingIndex], listofTmpSpriteAssets.SpriteAssets[(int)SaveData.CurrSaveData.ButtonDisplayType]);
        }
        else
        {
            //Keyboard binds
            bindingName = Utils.GetSpriteForBinding(currAction.action.bindings[bindingIndex], listofTmpSpriteAssets.SpriteAssets[0]);
        }

        currButton.SetLabel(bindingName);*/

        if (isControllerMenu)
            currButton.SetLabel(GetNameOfBinding(actionRef.InputID, SaveData.CurrSaveData.ButtonDisplayType));
        else
            currButton.SetLabel(GetNameOfBinding(actionRef.InputID, ButtonDisplayTypeEnum.Keyboard));
    }

    public string GetNameOfBinding(InputID _button, ButtonDisplayTypeEnum controllerType = ButtonDisplayTypeEnum.Keyboard)
    {
        InputActionReference currAction = actionsMap[(int)_button].Action;

        int bindingIndex;
        if ((int)_button <= (int)InputID.RIGHT)
        {
            try
            {
                int index = (int)_button;
                var inputControls = currAction.action.controls;
                InputControl inputControl;

                if (inputControls.Count == 4)
                {
                    inputControl = inputControls[index];
                }
                else
                {
                    int offset = (4 - inputControls.Count);
                    int newIndex = Math.Max(0, index - offset);
                    inputControl = inputControls[newIndex];
                }

                bindingIndex = currAction.action.GetBindingIndexForControl(inputControl);

                //bindingIndex = currAction.action.GetBindingIndexForControl(currAction.action.controls[(int)_button]);
                //if (controllerType != ButtonDisplayTypeEnum.Keyboard)
                //bindingIndex++;
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                int index = 0;
                var inputControls = currAction.action.controls;
                var inputControl = inputControls[index];
                bindingIndex = currAction.action.GetBindingIndexForControl(inputControl);
            }
        }
        else
        {
            //if (controllerType == ButtonDisplayTypeEnum.Keyboard)
            bindingIndex = currAction.action.GetBindingIndexForControl(currAction.action.controls[0]);
            //else
            //bindingIndex = currAction.action.GetBindingIndexForControl(currAction.action.controls[1]);
        }

        if (currAction.action.bindings[bindingIndex].effectivePath.Contains("<Gamepad>/"))
            return Utils.GetSpriteForBinding(currAction.action.bindings[bindingIndex], listofTmpSpriteAssets.SpriteAssets[1]);


        //if (controllerType == ButtonDisplayTypeEnum.Keyboard)
        return Utils.GetSpriteForBinding(currAction.action.bindings[bindingIndex], listofTmpSpriteAssets.SpriteAssets[0]);

        //else
        //return Utils.GetSpriteForBinding(currAction.action.bindings[bindingIndex], listofTmpSpriteAssets.SpriteAssets[(int)SaveData.CurrSaveData.ButtonDisplayType]);
    }
}
