using UnityEngine;

namespace Blockly {

public class FingerTrail : MonoBehaviour {
  public Color trailColor = new Color(1, 0, 0.38f);
  public float startWidth = 0.001f;
  public float endWidth = 0.001f;
  public float trailTime = Mathf.Infinity;

  private GameObject trailObj;
  private TrailRenderer trail;
  private Transform trailTransform;
  public Transform indexFingerTip;

  private Transform playerTransform;

  public void Start() {
    trailObj = new GameObject("Finger Trail");
    trailTransform = trailObj.transform;
    trail = trailObj.AddComponent<TrailRenderer>();
    trail.time = -1f;
    trail.enabled = false;
    trailTransform.position = indexFingerTip.position;
    trail.time = trailTime;
    trail.startWidth = startWidth;
    trail.endWidth = endWidth;
    trail.numCapVertices = 1;
    trail.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
    trail.sharedMaterial.color = trailColor;

    playerTransform = gameObject.GetComponentInChildren(typeof(EditorPlayer)).transform;
  }

  public void OnUpdatePose(string poseName) {
    if (poseName == "Point") {
      trail.Clear();
      trail.enabled = true;
    } else if (trail.enabled) {
      // Debug.Log("end move gesture");
      // string dir = RecognizeDirection();
      // if (dir != null) {
      //   OnRecognizeDirection.Invoke(dir);
      // }
      trail.enabled = false;
    }
  }

  public void Update() {
    trailTransform.position = indexFingerTip.position;
  }

  public string RecognizeDirection() {
    Vector3[] positions = new Vector3[trail.positionCount];
    trail.GetPositions(positions);
    if (positions.Length < 2) {
      Debug.Log("ignoring direction gesture with too few positions");
      return null;
    }
    for (int i = 1; i < positions.Length - 1; i++) {
      Vector3 prev = (positions[i] - positions[i-1]).normalized;
      Vector3 curr = (positions[i+1] - positions[i]).normalized;
      float alignment = Vector3.Dot(prev, curr);
      if (alignment < 0.7) {
        Debug.Log("ignoring loopy direction gesture");
        return null;
      }
    }
    Vector3 start = playerTransform.InverseTransformPoint(positions[0]);
    Vector3 end = playerTransform.InverseTransformPoint(positions[positions.Length-1]);
    Vector3 dir = (end - start).normalized;
    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) && Mathf.Abs(dir.x) > Mathf.Abs(dir.z)) {
      // x component is the largest
      if (dir.x > 0f) {
        return "Right";
      } else {
        return "Left";
      }
    } else if (Mathf.Abs(dir.y) > Mathf.Abs(dir.x) && Mathf.Abs(dir.y) > Mathf.Abs(dir.z)) {
      // y component is the largest
      if (dir.y > 0f) {
        return "Up";
      } else {
        return "Down";
      }
    } else {
      // z component is the largest
      if (dir.z > 0f) {
        return "Forward";
      } else {
        return "Backward";
      }
    }
  }
}

}
