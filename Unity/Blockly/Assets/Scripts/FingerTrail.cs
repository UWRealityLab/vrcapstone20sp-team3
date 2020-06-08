using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Blockly {

public class FingerTrail : MonoBehaviour {
  public Color trailColor = new Color(1, 0, 0.38f);
  public float startWidth = 0.001f;
  public float endWidth = 0.001f;
  public float trailTime = Mathf.Infinity;
  public Text loopIterationText;

  public OVRSkeleton.SkeletonType skeletonType;

  private GameObject trailObj;
  private TrailRenderer trail;
  private Transform trailTransform;

  private Transform playerTransform;
  private Transform indexFingerTip;

  private Vector3 startPos;
  private int lastCircleComplete;
  private int numLoopIterations;

  public void Awake() {
    #if UNITY_EDITOR
      // finger trails don't work in the editor
      Debug.Log("destroying finger trail");
      Destroy(this);
      return;
    #endif

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
    BlocklyPlayer player = GetComponent<BlocklyPlayer>();
    playerTransform = player.GetTransform();
    if (skeletonType == OVRSkeleton.SkeletonType.HandLeft) {
      Debug.Log("left finger trail start");
      indexFingerTip = player.GetLeftIndexTipTransform();
    } else if (skeletonType == OVRSkeleton.SkeletonType.HandRight) {
      Debug.Log("right finger trail start");
      indexFingerTip = player.GetRightIndexTipTransform();
    } else {
      Debug.Assert(false);
    }
    Debug.Log(indexFingerTip);
    Debug.Assert(indexFingerTip != null);

    trailTransform.position = indexFingerTip.position;
  }

  public void OnUpdatePose(string poseName) {
    if (poseName == "Point") {
      trail.Clear();
      trail.enabled = true;
      startPos = indexFingerTip.position;
      lastCircleComplete = 0;
      numLoopIterations = 0;
    } else if (trail.enabled) {
      trail.enabled = false;
    }
    loopIterationText.text = "";
  }

  public void Update() {
    trailTransform.position = indexFingerTip.position;
    if (trail.enabled) {
      float distToStart = Vector3.Distance(trailTransform.position, startPos);
      // Assume it takes at least 5 points to make a circle
      if (distToStart < 0.1 && trail.positionCount - lastCircleComplete > 5) {
        lastCircleComplete = trail.positionCount;
        numLoopIterations++;
        loopIterationText.text = "Loop Iterations: " + numLoopIterations;
      }
    }
  }

  public Vector3[] GetPositions() {
    Vector3[] positions = new Vector3[trail.positionCount];
    trail.GetPositions(positions);
    return positions;
  }

  public int GetNumLoopIterations() {
    return numLoopIterations;
  }
}

}
