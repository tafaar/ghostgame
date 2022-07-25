using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UI;

public class ArrayTest : MonoBehaviour
{
    public TextMeshProUGUI text;

    public TextMeshProUGUI beginning;
    public TextMeshProUGUI end;

    public int desiredRooms = 5;

    //help me pls
    public int help;

    bool print = false;

    int[,] a = {

        {0, 0, 0, 0, 0, 0 , 0 },
        {0, 0, 0, 0, 0, 0 , 0 },
        {0, 0, 0, 0, 0, 0 , 0 },
        {0, 0, 0, 0, 0, 0 , 0 },
        {0, 0, 0, 0, 0, 0 , 0 },
        {0, 0, 0, 0, 0, 0 , 0 },
        {0, 0, 0, 0, 0, 0 , 0 },

    };

    int[,] aStored;

    void Start()
    {
        aStored = a;
        StartCoroutine(Iterate(a));
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            a = aStored;
            StopAllCoroutines();
            beginning.text = "";
            StartCoroutine(Iterate(a));
        }
    }

    IEnumerator Iterate(int[,] a)
    {
        Debug.Log("Beginning iteration");

        int count = 0;

        foreach(int number in a)
        {
            count += 1;
            beginning.text += number;
            if(count == a.GetLength(0))
            {
                beginning.text += "\n";
                count = 0;
            }
        }

        int roomCheck = desiredRooms;

        while (roomCheck > 0)
        {
            Debug.Log("Room requirement not met, running an iteration");

            roomCheck = desiredRooms;

            int i = 0;
            int j = 0;

            for (i = 0; i < a.GetLength(0); i++)
            {
                for (j = 0; j < a.GetLength(1); j++)
                {
                    Debug.Log("Checking cell " + a[i, j]);
                    yield return new WaitForEndOfFrame();

                    if (a[i,j] == 1)
                    {
                        roomCheck -= 1;
                        if(roomCheck <= 0)
                        {
                            break;
                        }
                    }

                    if (a[i, j] == 2)
                    {

                        while (print == false)
                        {

                            int dir = UnityEngine.Random.Range(0, 4);

                            Debug.Log("Direction: " + dir);

                            //Right
                            if (dir == 0 && print == false)
                            {
                                if (j + 1 <= a.GetLength(0))
                                {
                                    if (a[i, j + 1] == 0 || a[i, j + 1] == 1)
                                    {
                                        a[i, j + 1] = 3;
                                    }

                                    Debug.Log("WTFFF " + (j + 1) + "VS" + a.GetLength(0));

                                    print = true;
                                }
                            }
                            else

                            //Up
                            if (dir == 1 && print == false)
                            {
                                if (i - 1 >= 0)
                                {
                                    if (a[i - 1, j] == 0 || a[i - 1, j] == 1)
                                    {
                                        a[i - 1, j] = 3;
                                    }

                                    print = true;
                                }
                            }
                            else

                            //Left
                            if (dir == 2 && print == false)
                            {
                                if (j - 1 >= 0)
                                {
                                    if (a[i, j - 1] == 0 || a[i, j - 1] == 1)
                                    {
                                        a[i, j - 1] = 3;
                                    }

                                    print = true;
                                }
                            }
                            else

                            //Down
                            if (dir == 3 && print == false)
                            {
                                if (i + 1 <= a.GetLength(1))
                                {
                                    if (a[i + 1, j] == 0 || a[i + 1, j] == 1)
                                    {
                                        a[i + 1, j] = 3;
                                    }

                                    Debug.Log("WTFFF " + (i + 1) + "VS" + a.GetLength(1));

                                    print = true;
                                }
                            }
                        }
                        a[i, j] = 1;
                        print = false;

                    }
                }
            }

            Debug.Log("Found " + (desiredRooms - roomCheck) + " rooms");

            beginning.text = "";
            foreach (int number in a)
            {
                count += 1;
                beginning.text += number;
                if (count == a.GetLength(0))
                {
                    beginning.text += "\n";
                    count = 0;
                }
            }

            yield return new WaitForEndOfFrame();

            Debug.Log("Turning 3 to 2");

            i = 0;
            j = 0;

            for (i = 0; i < 5; i++)
            {
                for (j = 0; j < 5; j++)
                {
                    if (a[i, j] == 3)
                    {
                        a[i, j] = 2;
                    }
                }
            }

            yield return new WaitForEndOfFrame();

            beginning.text = "";
            foreach (int number in a)
            {
                count += 1;
                beginning.text += number;
                if (count == a.GetLength(0))
                {
                    beginning.text += "\n";
                    count = 0;
                }
            }

            Debug.Log("end of loop");
        }

        yield return null;
    }
}
