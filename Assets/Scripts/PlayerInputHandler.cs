using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerInpHandler : MonoBehaviour
{
    private GameInputActions inpAct;

    private void Awake()
    {
        inpAct = new GameInputActions();
    }

    private void OnEnable()
    {
        inpAct.Player.Enable();
        inpAct.Player.Move.performed += OnMovePerf;
        inpAct.Player.Move.canceled += OnMoveCancel;
        inpAct.Player.SwipeMouse.performed += OnSwipeMouse;
        inpAct.Player.SwipeTouch.performed += OnSwipeTouch;
    }

    private void OnDisable()
    {
        inpAct.Player.Move.performed -= OnMovePerf;
        inpAct.Player.Move.canceled -= OnMoveCancel;
        inpAct.Player.SwipeMouse.performed -= OnSwipeMouse;
        inpAct.Player.SwipeTouch.performed -= OnSwipeTouch;
        inpAct.Player.Disable();
    }

    private void OnMovePerf(InputAction.CallbackContext ctx)
    {
        Vector2 inp = ctx.ReadValue<Vector2>();
        Debug.Log($"[Inp] Mv: {inp}");
        float ax = Mathf.Abs(inp.x);
        float ay = Mathf.Abs(inp.y);
        if (ax > ay)
        {
            if (inp.x > 0)
            {
                Debug.Log("[Inp] MR");
                GameField.Instance.MoveRight();
            }
            else
            {
                Debug.Log("[Inp] ML");
                GameField.Instance.MoveLeft();
            }
        }
        else
        {
            if (inp.y > 0)
            {
                Debug.Log("[Inp] MU");
                GameField.Instance.MoveUp();
            }
            else
            {
                Debug.Log("[Inp] MD");
                GameField.Instance.MoveDown();
            }
        }
    }

    private void OnMoveCancel(InputAction.CallbackContext ctx)
    {
        Debug.Log("[Inp] Mv cancel");
    }

    private float swTh = 100f;

    private void OnSwipeMouse(InputAction.CallbackContext ctx)
    {
        Vector2 del = ctx.ReadValue<Vector2>();
        Debug.Log($"[Inp] MSwp: {del}");
        if (del.magnitude < swTh)
        {
            return;
        }
        if (Mathf.Abs(del.x) > Mathf.Abs(del.y))
        {
            if (del.x > 0)
            {
                Debug.Log("[Inp] MSwipe R");
                GameField.Instance.MoveRight();
            }
            else
            {
                Debug.Log("[Inp] MSwipe L");
                GameField.Instance.MoveLeft();
            }
        }
        else
        {
            if (del.y > 0)
            {
                Debug.Log("[Inp] MSwipe U");
                GameField.Instance.MoveUp();
            }
            else
            {
                Debug.Log("[Inp] MSwipe D");
                GameField.Instance.MoveDown();
            }
        }
    }

    private void OnSwipeTouch(InputAction.CallbackContext ctx)
    {
        Vector2 del = ctx.ReadValue<Vector2>();
        Debug.Log($"[Inp] TSwipe: {del}");
        if (del.magnitude < swTh)
        {
            return;
        }
        if (Mathf.Abs(del.x) > Mathf.Abs(del.y))
        {
            if (del.x > 0)
            {
                Debug.Log("[Inp] TSwipe R");
                GameField.Instance.MoveRight();
            }
            else
            {
                Debug.Log("[Inp] TSwipe L");
                GameField.Instance.MoveLeft();
            }
        }
        else
        {
            if (del.y > 0)
            {
                Debug.Log("[Inp] TSwipe U");
                GameField.Instance.MoveUp();
            }
            else
            {
                Debug.Log("[Inp] TSwipe D");
                GameField.Instance.MoveDown();
            }
        }
    }

    public void HandleMove(Vector2 inp)
    {
        float ax = Mathf.Abs(inp.x);
        float ay = Mathf.Abs(inp.y);
        if (ax > ay)
        {
            if (inp.x > 0)
            {
                Debug.Log("[Inp] MR");
                GameField.Instance.MoveRight();
            }
            else
            {
                Debug.Log("[Inp] ML");
                GameField.Instance.MoveLeft();
            }
        }
        else
        {
            if (inp.y > 0)
            {
                Debug.Log("[Inp] MU");
                GameField.Instance.MoveUp();
            }
            else
            {
                Debug.Log("[Inp] MD");
                GameField.Instance.MoveDown();
            }
        }
    }

}
