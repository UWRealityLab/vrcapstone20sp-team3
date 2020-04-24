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
  public TextMesh display;

  // Start is called before the first frame update
  void Start() {
    trailObj = new GameObject("Mouse Trail");
    trailTransform = trailObj.transform;
    trail = trailObj.AddComponent<TrailRenderer>();
    trail.time = -1f;

    // trailTransform.position = indexFingerTip.position;

    // trail.time = trailTime;
    // trail.startWidth = startWidth;
    // trail.endWidth = endWidth;
    // trail.numCapVertices = 1;
    // trail.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
    // trail.sharedMaterial.color = trailColor;
  }

  // Update is called once per frame
  void Update() {
    if (display.text == "Point") {
      trail.enabled = true;
    } else {
      if (trail.enabled) {
        Debug.Log("end move gesture");
        Vector3[] positions = new Vector3[trail.positionCount];
        trail.GetPositions(positions);
        RecognizeGesture(positions);
      }
      trail.enabled = false;
      trail.Clear();
    }
    trailTransform.position = indexFingerTip.position;

    // TODO move these back into init
    trail.time = trailTime;
    trail.startWidth = startWidth;
    trail.endWidth = endWidth;
    trail.numCapVertices = 1;
    trail.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
    trail.sharedMaterial.color = trailColor;
  }

  void RecognizeGesture(Vector3[] positions) {
    Debug.Log("[Positions]");
    foreach (var pos in positions) {
      Debug.Log($"  {pos}");
    }
  }
}

}
