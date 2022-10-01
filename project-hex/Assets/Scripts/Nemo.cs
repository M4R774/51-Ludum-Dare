using UnityEngine;

public class Nemo : MonoBehaviour
{
  [SerializeField] GameObject tower;
  [SerializeField] GameObject canon;
  public GameObject target;
  void Update() 
  {
    if(target != null)
    {
      tower.transform.LookAt(target.transform.position, -Vector3.up);
      tower.transform.localEulerAngles = new Vector3(0, tower.transform.localEulerAngles.y, 0);
    
      //canon.transform.LookAt(target.transform.position, -Vector3.up);
      //canon.transform.localEulerAngles = new Vector3(canon.transform.localEulerAngles.x, 0, 0);
    }
  }
}