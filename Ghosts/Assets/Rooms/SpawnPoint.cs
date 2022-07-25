using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public GameObject parent;

    enum CheckDirection { DOWN, LEFT, RIGHT, UP}
    [SerializeField] CheckDirection checkDirection;

    public string roomsNeeded;

    private void Awake()
    {
        parent = transform.parent.gameObject;
    }

    public string CheckRoom()
    {
        Collider2D[] neighbors;

        if (checkDirection == CheckDirection.DOWN)
        {
            neighbors = Physics2D.OverlapCircleAll(transform.position, 0.05f);

            if(neighbors != null) {
                foreach(Collider2D neighbor in neighbors)
                {
                    if(neighbor.gameObject.CompareTag("Template") && neighbor.gameObject != parent)
                    {
                        return "D";
                    }
                    else
                    {
                        return "";
                    }
                }
                return "";
            }
            else
            {
                return "";
            }
        }else if (checkDirection == CheckDirection.LEFT)
        {
            neighbors = Physics2D.OverlapCircleAll(transform.position, 0.05f);

            if (neighbors != null)
            {
                foreach (Collider2D neighbor in neighbors)
                {
                    if (neighbor.gameObject.CompareTag("Template") && neighbor.gameObject != parent)
                    {
                        return "L";
                    }
                    else
                    {
                        return "";
                    }
                }
                return "";
            }
            else
            {
                return "";
            }
        }else if (checkDirection == CheckDirection.RIGHT)
        {
            neighbors = Physics2D.OverlapCircleAll(transform.position, 0.05f);

            if (neighbors != null)
            {
                foreach (Collider2D neighbor in neighbors)
                {
                    if (neighbor.gameObject.CompareTag("Template") && neighbor.gameObject != parent)
                    {
                        return "R";
                    }
                    else
                    {
                        return "";
                    }
                }
                return "";
            }
            else
            {
                return "";
            }
        }else if (checkDirection == CheckDirection.UP)
        {
            neighbors = Physics2D.OverlapCircleAll(transform.position, 0.05f);

            if (neighbors != null)
            {
                foreach (Collider2D neighbor in neighbors)
                {
                    if (neighbor.gameObject.CompareTag("Template") && neighbor.gameObject != parent)
                    {
                        return "U";
                    }
                    else
                    {
                        return "";
                    }
                }
                return "";
            }
            else
            {
                return "";
            }
        }
        else
        {
            return "";
        }
    }
}
