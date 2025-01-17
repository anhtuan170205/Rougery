using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemyObject : CellObject
{
    public int hitPoints = 2;
    public int damage = 2;
    private int currentHitPoints;
    private Vector2Int enemyCell;
    private Animator animator;
    private void Awake()
    {
        GameManager.instance.turnManager.OnTick += TurnHappen;
        animator = GetComponent<Animator>();
    }
    private void OnDestroy() 
    {
        GameManager.instance.turnManager.OnTick -= TurnHappen;
    }
    public override void Init(Vector2Int cellPosition)
    {
        base.Init(cellPosition);
        currentHitPoints = hitPoints;
        enemyCell = cellPosition;
    }
    public override bool PlayerWantToEnter()
    {
        currentHitPoints--;
        AudioPlayer.instance.PlayDigClip(transform.position);
        if (currentHitPoints <= 0)
        {
            Destroy(gameObject);
        }
        return false;
    }
    bool MoveTo(Vector2Int cellPosition)
    {
        var board = GameManager.instance.boardManager;
        var targetCell = board.GetCellData(cellPosition);
        if (targetCell == null || !targetCell.Passable || targetCell.containedObject != null)
        {
            return false;
        }
        if (cellPosition == GameManager.instance.player.GetCellPosition())
        {
            return false;
        }
        var currentCell = board.GetCellData(enemyCell);
        currentCell.containedObject = null;

        targetCell.containedObject = this;
        enemyCell = cellPosition;
        StartCoroutine(SmoothMove(board.CellToWorld(cellPosition)));
        return true;
    }
    void TurnHappen()
    {
        var playerCell = GameManager.instance.player.GetCellPosition();
        int xDist = playerCell.x - enemyCell.x;
        int yDist = playerCell.y - enemyCell.y;

        int absXDist = Mathf.Abs(xDist);
        int absYDist = Mathf.Abs(yDist);

        if ((xDist == 0 && yDist == 1) || (xDist == 1 && yDist == 0) || (xDist == 0 && yDist == -1) || (xDist == -1 && yDist == 0))
        {
            TriggerAttackAnimation();
            GameManager.instance.player.TriggerDigAnimation();
            GameManager.instance.ChangeFood(-damage);
        }
        else 
        {
            if (absXDist > absYDist)
            {
                if (!TryMoveInX(xDist))
                {
                    TryMoveInY(yDist);
                }
            }
            else
            {
                if (!TryMoveInY(yDist))
                {
                    TryMoveInX(xDist);
                }
            }
        }
    }
    bool TryMoveInX(int xDist)
    {
        if (xDist > 0)
        {
            return MoveTo(enemyCell + Vector2Int.right);
        }
        else if (xDist < 0)
        {
            return MoveTo(enemyCell + Vector2Int.left);
        }
        return false;
    }
    bool TryMoveInY(int yDist)
    {
        if (yDist > 0)
        {
            return MoveTo(enemyCell + Vector2Int.up);
        }
        else if (yDist < 0)
        {
            return MoveTo(enemyCell + Vector2Int.down);
        }
        return false;
    }
    IEnumerator SmoothMove(Vector3 targetPosition)
    {
        Vector3 startingPosition = transform.position;
        float elapsedTime = 0f;
        float duration = 0.2f; // Duration of the movement
        FlipSprite(transform.position.x - targetPosition.x);
        animator.SetBool("isMoving", true);

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait until the next frame
        }
        animator.SetBool("isMoving", false);
        transform.position = targetPosition; // Ensure final position is exact
    }
    void TriggerAttackAnimation()
    {
        animator.SetTrigger("isAttacking");
        StartCoroutine(ResetAttackAnimation());
    }
    IEnumerator ResetAttackAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        animator.ResetTrigger("isAttacking");
    }
    void FlipSprite(float horizontalInput)
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
}
