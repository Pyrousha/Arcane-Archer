using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : Singleton<InputHandler>
{
    private enum ButtonIndices
    {
        Jump = 0,
        Shoot = 1,
        Explode = 2,
        Slam = 3,
        Restart = 4
    }

    public Vector2 MoveXZ
    {
        get;
        private set;
    }
    public Vector2 Look
    {
        get;
        private set;
    }
    public Vector2 Scroll
    {
        get;
        private set;
    }
    public ButtonState Jump
    {
        get { return buttons[(int)ButtonIndices.Jump]; }
    }
    public ButtonState Shoot
    {
        get { return buttons[(int)ButtonIndices.Shoot]; }
    }
    public ButtonState Explode
    {
        get { return buttons[(int)ButtonIndices.Explode]; }
    }
    public ButtonState Slam
    {
        get { return buttons[(int)ButtonIndices.Slam]; }
    }
    public ButtonState Restart
    {
        get { return buttons[(int)ButtonIndices.Restart]; }
    }

    private int buttonCount = 5;
    [SerializeField] private short bufferFrames = 5;
    [SerializeField] private bool bufferEnabled = false;
    private short IDSRC = 0;
    private ButtonState[] buttons;
    private Queue<Dictionary<short, short>> inputBuffer = new Queue<Dictionary<short, short>>();
    private Dictionary<short, short> currentFrame;

    public void Start()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        buttons = new ButtonState[buttonCount];
        for (int i = 0; i < buttonCount; i++)
            buttons[i].Init(ref IDSRC, this);
    }

    private void FixedUpdate()
    {
        for (int i = 0; i < buttonCount; i++)
            buttons[i].Reset();

        if (bufferEnabled)
        {
            UpdateBuffer();
        }
    }

    //Input functions
    public void CTX_MoveXZ(InputAction.CallbackContext _ctx)
    {
        MoveXZ = _ctx.ReadValue<Vector2>();
    }
    public void CTX_Look(InputAction.CallbackContext _ctx)
    {
        Look = _ctx.ReadValue<Vector2>();
    }
    public void CTX_Scroll(InputAction.CallbackContext _ctx)
    {
        Scroll = _ctx.ReadValue<Vector2>();

        PlayerController.turnSpeedX = Mathf.Max(1f, PlayerController.turnSpeedX + Scroll.y * (0.5f / 120f));
        PlayerPrefs.SetFloat("Sens", PlayerController.turnSpeedX);
        SensitivityText.Instance.UpdateText();
    }

    //Buttons
    public void CTX_Jump(InputAction.CallbackContext _ctx)
    {
        buttons[(int)ButtonIndices.Jump].Set(_ctx);
    }
    public void CTX_Shoot(InputAction.CallbackContext _ctx)
    {
        buttons[(int)ButtonIndices.Shoot].Set(_ctx);
    }
    public void CTX_Explode(InputAction.CallbackContext _ctx)
    {
        buttons[(int)ButtonIndices.Explode].Set(_ctx);
    }
    public void CTX_Slam(InputAction.CallbackContext _ctx)
    {
        buttons[(int)ButtonIndices.Slam].Set(_ctx);
    }
    public void CTX_Restart(InputAction.CallbackContext _ctx)
    {
        buttons[(int)ButtonIndices.Restart].Set(_ctx);
    }


    //Buffer functions
    public void FlushBuffer()
    {
        inputBuffer.Clear();
    }

    public void UpdateBuffer()
    {
        if (inputBuffer.Count >= bufferFrames)
            inputBuffer.Dequeue();
        currentFrame = new Dictionary<short, short>();
        inputBuffer.Enqueue(currentFrame);
    }

    public void PrintBuffer()
    {
        string bufferData = $"InputBuffer: count-{inputBuffer.Count}";
        foreach (var frame in inputBuffer)
            if (frame.Count > 0)
                bufferData += $"\n{frame.Count}";
        Debug.Log(bufferData);
    }

    public struct ButtonState
    {
        private short id;
        private static short STATE_PRESSED = 0,
                                STATE_RELEASED = 1;
        private InputHandler handler;
        private bool firstFrame;
        public bool Holding
        {
            get;
            private set;
        }
        public bool Down
        {
            get
            {
                if (handler.bufferEnabled && handler.inputBuffer != null)
                {
                    foreach (var frame in handler.inputBuffer)
                    {
                        if (frame.ContainsKey(id) && frame[id] == STATE_PRESSED)
                        {
                            return frame.Remove(id);
                        }
                    }
                    return false;
                }
                return Holding && firstFrame;
            }
        }

        public bool Up
        {
            get
            {
                if (handler.bufferEnabled && handler.inputBuffer != null)
                {
                    foreach (var frame in handler.inputBuffer)
                    {
                        if (frame.ContainsKey(id) && frame[id] == STATE_RELEASED)
                        {
                            return frame.Remove(id);
                        }
                    }
                    return false;
                }
                return !Holding && firstFrame;
            }
        }

        public void Set(InputAction.CallbackContext ctx)
        {
            Holding = !ctx.canceled;
            firstFrame = true;

            if (handler.bufferEnabled && handler.currentFrame != null)
            {
                handler.currentFrame.TryAdd(id, Holding ? STATE_PRESSED : STATE_RELEASED);
            }
        }

        public void Reset()
        {
            firstFrame = false;
        }

        public void Init(ref short IDSRC, InputHandler handler)
        {
            id = IDSRC++;
            this.handler = handler;
        }
    }
}