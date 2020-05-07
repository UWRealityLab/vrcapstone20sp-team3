using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OVRSkeleton;

namespace Blockly {

public interface IPlayer {
  Transform GetTransform();
  Transform GetLeftIndexTipTransform();
  Transform GetRightIndexTipTransform();
  SkeletonPoseData GetCurrLeftPose();
  SkeletonPoseData GetCurrRightPose();
}

public class PlayerManager : MonoBehaviour {
  private IPlayer chosenPlayer;

  public void Awake() {
    EditorPlayer editorPlayer = GetComponentInChildren<EditorPlayer>();
    QuestPlayer questPlayer = GetComponentInChildren<QuestPlayer>();

    #if UNITY_EDITOR
      Debug.Log("chose editor player");
      editorPlayer.gameObject.SetActive(true);
      questPlayer.gameObject.SetActive(false);
      chosenPlayer = editorPlayer;
    #else
      Debug.Log("chose Quest player");
      editorPlayer.gameObject.SetActive(false);
      questPlayer.gameObject.SetActive(true);
      chosenPlayer = questPlayer;
    #endif
  }

  public IPlayer GetPlayer() {
    return chosenPlayer;
  }
}

}
