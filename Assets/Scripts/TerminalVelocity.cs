using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalVelocity : MonoBehaviour
{
    [SerializeField]
    private float terminalVelocity;

    private Rigidbody _rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = new Vector3(
            _rigidbody.velocity.x,
            Mathf.Max(terminalVelocity, _rigidbody.velocity.y),
            _rigidbody.velocity.z
            );
    }
}
