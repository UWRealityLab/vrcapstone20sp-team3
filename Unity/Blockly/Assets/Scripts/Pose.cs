using System.Collections.Generic;
using UnityEngine;

namespace Blockly {

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

[System.Serializable]
public enum Chirality {
  Left,
  Right
}

}
