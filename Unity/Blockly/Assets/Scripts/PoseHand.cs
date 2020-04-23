using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

// needs to be placed on a game object that has an OVRCustomSkeleton component.
public class PoseHand : MonoBehaviour {
  [System.Serializable]
  public struct Finger {
    public List<Quaternion> segmentRotations;
  }

  [System.Serializable]
  public struct Pose {
    public string name;
    public List<Finger> fingers;
    public Quaternion wristRotation;
  }

  public OVRCustomSkeleton skeleton;

  public List<Pose> poses;

  public void Awake() {
    skeleton = GetComponent<OVRCustomSkeleton>();
    if (skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandLeft) {
      Debug.Log("left hand");
    } else if (skeleton.GetSkeletonType() == OVRSkeleton.SkeletonType.HandRight) {
      Debug.Log("right hand");
    }
  }

  public void Update() {
  }
}

}
