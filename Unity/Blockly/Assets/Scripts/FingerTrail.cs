using UnityEngine;
using UnityEngine.Rendering;

namespace Blockly {

public class FingerTrail : MonoBehaviour {
  public Color trailColor = new Color(1, 0, 0.38f);
  public float startWidth = 0.001f;
  public float endWidth = 0.001f;
  public float trailTime = Mathf.Infinity;

  private GameObject trailObj;
  private TrailRenderer trail;
  private Transform trailTransform;

  private Transform playerTransform;
  private Transform indexFingerTip;

  public void Awake() {
    trailObj = new GameObject("Finger Trail");
    trailTransform = trailObj.transform;
    trail = trailObj.AddComponent<TrailRenderer>();
    trail.time = -1f;
    trail.enabled = false;

    trail.time = trailTime;
    trail.startWidth = startWidth;
    trail.endWidth = endWidth;
    trail.numCapVertices = 1;
    trail.minVertexDistance = 0.05f;
    trail.shadowCastingMode = ShadowCastingMode.Off;
    trail.receiveShadows = false;
    trail.sharedMaterial = new Material(Shader.Find("Unlit/Color"));
    trail.sharedMaterial.color = trailColor;
  }

  public void Start() {
    IPlayer player = GetComponent<PlayerManager>().GetPlayer();
    playerTransform = player.GetTransform();
    indexFingerTip = player.GetLeftIndexTipTransform();

    trailTransform.position = indexFingerTip.position;
  }

  public void OnUpdatePose(string poseName) {
    if (poseName == "Point") {
      trail.Clear();
      trail.enabled = true;
    } else if (trail.enabled) {
      trail.enabled = false;
    }
  }

  public void Update() {
    trailTransform.position = indexFingerTip.position;
  }

  public Vector3[] GetPositions() {
    Vector3[] positions = new Vector3[trail.positionCount];
    trail.GetPositions(positions);
    return positions;
  }
}

}
