using UnityEngine;

public class CameraFollow2D : MonoBehaviour {
  public Transform target; public float smooth=8f;
  public Vector2 minBounds = new(-10,-6), maxBounds=new(10,6);
  void LateUpdate(){
    if(!target) return;
    var pos = Vector3.Lerp(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), smooth*Time.deltaTime);
    pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
    pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
    transform.position = pos;
  }
}