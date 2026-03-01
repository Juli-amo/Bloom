using UnityEngine; using TMPro;
public class FloatingText : MonoBehaviour {
  public TextMeshPro text; public float upSpeed=1.2f; public float life=0.9f;
  void Awake(){ if(!text) text = GetComponent<TextMeshPro>(); }
  void Update(){ transform.position += Vector3.up*upSpeed*Time.deltaTime;
                 life -= Time.deltaTime;
                 if(text) text.alpha = Mathf.InverseLerp(0f,0.3f,life);
                 if(life<=0f) Destroy(gameObject); }
  public void Set(string msg){ if(text) text.text = msg; }
}

