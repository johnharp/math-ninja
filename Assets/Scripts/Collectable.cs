using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private AudioClip collectSound = null;

    [SerializeField]
    private Vector3 collectVelocity = new Vector3(0, 3, 0);

    [SerializeField]
    private float collectDestroyTime = 1.5f;

    private Rigidbody _rigidbody = null;
    private bool isCollected = false;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isCollected == false)
        {
            isCollected = true;
            AudioSource.PlayClipAtPoint(collectSound, transform.position, 0.5f);
            _rigidbody.velocity = collectVelocity;

            _rigidbody.useGravity = true;
            Destroy(this.gameObject, collectDestroyTime);
            // Destroy(this.gameObject);
        }
    }
}