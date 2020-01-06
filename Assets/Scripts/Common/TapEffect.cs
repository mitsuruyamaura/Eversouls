using UnityEngine;

public class TapEffect : MonoBehaviour
{
    public ParticleSystem tapEffect;
    public Camera _camera;

    void Awake() {
        DontDestroyOnLoad(gameObject);    
    }

    void Update(){
        if (Input.GetMouseButtonDown(0)) {
            Vector3 pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 10);
            tapEffect.transform.position = pos;
            tapEffect.Emit(1);
        }
    }
}
