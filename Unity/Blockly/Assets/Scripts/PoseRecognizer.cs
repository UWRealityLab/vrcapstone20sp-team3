using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

public class PoseRecognizer : MonoBehaviour {
  public StringEvent OnUpdateLeftPose;
  public StringEvent OnUpdateRightPose;

  public float threshold = 43.8f;

  public List<Pose> leftPoses;
  public List<Pose> rightPoses;

  private IPlayer player;

  private Pose currLeftPose;
  public Pose CurrLeftPose { get => currLeftPose; }

  private Pose currRightPose;
  public Pose CurrRightPose { get => currRightPose; }

  public void Start() {
    player = GetComponent<PlayerManager>().GetPlayer();
  }

  public void Update() {
    Pose leftPose = Recognize(player.GetCurrLeftPose(), leftPoses);
    Pose rightPose = Recognize(player.GetCurrRightPose(), rightPoses);
    if (leftPose.name != currLeftPose.name) {
      OnUpdateLeftPose.Invoke(leftPose.name);
    }
    if (rightPose.name != currRightPose.name) {
      OnUpdateRightPose.Invoke(rightPose.name);
    }
    currLeftPose = leftPose;
    currRightPose = rightPose;
  }

  public Pose Recognize(Pose pose, List<Pose> validPoses) {
    float bestError = Mathf.Infinity;
    Pose bestCandidate = new Pose();
    bestCandidate.name = "No Pose";

    // For each pose
    foreach (var validPose in validPoses) {
      Debug.Assert(pose.fingers.Count == validPose.fingers.Count);
      float error = 0f;
      bool discardPose = false;
      for (int i = 0; i < pose.fingers.Count; i++) {
        Finger finger = pose.fingers[i];
        Finger validFinger = validPose.fingers[i];
        for (int j = 0; j < finger.segmentRotations.Count; j++) {
          float currError = Quaternion.Angle(finger.segmentRotations[j], validFinger.segmentRotations[j]);
          if (currError > threshold) {
            discardPose = true;
            break;
          }
          error += currError;
        }
      }

      if (discardPose) {
        continue;
      }
      if (error < bestError) {
        bestCandidate = validPose;
        bestError = error;
      }
    }
    return bestCandidate;
  }
}

}
