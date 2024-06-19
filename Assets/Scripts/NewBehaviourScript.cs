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

    struct SingleResult
    {
        public List<int[]> TaraQueue;
        public List<int> ExcludeIndex;
    }

    struct PAIQueue
    {
        List<PAI> Queue;
    }

    int[] Nums = new int[10] { 5, 6, 6, 6, 7, 8, 8, 8, 8, 10 };
    int[] Colors = new int[10] { 1, 1, 2, 3, 1, 2, 3, 4, 1, 1 };

    int[] Nums1 = new int[10] { 6, 6, 6, 6, 7, 8, 8, 8, 8, 10 };
    int[] Colors1 = new int[10] { 1, 2, 3, 4, 1, 1, 2, 3, 4, 1 };


    //Dictionary<int, int> colorDict = new Dictionary<int, int> { { 5, 1 }, { 6, 1 }, { 6, 2 }, { 6, 3 }, { 7, 1 }, { 8, 2 }, { 8, 3 }, { 8, 4 }, { 9, 1 }, { 10, 1 } };
    //Dictionary<int, int> colorDict = new Dictionary<int, int> { { 5, 1 }, { 6, 1 }, { 6, 2 }, { 6, 3 }, { 7, 1 }, { 8, 2 }, { 8, 3 }, { 8, 4 }, { 9, 1 }, { 10, 1 } };
    Dictionary<int, int[]> colorDict = new Dictionary<int, int[]>();
    List<SingleResult> Results = new List<SingleResult>();
    void Start()
    {

        //Nums = colorDict.Keys.ToArray();
        //Array.Sort(Nums);

        //for(int i=0; i<10; i++)
        //{
        //    Colors[i] = colorDict[Nums[i]];
        //}
        //PresetNums(Nums);


        //PresetStraight(Nums, Colors);
        //GetOptimalSolution(Nums, Colors, new int[10], Results);
        Results.Clear();
        GetAllSolutions(Nums, Colors, Results);

        foreach(SingleResult singleResult in Results)
        {
            string str = "";
            foreach(var taraIndexes in singleResult.TaraQueue)
            {
                foreach(var taraIndex in taraIndexes)
                {
                    str += Nums[taraIndex];
                }
 
            }
            for(int index=0; index<Nums.Length;index++)
            {
                if (singleResult.ExcludeIndex.Contains(index)) continue;
                str += Nums[index];
            }
            Debug.Log("queue" + str);
        }
    }

    //void GetOptimalSolution(int[] nums, int[] colors, int[] excludeIndex, List<SingleResult> results)
    //{
    //    PresetNums(nums, colors, excludeIndex);
    //    PresetStraight(nums, colors, excludeIndex);
    //}

    private SingleResult GetDefaultSingleResult()
    {
        return new SingleResult()
        {
            TaraQueue = new List<int[]>(),
            ExcludeIndex = new List<int>(),
        };
    }

    void GetAllSolutions(int[] nums, int[] colors, List<SingleResult> results)
    {
        bool hasTara = false;
        if (results.Count == 0)
        {
            hasTara = PresetNums(nums, GetDefaultSingleResult(), results);
            //PresetStraight(nums, GetDefaultSingleResult(), colors, excludeIndex);
        }
        else
        {
            foreach(SingleResult singleResult in results)
            {
                if (singleResult.ExcludeIndex.Count < nums.Length)
                {
                    hasTara = PresetNums(nums, singleResult, results);
                }

            }

            //PresetStraight(nums, colors, excludeIndex);
        }


        // 更新excludeIndex;  excludeIndex应该是和单个结果绑定 换成还存在的下标，即散牌


        // 怎么判断递归继续和打断
        // 只要这次递归有一个队列继续发现了塔拉，证明还可能有塔拉
        if(hasTara)
        {
            //继续递归
            GetAllSolutions(nums, colors, results);
        }
    }

    bool PresetNums(int[] nums, SingleResult singleResult, List<SingleResult> results)
    {
        // 处理三条及以上的情况,即相同数字大于等于三个
        int min = 0;
        int step = 2;
        bool findTala = false;
        bool hasTara = false;
        while (min < nums.Length)
        {
            if (singleResult.ExcludeIndex.Contains(min)) continue;

            int max = min + step;
            Debug.Log("Min:" + min);
            if (max < nums.Length && nums[min] == nums[max]) // 符合塔拉规则
            {
                int[] tara = new int[max - min + 1];
                for(int index = min; index <= max; index++)
                {
                    //List<int> pai = new List<int>() { nums[index], colors[index]};
                    //tala.Add(pai);
                    tara[index - min] = index;

                }

                SingleResult newSingleResult = GetDefaultSingleResult();
                newSingleResult.TaraQueue.AddRange(singleResult.TaraQueue);
                newSingleResult.TaraQueue.Add(tara);
                newSingleResult.ExcludeIndex.AddRange(tara);

                results.Add(newSingleResult);
                //results.Add(tala);
                Debug.Log(min);
                Debug.Log($"{step}");
                step++;
                findTala = true;
                hasTara = true;
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
        return hasTara;
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
