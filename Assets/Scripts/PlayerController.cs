using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BoardManager boardManager;
    private Animator animator;
    private Vector2Int cellPosition;
    public float moveSpeed = 5f;
    private bool isMoving;
    private Vector3 moveTarget;
    private bool isGameOver;
    private void Awake() 
    {
        animator = GetComponent<Animator>();
    }
    public void Spawn(BoardManager boardManagerInput, Vector2Int cellPositionInput)
    {
        boardManager = boardManagerInput;
        cellPosition = cellPositionInput;
        transform.position = boardManager.CellToWorld(cellPosition);
    }
    public void Init()
    {
        isMoving = false;
    }
    public void MoveTo(Vector2Int cellPositionInput, bool animate = true)
    {
        cellPosition = cellPositionInput;
        isMoving = true;
        moveTarget = boardManager.CellToWorld(cellPosition);
    }
    private void Update() 
    {
        if (isGameOver)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                isGameOver = false;
                GameManager.instance.boardManager.Clean();
                GameManager.instance.StartNewGame();
            }
            return;
        }
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
            if (transform.position == moveTarget)
            {
                isMoving = false;
                var cellData = boardManager.GetCellData(cellPosition);
                if (cellData.containedObject != null)
                {
                    cellData.containedObject.PlayerEntered();
                }
            }
        }
        Vector2Int newCellTarget = cellPosition;
        bool hasMoved = false;
        if (Keyboard.current.wKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            hasMoved = true;
            FlipSprite(-1);
        }
        else if (Keyboard.current.dKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            hasMoved = true;
            FlipSprite(1);
        }
        if (hasMoved)
        {
            BoardManager.CellData cellData = boardManager.GetCellData(newCellTarget);
            if (cellData != null && cellData.Passable)
            {
                GameManager.instance.turnManager.Tick();
                if (cellData.containedObject == null)
                {
                    MoveTo(newCellTarget);
                }
                else 
                {
                    if (cellData.containedObject.PlayerWantToEnter())
                    {
                        MoveTo(newCellTarget);
                    }
                }
            }
        }
    }
    public void TriggerDigAnimation()
    {
        animator.SetTrigger("isDigging");
        StartCoroutine(ResetDigAnimation());
    }
    private IEnumerator ResetDigAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger("isDigging");
    }
    private void FlipSprite(float horizontalInput)
    {
        if (horizontalInput < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (horizontalInput > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }
    public Vector2Int GetCellPosition()
    {
        return cellPosition;
    }
    public void GameOver()
    {
        isGameOver = true;
    }
}
