using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("Dialogue Configuration")]
    [SerializeField] private List<DialogueNode> dialogueNodes;
    [Tooltip("会話時にプレイヤーが立つ位置の距離")]
    [SerializeField] private float playerStandOffset = 1.5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.SetInteractable(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.RemoveInteractable(this);
            }
        }
    }

    public void Interact()
    {
        Debug.Log("DialogueTrigger: Interact called");
        if (DialogueManager.Instance != null && dialogueNodes != null && dialogueNodes.Count > 0)
        {
            if (PlayerController.Instance != null)
            {
                // NPCの向き (scale.x が正なら右向き(1)、負なら左向き(-1))
                float npcFacingDir = Mathf.Sign(transform.localScale.x);

                // NPCの向いている方向に offsetting したX座標
                float targetX = transform.position.x + (npcFacingDir * playerStandOffset);
                Vector2 targetPos = new Vector2(targetX, PlayerController.Instance.transform.position.y);

                // NPCが右向きならプレイヤーは左を向き、左向きなら右を向くように設定する
                bool playerShouldFaceRight = npcFacingDir < 0;

                PlayerController.Instance.SetPositionAndFacing(targetPos, playerShouldFaceRight);
            }

            DialogueManager.Instance.StartDialogue(dialogueNodes);
        }
    }
}
