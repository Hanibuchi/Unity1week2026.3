using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour, IInteractable
{
    [Header("Dialogue Configuration")]
    [SerializeField] private List<DialogueNode> dialogueNodes;
    [Tooltip("会話時にプレイヤーの位置を自動で調整するかどうか")]
    [SerializeField] private bool adjustPlayerPosition = true;
    [Tooltip("会話時にプレイヤーが立つ位置の距離")]
    [SerializeField] private float playerStandOffset = 1.5f;
    [Tooltip("プレイヤーが範囲に入った時に自動で会話を開始するかどうか")]
    [SerializeField] private bool autoStart = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (PlayerController.Instance != null)
            {
                PlayerController.Instance.SetInteractable(this);
            }

            if (autoStart)
            {
                Interact();
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
                if (adjustPlayerPosition)
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
                
                // カメラにNPCの親または自身と、プレイヤーを両方映す
                if (CameraController.Instance != null)
                {
                    Transform npcTarget = transform.parent != null ? transform.parent : transform;
                    CameraController.Instance.SetMultipleTargets(npcTarget, PlayerController.Instance.transform);
                }
            }

            DialogueManager.Instance.StartDialogue(dialogueNodes, () =>
            {
                // 会話終了時にカメラターゲットをプレイヤーに戻す
                if (CameraController.Instance != null && PlayerController.Instance != null)
                {
                    CameraController.Instance.SetTrackingTarget(PlayerController.Instance.transform);
                }
            });
        }
    }
}
