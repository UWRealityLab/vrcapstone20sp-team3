using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RotationBoundsChecker : MonoBehaviour {
  [NotNull]
  public Transform[] referenceTransforms;
  [NotNull]
  public Transform candidateTransform;

  // if the dot product between some reference bone rotation vectors is lower
  // than this constant, then those bone rotations aren't very strict for the
  // pose they correspond to, and we always return zero error.
  private static float MIN_ALLOWED_REF_DOT = 0.3f;
  // if the candidate forward vector is too misaligned with the reference area,
  // then the calculated barycentric coordinates won't be well-defined
  private static float MIN_ALLOWED_CAND_DOT = 0.3f;
  private static float MAX_ERROR = 2f;

  private Mesh validAreaTriMesh;
  private Vector3[] validAreaVerts;
  private static int[] validAreaTris = new int[] { 0, 1, 2, 2, 1, 0 };
  private static float validAreaTriDistCoeff = 10f;

  public void Awake() {
    Debug.Assert(referenceTransforms.Length == 3);
    validAreaTriMesh = GetComponent<MeshFilter>().mesh;
  }

  public void Update() {
    float error = CalcError();
    Debug.Log($"error: {error}");
    Color candColor = Vector4.Lerp(Color.green, Color.red, error / MAX_ERROR);
    candidateTransform.gameObject.GetComponentInChildren<Renderer>().material.color = candColor;

  }

  // TODO should take in reference poses and a candidate pose
  private float CalcError() {
    GetComponent<MeshRenderer>().enabled = true;
    for (int i = 0; i < referenceTransforms.Length; i++) {
      for (int j = 0; j < referenceTransforms.Length; j++) {
        if (i >= j) continue;
        if (Vector3.Dot(referenceTransforms[i].transform.forward, referenceTransforms[j].transform.forward) < MIN_ALLOWED_REF_DOT) {
          GetComponent<MeshRenderer>().enabled = false;
          return 0f;
        }
      }
    }

    validAreaVerts = new Vector3[] {
      referenceTransforms[0].transform.forward * validAreaTriDistCoeff,
      referenceTransforms[1].transform.forward * validAreaTriDistCoeff,
      referenceTransforms[2].transform.forward * validAreaTriDistCoeff
    };
    validAreaTriMesh.Clear();
    validAreaTriMesh.vertices = validAreaVerts;
    validAreaTriMesh.triangles = validAreaTris;

    Vector3 bary = CalcBarycentric(validAreaVerts, candidateTransform.forward);
    float[] baryCoords = { bary.x, bary.y, bary.z };
    float minBaryCoord = baryCoords.Min();

    Vector3 refAvg = Vector3.zero;
    for (int i = 0; i < referenceTransforms.Length; i++) {
      refAvg += referenceTransforms[i].forward;
    }
    refAvg.Normalize();

    float candDotRefAvg = Vector3.Dot(candidateTransform.forward, refAvg);

    if (candDotRefAvg > MIN_ALLOWED_CAND_DOT && minBaryCoord >= 0f) {
      // we're inside the triangle, so no error
      return 0f;
    } else {
      // we're outside the triangle, so use the dot prod to calc an error term
      return 1f - candDotRefAvg;
    }
  }

  private Vector3 CalcBarycentric(Vector3[] triVerts, Vector3 candVert) {
    Vector3 v0 = triVerts[1].normalized - triVerts[0].normalized;
    Vector3 v1 = triVerts[2].normalized - triVerts[0].normalized;
    Vector3 v2 = candVert.normalized - triVerts[0].normalized;
    float d00 = Vector3.Dot(v0, v0);
    float d01 = Vector3.Dot(v0, v1);
    float d11 = Vector3.Dot(v1, v1);
    float d20 = Vector3.Dot(v2, v0);
    float d21 = Vector3.Dot(v2, v1);
    float denom = d00 * d11 - d01 * d01;
    float v = (d11 * d20 - d01 * d21) / denom;
    float w = (d00 * d21 - d01 * d20) / denom;
    float u = 1.0f - v - w;
    return new Vector3(u, v, w);
  }
}
