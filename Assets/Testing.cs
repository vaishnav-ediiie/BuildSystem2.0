using CustomBuildSystem;
using CustomGridSystem;
using UnityEngine;

[ExecuteAlways]
public class Testing : MonoBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private Transform pointer;
    
    void Start()
    {
        
    }

    void Update()
    {
        Debug.Log((pointer.position - center.position).GetDirection());
    }
}
