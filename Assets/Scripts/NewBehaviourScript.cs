using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update

    struct PAI
    {
        int num;
        int color;
    }

    int[] Nums = new int[10] { 5, 6, 6, 6, 7, 8, 8, 8, 8, 10 };
    int[] Colors = new int[10] { 1, 1, 2, 3, 1, 2, 3, 4, 1, 1 };


    //Dictionary<int, int> colorDict = new Dictionary<int, int> { { 5, 1 }, { 6, 1 }, { 6, 2 }, { 6, 3 }, { 7, 1 }, { 8, 2 }, { 8, 3 }, { 8, 4 }, { 9, 1 }, { 10, 1 } };
    //Dictionary<int, int> colorDict = new Dictionary<int, int> { { 5, 1 }, { 6, 1 }, { 6, 2 }, { 6, 3 }, { 7, 1 }, { 8, 2 }, { 8, 3 }, { 8, 4 }, { 9, 1 }, { 10, 1 } };
    Dictionary<int, int[]> colorDict = new Dictionary<int, int[]>();
    List<List<PAI>> Result1 = new List<List<PAI>>();
    void Start()
    {

        //Nums = colorDict.Keys.ToArray();
        //Array.Sort(Nums);

        //for(int i=0; i<10; i++)
        //{
        //    Colors[i] = colorDict[Nums[i]];
        //}
        PresetNums(Nums);
        PresetStraight(Nums, Colors);
    }

    void PresetNums(int[] nums)
    {
        // 处理三条及以上的情况,即相同数字大于等于三个
        int min = 0;
        int step = 2;
        bool findTala = false;
        while (min < nums.Length)
        {
            int max = min + step;
            Debug.Log("Min:" + min);
            if (max < nums.Length && nums[min] == nums[max]) // 符合塔拉规则
            {
                List<List<int>> tala = new List<List<int>>();
                for(int index = min; index <= max; index++)
                {
                    //List<int> pai = new List<int>() { nums[index], colors[index]};
                    //tala.Add(pai);
                }
                //results.Add(tala);
                Debug.Log(min);
                Debug.Log($"{step}");
                step++;
                findTala = true;

            }
            else
            {
                if (findTala)
                {
                    min = max;
                    step = 2;
                }
                else
                {
                    min++;
                    step = 2;
                }
                findTala = false;
            }
        }

        
    }

    void PresetStraight(int[] nums, int[] colors)
    {
        // 处理同花顺的情况
        int min = 0;
        int count = 1;
        for (min=0; min < nums.Length; min++)
        {
            count = 1;
            int numMin = nums[min];
            while (true)
            {
                int num = numMin + 1;
                if (!nums.Contains(num)) break;
                for (int max = min + 1; max < nums.Length; max++)// 这里需要改成直接获取数字对应的值
                {
                    if (nums[max] != num) continue;

                    if (colors[max] == colors[min])
                    {
                        count++;
                        if (count >= 3)
                        {
                            Debug.Log($"num:{num},color:{colors[max]}, count:{count}");
                        }
                    }
                }
                numMin++;
            }
           


        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
