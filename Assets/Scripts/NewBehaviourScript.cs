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
        GetAllSolutions(Nums, Colors, Results, true);

        foreach(SingleResult singleResult in Results)
        {
            string str = "";
            foreach(var taraIndexes in singleResult.TaraQueue)
            {
                str += "Tara:";
                foreach (var taraIndex in taraIndexes)
                {
                    str += $"{Nums[taraIndex]}:{Colors[taraIndex]}/";
                }
 
            }
            str += "san:";
            for(int index=0; index<Nums.Length;index++)
            {
                if (singleResult.ExcludeIndex.Contains(index)) continue;
                str += $"{Nums[index]}:{Colors[index]}/"; ;
            }
            Debug.Log("queue:" + str);
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

    void GetAllSolutions(int[] nums, int[] colors, List<SingleResult> results, bool start)
    {
        bool hasNumsTara = false;
        bool hasStraightTara = false;
        if (start)
        {
            hasNumsTara = PresetNums(nums, colors, GetDefaultSingleResult(), results);
            hasStraightTara = PresetStraight(nums, colors, GetDefaultSingleResult(), results);
        }
        else
        {
            for(int i=0; i<results.Count; i++)
            {
                SingleResult singleResult = results[i];
                if (singleResult.ExcludeIndex.Count < nums.Length)
                {
                    bool findNumsTara = PresetNums(nums, colors, singleResult, results);
                    hasNumsTara = findNumsTara ? findNumsTara : hasNumsTara;
                    bool findStraightTara = PresetStraight(nums, colors, singleResult, results);
                    hasStraightTara = findStraightTara ? findStraightTara : hasStraightTara;
                }
                results.Remove(singleResult);
            }

        }


        // 更新excludeIndex;  excludeIndex应该是和单个结果绑定 换成还存在的下标，即散牌


        // 怎么判断递归继续和打断
        // 只要这次递归有一个队列继续发现了塔拉，证明还可能有塔拉
        if(hasNumsTara || hasStraightTara)
        {
            //继续递归
            GetAllSolutions(nums, colors, results, false);
        }
    }

    bool PresetNums(int[] nums, int[] colors, SingleResult singleResult, List<SingleResult> results) // 需要排序
    {
        // 处理三条及以上的情况,即相同数字大于等于三个
        int min = 0;
        int step = 2;
        bool findTala = false;
        bool hasTara = false;

        // 找到所有三条和四条的情况 考虑花色
        while (min < nums.Length)
        {
            if (singleResult.ExcludeIndex.Contains(min))
            {
                min++;
                step = 2;
                continue;
            }

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
                newSingleResult.ExcludeIndex.AddRange(singleResult.ExcludeIndex);
                newSingleResult.ExcludeIndex.AddRange(tara);
                results.Add(newSingleResult);

                if (step == 3) //四条的情况 三条需要排列组合 总共四种情况 012 013 023 123
                {
                    int[] tara1 = new int[3] { min, min + 1, min + 2 }; // 上一次已经添加过了
                    int[] tara2 = new int[3] { min, min + 1, min + 3 };
                    int[] tara3 = new int[3] { min, min + 2, min + 3 };
                   // int[] tara4 = new int[3] { min + 1, min + 2, min + 3 };

                    //SingleResult newSingleResult1 = GetDefaultSingleResult();
                    //newSingleResult1.TaraQueue.AddRange(singleResult.TaraQueue);
                    //newSingleResult1.TaraQueue.Add(tara1);
                    //newSingleResult1.ExcludeIndex.AddRange(singleResult.ExcludeIndex);
                    //newSingleResult1.ExcludeIndex.AddRange(tara1);
                    //results.Add(newSingleResult1);

                    SingleResult newSingleResult2 = GetDefaultSingleResult();
                    newSingleResult2.TaraQueue.AddRange(singleResult.TaraQueue);
                    newSingleResult2.TaraQueue.Add(tara2);
                    newSingleResult2.ExcludeIndex.AddRange(singleResult.ExcludeIndex);
                    newSingleResult2.ExcludeIndex.AddRange(tara2);
                    results.Add(newSingleResult2);

                    SingleResult newSingleResult3 = GetDefaultSingleResult();
                    newSingleResult3.TaraQueue.AddRange(singleResult.TaraQueue);
                    newSingleResult3.TaraQueue.Add(tara3);
                    newSingleResult3.ExcludeIndex.AddRange(singleResult.ExcludeIndex);
                    newSingleResult3.ExcludeIndex.AddRange(tara3);
                    results.Add(newSingleResult3);

                    //SingleResult newSingleResult4 = GetDefaultSingleResult();
                    //newSingleResult4.TaraQueue.AddRange(singleResult.TaraQueue);
                    //newSingleResult4.TaraQueue.Add(tara4);
                    //newSingleResult4.ExcludeIndex.AddRange(singleResult.ExcludeIndex);
                    //newSingleResult4.ExcludeIndex.AddRange(tara4);
                    //results.Add(newSingleResult4);
                }

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
                    min++;
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

    bool PresetStraight(int[] nums, int[] colors, SingleResult singleResult, List<SingleResult> results)
    {
        // 处理同花顺的情况
        bool findStraight = false;
        int min = 0;
        int count = 1;

        List<int> tara = new List<int>();
        for (min=0; min < nums.Length; min++) // 找到所有可能的同花顺
        {
            if (singleResult.ExcludeIndex.Contains(min)) continue; // 下标已经被排除

            count = 1;
            tara.Clear();
            tara.Add(min);
            int numMin = nums[min]; // 开始的数字
            while (true)
            {
                int compareNum = numMin + 1;
                if (!nums.Contains(compareNum)) break; // 手牌没有对应的数字

                for (int max = min + 1; max < nums.Length; max++)// 在后续的值中找数字 这里需要改成直接获取数字对应的值
                {
                    if (singleResult.ExcludeIndex.Contains(max)) continue; // 下标已经被排除
                    if (nums[max] != compareNum) continue;

                    if (colors[max] == colors[min]) // 花色相同
                    {

                        count++;
                        tara.Add(max);
                        if (count >= 3)
                        {
                            findStraight = true;
                            Debug.Log($"num:{compareNum}, count:{count},color:{colors[max]}");

                            SingleResult newSingleResult = GetDefaultSingleResult();
                            newSingleResult.TaraQueue.AddRange(singleResult.TaraQueue);
                            newSingleResult.TaraQueue.Add(tara.ToArray());
                            newSingleResult.ExcludeIndex.AddRange(singleResult.ExcludeIndex);
                            newSingleResult.ExcludeIndex.AddRange(tara.ToArray());

                            results.Add(newSingleResult);
                        }
                    }
                }
                numMin++;
            }
        }

        return findStraight;
    }

}
