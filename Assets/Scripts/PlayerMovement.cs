using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveTime = 0.2f;
    public bool isMoving;
    private Vector3 origPos;

    public LayerMask layerMask;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            Vector3 dir = new Vector3 (0,0,0);

            if (Input.GetAxis("Horizontal") > 0)
            {
                dir.x = 1;
            }
            else if (Input.GetAxis("Horizontal") < 0)
            {
                dir.x = -1;
            }

            if (Input.GetAxis("Vertical") > 0)
            {
                dir.y = 1;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                dir.y = -1;
            }
            Move(dir);
        }

        animator.SetBool("Running", isMoving);

        /**if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.W))
        {
            Move(new Vector3(-1, 1, 0));
        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.W))
        {
            Move(new Vector3(1, 1, 0));
        }
        else if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.S))
        {
            Move(new Vector3(-1, -1, 0));
        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.S))
        {
            Move(new Vector3(1, -1, 0));
        }

        else if (Input.GetKey(KeyCode.W))
        {
            Move(new Vector3(0, 1, 0));
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Move(new Vector3(-1, 0, 0));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Move(new Vector3(0, -1, 0));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(new Vector3(1, 0, 0));
        }**/
    }


    public void Move(Vector3 direction)
    {
        if (!isMoving && direction != Vector3.zero)
        {
            Debug.DrawRay(transform.position, direction);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1, layerMask); // tile hit
            if (hit.collider != null)
            {
                StartCoroutine(BadMoveUnit(direction));
            }
            else
            {
                StartCoroutine(MoveUnit(direction));
            }
        }
    }

    private IEnumerator MoveUnit(Vector3 direction)
    {
        isMoving = true;
        float elapsedTime = 0;
        origPos = transform.position;
        Vector3 targetPos = origPos + direction;

        while (elapsedTime < MoveTime)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / MoveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isMoving = false;
    }

    private IEnumerator BadMoveUnit(Vector3 direction)
    {
        isMoving = true;
        float elapsedTime = 0;
        origPos = transform.position;
        Vector3 targetPos = origPos + direction;
        Vector3 halfPos;

        while (elapsedTime < MoveTime / 2)
        {
            transform.position = Vector3.Lerp(origPos, targetPos, (elapsedTime / (MoveTime)));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        halfPos = transform.position;
        while (elapsedTime < MoveTime)
        {
            transform.position = Vector3.Lerp(halfPos, origPos, (elapsedTime / MoveTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = origPos;

        isMoving = false;
    }
}
