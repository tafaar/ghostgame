using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour
{
    [SerializeField] GameObject _brokenPot;
    [SerializeField] GameObject _plant;
    Rigidbody2D _rb;
    PossessableObject possScript;
    [SerializeField] GameObject potPiece;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (TryGetComponent(out PossessableObject poss))
        {
            possScript = poss;
        }
    }

    private void Update()
    {
        GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(transform.position.y);
        if (_plant != null) _plant.GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(transform.position.y) -1;

        if (possScript.targeted == true)
        {
            _rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer != LayerMask.NameToLayer("Enemies"))
        {
            if(possScript == null || possScript.targeted == false)
            if(collision.relativeVelocity.magnitude >= 10)
            {
                GameObject shatteredPot = Instantiate(_brokenPot, transform.position, Quaternion.identity, transform.parent);
                Rigidbody2D[] shatteredRbs = shatteredPot.GetComponentsInChildren<Rigidbody2D>();

                foreach(Rigidbody2D sRb in shatteredRbs)
                {
                    sRb.AddTorque(10f * sRb.mass * Random.Range(-1, 2));
                    sRb.AddForce(collision.GetContact(0).normal * _rb.velocity.magnitude * Random.Range(1f, 3f), ForceMode2D.Impulse);
                    StartCoroutine(DisableRb(sRb));
                }

                if(_plant != null)
                {
                    _plant.transform.parent = transform.parent;
                }

                StartCoroutine(DelayedDestruct());
            }
        }
    }

    IEnumerator DisableRb(Rigidbody2D rigidBody)
    {
        yield return new WaitForSeconds(3f);
        GameObject piece = Instantiate(potPiece, rigidBody.transform.position, rigidBody.transform.rotation, rigidBody.transform.parent.parent);
        piece.AddComponent<SpriteRenderer>().sprite = rigidBody.GetComponent<SpriteRenderer>().sprite;
        piece.GetComponent<SpriteRenderer>().sortingOrder = rigidBody.GetComponent<SpriteRenderer>().sortingOrder;
        piece.name = "plantPiece";
        Destroy(rigidBody.gameObject);

    }

    IEnumerator DelayedDestruct()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;

        if(possScript != null)
        {
            possScript.possessedSprite.enabled = false;
            possScript.enabled = false;
        }

        yield return new WaitForSeconds(3f);

        _plant.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder -40;

        Destroy(gameObject);
    }
}
